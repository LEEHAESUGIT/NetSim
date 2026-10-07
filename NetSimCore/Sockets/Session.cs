using NETSIM.Context;
using NETSIM.Measure;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets;
using NETSIM.Packet.Tool;
using NETSIM.Server.Router.Pipe;
using NETSIM.System.Buffer;
using Shared.Configration;
using Shared.Enum;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NETSIM.Sockets
{
	internal class Session
	{
		// Local
		private int _disconnectedLock = 0; // 1 일때 lock
		private PacketBufferSystem _packetBuffer;
		private PacketFormatter _formatter;

		// Token
		private CancellationTokenSource _linkedCts;
		private CancellationTokenSource _sessionCts;
		private CancellationToken _loopToken;

		// Pipe
		private ModulePipe _modulePipe;

		private TcpClient _client;
		private NetworkStream _stream;





		internal DateTime LastHeartBeatTime { get; set; }
		internal long LastHeartBeatTick { get; set; }



		internal int SessionID { get; init; }
		//internal int PlayerID { get; private set; }

		internal Action<int> OnDisconnected;


		internal Session(int sessionID)
		{
			this.SessionID = sessionID;
			this._packetBuffer = new PacketBufferSystem(4096);
			this._formatter = new PacketFormatter();
		}

		internal void Init(TcpClient client, ModulePipe modulePipe, CancellationToken serverShutDownToken)
		{
			this._client = client;
			this._stream = client.GetStream();

			this._sessionCts = new CancellationTokenSource();
			this._linkedCts = CancellationTokenSource.CreateLinkedTokenSource(serverShutDownToken, _sessionCts.Token);
			this._loopToken = _linkedCts.Token;

			this._modulePipe = modulePipe;

			this._disconnectedLock = 0;

			LastHeartBeatTime = DateTime.UtcNow;
			LastHeartBeatTick = Environment.TickCount64;
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

		}

		internal void Disconnect()
		{
			if (Interlocked.Exchange(ref _disconnectedLock, 1) == 1) return;

			try
			{
				_sessionCts?.Cancel();

				Stop();
				Clear();
				OnDisconnected?.Invoke(this.SessionID);
			}
			catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }
		}

		private void Clear()
		{
			if (this._stream != null && this._client != null)
			{
				this._stream?.Close();
				this._client?.Close();
			}
			this._stream = null;
			this._client = null;

			this._linkedCts.Dispose();
			this._sessionCts.Dispose();

			this._modulePipe = null;

			this._packetBuffer.Recv.Clear();

			LastHeartBeatTime = DateTime.MinValue;
			SessionMeasure.Instance.Clear(SessionID);
		}


		internal void Start()
		{
			SessionMeasure.Instance.OnTimer(this.SessionID);
			SessionMeasure.Instance.OnSessionID(this.SessionID);
			SessionMeasure.Instance.OnStateLight(this.SessionID, EStateLight.GREEN);
			_ = ReceivedLoopAsync();
		}

		internal void Stop()
		{

			SessionMeasure.Instance.OnStateLight(this.SessionID, EStateLight.NONE);
			SessionMeasure.Instance.OffTimer(this.SessionID);

		}

		internal async Task<bool> Send(ReadOnlyMemory<byte> packetData)
		{
			try
			{
				if (this._disconnectedLock == 1)
					return false;
				await _stream.WriteAsync(packetData);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				return false;
			}
		}

		private async Task ReceivedLoopAsync()
		{
			try
			{
				while (!_loopToken.IsCancellationRequested)
				{

					if (_packetBuffer.Recv.FreeSize == 0)
					{
						_packetBuffer.Recv.TryWritePrepare(1);
						if (_packetBuffer.Recv.FreeSize == 0) break;
					}

					int received = await _stream.ReadAsync(_packetBuffer.Recv.WriteSpace, _loopToken);
					if (received <= 0 || _disconnectedLock == 1)
					{
						Disconnect();
						break;
					}

					_packetBuffer.Recv.OnWrite(received);

					PacketProcess();
				}
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($"정상적인 해제 {this.SessionID}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[수신 에러] SessionID: {SessionID}, 원인: {ex.Message}");
			}
			finally
			{
				Disconnect();
			}
		}

		private void PacketProcess()
		{
			while (true)
			{
				// 최소 헤더 크기
				if (_packetBuffer.Recv.Datasize < 4) return;

				// 헤더 크기 파싱
				ushort PacketSize = BinaryPrimitives.ReadUInt16LittleEndian(_packetBuffer.Recv.ReadSpace);

				if (PacketSize >= ServerConfigurationData.MaxPacketSize)
				{
					Disconnect();
					return;
				}

				if (_packetBuffer.Recv.Datasize < PacketSize) return;

				ReadOnlySpan<byte> packetSpan = _packetBuffer.Recv.ReadSpace.Slice(0, PacketSize);

				if (_formatter.TryParse(packetSpan, out IPacket? parsePacket))
				{
					BranchPacket(new ReceivePacketContext { SessionID = this.SessionID, Packet = parsePacket });
				}
				else
				{
					Disconnect();
					return;
				}
				_packetBuffer.Recv.OnRead(PacketSize);
			}
		}

		private void BranchPacket(ReceivePacketContext packetContext)
		{
			int packetType = (int)packetContext.Packet.packetID;

			// IngamePacket
			if (0 < packetType && packetType < 100)
			{
				_modulePipe.GamePacketPipe.InBoundPipe.TryWrite(packetContext);
				return;
			}
			// SystemPacket
			if (99 < packetType && packetType < 200)
			{
				_modulePipe.SystemPacketPipe.InBoundPipe.TryWrite(packetContext);
				return;
			}
		}


	}
}

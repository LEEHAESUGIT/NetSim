using NETSIM.Common;
using NETSIM.MeasureCore.Measure;
using NETSIM.Packet.Tool;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Sockets.Buffer;
using NETSIM.Types.Interface;
using NETSIM_ConsoleView.LogMonitor;
using Shared.Enum;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Threading.Channels;

namespace NETSIM.Sockets
{
	internal class Session
	{
		// Local
		private TcpClient _client;
		private NetworkStream _stream;
		//private PacketBufferSystem _packetBuffer;

		private ReceiveBuffer _receiveBuff;
		private PacketFormatter _formatter;

		internal int SessionID { get; init; }
		private int _sessionGeneration = 0;
		internal int Generation => _sessionGeneration;

		private int _disconnectedLock = 0;

		internal long LastHeartBeatTick { get; set; }
		internal DateTime LastHeartBeatTime { get; set; }

		// Token
		private CancellationTokenSource _linkedCts;
		private CancellationTokenSource _sessionCts;
		private CancellationToken _loopToken;

		// Pipe
		private ModulePipe _modulePipe;


		//internal Action<int> OnDisconnected;


		internal Session(int sessionID)
		{
			SessionID = sessionID;
		}

		internal void Init(TcpClient client, ModulePipe modulePipe, CancellationToken serverShutDownToken)
		{
			_receiveBuff = new ReceiveBuffer(4096);
			_formatter = new PacketFormatter();

			_client = client;
			_stream = client.GetStream();

			_sessionCts = new CancellationTokenSource();
			_linkedCts = CancellationTokenSource.CreateLinkedTokenSource(serverShutDownToken, _sessionCts.Token);
			_loopToken = _linkedCts.Token;

			_modulePipe = modulePipe;

			_disconnectedLock = 0;

			LastHeartBeatTick = Environment.TickCount64;
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

			Interlocked.Increment(ref _sessionGeneration);
		}

		internal void InitClearToken()
		{
			_linkedCts.Dispose();
			_sessionCts.Dispose();

		}




		//internal void Disconnect()
		//{
		//	if (Interlocked.Exchange(ref _disconnectedLock, 1) == 1) return;

		//	try
		//	{
		//		LogView.Instance.WriteLogFile(ELogType.INFO, $"PlayerDisconnect {SessionID}");
		//		_sessionCts?.Cancel();

		//		Stop();
		//		Clear();
		//		//OnDisconnected?.Invoke(SessionID);
		//	}
		//	catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }
		//}



		//private void Clear()
		//{
		//	if (_stream != null && _client != null)
		//	{
		//		_stream?.Close();
		//		_client?.Close();
		//	}
		//	_stream = null;
		//	_client = null;

		//	_linkedCts.Dispose();
		//	_sessionCts.Dispose();

		//	_modulePipe = null;

		//	_receiveBuff.Clear();

		//	ClearSendPipe();

		//	SessionMeasure.Instance.Clear(SessionID);
		//}



		internal void Start()
		{
			SessionMeasure.Instance.OnTimer(SessionID);
			SessionMeasure.Instance.OnSessionID(SessionID);
			SessionMeasure.Instance.OnStateLight(SessionID, EStateLight.GREEN);
		}

		internal void Stop()
		{
			SessionMeasure.Instance.OnStateLight(SessionID, EStateLight.NONE);
			SessionMeasure.Instance.OffTimer(SessionID);

		}

		internal void RequestStop()
		{
			if (Interlocked.Exchange(ref _disconnectedLock, 1) == 1) return;

			try
			{
				_sessionCts.Cancel();
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"_sessionCts.Cancel : {ex}");
			}

			try
			{
				_client.Close();
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"_client.Close : {ex}");
			}
			Stop();


		}
		internal async Task RunAsync()
		{
			Task SendLoopAsyncTask = SendLoopAsync();
			Task ReceivedLoopAsyncTask = ReceivedLoopAsync();

			await Task.WhenAny(SendLoopAsyncTask, ReceivedLoopAsyncTask);

			RequestStop();

			try
			{
				await Task.WhenAll(SendLoopAsyncTask, ReceivedLoopAsyncTask);
			}
			catch (OperationCanceledException) when (_loopToken.IsCancellationRequested)
			{
				// 이 세션에 요청된 취소로 루프가 종료됨
			}
			finally
			{
				Clear();
			}
		}
		private void Clear()
		{
			ClearSendPipe();
			_modulePipe = null;

			_linkedCts.Dispose();
			_sessionCts.Dispose();
			_client.Dispose();
		}



		#region Send
		private Channel<ReadOnlyMemory<byte>> _sendPipe = Channel.CreateUnbounded<ReadOnlyMemory<byte>>();
		internal bool InputPacketData(ReadOnlyMemory<byte> context)
		{
			if (_disconnectedLock == 1)
				return false;
			else
			{
				return _sendPipe.Writer.TryWrite(context);
			}
		}
		private void ClearSendPipe()
		{
			while (_sendPipe.Reader.TryRead(out _)) { }
		}
		private async Task SendLoopAsync()
		{
			while (await _sendPipe.Reader.WaitToReadAsync(_loopToken))
			{
				while (_sendPipe.Reader.TryRead(out ReadOnlyMemory<byte> packetData))
				{
					if (_disconnectedLock == 1) return;

					await _stream.WriteAsync(packetData, _loopToken);
					ServerMeasure.Instance.OnAddSendByteLength(packetData.Length);
				}
			}
		}
		#endregion
		#region Received
		private async Task ReceivedLoopAsync()
		{
			try
			{
				while (!_loopToken.IsCancellationRequested)
				{

					if (_receiveBuff.FreeSize == 0)
					{
						_receiveBuff.TryWritePrepare(1);
						if (_receiveBuff.FreeSize == 0) break;
					}

					int received = await _stream.ReadAsync(_receiveBuff.WriteSpace, _loopToken);
					ServerMeasure.Instance.OnAddRecvByteLength((long)received);
					if (_disconnectedLock == 1)
						return;
					if (received == 0)
					{
						LogView.Instance.WriteLogFile(ELogType.INFO, $"Peer closed connection. SessionID: {SessionID} ");
						RequestStop();
						break;
					}

					_receiveBuff.OnWrite(received);

					PacketProcess();
				}
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($"정상적인 해제 {SessionID}");
			}
			catch (Exception ex)
			{
				//Console.WriteLine($"[수신 에러] SessionID: {SessionID}, 원인: {ex.Message}");
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"[수신 에러] SessionID: {SessionID}, 원인: {ex.Message}");
			}
			//catch (IOException ex)
			//	when (ex.InnerException is SocketException socketEx
			//	  && socketEx.SocketErrorCode == SocketError.ConnectionReset)
			//{
			//	LogView.Instance.WriteLogFile(
			//		ELogType.WARN,
			//		$"Connection reset by peer. " +
			//		$"SessionID: {SessionID}, Generation: {Generation}, " +
			//		$"SocketError: {socketEx.SocketErrorCode}, " +
			//		$"NativeError: {socketEx.NativeErrorCode}, " +
			//		$"StopRequested: {_loopToken.IsCancellationRequested}. " +
			//		$"{ex}");
			//}
			//catch (Exception ex)
			//{
			//	LogView.Instance.WriteLogFile(
			//		ELogType.ERROR,
			//		$"Receive loop failed. " +
			//		$"SessionID: {SessionID}, Generation: {Generation}, " +
			//		$"StopRequested: {_loopToken.IsCancellationRequested}. " +
			//		$"{ex}");
			//}
			finally
			{
				LogView.Instance.WriteLogFile(ELogType.INFO, $"Token Close {SessionID}");
				//Disconnect();
				//OnDisconnected.Invoke(SessionID);
			}
		}

		private void PacketProcess()
		{
			while (true)
			{
				// 최소 헤더 크기
				if (_receiveBuff.Datasize < 6) return;

				// 헤더 크기 파싱
				ushort PacketSize = BinaryPrimitives.ReadUInt16LittleEndian(_receiveBuff.ReadSpace);

				if (PacketSize >= ServerDataConfigration.ReceivedMaxPacketSize)
				{
					LogView.Instance.WriteLogFile(ELogType.WARN, $"packetsize >= MaxPacketSize {SessionID}");
					RequestStop();
					return;
				}

				if (_receiveBuff.Datasize < PacketSize) return;

				ReadOnlySpan<byte> packetSpan = _receiveBuff.ReadSpace.Slice(0, PacketSize);

				if (_formatter.TryParse(packetSpan, out IPacket? parsePacket))
				{
					BranchPacket(new ReceivePacketDispatch(SessionID, Generation, parsePacket));
				}
				else
				{
					LogView.Instance.WriteLogFile(ELogType.WARN, $"Failed Packet Parse{SessionID}");
					RequestStop();
					return;
				}
				_receiveBuff.OnRead(PacketSize);
			}
		}

		private void BranchPacket(ReceivePacketDispatch packetContext)
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
		#endregion


	}
}

using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets;
using NETSIM.Packet.Packets.ClientToServerPacket;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Packet.Packets.ForSystem;
using System;
using System.Buffers.Binary;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Tool
{

	internal class PacketFormatter
	{


		private FrozenDictionary<EPacketID, Func<PacketReader, IPacket>> _parseMap;
		internal PacketFormatter()
		{
			_parseMap = InitParseMap();
		}

		// 송신하기 위한 데이터 패킷을 byte배열형 으로 변환
		internal ReadOnlyMemory<byte> Format(IPacket packet)
		{
			PacketWriter _packetWriter = new PacketWriter();
			_packetWriter.Clear();

			_packetWriter.Write((ushort)0); // size 자료형 ushort(2byte)
			_packetWriter.Write((int)0);    // packetID 자료형 int(4byte)

			packet.Serialize(_packetWriter);

			ushort totalSize = (ushort)_packetWriter.GetMutableSpan().Length;
			Span<byte> headerSpan = _packetWriter.GetMutableSpan().Slice(0, 6);

			BinaryPrimitives.WriteUInt16LittleEndian(headerSpan.Slice(0, 2), totalSize);
			BinaryPrimitives.WriteInt32LittleEndian(headerSpan.Slice(2, 4), (int)packet.packetID);

			return _packetWriter.writterMemory;

		}

		// 수신한 바이트배열 형식의 패킷을 패킷형태로 변환
		//internal bool TryParse(ReadOnlySpan<byte> packetData, out IPacket? packet)
		//{
		//	PacketReader _packetReader = new PacketReader();
		//	packet = null;
		//	try
		//	{
		//		_packetReader.Clear();

		//		_packetReader.Init(packetData);

		//		PacketHeader header = PacketHeader.Deserialize(_packetReader);

		//		switch (header.packetID)
		//		{
		//			case EPacketID.C_HeartBeat:
		//				packet = C2S_HeartBeat.Deserialize(_packetReader);
		//				return true;
		//			default:
		//				//Console.WriteLine($"Unknown Packet : {header.packetID}");
		//				return false;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//Console.WriteLine(ex.Message);
		//		return false;
		//	}
		//}


		internal bool TryParse(ReadOnlySpan<byte> packetData, out IPacket packet)
		{
			PacketReader _packetReader = new PacketReader();
			packet = null;
			try
			{
				_packetReader.Clear();
				_packetReader.Init(packetData);
				PacketHeader header = PacketHeader.Deserialize(_packetReader);

				if (_parseMap.TryGetValue(header.packetID, out Func<PacketReader, IPacket> func))
				{
					packet = func.Invoke(_packetReader);
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				return false;
			}
		}
		private FrozenDictionary<EPacketID, Func<PacketReader, IPacket>> InitParseMap()
		{
			var tempInitParseMap = new Dictionary<EPacketID, Func<PacketReader, IPacket>>();
			// TODO: [Perf Optimization]
			// 현재 Func<PacketReader, IPacket> 반환 시 struct -> interface 캐스팅으로 인한 Boxing(GC Alloc) 발생 중.
			// 추후 Action<Session, PacketReader> 기반의 In-place 파싱 및 핸들러 직접 호출 구조로 전환하여 제로 알로케이션 달성할 것.
			tempInitParseMap.Add(EPacketID.C_Spawn, (_packetReader) => C2S_SpawnRequest.Deserialize(_packetReader));
			tempInitParseMap.Add(EPacketID.C_Move, (_packetReader) => C2S_MovePacket.Deserialize(_packetReader));


			tempInitParseMap.Add(EPacketID.C_HeartBeat, (_packetReader) => C2S_HeartBeat.Deserialize(_packetReader));
			tempInitParseMap.Add(EPacketID.C_Init, (_packetReader) => C2S_InitRequest.Deserialize(_packetReader));



			return tempInitParseMap.ToFrozenDictionary();
		}

	}
}
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Tool
{
	internal class PacketWriter
	{
		private readonly ArrayBufferWriter<byte> _buffer = new();

		internal ReadOnlyMemory<byte> writterMemory => _buffer.WrittenMemory;
		internal Span<byte> GetMutableSpan()
		{
			ReadOnlySpan<byte> readonlySpan = _buffer.WrittenSpan;
			return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(readonlySpan), readonlySpan.Length);
		}
		internal void Write(bool value)
		{
			int spanSize = sizeof(bool);
			Span<byte> span = _buffer.GetSpan(spanSize);
			span[0] = (byte)(value ? 1 : 0);
			_buffer.Advance(spanSize);
		}
		internal void Write(int value)
		{
			int spanSize = sizeof(int);
			Span<byte> span = _buffer.GetSpan(spanSize);
			BinaryPrimitives.WriteInt32LittleEndian(span, value);
			_buffer.Advance(spanSize);
		}
		internal void Write(float value)
		{
			int spanSize = sizeof(float);
			Span<byte> span = _buffer.GetSpan(spanSize);
			BinaryPrimitives.WriteSingleLittleEndian(span, value);
			_buffer.Advance(spanSize);
		}
		internal void Write(long value)
		{
			int spanSize = sizeof(long);
			Span<byte> span = _buffer.GetSpan(spanSize);
			BinaryPrimitives.WriteInt64LittleEndian(span, value);
			_buffer.Advance(spanSize);
		}
		internal void Write(ushort value)
		{
			int spanSize = sizeof(ushort);
			Span<byte> span = _buffer.GetSpan(spanSize);
			BinaryPrimitives.WriteUInt16LittleEndian(span, value);
			_buffer.Advance(spanSize);
		}
		internal void Write(string value)
		{
			int stringSize = Encoding.UTF8.GetByteCount(value);
			Write((ushort)stringSize);
			Span<byte> span = _buffer.GetSpan(stringSize);
			int writtenBytes = Encoding.UTF8.GetBytes(value, span);
			_buffer.Advance(writtenBytes);

		}
		internal void Write(ReadOnlySpan<byte> value)
		{
			Span<byte> span = _buffer.GetSpan(value.Length);
			value.CopyTo(span);
			_buffer.Advance(value.Length);
		}

		internal void Clear() => _buffer.Clear();

	}
	internal class PacketReader
	{
		private readonly ArrayBufferWriter<byte> _buffer = new();
		private int _offset;
		internal ReadOnlyMemory<byte> ReaderMemory => _buffer.WrittenMemory;
		internal Span<byte> GetMutableSpan()
		{
			ReadOnlySpan<byte> readonlySpan = _buffer.WrittenSpan;
			return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(readonlySpan), readonlySpan.Length);
		}

		internal void Init(ReadOnlySpan<byte> buffer)
		{
			_buffer.Write(buffer);
			_offset = 0;
		}

		internal bool ReadBool()
		{
			bool value = _buffer.WrittenSpan[_offset] != 0;
			_offset += sizeof(bool);
			return value;
		}
		public int ReadInt()
		{
			int value = BinaryPrimitives.ReadInt32LittleEndian(_buffer.WrittenSpan.Slice(_offset));

			_offset += sizeof(int);
			return value;
		}
		public float ReadFloat()
		{
			float value = BinaryPrimitives.ReadSingleLittleEndian(_buffer.WrittenSpan.Slice(_offset));

			//float value = BitConverter.ToSingle(_buffer , _offset);
			_offset += sizeof(float);
			return value;
		}
		public ushort ReadUshort()
		{
			ushort value = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.WrittenSpan.Slice(_offset));

			//ushort value = BitConverter.ToUInt16(_buffer , _offset);
			_offset += sizeof(ushort);
			return value;
		}
		public long ReadLong()
		{
			long value = BinaryPrimitives.ReadInt64LittleEndian(_buffer.WrittenSpan.Slice(_offset));
			_offset += sizeof(long);
			return value;
		}
		public string ReadString()
		{
			ushort length = ReadUshort();
			//string value = BitConverter.ToString(GetMutableSpan(), _offset, length);
			string value = Encoding.UTF8.GetString(_buffer.WrittenSpan.Slice(_offset, length));
			_offset += length;
			return value;
		}
		internal void Clear() => _buffer.Clear();

	}
}

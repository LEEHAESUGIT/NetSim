using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Sockets.Buffer
{
	internal class ReceiveBuffer
	{
		private readonly byte[] _container;

		private int _writePos = 0;
		private int _readPos = 0;

		internal int Datasize => _writePos - _readPos;
		internal int FreeSize => _container.Length - _writePos;

		internal Memory<byte> WriteSpace => _container.AsMemory(_writePos);
		internal ReadOnlySpan<byte> ReadSpace => _container.AsSpan(_readPos, Datasize);

		internal ReceiveBuffer(int size)
		{
			_container = new byte[size];
		}
		internal bool OnWrite(int bytes)
		{
			if (bytes < 0 || bytes > FreeSize)	return false;
			
			_writePos += bytes;
			
			return true;
		}
		internal bool OnRead(int bytes)
		{
			if (bytes < 0 || bytes > Datasize)	return false;
			
			// readpos == writepos 면 논리적으로는 공간이 있지만 실제 남은 공간사용을 위해
			_readPos += bytes;
			if (Datasize == 0)
			{
				_readPos = 0;
				_writePos = 0;
			}
			return true;
		}

		internal bool TryWritePrepare(int packetLength)
		{
			if (FreeSize >= packetLength)	return true;

			Compact();

			return FreeSize >= packetLength;
		}

		private void Compact()
		{
			if (_readPos == 0)	return;

			int dataSize = Datasize;

			ReadOnlySpan<byte> dataSpan = _container.AsSpan(_readPos , Datasize);
			Span<byte> freeSpan = _container.AsSpan(0);

			dataSpan.CopyTo(freeSpan);

			_readPos = 0;
			_writePos = dataSize;
		}

		internal void Clear()
		{
			_readPos = 0;
			_writePos = 0;
		}
	}
}

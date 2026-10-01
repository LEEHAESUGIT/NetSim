using NETSIM.Sockets.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.System.Buffer
{
	internal class PacketBufferSystem
	{
		internal ReceiveBuffer Recv;

		internal PacketBufferSystem(int bufferSize)
		{
			this.Recv = new ReceiveBuffer(bufferSize);
		}


	}
}

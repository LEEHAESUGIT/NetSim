using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Sockets.Buffer
{
	internal class SendBuffer
	{
		internal readonly byte[] _container;

		public SendBuffer(int size)
		{
			_container = new byte[size];
			
		}
	}
}

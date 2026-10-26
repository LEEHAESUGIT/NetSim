using NETSIM.Pipe.PipeDispatch;
using NETSIM.Types.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Pipe
{
	internal class ModulePipe
	{
		internal SystemPacketPipe SystemPacketPipe = new SystemPacketPipe();
		internal GamePacketPipe GamePacketPipe = new GamePacketPipe();
	}
}

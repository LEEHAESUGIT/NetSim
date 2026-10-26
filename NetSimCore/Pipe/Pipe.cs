using NETSIM.Pipe.PipeDispatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM.Pipe
{
	// (Server -> Client) or Receive
	internal class InBoundPipe
	{
		private Channel<ReceivePacketDispatch> _jobs = Channel.CreateUnbounded<ReceivePacketDispatch>();
		internal bool TryWrite(ReceivePacketDispatch context) => _jobs.Writer.TryWrite(context);
		internal ValueTask<bool> WaitForPipe(CancellationToken token) => _jobs.Reader.WaitToReadAsync(token);
		internal bool TryRead(out ReceivePacketDispatch context) => _jobs.Reader.TryRead(out context);
	}
	// (Client -> Server) or Send
	internal class OutBoundPipe
	{
		private Channel<SendPacketDispatch> _jobs = Channel.CreateUnbounded<SendPacketDispatch>();
		internal bool TryWrite(SendPacketDispatch context) => _jobs.Writer.TryWrite(context);
		internal ValueTask<bool> WaitForPipe(CancellationToken token) => _jobs.Reader.WaitToReadAsync(token);
		internal bool TryRead(out SendPacketDispatch context) => _jobs.Reader.TryRead(out context);
	}
}

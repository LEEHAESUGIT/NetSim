using NETSIM.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM.Server.Router.Pipe
{
	// (Server -> Client) or Receive
	internal class InBoundPipe
	{
		private Channel<ReceivePacketContext> _jobs = Channel.CreateUnbounded<ReceivePacketContext>();
		internal bool TryWrite(ReceivePacketContext context) => _jobs.Writer.TryWrite(context);
		internal ValueTask<bool> WaitForPipe(CancellationToken token) => _jobs.Reader.WaitToReadAsync(token);
		internal bool TryRead(out ReceivePacketContext context) => _jobs.Reader.TryRead(out context);
	}
	// (Client -> Server) or Send
	internal class OutBoundPipe
	{
		private Channel<ResultPacketContext> _jobs = Channel.CreateUnbounded<ResultPacketContext>();
		internal bool TryWrite(ResultPacketContext context) => _jobs.Writer.TryWrite(context);
		internal ValueTask<bool> WaitForPipe(CancellationToken token) => _jobs.Reader.WaitToReadAsync(token);
		internal bool TryRead(out ResultPacketContext context) => _jobs.Reader.TryRead(out context);
	}
}

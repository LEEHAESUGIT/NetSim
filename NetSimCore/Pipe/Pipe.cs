using System.Threading.Channels;

namespace NETSIM.Pipe
{
	internal class Pipe<T> where T : struct
	{
		private Channel<T> _jobs = Channel.CreateUnbounded<T>();
		internal bool TryWrite(T context) => _jobs.Writer.TryWrite(context);
		internal ValueTask<bool> WaitForPipe(CancellationToken token) => _jobs.Reader.WaitToReadAsync(token);
		internal bool TryRead(out T context) => _jobs.Reader.TryRead(out context);
	}
}

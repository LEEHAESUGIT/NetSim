using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
	public class DTOWrappers<T> where T : struct
	{
		public int ServerID { get; set; }
		public DateTime CreateAt { get; set; }
		public int TotalSession { get; set; }

		public T[] Items { get; set; }



		public DTOWrappers(int serverID , int totalSession , ReadOnlySpan<T> stateSpan )
		{
			this.ServerID = serverID;
			this.CreateAt = DateTime.UtcNow;
			this.TotalSession = totalSession;

			this.Items = stateSpan.ToArray();
		}
	}
}

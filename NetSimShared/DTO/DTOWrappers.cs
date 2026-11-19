namespace Shared.DTO
{
	public class DTOWrappers<T> where T : struct
	{
		public int ServerID { get; set; }
		public DateTime CreateAt { get; set; }
		public T[] Items { get; set; }

		public DTOWrappers(int serverID, ReadOnlySpan<T> stateSpan)
		{
			ServerID = serverID;
			CreateAt = DateTime.UtcNow;
			Items = stateSpan.ToArray();
		}
	}
}

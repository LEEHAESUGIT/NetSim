namespace NETSIM.Common
{
	internal readonly struct ServerDataConfigration
	{
		#region ServerLocalData
		internal static int ServerID { get; } = 1;
		#endregion

		#region Max Allowed Session & Player Count
		internal static uint MaxSessionPoolCount { get; } = 8;
		internal static uint MaxPlayerPoolCount { get; } = 8;

		internal static int MinSessionCount { get; } = 0;
		internal static int MaxSessionCount { get; } = 8;
		internal static int MinPlayerCount { get; } = 0;
		internal static int MaxPlayerCount { get; } = 8;
		#endregion

		#region Tick
		internal static float OneSec { get; } = 1f;
		internal static uint OneSecPerTickCount { get; } = 64;
		#endregion

		#region Buffer
		internal static int BUFFERSIZE { get; } = 128;
		#endregion

		#region MS
		internal static double OneSecMS { get; } = 1000.0; // 1 Sec
		internal static double MaxAllowedDebtMS { get; } = -100.0;
		#endregion

		#region One Tick Received Packet Process 
		internal static int MaxPacketProcess { get; } = 500;
		#endregion

		#region Packet
		/// <summary>
		/// 6(HeaderSize)byte = 2(Ushort)byte + 4(EPacketID-int)byte
		/// </summary>
		internal static int PacketHeaderSize { get; } = 6;
		internal static int ReceivedMaxPacketSize { get; } = 4096;
		#endregion

		#region Packet ID Range
		internal static int SystemPacketIDMinRange { get; } = 1;
		internal static int SystemPacketIDMaxRange { get; } = 100;
		internal static int GamePacketIDMinRange { get; } = 101;
		internal static int GamePacketIDMaxRange { get; } = 200;
		#endregion

		#region SessionLifeTimer
		internal static double HeartBeatTimeOverMS { get; } = 15000.0; // 15 Sec
		#endregion





	}
}

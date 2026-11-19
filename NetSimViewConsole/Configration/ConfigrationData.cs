namespace NETSIM_ConsoleView.Configration
{
	/// <summary> 
	/// ConfigrationData <br/>
	/// ConsoleView Layout "ServerLayout" <br/>
	/// ConsoleView Layout "SessionLayout" <br/>
	/// </summary>
	internal class ConfigrationData
	{
		internal static readonly string ServerLayoutText = "ServerLayout";
		internal static readonly string SessionLayoutText = "SessionLayout";
	}

	internal class Slot
	{
		internal int ID { get; private set; }
		internal bool IsEmpty { get; private set; }


		private Slot()
		{
			ID = -1;
			IsEmpty = true;
		}
		private Slot(int id)
		{
			ID = id;
			IsEmpty = false;
		}

		internal static Slot CreateEmpty() => new Slot();
		internal static Slot Assign(int id) => new Slot(id);



	}


}

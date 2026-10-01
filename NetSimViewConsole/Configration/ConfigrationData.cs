using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM_ConsoleView.Configration
{
	internal class ConfigrationData 
	{ 
		//internal static readonly string ViewLayoutText = "ViewLayout";
		internal static readonly string ServerLayoutText = "ServerLayout";
		internal static readonly string SessionLayoutText = "SessionLayout";
	}








	internal class Slot
	{
		internal int ID { get;  private set; }
		internal bool IsEmpty { get; private set; }


		private Slot()
		{
			this.ID = -1;
			this.IsEmpty = true;
		}
		private Slot(int id)
		{
			this.ID = id;
			this.IsEmpty = false;
		}

		internal static Slot CreateEmpty() => new Slot();
		internal static Slot Assign(int id) => new Slot(id);
		


	}


}

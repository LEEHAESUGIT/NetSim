using NETSIM_ConsoleView.Configration;
using Shared.Configration;
using Shared.Enum;
using Spectre.Console;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM_ConsoleView.Common.Tool
{
	internal class ViewTool
	{
		internal static string StateLightof(EStateLight state)
		{
			string greenLight = "[green] ● [/]";
			string yellowLight = "[yellow] ● [/]";
			string redLight = "[red] ● [/]";
			string greyLight = "[grey] ● [/]";

			if (state == EStateLight.GREEN)
				return greenLight;
			if (state == EStateLight.YELLOW)
				return yellowLight;
			if (state == EStateLight.RED)
				return redLight;

			return greyLight;
		}
	}
}

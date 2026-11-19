using Shared.Enum;

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

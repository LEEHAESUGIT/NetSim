using NETSIM_ConsoleView.Common.Tool;
using NETSIM_ConsoleView.Configration;
using Shared.DTO;
using Shared.Enum;
using Spectre.Console;

namespace NETSIM_ConsoleView.Draw
{
	/// <summary>
	/// DrawTool 기능 : <br/>
	/// Server Table 생성 <br/>
	/// Server Panel 생성 <br/>
	/// Server Grid 생성 <br/>
	/// Session Table 생성 <br/>
	/// Session Panel 생성 <br/>
	/// Session Grid 생성 <br/>
	/// 빈 패널 생성 <br/>
	/// 투명 패널 생성 <br/>
	/// </summary>
	internal class DrawTool
	{
		internal static Layout SetLayout()
		{
			Layout layout = new Layout();
			layout.SplitRows(
				new Layout(ConfigrationData.ServerLayoutText),
				new Layout(ConfigrationData.SessionLayoutText)
				);
			return layout;
		}

		internal static Panel SetServerPanel(ServerStateDTO dto)
		{
			Panel panel;

			if (dto.StateLight == EStateLight.NONE)
				panel = EmptyPanel();
			else
			{
				Table table = CreateServerTable(dto);
				panel = new Panel(table)
					.Header(ViewTool.StateLightof(dto.StateLight));
			}
			return panel;
		}

		internal static Panel SetSessionPanel(SessionStateDTO dto)
		{
			Panel panel;

			if (dto.StateLight == EStateLight.NONE)
				panel = EmptyPanel();
			else
			{
				Table table = CreateSessionTable(dto);
				panel = new Panel(table)
					.Header(ViewTool.StateLightof(dto.StateLight));
			}
			return panel;
		}

		internal static Grid SetServerGrid(Panel serverPanel)
		{
			Grid grid = new Grid();
			for (int i = 0; i < 4; i++)
				grid.AddColumn(new GridColumn().PadLeft(1).PadRight(1));

			grid.AddRow(
				serverPanel,
				InvisiblePanel(),
				InvisiblePanel(),
				InvisiblePanel()
				);
			return grid;
		}

		internal static Grid SetSessionGrid(IReadOnlyDictionary<ESlotConfig, Panel> sessionPanels)
		{
			Grid grid = new Grid();
			for (int i = 0; i < 4; i++)
				grid.AddColumn(new GridColumn().PadLeft(1).PadRight(1));

			grid.AddRow(
				sessionPanels[ESlotConfig.SLOT1],
				sessionPanels[ESlotConfig.SLOT2],
				sessionPanels[ESlotConfig.SLOT3],
				sessionPanels[ESlotConfig.SLOT4]
				);
			grid.AddRow(
				sessionPanels[ESlotConfig.SLOT5],
				sessionPanels[ESlotConfig.SLOT6],
				sessionPanels[ESlotConfig.SLOT7],
				sessionPanels[ESlotConfig.SLOT8]
			);

			return grid;
		}

		private static Table CreateServerTable(ServerStateDTO dto)
		{
			Table table = new();

			table.AddColumn($"{dto.Name}")
				.AddRow($"TPS : {dto.TPS}")
				.AddRow($"Uptime : {dto.Uptime.ToString(@"dd\.hh\:mm\:ss")}")
				.AddRow($"SessionEntity : {dto.SessionEntityCount}")
				.AddRow($"Recv : {dto.RecvPerSec} B/s")
				.AddRow($"Send : {dto.SendPerSec} B/s");

			return table;
		}

		private static Table CreateSessionTable(SessionStateDTO dto)
		{
			Table table = new();

			table.AddColumn($"{dto.Name}")
				.AddRow($"SessioniD : {dto.SessionID}")
				.AddRow($"PlayerID : {dto.PlayerID}")
				.AddRow($"PlayerColor : {dto.PlayerColor}")
				.AddRow($"ping : {dto.Ping}")
				.AddRow($"Uptime : {dto.Uptime.ToString(@"dd\.hh\:mm\:ss")}")
				.AddRow($"SystemCommand : {dto.SystemCommand}")
				.AddRow($"GameCommand : {dto.GameCommand}");

			return table;
		}

		internal static Panel EmptyPanel()
		{
			Markup emptyMarkeUp = new Markup("\n\n[grey]Empty[/]\n\n");
			Align alignedEmpty = Align.Center(emptyMarkeUp, VerticalAlignment.Middle);
			Padder padder = new Padder(alignedEmpty, new Padding(0, 3));

			return new Panel(padder).Header(ViewTool.StateLightof(EStateLight.NONE));
		}
		private static Panel InvisiblePanel()
		{
			Markup emptyMarkeUp = new Markup("");
			Align alignedEmpty = Align.Center(emptyMarkeUp, VerticalAlignment.Middle);
			Padder padder = new Padder(alignedEmpty, new Padding(0, 3));

			return new Panel(padder).Border(BoxBorder.None);
		}


	}
}

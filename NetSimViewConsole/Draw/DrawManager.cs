using NETSIM_ConsoleView;
using NETSIM_ConsoleView.Common.Tool;
using NETSIM_ConsoleView.Configration;
using Shared.DTO;
using Shared.Enum;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM_ConsoleView.Draw
{
	internal class DrawManager
	{
		// Local
		//private bool _isDrawAccept = false;
		private bool _isRefresh = false;

		private static Panel _serverPanel = DrawTool.emptyPanel();
		private static ConcurrentDictionary<ESlotConfig, Panel> _sessionPanels = new();



		private SlotController _slotController = new SlotController();


		internal DrawManager(ref bool isRefresh)
		{
			this._isRefresh = isRefresh;
		}


		[ModuleInitializer]
		internal static void AutoPanelAsign()
		{
			_serverPanel = DrawTool.emptyPanel();
			_sessionPanels.TryAdd(ESlotConfig.SLOT1, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT2, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT3, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT4, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT5, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT6, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT7, DrawTool.emptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT8, DrawTool.emptyPanel());
		}

		#region Draw
		internal void Live()
		{
			AnsiConsole.Live(Draw())
				.AutoClear(false)
				.Start(ctx =>
				{
					while (true)
					{
						if (this._isRefresh)
						{
							ctx.UpdateTarget(Draw());
							ctx.Refresh();
							//_isDrawAccept = false;
						}
						Thread.Sleep(1000);
					}
				});
		}
		private Layout Draw()
		{
			Grid serverGrid = DrawTool.SetServerGrid(_serverPanel);
			Grid sessionGrid = DrawTool.SetSessionGrid(_sessionPanels);
			Layout viewLayout = DrawTool.SetLayout();
			viewLayout[ConfigrationData.ServerLayoutText].Update(serverGrid).Size(13);
			viewLayout[ConfigrationData.SessionLayoutText].Update(sessionGrid).Size(30);

			return viewLayout;
		}
		#endregion


		#region StateControl
		internal void ConversionServerState(ServerStateDTO dto)
		{
			if (dto == default)
				return;

			DrawManager._serverPanel = DrawTool.SetServerPanel(dto);
		}
		internal void ConversionSessionStates(ReadOnlySpan<SessionStateDTO> sessionStateDTOs)
		{
			if (sessionStateDTOs == null || sessionStateDTOs.Length <= 0)
				return;

			//_slotController.TryAllClearSlot();
			for (int i = 0; i < _slotController.Slots.Count; i++)
			{
				if (_slotController.Slots[(ESlotConfig)i].IsEmpty == false)
				{
					_sessionPanels.TryUpdate((ESlotConfig)i, DrawTool.emptyPanel(), _sessionPanels[(ESlotConfig)i]);
				}
			}
			for (int i = 0; i < sessionStateDTOs.Length; i++)
			{
				if (_slotController.TryOccupySlot(sessionStateDTOs[i].SessionID, out ESlotConfig occupySlot))
				{
					Panel panel = DrawTool.SetSessionPanel(sessionStateDTOs[i] );
					_sessionPanels.TryUpdate(occupySlot, panel, _sessionPanels[occupySlot]);
				}
				else if (_slotController.TryVacateSlot(sessionStateDTOs[i].SessionID))
				{

				}
				//Panel panel = DrawTool.SetSessionPanel(sessionStateDTOs[i]);
				//_sessionPanels.TryUpdate(occupySlot, panel, _sessionPanels[occupySlot]);

			}
			// 만약 슬롯의 갯수가 부족 하던가 많다하면 초기화 시킨다.
			if (_sessionPanels.Count != 8) DrawManager.AutoPanelAsign();


			

		}
		//internal void ConversionSessionStates(DTOWrappers<SessionStateDTO> dtos)
		//{
		//	if (dtos == null || dtos.Items.Length <= 0)
		//		return;

		//	for(int i = 0; i < dtos.Items.Length ; i++)
		//	{
		//		ESlotConfig slot = ViewTool.TryOccupySlot(dtos.Items[i].ID);

		//		Panel panel = DrawTool.SetSessionPanel(dtos.Items[i]);

		//		var targetPanel = _sessionPanels[slot];
		//		_sessionPanels.TryUpdate(slot, panel, targetPanel);
		//	}
		//}


		//internal void DTOConversionForDraw(ServerStateDTO serverStateDTO , DTOWrappers<SessionStateDTO> sessionStateDTOs)
		//{
		//	DrawManager._serverPanel = DrawTool.SetServerPanel(serverStateDTO);
		//	AssembleSessionPanels(sessionStateDTOs);
		//}


		//private void AssembleSessionPanels(DTOWrappers<SessionStateDTO> dtos)
		//{
		//	if (dtos == null || dtos.Items.Length <= 0)
		//		return;

		//	ESlotConfig slotConfig;

		//	for (int i = 0; i < dtos.Items.Length; i++)
		//	{
		//		slotConfig = (ESlotConfig)i;

		//		Panel panel = DrawTool.SetSessionPanel(dtos.Items[i]);

		//		var targetPanel = _sessionPanels[slotConfig];
		//		_sessionPanels.TryUpdate(slotConfig, panel, targetPanel);
		//	}
		//}
		#endregion

























		//internal void InitDraw()
		//{
		//	Layout layout = new Layout();
		//	layout.SplitRows(
		//		new Layout("ServerLayout"),
		//		new Layout("SessionLayout")
		//		);


		//	ServerStateDTO serverStateInitDTO =
		//		new ServerStateDTO
		//		{
		//			Name = "NETSIM_Server",
		//			TPS = 0,
		//			Uptime = TimeSpan.MinValue,
		//			SessionEntityCount = 0,
		//			RecvPerSec = 0,
		//			SendPerSec = 0,
		//			Connect = EStateLight.NONE
		//		};
		//	SessionStateDTO sessionStateDTO =
		//		new SessionStateDTO
		//		{
		//			Name = "NETSIM_Session",
		//			Ping = -1,
		//			ID = -1,
		//			LastCommand = "EMPTY",
		//			Connect = EStateLight.NONE
		//		};

		//	Table serverTable = CreateTableforServerof(serverStateInitDTO);
		//	Panel serverPanel = CreatePanelforServerof(serverStateInitDTO, serverTable);

		//	SessionStateDTO[] temp = new SessionStateDTO[8] ;

		//	for (int i = 0; i < 8; i++)
		//	{
		//		temp[i] = sessionStateDTO;
		//	}

		//	DTOWrappers<SessionStateDTO> dTOWrappers = new DTOWrappers<SessionStateDTO>(-1, -1, temp);

		//	AssembleSessionPanels(dTOWrappers);


		//	layout["ServerLayout"].Update(serverPanel).Size(10);
		//	//layout["ServerLayout"].Update(grid).Size(10);
		//	layout["SessionLayout"].Update(SessionPanelSetGrid()).Size(22);

		//	this._isDrawAccept = true;
		//	DrawManager._layoutData = layout;

		//}

		//internal void SetDraw(ServerStateDTO serverDTO, DTOWrappers<SessionStateDTO> sessionDTOs)
		//{

		//	Table serverTable;
		//	Panel serverPanel;

		//	if (serverDTO.Connect == EStateLight.NONE)
		//		serverPanel = serverEmptyPanel();
		//	else
		//	{
		//		serverTable = CreateTableforServerof(serverDTO);
		//		serverPanel = CreatePanelforServerof(serverDTO, serverTable);

		//	}















		//		Layout layout = new Layout();
		//	layout.SplitRows(
		//		new Layout("ServerLayout"),
		//		new Layout("SessionLayout"),
		//		new Layout("ErrorLayout")
		//		);

		//	Table serverTable = CreateTableforServerof(serverDTO);
		//	Panel serverPanel = CreatePanelforServerof(serverDTO, serverTable);

		//	//AssembleSessionPanels(sessionDTOs);
		//	layout["ServerLayout"].Update(serverPanel).Size(12);
		//	layout["SessionLayout"].Update(SessionPanelSetGrid()).Size(12);
		//	this._isDrawAccept = true;
		//	DrawManager._layoutData = layout;

		//	//return layout;
		//}








		//// Create Table
		//private Table CreateTableforServerof(ServerStateDTO? dto)
		//{
		//	//if (dto == null)
		//	//	return table;

		//	Table table = new Table();
		//	table.AddColumn($"Server")
		//		.AddRow($"TPS : 0")
		//		.AddRow($"Uptime : 0")
		//		.AddRow($"SessionEntity : 0")
		//		.AddRow($"Recv/s : 0")
		//		.AddRow($"Send/s : 0");

		//	//table.AddColumn($"{dto.Name}")
		//	//	.AddRow($"TPS : {dto.TPS}")
		//	//	.AddRow($"Uptime : {dto.Uptime.ToString(@"dd\.hh\:mm\:ss")}")
		//	//	.AddRow($"SessionEntity : {dto.SessionEntityCount}")
		//	//	.AddRow($"Recv/s : {dto.RecvPerSec}")
		//	//	.AddRow($"Send/s : {dto.SendPerSec}");
		//	return table;
		//}
		//private Table CreateTableforSessionof(SessionStateDTO dto)
		//{
		//	Table table = new Table();
		//	//if (dto == null)
		//	//	return table;

		//	table.AddColumn($"0")
		//		.AddRow($"ping : 0")
		//		.AddRow($"EntityID : 0")
		//		.AddRow($"LastCommand : 0");

		//	//table.AddColumn($"{dto.Name}")
		//	//	.AddRow($"ping : {dto.Ping}")
		//	//	.AddRow($"EntityID : {dto.ID}")
		//	//	.AddRow($"LastCommand : {dto.LastCommand}");

		//	//slotController.AssignSlot(sessionState.ID);
		//	return table;
		//}

		//// Create Panel
		//private Panel CreatePanelforServerof(ServerStateDTO dto, Table table)
		//{
		//	Panel panel;

		//	if (dto.Connect == EStateLight.NONE)
		//		panel = serverEmptyPanel();
		//	else
		//		panel = new Panel(table)
		//			.Header(ViewTool.StateLightof(dto.Connect));

		//	return panel;
		//}
		//private Panel CreatePanelforSessionof(SessionStateDTO dto, Table table)
		//{
		//	Panel panel;

		//	if (dto.Connect == EStateLight.NONE)
		//		panel = sessionEmptyPanel();
		//	else
		//		panel = new Panel(table)
		//			.Header(ViewTool.StateLightof(dto.Connect));

		//	return panel;
		//}

		//private Panel serverEmptyPanel()
		//{
		//	Panel panel;
		//	Markup emptyMarkeUp = new Markup("\n\n[grey]Empty[/]\n\n");
		//	Align alignedEmpty = Align.Center(emptyMarkeUp, VerticalAlignment.Middle);
		//	Padder padder = new Padder(alignedEmpty, new Padding(9,1,9,1));
		//	panel = new Panel(padder).Header(ViewTool.StateLightof(EStateLight.NONE));

		//	return panel;
		//}
		//private Panel sessionEmptyPanel()
		//{
		//	Panel panel;
		//	Markup emptyMarkeUp = new Markup("\n\n[grey]Empty[/]\n\n");
		//	Align alignedEmpty = Align.Center(emptyMarkeUp, VerticalAlignment.Middle);
		//	Padder padder = new Padder(alignedEmpty , new Padding(9,2,9,2));
		//	panel = new Panel(padder).Header(ViewTool.StateLightof(EStateLight.NONE));

		//	return panel;
		//}



		//private Grid serverPanelSetGrid(Panel serverPanel)
		//{
		//	Grid grid = new Grid();
		//	for(int i = 0; i < 4 ; i++)
		//		grid.AddColumn();

		//	grid.AddRow(
		//		serverPanel,
		//		new Panel(""),
		//		new Panel(""),
		//		new Panel("")
		//		);

		//	return grid;
		//}


		//private Grid SessionPanelSetGrid()
		//{
		//	Grid grid = new Grid();
		//	for (int i = 0; i < 4; i++)
		//		grid.AddColumn();


		//	grid.AddRow(
		//		_sessionPanels[ESlotConfig.SLOT1],
		//		_sessionPanels[ESlotConfig.SLOT2],
		//		_sessionPanels[ESlotConfig.SLOT3],
		//		_sessionPanels[ESlotConfig.SLOT4]
		//		);
		//	grid.AddRow(
		//		_sessionPanels[ESlotConfig.SLOT5],
		//		_sessionPanels[ESlotConfig.SLOT6],
		//		_sessionPanels[ESlotConfig.SLOT7],
		//		_sessionPanels[ESlotConfig.SLOT8]
		//	);

		//	return grid;
		//}



	}
}

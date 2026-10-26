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
				if (_slotController.TryVacateSlot(sessionStateDTOs[i].SessionID))
				{}
				if (_slotController.TryOccupySlot(sessionStateDTOs[i].SessionID, out ESlotConfig occupySlot))
				{
					Panel panel = DrawTool.SetSessionPanel(sessionStateDTOs[i] );
					_sessionPanels.TryUpdate(occupySlot, panel, _sessionPanels[occupySlot]);
				}
			}
			// 만약 슬롯의 갯수가 부족 하던가 많다하면 초기화 시킨다.
			if (_sessionPanels.Count != 8) DrawManager.AutoPanelAsign();

		}
		#endregion
	}
}

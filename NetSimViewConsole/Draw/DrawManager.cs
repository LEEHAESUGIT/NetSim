using NETSIM_ConsoleView.Common.Tool;
using NETSIM_ConsoleView.Configration;
using Shared.DTO;
using Shared.Enum;
using Spectre.Console;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace NETSIM_ConsoleView.Draw
{
	/// <summary>
	/// DrawManager 기능 : <br/>
	/// 콘솔 갱신 <br/>
	/// 콘솔 그리기 <br/>
	/// DTO 변환 <br/>
	/// </summary>
	internal class DrawManager
	{
		private bool _isRefresh = false;

		private static Panel _serverPanel = DrawTool.EmptyPanel();
		private static ConcurrentDictionary<ESlotConfig, Panel> _sessionPanels = new();

		private SlotController _slotController = new SlotController();


		internal DrawManager(ref bool isRefresh)
		{
			_isRefresh = isRefresh;
		}

#pragma warning disable CA2255 // 'ModuleInitializer' 특성은 라이브러리에서 사용하면 안 됩니다.
		[ModuleInitializer]
#pragma warning restore CA2255 // 'ModuleInitializer' 특성은 라이브러리에서 사용하면 안 됩니다.
		internal static void AutoPanelAsign()
		{
			_serverPanel = DrawTool.EmptyPanel();
			_sessionPanels.TryAdd(ESlotConfig.SLOT1, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT2, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT3, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT4, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT5, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT6, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT7, DrawTool.EmptyPanel());
			_sessionPanels.TryAdd(ESlotConfig.SLOT8, DrawTool.EmptyPanel());
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
						if (_isRefresh)
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
					_sessionPanels.TryUpdate((ESlotConfig)i, DrawTool.EmptyPanel(), _sessionPanels[(ESlotConfig)i]);
				}
			}
			for (int i = 0; i < sessionStateDTOs.Length; i++)
			{
				if (_slotController.TryDeallocateSlot(sessionStateDTOs[i].SessionID))
				{ }
				if (_slotController.TryAllocateSlot(sessionStateDTOs[i].SessionID, out ESlotConfig occupySlot))
				{
					Panel panel = DrawTool.SetSessionPanel(sessionStateDTOs[i]);
					_sessionPanels.TryUpdate(occupySlot, panel, _sessionPanels[occupySlot]);
				}
			}
			// 만약 슬롯의 갯수가 부족 하던가 많다하면 초기화 시킨다.
			if (_sessionPanels.Count != 8) DrawManager.AutoPanelAsign();
		}
		#endregion
	}
}

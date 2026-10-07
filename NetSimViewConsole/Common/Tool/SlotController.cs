using NETSIM_ConsoleView.Configration;
using Shared.Enum;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM_ConsoleView.Common.Tool
{
	internal class SlotController
	{
		internal Dictionary<ESlotConfig, Slot> Slots = new Dictionary<ESlotConfig, Slot>();

		internal SlotController()
		{
			for (int i = 0; i < 8; i++)
			{
				Slots.Add((ESlotConfig)i, Slot.CreateEmpty());
			}
		}

		internal bool TryOccupySlot(int sessionID, out ESlotConfig slot)
		{
			if (sessionID == -1)
			{
				slot = ESlotConfig.NONE;
				return false;
			}
			try
			{
				if (!IsExist(sessionID))
				{
					ESlotConfig targetSlotKey = SearchEmptySlotKey();
					if (targetSlotKey == ESlotConfig.NONE)
					{
						slot = ESlotConfig.NONE;
						return false;
					}
					if (FillSlot(targetSlotKey, Slot.Assign(sessionID)))
					{
						slot = targetSlotKey;
						return true;
					}
					slot = ESlotConfig.NONE;
					return false;
				}
				slot = SearchOccupySlotKey(sessionID);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				slot = ESlotConfig.NONE;
				return false;
			}
		}
		internal bool TryVacateSlot(int sessionID)
		{
			try
			{
				ESlotConfig occupySlotKey = SearchOccupySlotKey(sessionID);

				if (occupySlotKey == ESlotConfig.NONE)
				{
					return false;
				}

				//if (IsEmptySlot(occupySlotKey))
				//{
				//	ClearSlot(occupySlotKey);
				//	return false;
				//}

				ClearSlot(occupySlotKey);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				return false;
			}
		}

		internal bool TryAllClearSlot()
		{
			try
			{
				Slots.Clear();
				for (int i = 0; i < 8; i++)
				{
					Slots.Add((ESlotConfig)i, Slot.CreateEmpty());
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				return false;
			}
		}



		// 중복 확인
		// 이미 존재하다면 key 반환,
		// 존재 하지 않다면 진행.
		private bool IsExist(int sessionID)
		{
			foreach (var slot in Slots.Values)
			{
				if (slot.ID == sessionID)
					return true;
			}
			return false;
		}
		// 슬롯의 낮은 키 부터 순차적으로 확인해 빈 슬롯 찾기
		private ESlotConfig SearchEmptySlotKey()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[(ESlotConfig)i].IsEmpty)
					return (ESlotConfig)i;
			}
			return ESlotConfig.NONE;
		}

		// ID를 가지고 현재 점유하고 있는 슬롯 찾기
		private ESlotConfig SearchOccupySlotKey(int sessionID)
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (Slots[(ESlotConfig)i].ID == sessionID)
					return (ESlotConfig)i;
			}
			return ESlotConfig.NONE;
		}

		// 슬롯 채우기
		private bool FillSlot(ESlotConfig slotKey, Slot fillEntity)
		{

			Slots[slotKey] = fillEntity;
			return true;

			//if (Slots.TryGetValue(slotKey, out Slot? slot))
			//{
			//	slot = fillEntity;
			//	return true;
			//}
			//throw new ArgumentOutOfRangeException(nameof(slotKey), $"Invalid slot key requested: {slotKey}");

		}
		// 특정 슬롯이 비었는지 확인
		internal bool IsEmptySlot(ESlotConfig slotKey)
		{
			if (Slots.TryGetValue(slotKey, out Slot? slot))
				return slot.IsEmpty;

			throw new ArgumentOutOfRangeException(nameof(slotKey), $"Invalid slot key requested: {slotKey}");
		}
		// 특정 슬롯 비우기
		private void ClearSlot(ESlotConfig slotKey) => Slots[slotKey] = Slot.CreateEmpty();







	}
}

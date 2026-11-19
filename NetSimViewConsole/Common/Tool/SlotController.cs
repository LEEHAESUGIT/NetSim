using NETSIM_ConsoleView.Configration;
using Shared.Enum;

namespace NETSIM_ConsoleView.Common.Tool
{
	/// <summary> 
	/// SlotController 기능 :  <br/>
	/// 중복검사 <br/> 
	/// 슬롯할당 <br/> 
	/// 슬롯비우기 <br/> 
	/// 슬롯찾기 <br/> 
	/// </summary>
	internal class SlotController
	{
		private const int MAXSLOTCOUNT = 8;
		internal Dictionary<ESlotConfig, Slot> Slots = new Dictionary<ESlotConfig, Slot>(MAXSLOTCOUNT);

		internal SlotController()
		{
			for (int i = 0; i < MAXSLOTCOUNT; i++)
			{
				Slots.Add((ESlotConfig)i, Slot.CreateEmpty());
			}
		}
		#region
		internal bool TryAllocateSlot(int sessionID, out ESlotConfig slot)
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
					ESlotConfig targetSlotKey = FindEmptySlotSequentiallyASC();
					if (targetSlotKey == ESlotConfig.NONE)
					{
						slot = ESlotConfig.NONE;
						return false;
					}
					if (AllocateSlot(targetSlotKey, Slot.Assign(sessionID)))
					{
						slot = targetSlotKey;
						return true;
					}
					slot = ESlotConfig.NONE;
					return false;
				}
				slot = FindAllocatedSlotOf(sessionID);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				slot = ESlotConfig.NONE;
				return false;
			}
		}
		internal bool TryDeallocateSlot(int sessionID)
		{
			try
			{
				ESlotConfig occupySlotKey = FindAllocatedSlotOf(sessionID);

				if (occupySlotKey == ESlotConfig.NONE)
				{
					return false;
				}

				DeallocateSlot(occupySlotKey);
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
				for (int i = 0; i < MAXSLOTCOUNT; i++)
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
		#endregion
		#region
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
		private ESlotConfig FindEmptySlotSequentiallyASC()
		{
			for (int i = 0; i < MAXSLOTCOUNT; i++)
			{
				if (Slots[(ESlotConfig)i].IsEmpty)
					return (ESlotConfig)i;
			}
			return ESlotConfig.NONE;
		}

		// ID를 가지고 현재 점유하고 있는 슬롯 찾기
		private ESlotConfig FindAllocatedSlotOf(int sessionID)
		{
			for (int i = 0; i < MAXSLOTCOUNT; i++)
			{
				if (Slots[(ESlotConfig)i].ID == sessionID)
					return (ESlotConfig)i;
			}
			return ESlotConfig.NONE;
		}

		// 특정 슬롯이 비었는지 확인
		internal bool IsEmpty(ESlotConfig slotKey)
		{
			if (Slots.TryGetValue(slotKey, out Slot? slot))
				return slot.IsEmpty;

			throw new ArgumentOutOfRangeException(nameof(slotKey), $"Invalid slot key requested: {slotKey}");
		}
		// 슬롯 채우기
		private bool AllocateSlot(ESlotConfig slotKey, Slot fillEntity)
		{
			Slots[slotKey] = fillEntity;
			return true;
		}
		// 특정 슬롯 비우기
		private void DeallocateSlot(ESlotConfig slotKey) => Slots[slotKey] = Slot.CreateEmpty();
		#endregion
	}
}

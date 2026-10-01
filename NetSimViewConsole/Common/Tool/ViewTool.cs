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

		//#region SlotController
		//internal static ConcurrentDictionary<ESlotConfig , Slot> SessionSlot = new();
			
		//[ModuleInitializer]
		//internal static void AutoSlotInit()
		//{
		//	for(int i = 0 ; i < ServerConfigurationData.MaxConnectedSessionCount; i++)
		//		SessionSlot.TryAdd((ESlotConfig)i,Slot.CreateEmpty());
		//}
		//internal static ESlotConfig TryOccupySlot(int sessionID)
		//{
		//	// 순서
		//	// 세션스테이트 중복검사
		//	// 검사결과에 따라 다음 행동이 정해짐.
		//	//	true : 기존의 딕셔너리 내부에 같은 아이디가 저장된 슬롯업데이트
		//	//	false : 새로 빈 엔티티를 찾아 할당.

		//	// 결과값
		//	// 세션을 새로운 슬롯에 할당.
		//	// 세션을 기존의 할당되어 있던 기존의 슬롯에 업데이트
		//	// 잘못된 세션
		//	//ESlotConfig tempSlotConfig;

		//	// 중복검사
		//	if (IsExist(sessionID, out ESlotConfig existingSlotConfig))
		//	{
		//		var existingSlot = SessionSlot[existingSlotConfig];
		//		if (SessionSlot.TryUpdate(existingSlotConfig, Slot.Assign(sessionID) , existingSlot))
		//			return existingSlotConfig;
		//	}

		//	// 빈슬롯 검사 & 재검사
		//	int retrySearch = 0;
		//	while(retrySearch < ServerConfigurationData.MaxConnectedSessionCount)
		//	{
		//		ESlotConfig emptySlotConfig = SearchEmptySlot();

		//		if(emptySlotConfig == ESlotConfig.NONE)
		//			return ESlotConfig.NONE;

		//		var emptySlot = SessionSlot[emptySlotConfig];
		//		if (SessionSlot.TryUpdate(emptySlotConfig, Slot.Assign(sessionID), emptySlot))
		//			return emptySlotConfig;

		//		retrySearch++;
		//	}
		//	return ESlotConfig.NONE;
		//}
		//internal static bool IsEmptySlot(int slotKey)
		//{
		//	// 키에 대응하는 슬롯의 빈자리 확인.
		//	// 조건 : 슬롯키는 슬롯의 전체 갯수보다 클수 없고 , 0 보다 작을수 없음
		//	// true : 슬롯 자리가 비어있음
		//	// false : 슬롯 자리가 차있음
		//	if (slotKey >= ServerConfigurationData.MaxConnectedSessionCount || slotKey < 0)
		//		return false;

		//	return SessionSlot[(ESlotConfig)slotKey].IsEmpty;
		//}
		//internal static bool ClearSlot(int sessionID)
		//{
		//	if (IsExist(sessionID, out ESlotConfig slotConfig))
		//	{
		//		var targetSlot = SessionSlot[slotConfig];
		//		if (SessionSlot.TryUpdate(slotConfig, Slot.CreateEmpty(), targetSlot))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//private static ESlotConfig SearchEmptySlot()
		//{
		//	for(int i = 0; i < ServerConfigurationData.MaxConnectedSessionCount; i++)
		//	{
		//		if (SessionSlot[(ESlotConfig)i].IsEmpty)
		//		{
		//			return (ESlotConfig)i;
		//		}
		//	}
		//	return ESlotConfig.NONE;
		//}
		//private static bool IsExist(int sessionID , out ESlotConfig slotConfig)
		//{
		//	// id 를 대상으로 현재 슬롯들을 순회하면서 , 세션id가 이미 할당 되어있는지 확인.
		//	// true : 확인이 된다면 id에 맞는 슬롯 반환
		//	// false : 확인이 되지 않는다면 , None 반환
		//	for(int i = 0 ; i < ServerConfigurationData.MaxConnectedSessionCount; i++)
		//	{
		//		if (SessionSlot[(ESlotConfig)i].ID == sessionID)
		//		{
		//			slotConfig = (ESlotConfig)i;
		//			return true;
		//		}
		//	}
		//	slotConfig = ESlotConfig.NONE;
		//	return false;
		//}
		//#endregion



	}
}

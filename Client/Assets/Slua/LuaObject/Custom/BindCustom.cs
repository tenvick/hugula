using System;
using System.Collections.Generic;
namespace SLua {
	[LuaBinder(3)]
	public class BindCustom {
		public static Action<IntPtr>[] GetBindList() {
			Action<IntPtr>[] list= {
				Lua_CryptographHelper.reg,
				Lua_DESHelper.reg,
				Lua_CHighway.reg,
				Lua_HighwayEventArg.reg,
				Lua_CQueueRequest.reg,
				Lua_CRequest.reg,
				Lua_CTransport.reg,
				Lua_LHighway.reg,
				Lua_LRequest.reg,
				Lua_LNet.reg,
				Lua_Msg.reg,
				Lua_PLua.reg,
				Lua_ReferGameObjects.reg,
				Lua_CUtils.reg,
				Lua_FileHelper.reg,
				Lua_LuaHelper.reg,
				Lua_Session.reg,
				Lua_TcpServer.reg,
				Lua_UdpMasterServer.reg,
				Lua_ActivateMonos.reg,
				Lua_CEventReceive.reg,
				Lua_Localization.reg,
				Lua_NGUIMath.reg,
				Lua_NGUITools.reg,
				Lua_UGUIEvent.reg,
				Lua_UIEventLuaTrigger.reg,
				Lua_UGUIEventSystem.reg,
				Lua_UGUILocalize.reg,
				Lua_UIPanelCamackTable.reg,
				Lua_LeanTweenType.reg,
				Lua_LTDescr.reg,
				Lua_LTRect.reg,
				Lua_LTBezier.reg,
				Lua_LTBezierPath.reg,
				Lua_LTSpline.reg,
				Lua_TweenAction.reg,
				Lua_LeanTween.reg,
				Lua_iTween.reg,
				Lua_Custom.reg,
				Lua_Deleg.reg,
				Lua_foostruct.reg,
				Lua_SLuaTest.reg,
				Lua_System_Collections_Generic_List_1_int.reg,
				Lua_XXList.reg,
				Lua_AbsClass.reg,
				Lua_HelloWorld.reg,
				Lua_System_Collections_Generic_Dictionary_2_int_string.reg,
				Lua_System_String.reg,
			};
			return list;
		}
	}
}

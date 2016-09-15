---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: moban.lua
--data:2016..
--author:
--desc:功能
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local your_name = LuaItemManager:get_item_obejct("your_name")
---------global lua to local------------------
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local Proxy = Proxy --网络代理
local get_value = get_value --多国语言
---------C#----------------


---------local----------

--UI资源
your_name.assets=
{
    -- Asset("youresource.u3d"),
    -- Asset("youresource1.u3d",{"yourItemName"})
}

------------------private-----------------


------------------public------------------


------------------受保护的方法,不要占用------------
-- function your_name:show() end --显示assets
-- function your_name:hide() end --隐藏assets
-- function your_name:dispose() end --消耗当前item的所有assets,销毁之前会调用on_dispose()方法
-- function your_name:on_focus() end --获取焦点时候调用
-- function your_name:on_blur() end --失去焦点时候调用
------------------override ----------------
--即将获取焦点 调用on_focus之前 用于对上一个状态处理
function your_name:on_focusing( previous_state ) 

end

--获取焦点之后 调用on_focus之后，此时资源可能还没加载完成
function your_name:on_focused( previous_state ) 

end

--某一个资源asset加载完毕
function your_name:on_asset_load(key,asset)

end

-- 当前全部资源assets加载完成时候调用方法
function your_name:on_assets_load(items)
	-- local refer = LuaHelper.GetComponent(self.assets[1].root,CSNameSpace.ReferGameObjects) 
	-- local refer1 = LuaHelper.GetComponent(self.assets[2].items["yourItemName"],CSNameSpace.ReferGameObjects)
end

--显示时候调用 
function your_name:on_showed( ... )

end

--当前StateManager:get_current_state()所有原始itemObject显示完毕后调用
function your_name:on_state_showed( ... )
	-- body
end

--失去焦点之前调用
function your_name:on_bluring( new_state ) 

end

--失去焦点之后调用
function your_name:on_blured( new_state ) 
	
end

--每次隐藏时候调用
function your_name:on_hide( ... )
	
end

--返回时候调用
-- function your_name:on_back() 

-- end

---dispose之前调用，用来清理缓存数据和引用
function your_name:on_dispose()
	
end

--点击事件
function your_name:on_click(obj,arg)
	local _cmd =obj.name
	if "btn_name1" == _cmd then

    elseif "btn_name2" == _cmd then

    end
end

--初始化函数只会调用一次
function your_name:initialize()

end


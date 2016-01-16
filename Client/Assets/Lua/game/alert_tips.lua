local alert_tips = LuaItemManager:get_item_obejct("alert_tips")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
alert_tips.priority=99
alert_tips.assets=
{
    Asset("uipublic.u3d",{"Alert"})
}

------------------private-----------------
local alert_refer,tips_label,yes_btn,buy_btn
local show_arg,text

------------------public------------------

function alert_tips:on_assets_load(items)
  -- print("alertTips ................. is loaded")
  alert_refer = LuaHelper.GetComponent(self.assets[1].items["AlertPanel"],"ReferGameObjects") 
  tips_label=alert_refer.monos[0]
  yes_btn = alert_refer.refers[0]
  self:show()
end

--初始化函数只会调用一次
function alert_tips:initialize()
    -- print(self.name.." initialize")
end

--显示提示信息
function alert_tips:show_tips(msg,args)
    text=msg
    show_arg = args
    self:addToState()
end

function alert_tips:on_showed()
    tips_label.text = text
    -- print(" show alertTips lable"..text)
    -- if show_arg then

    -- end
end

--点击事件
function alert_tips:on_click(obj,arg)
    local cmd =obj.name
    if cmd == "BtnSure" then
      self:remove_from_state()
      if show_arg and show_arg.yes then 
        show_arg.yes()
      end
    elseif cmd == "BtnClose" then
      self:remove_from_state()
      if show_arg and show_arg.no then
        show_arg.no()
      end
    end
    show_arg = nil      
    return true
end

-----------------global function --------------------------
function show_tips(msg,args)
  alert_tips:showTips(msg,args)
end
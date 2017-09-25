local alert_tips = LuaItemManager:get_item_object("alert_tips")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
alert_tips.priority=99
alert_tips.assets=
{
    View("uipublic_alert_view",alert_tips)
}

------------------private-----------------
local show_arg,text

------------------public------------------

function alert_tips:get_show_arg()
  return show_arg
end

function alert_tips:get_text()
  return text
end

function alert_tips:set_text(val)
  text = val 
  self:raise_property_changed(self.set_text)
end

--显示提示信息
function alert_tips:show_tips(msg,args)
    self:set_text(msg)
    show_arg = args
    self:addToState()
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
  alert_tips:show_tips(msg,args)
end
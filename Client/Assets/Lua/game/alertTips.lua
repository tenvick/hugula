local alertTips = LuaItemManager:getItemObject("alertTips")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
alertTips.priority=99
alertTips.assets=
{
    Asset("UIPublic.u3d",{"Alert"})
}

------------------private-----------------
local alertRefer,tipsLabel,yesBtn,buyBtn
local showArg,text

------------------public------------------

function alertTips:onAssetsLoad(items)
  -- print("alertTips ................. is loaded")
  alertRefer = LuaHelper.GetComponent(self.assets[1].items["AlertPanel"],"ReferGameObjects") 
  tipsLabel=alertRefer.monos[0]
  yesBtn = alertRefer.refers[0]
  self:show()
end

--初始化函数只会调用一次
function alertTips:initialize()
    -- print(self.name.." initialize")
end

--显示提示信息
function alertTips:showTips(msg,args)
    text=msg
    showArg = args
    self:addToState()
end

function alertTips:onShowed()
    tipsLabel.text = text
    -- print(" show alertTips lable"..text)
    -- if showArg then

    -- end
end

--点击事件
function alertTips:onClick(obj,arg)
    local cmd =obj.name
    if cmd == "BtnSure" then
      self:removeFromState()
      if showArg and showArg.yes then 
        showArg.yes()
      end
    elseif cmd == "BtnClose" then
      self:removeFromState()
      if showArg and showArg.no then
        showArg.no()
      end
    end
    showArg = nil      
    return true
end

-----------------global function --------------------------
function showTips(msg,args)
  alertTips:showTips(msg,args)
end
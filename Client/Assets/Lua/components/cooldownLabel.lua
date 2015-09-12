------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local LuaHelper=LuaHelper
local CUtils=CUtils
local delay = delay
-- local Timer = luanet.Timer
local fun = fun
local CooldownLabel=class(function(self,luaObj)
	self.items={}
	self.luaObj=luaObj
	self.assets = nil
	self.enable = true
end)

function CooldownLabel:cutdown(time)
	local str = "00:00" 
	local ts = 0
	if self.beginTime then
		if self.type == nil then
			ts = time-self.beginTime
		else
			ts  = self.along_time - time
		end
		if ts > 0 then
		  	local sec = string.format("%02.0f", ts % 60)
		  	local hour = string.format("%02.0f", math.modf(ts / 3600))
		  	local min = string.format("%02.0f", math.modf((ts - hour * 3600) / 60))
		   	if self.len == nil then
		   		str = min .. ":" .. sec
		   	else
		   		str = hour .. ":" .. min .. ":" .. sec
		   	end
	  	end	
	end
	if self.label then self.label.text=str end
end

function CooldownLabel:call()
	local time = os.time()
	self:cutdown(time)
	if time>=self.along_time then
  		self.enable = false
  		if self.onCooldownFn then self:onCooldownFn() end
  	elseif(self.enable==true) then
  		delay(self.call,1,self)
  	end
end

--暂停
function CooldownLabel:pause()
    self.pause_time = os.time() --记录暂停时间
    self.enable = false
end

--继续
function CooldownLabel:continue()
    self.enable = true
    local dt = os.time() -  self.pause_time
    self.beginTime=self.beginTime+dt
    self.along_time = self.along_time+dt
    self:call()
--    delay(self.call,1,self)
end

----------------------------------------------------------------------------------------------------
--功能：开启计时器
--参数：@total:单位秒,总计时时间
--     @type:计时类型(nil:计数器计时，notnil:倒计时）
--返回：
----------------------------------------------------------------------------------------------------
function CooldownLabel:begin(total,type,len)
	self.type = type
	self.len = len
	self.beginTime = os.time()
	self.along_time = self.beginTime+total
	self.enable = true
	self:call()
--    delay(self.call,1,self)
end

return CooldownLabel
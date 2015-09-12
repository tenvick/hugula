local Input=UnityEngine.Input
local Vector3=UnityEngine.Vector3
local KeyCode=UnityEngine.KeyCode
local Mathf=UnityEngine.Mathf
local Random=UnityEngine.Random
local Quaternion = UnityEngine.Quaternion
local Time=UnityEngine.Time
local LuaHelper=LuaHelper
local RuntimePlatform = UnityEngine.RuntimePlatform
local Application = UnityEngine.Application
local RunTime
local TouchPhase = UnityEngine.TouchPhase

local touchClickThreshold = 0.04
local lastInputTime,laset = 1
local startPos,directionChosen,direction
local beginTime,clickDt=0,0
local startPanel,endPanel,scoreLabel
local state ,lastState= 0,0
local block

local UIBlock = class(function(self,luaObj )
	self.luaObj=luaObj
	self.enable=true
	RunTime =Application.platform
end)
-----------------------------private-------------------------------------

-------------------------------public ------------------------------------
function UIBlock:setState( ste)
	lastState = state
	state = ste
	self:changeState()
end

function UIBlock:getState()
	return state
end

function UIBlock:onAssetsLoad(items)

	print(" UIBlock .. onAssetsLoad...")
	local asserts = self.luaObj.components.assetLoader.assets
	startPanel = asserts.blockroot.items.StartPanel
	endPanel = asserts.blockroot.items.EndPanel
	startPanel:SetActive(true)
	state = 1
	block = self.luaObj.components.block

	local function onItem(i,obj)
		print(obj.name)
		if obj.name == "LabelScore" then
			scoreLabel=LuaHelper.GetComponent(obj,"UnityEngine.UI.Text")
		end
	end
	print(endPanel)
	LuaHelper.ForeachChild(endPanel,onItem)
end

function UIBlock:setScore(score)
	scoreLabel.text = string.format(" you got %s ",score)
end

function UIBlock:onUpdate(time)
	if RunTime==RuntimePlatform.WindowsEditor or RunTime==RuntimePlatform.OSXEditor or 
		RunTime == RuntimePlatform.WindowsPlayer or RunTime == RuntimePlatform.OSXPlayer  then
		local dt = time-lastInputTime
		if (lastInputTime==0) then lastInputTime=time-0.03 end
		if dt>0.15 and Input.anyKey then
			self:onTapClick()
			lastInputTime = time
		end
	else
		if Input.touchCount > 0  then
			local touch = Input.GetTouch(0)
			if touch then
				if touch.phase ==TouchPhase.Began then
					beginTime=time
				elseif	touch.phase == TouchPhase.Moved then
					
				elseif	touch.phase == TouchPhase.Ended then
					clickDt=time - beginTime
					if clickDt>touchClickThreshold then
						self:onTapClick(touch.position)
					end
				end
			end
		end
	end
end

function UIBlock:onTapClick(pos)
		print("you are onclick "..tostring(state))
		if state==1   then state =2 
		elseif state == 3   then 
			self.luaObj.components.block:endGame()
			state = 1
		end

		self:changeState()
end

function UIBlock:changeState( ... )
	if state == 1 then
		endPanel:SetActive(false)
		startPanel:SetActive(true)
	elseif state == 2 then
		endPanel:SetActive(false)
		startPanel:SetActive(false)
		block:begin()
		self.enable = false
	elseif state ==3 then
		startPanel:SetActive(false)
		endPanel:SetActive(true)
		self.enable = true
	end
end

function UIBlock:__tostring()
    return string.format("Block.name = %s ", self.name)
end

return UIBlock
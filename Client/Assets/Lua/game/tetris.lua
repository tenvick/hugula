local tetris = LuaItemManager:getItemObject("tetris")

local LuaObject=LuaObject
local isover=false

local assets=
{
 	Asset("blockroot.u3d")
}

local function ceraterussiaBlock( ... )
	 local bolck = LuaObject("bolck")
	bolck:addComponent("russianBlocks.block")
	bolck:addComponent("russianBlocks.blockManager")
	bolck:addComponent("assetLoader"):load(assets)
	bolck:addComponent("russianBlocks.uiBlock")
	return bolck
end

function tetris:onClick(obj,arg)
	-- print("you are click "..obj.name )
	local cmd =obj.name 
	-- print(self.bolck)
	-- print(self.bolck.enable)
	if self.bolck~=nil and self.bolck.enable then
		if cmd == "Up" then
			self.bolck:move(0)
		elseif cmd == "Down" then
			self.bolck:move(2)
		elseif cmd == "Left" then
			self.bolck:move(3)
		elseif cmd == "Right" then
			self.bolck:move(1)
		end
	end

	local blockManager=self.bolckObj.components.blockManager

	if  cmd == "ReplayBtn" then--isover and (cmd=="NGUIEvent" or  cmd == "Camera" ) then
		if 	blockManager.leave>=1 then
			blockManager:gameStart()
		else
			self:Quit( )
		end
	elseif cmd == "Quite" then
		self:Quit( )
	elseif cmd == "StartPanel" then	
		if 	blockManager.leave>=1 then
			blockManager:gameStart()
		else
			self:Quit( )
		end
	end

end

function tetris:Quit( )
	-- print("Quit")
end

function tetris:gameOver(blockManager)
	print("russia:gameOver")
	isover=true
	luaGC()
end

function tetris:onBlur()
 	if self.bolck~=nil then self.bolck:hide() end
 	self.bolck = nil
end

function tetris:onFocus( ... )
	-- print("onFocus")
	if self.bolckObj==nil then
		self.bolckObj=ceraterussiaBlock()
		self.bolckObj.parent=self
		self.bolck=self.bolckObj.components.block
	else
		self.bolck.components.blockManager:gameStart()
	end
	isover=false
end
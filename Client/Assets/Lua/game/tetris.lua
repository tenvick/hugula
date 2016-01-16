local tetris = LuaItemManager:get_item_obejct("tetris")

local LuaObject=LuaObject
local isover=false

local assets=
{
 	Asset("blockroot.u3d")
}

local function ceraterussia_block( ... )
	 local bolck = LuaObject("bolck")
	bolck:add_component("russian_blocks.block")
	bolck:add_component("russian_blocks.block_manager")
	bolck:add_component("asset_loader"):load(assets)
	bolck:add_component("russian_blocks.ui_block")
	return bolck
end

function tetris:on_click(obj,arg)
	print("you are click "..obj.name )
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

	local block_manager=self.bolckObj.components.block_manager

	if  cmd == "ReplayBtn" then--isover and (cmd=="NGUIEvent" or  cmd == "Camera" ) then
		if 	block_manager.leave>=1 then
			block_manager:gameStart()
		else
			self:quit( )
		end
	elseif cmd == "Quite" then
		self:quit( )
	elseif cmd == "StartPanel" then	
		if 	block_manager.leave>=1 then
			block_manager:gameStart()
		else
			self:quit( )
		end
	end

end

function tetris:quit( )
	-- print("Quit")
end

function tetris:game_over(block_manager)
	print("russia:gameOver")
	isover=true
	luagc()
end

function tetris:on_blur()
 	if self.bolck~=nil then self.bolck:hide() end
 	self.bolck = nil
end

function tetris:on_focus( ... )
	-- print("onFocus")
	if self.bolckObj==nil then
		self.bolckObj=ceraterussia_block()
		self.bolckObj.parent=self
		self.bolck=self.bolckObj.components.block
	else
		self.bolck.components.block_manager:game_start()
	end
	isover=false
end
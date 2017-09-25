local tetris = LuaItemManager:get_item_object("tetris")

local isover=false

tetris.assets=
{
 	Asset("blockroot.u3d")
}

local function ceraterussia_block( ... )
	tetris:add_component("russian_blocks.block")
	tetris:add_component("russian_blocks.block_manager")
	tetris:add_component("russian_blocks.ui_block")
end

ceraterussia_block()
tetris.bolck=tetris.components.block

-- print(tetris.components.block_manager)
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

	local block_manager=self.components.block_manager

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
	print("Quit")
	StateManager:set_current_state(StateManager.welcome)
end

function tetris:game_over(block_manager)
	print("russia:gameOver")
	isover=true
	luagc()
end

function tetris:on_showed()
	-- body
end

function tetris:on_hide( ... )
	-- body
end

------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   NGUIEvent
--  author pu
------------------------------------------------
local UGUIEvent=UGUIEvent --.instance
local Plua = PLua.instance
local StateManager = StateManager
--local InputEvent = {}
local function on_press(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_press",sender,arg)
	end
end

local function on_click(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_click",sender,arg)
	end
end

local function on_drag(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_drag",sender,arg)
	end
end

local function on_drop(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_drop",sender,arg)
	end
end

local function on_customer(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_customer",sender,arg)
	end
end

local function on_select(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_select",sender,arg)
	end
end

local function on_cancel(sender,arg)
	if StateManager:get_input_enable() then
		StateManager:get_current_state():on_event("on_cancel",sender,arg)
	end
end

local function on_app_pause(sender,arg)
	if StateManager._current_game_state then
		StateManager:get_current_state():on_event("on_app_pause",sender,arg)
	end
end

local function on_app_focus(sender,arg)
	if StateManager._current_game_state then
		StateManager:get_current_state():on_event("on_app_focus",sender,arg)
	end
end

UGUIEvent.onSelectFn=on_select
UGUIEvent.onCancelFn=on_cancel
UGUIEvent.onCustomerFn=on_customer
UGUIEvent.onPressFn=on_press
UGUIEvent.onClickFn=on_click
UGUIEvent.onDragFn=on_drag
UGUIEvent.onDropFn=on_drop

Plua.onAppFocusFn = on_app_focus
Plua.onAppPauseFn = on_app_pause --游戏暂停
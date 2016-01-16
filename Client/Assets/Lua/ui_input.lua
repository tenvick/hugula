------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   NGUIEvent
--  author pu
------------------------------------------------
local UGUIEvent=UGUIEvent --.instance
local StateManager = StateManager
--local InputEvent = {}
local function on_press(sender,arg)
	StateManager:get_current_state():on_event("on_press",sender,arg)
end

local function on_click(sender,arg)
	StateManager:get_current_state():on_event("on_click",sender,arg)
end

local function on_drag(sender,arg)
	StateManager:get_current_state():on_event("on_drag",sender,arg)
end

local function on_drop(sender,arg)
	StateManager:get_current_state():on_event("on_drop",sender,arg)
end

local function on_customer(sender,arg)
	StateManager:get_current_state():on_event("on_customer",sender,arg)
end

local function on_select(sender,arg)
	StateManager:get_current_state():on_event("on_select",sender,arg)
end

local function on_cancel(sender,arg)
	StateManager:get_current_state():on_event("on_cancel",sender,arg)
end

UGUIEvent.onSelectFn=on_select
UGUIEvent.onCancelFn=on_cancel
UGUIEvent.onCustomerFn=on_customer
UGUIEvent.onPressFn=on_press
UGUIEvent.onClickFn=on_click
UGUIEvent.onDragFn=on_drag
UGUIEvent.onDropFn=on_drop

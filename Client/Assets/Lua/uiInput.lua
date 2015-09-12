------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   NGUIEvent
--  author pu
------------------------------------------------
local UGUIEvent=UGUIEvent --.instance
local StateManager = StateManager
--local InputEvent = {}
local function onPress(sender,arg)
	StateManager:getCurrentState():onEvent("onPress",sender,arg)
end

local function onClick(sender,arg)
	StateManager:getCurrentState():onEvent("onClick",sender,arg)
end

local function onDrag(sender,arg)
	StateManager:getCurrentState():onEvent("onDrag",sender,arg)
end

local function onDrop(sender,arg)
	StateManager:getCurrentState():onEvent("onDrop",sender,arg)
end

local function onCustomer(sender,arg)
	StateManager:getCurrentState():onEvent("onCustomer",sender,arg)
end

local function onSelect(sender,arg)
	StateManager:getCurrentState():onEvent("onSelect",sender,arg)
end

local function onCancel(sender,arg)
	StateManager:getCurrentState():onEvent("onCancel",sender,arg)
end

UGUIEvent.onSelectFn=onSelect
UGUIEvent.onCancelFn=onCancel
UGUIEvent.onCustomerFn=onCustomer
UGUIEvent.onPressFn=onPress
UGUIEvent.onClickFn=onClick
UGUIEvent.onDragFn=onDrag
UGUIEvent.onDropFn=onDrop

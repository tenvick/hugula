------------------------------------------------
--  Copyright © 2013-2022   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local string_format = string.format
local setmetatable = setmetatable
local mt = {}
mt.__index = function(t, k)
    error(string_format(" %s does't set key (%s) config  ", t.name, k))
end
---
---
--------------------------------------------------------------------------
-------------------------------viewmodel配置-------------------------------
---------------------------------------------------------------------------

--- {vm="view mode path",async=false}
local vm_config = {name = "ViewModelConfig file:  vm_config.lua"}
setmetatable(vm_config, mt)
---
vm_config.welcome = {vm = "viewmodels.welcome"} ---  示例列表
vm_config.bag = {vm = "viewmodels.bag"} --- 背包
vm_config.back_tips = {vm="viewmodels.back_tips"} --以luaModule
vm_config.binding_demo = {vm = "viewmodels.binding_demo.binding_demo"} --- 绑定示例
vm_config.chat_demo = {vm = "viewmodels.chat_demo"} --- 聊天示例

--------------------------------------------------------------------------
-------------------------------viewmodel 结束-------------------------------
--=======================================================================--


--------------------------------------------------------------------------
------------------------viewmodel group 配置-------------------------------
---------------------------------------------------------------------------
local vm_group = {name = "ViewModelGroup  file: vm_config.lua"}
setmetatable(vm_group, mt)
---
vm_group.welcome = {"welcome"}
vm_group.bag = {"bag","back_tips"}
vm_group.binding_demo = {"binding_demo","back_tips"}
vm_group.chat_demo = {"chat_demo","back_tips"}
--------------------------------------------------------------------------
------------------------viewmodel group 结束-------------------------------
--=======================================================================--





--- view model的配置信息
---@class WindowConfig
---@field logo table
---@field scroll_rect_table table
VMConfig = vm_config


--- vm配置组，为了增加模块独立性，使用一组vm模块来切换功能。
---@class VMGroup
---@field welcome table
---@field source_demo table
---@field scroll_rect_table table
VMGroup = vm_group

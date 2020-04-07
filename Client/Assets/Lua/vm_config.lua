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
vm_config.scroll_rect_table = {vm = "viewmodels.scroll_rect_table"} --- 大数据列表
vm_config.source_demo = {vm = "viewmodels.binding_demo.binding_demo"} --- 绑定示例
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
vm_group.scroll_rect_table = {"scroll_rect_table"}
vm_group.source_demo = {"source_demo"}
vm_group.chat_demo = {"chat_demo"}
-- vm_group.tetris = {"tetris"}
-- vm_group.load_extends = {"load_extends"}
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

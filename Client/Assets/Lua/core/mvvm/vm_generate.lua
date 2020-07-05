------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
local rawset = rawset
local rawget = rawget
local setmetatable = setmetatable

local VMConfig = require("vm_config")[1]

---根据 WindowConfig的配置来require view model文件
local function _require_vm(t, k)
    local raw = rawget(t, k)
    if raw == nil then --require
        local conf = VMConfig[k]
        if conf and conf.vm then
            local re = require(conf.vm)
            rawset(t, k, re)
            rawset(re, "name", k) ---设置view model的 name
            return re
        else
            error(string.format("/vm_config.lua does't contains vm_config.%s",k))
        end
    else
        return raw
    end
end

local mt = {}
mt.__index = _require_vm

---
---@class vm_generate
local vm_generate = {}
setmetatable(vm_generate,mt)

--- 保持对所有view models的实例引用 __index回自动require vm_config 对应路径的viewmodel lua文件
---@class VMGenerate
---@field index string
return vm_generate

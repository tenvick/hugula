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
        local vm_name = conf and conf.vm
        if vm_name then

            local re = require(vm_name)
            local initialize = re.initialize
            rawset(t, k, re)
            rawset(re, "_require_name", k) ---设置view model的 name
            if initialize then --调用初始化函数
                initialize(re)
            end
            
            return re
        else
            error(string.format("/vm_config.lua does't contains vm_config.%s", k))
        end
    else
        return raw
    end
end

--@overload fun(self:VMBase,key:string,hold_child:boolean)
local function _reload_vm(t, k, hold_child)
    --@type VMBase
    local conf = VMConfig[k]
    local vm_name = conf and conf.vm
    if vm_name then
        local old = package.loaded[vm_name]
        if old then
            local destructor = old.destructor
            if destructor then
                destructor(old)
            end
        end
        package.loaded[vm_name] = nil
        -- print("reload vm", vm_name)
        local re = require(vm_name) --重新require lua
        local initialize = re.initialize
        rawset(t, k, re)
        rawset(re, "_require_name", k) ---设置view model的 name
        if initialize then --调用初始化函数
            initialize(re)
        end
        return re
    else
        error(string.format("/vm_config.lua does't contains vm_config.%s", k))
    end
end

--@overload fun(self:VMBase,key:string,hold_child:boolean)
local function _release_vm(t, k)
    --@type VMBase
    local raw = rawget(t, k)
    if raw ~= nil then --
        local conf = VMConfig[k]
        local vm_name = conf and conf.vm
        if vm_name then
            local old = package.loaded[vm_name]
            old._need_destructor = true --标记析构
            package.loaded[vm_name] = nil
            require("core.mvvm.vm_manager"):destroy(k) --销毁资源

            rawset(t, k, nil)
            return nil
        else
            error(string.format("/vm_config.lua does't contains vm_config.%s", k))
        end
    end
end

local function _rawget(t, k)
    local raw = rawget(t, k)
    return raw
end

local mt = {}
mt.__index = _require_vm

---
---@class vm_generate
local vm_generate = {}
setmetatable(vm_generate, mt)
vm_generate.reload_vm = _reload_vm
vm_generate.release_vm = _release_vm
vm_generate.rawget = _rawget

VMGenerate = vm_generate --增加全局访问
--- 保持对所有view models的实例引用 __index回自动require vm_config 对应路径的viewmodel lua文件
---@class VMGenerate
---@field index string
return vm_generate

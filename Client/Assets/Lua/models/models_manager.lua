------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  数据模型管理中心
--  author pu
--  date 2020.11.23
------------------------------------------------
local require = require
local rawset = rawset
local rawget = rawget
local setmetatable = setmetatable
local pairs = pairs
local type = type

local check_type = "table"

local models = {}
-- models = _models

---根据 WindowConfig的配置来require view model文件
local function _require_model(t, k)
    local raw = rawget(t, k)
    if raw == nil then --require
        local re = require("models." .. k)
        local initialize = re.initialize
        rawset(t, k, re)
        rawset(re, "name", k) ---设置view model的 name
        if initialize then --调用初始化函数
            initialize(re)
        end
        return re
    else
        return raw
    end
end

-- package.loaded["models." .. k] = nil

local mt = {}
mt.__index = _require_model

setmetatable(models, mt)

function models.destroy()
    --todo
    for k, v in pairs(models) do
        if type(v) == check_type then
            local on_destroy = v.on_destroy
            if on_destroy then
                on_destroy(v)
            end
        end
    end
end

function models.init()
    for k, v in pairs(models) do
        if type(v) == check_type then
            local init = v.on_init
            if init then
                init(v)
            end
        end
    end
end

return models

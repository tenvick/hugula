------------------------------------------------
--  Copyright © 2013-2024   Hugula mvvm framework
--  discription {name}
--  author {author}
--  date {date}
------------------------------------------------
local View = View
local VMState = VMState
local NotifyTable = NotifyTable
local NotifyObject = NotifyObject
--lua
local DIS_TYPE = DIS_TYPE
-- local lua_binding = lua_binding
-- local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
local CS = CS

---@class {name}:VMBase
---@type {name}
local {name} = VMBase()
{name}.views = {
    View({name}, {key = "{name}"}) ---加载prefab
}
--------------------    定义变量    --------------------



--------------------    模板生成 绑定属性    --------------------
{property}

-------------------    模板生成 方法    --------------------
{method}

-------------------    模板生成 事件响应    --------------------
{command}

--------------------    自定义  消息处理    --------------------
{message}

-------------------     自定义 方法   --------------------


--------------------    生命周期    --------------------
function {name}:on_push_arg(arg)

end

--view资源全部加载完成时候调用
-- function {name}:on_assets_load()
--     Logger.Log("{name}:on_assets_load")
-- end

function {name}:on_active()
    Logger.Log("{name}:on_active")
end

function {name}:on_deactive()
    Logger.Log("{name}:on_deactive")
end

-- --在销毁的时候调用此函数
-- function {name}:on_destroy()
--     print("{name}:on_deactive")
-- end

--初始化方法只调用一次
-- function {name}:initialize()
--     -- body
-- end

return {name}

--[[

vm_config.{name} = {vm = "viewmodels.{name}", gc_type = VM_GC_TYPE.ALWAYS} 
vm_group.{name} = {"{name}"}

---]]
------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
local class = class
local table = table
local pairs = pairs
local type = type
local ipairs = ipairs
local table_remove_item = table.remove_item
local table_insert = table.insert
local set_target_context = BindingExpression.set_target_context

---所有视图的基类
---@class view_base
---@overload fun(vm_base:ViewBase)
---@param vm_base ViewBase
---@return ViewBase
local view_base =
    class(
    function(self, ...)
        self._children = {}
    end
)

-- ---
-- ---视图的ui加载完成时候调用
-- ---@overload fun(key:string,container:BindableContainer)
-- ---@param key string
-- ---@param container BindableContainer
-- local function on_asset_load(self,key, container)

-- end

local function set_active(self, enable)
    local children = self._children
    for k, container in ipairs(children) do
        container.gameObject:SetActive(enable)
    end
end

---添加子控件
---@overload fun(bindable_object:BindableObject)
---@param bindable_object BindableObject
local function add_child(self, bindable_object)
    local children = self._children
    table_insert(children, bindable_object)
end

---移除子对象
---@overload fun(bindable_object:BindableObject)
---@return void
local function remove_child(self, bindable_object)
    table_remove_item(self._children, bindable_object)
end

---设置子对象的context
---@overload fun(context:any)
---@param context any
local function set_children_context(self, context)
    local children = self._children
    for k, child in ipairs(children) do
        set_target_context(child, context)
    end
end

---
---@overload fun(key:string,container:BindableContainer)
---@param key string
---@param container BindableContainer
local function dispose(self)
    self._vm_base = nil
end

-- view_base.on_asset_load = on_asset_load
view_base.add_child = add_child
view_base.set_active = set_active
view_base.set_children_context = set_children_context
view_base.dispose = dispose

---
---所有视图的基类，提供资源的配置信息和加载完成的相关处理。
---注意:ViewBase.assets
---配置格式为 assets[prefab_name]=assetbundle_name 。
---当assets[__xxx]=findpath时候调用 GameObject.Find(findpath)来寻找对象。
---@class ViewBase
---@overload fun(vm_base:VMBase):ViewBase
---@param vm_base ViewBase
---@return ViewBase
---@field on_asset_load function
---@field set_child function
---@field _vm_base VMBase
---@field set_context any
---@field set_active function
---@field property_changed function
---@field asset_name string
---@field assetbundle string
---@field find_path string
--- "welcome"
ViewBase = view_base

---视图创建，
---一个View对应一个资源
---@overload fun(vm_base:VMBase,arg:any,view_path:string)
---@param vm_base VMBase
---@param arg any
---@param view_path string
View = function(vm_base, arg, view_path)
    ---@class ViewBase
    local view_inst
    local arg_is_string = type(arg) == "string"
    if arg_is_string then
        view_path = arg
    end
    if view_path == nil then
        view_inst = view_base()
    else
        view_inst = require(view_path) ---返回是当前实例
    end
    view_inst._vm_base = vm_base
    if arg_is_string ~= true then --- string 表示路径
        for k, v in pairs(arg) do
            -- if k == "__find" then
            --     view_inst.find_path = v
            -- else
            --     view_inst.asset_name = k
            --     view_inst.assetbundle = v
            -- end
            view_inst[k] = v
        end
    end
    return view_inst
end

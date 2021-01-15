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
local LuaHelper = CS.Hugula.Utils.LuaHelper
---所有视图的基类
---@class view_base
---@overload fun(vm_base:ViewBase)
---@param vm_base ViewBase
---@return ViewBase
local view_base =
    class(
    function(self, ...)
        self._nitialized = false
    end
)

-- ---
-- ---视图的ui加载完成时候调用
-- ---@overload fun(key:string,container:BindableContainer)
-- ---@param key string
-- ---@param container BindableContainer
-- local function on_asset_load(self,key, container)

-- end

local function is_scene(self)
    local scene_name = self.scene_name
    return scene_name ~= nil
end

local function set_active(self, enable)
    local child = self._child
    if not self:is_scene() and child then
        if enable then
            LuaHelper.Active(child)
        else
            LuaHelper.DelayDeActive(child.gameObject)
        end
    end
    -- Logger.Log(string.format("set_active %s ,scene_name=%s,self._child=%s,self._context=%s",enable,self.scene_name,self._child,self._context));
end

---添加子控件
---@overload fun(bindable_object:BindableObject)
---@param bindable_object BindableObject
local function set_child(self, bindable_object)
    self._child = bindable_object
end

---资源是否准备好
---@overload fun()
---@return  bool
local function has_child(self)
    return self._child ~= nil
end

---view是否有关联的context
---@overload fun()
---@return  bool
local function has_context(self)
    return self._context ~= nil
end

---设置子对象的context
---@overload fun(context:any)
---@param context any
local function set_child_context(self, context)
    local child = self._child
    self._context = context

    if not self:is_scene() then
        set_target_context(child, context)
    end
end

---是否初始化了
---@overload fun()
---@return  bool
local function is_initialized(self)
    return self._nitialized == true and self:has_child()
end

---设置初始化状态
---@overload fun()
---@return  bool
local function initialized(self)
    self._nitialized = true
end

---销毁child
---@overload fun()
local function clear(self)
    --scene
    local scene_name = self.scene_name
    if scene_name ~= nil and self._child then
        LuaHelper.UnloadScene(scene_name)
    else
        local child = self._child
        if child then
            LuaHelper.DelayDestroy(child.gameObject)
        end
    end
    -- Logger.Log(string.format("clear ,scene_name=%s,self._child=%s,self._context=%s",self.scene_name,self._child,self._context));
    self._child = nil
    self._context = nil
    self._nitialized = false
end

---
---@overload fun(key:string,container:BindableContainer)
---@param key string
---@param container BindableContainer
local function dispose(self)
    self._vm_base = nil
    self:clear()
end

local function tostring(self)
    return string.format("asset=%s,is_scene=%s,child=%s ", self.key or self.scene_name, self:is_scene(), self._child)
end

-- view_base.on_asset_load = on_asset_load
view_base.is_scene = is_scene
view_base.set_child = set_child
view_base.set_active = set_active
view_base.has_child = has_child
view_base.is_initialized = is_initialized
view_base.initialized = initialized
view_base.set_child_context = set_child_context
view_base.has_context = has_context
view_base.clear = clear
view_base.dispose = dispose
view_base.__tostring = tostring
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
---@field has_context function 是否设置了context
---@field has_child function 是否有BindableObject
---@field key string
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
            view_inst[k] = v
        end
    end
    return view_inst
end

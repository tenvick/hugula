------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
local string = string
local table = table
local pairs = pairs
local type = type
local ipairs = ipairs
local table_remove_item = table.remove_item
local table_insert = table.insert

local LuaHelper = CS.Hugula.Utils.LuaHelper
---所有视图的基类
---@class view_base
---@overload fun(vm_base:ViewBase)
---@param vm_base ViewBase
---@return ViewBase
local view_base =
    class(
    function(self, ...)
        self._initialized = false
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
    if child == true then

    elseif not self:is_scene() and child then
        if enable then
            LuaHelper.Active(child,self._active_index or 0)
        else
            LuaHelper.DelayDeActive(child)
        end
    end
    -- Logger.Log(string.format("set_active %s ,scene_name=%s,self._child=%s,self._context=%s,_active_index=%s",enable,self.scene_name,self._child,self._context,self._active_index));
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
    local _child = self._child

    if _child == true then  
        return true
    elseif self:is_scene() then  
        return _child ~= nil
    else
        return _child ~= nil and not LuaHelper.IsNull(_child)
    end
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
    -- Logger.Log("set_child_context", child,not self:is_scene(),self._context ~= context ,self._context)
    if child == true then

    elseif not self:is_scene() and self:has_child() and self._context ~= context then
        self._context = context
        child.context = context
    end
end

---是否初始化了
---@overload fun()
---@return  bool
local function is_initialized(self)
    return self._initialized == true and self:has_child()
end

---设置初始化状态
---@overload fun()
---@return  bool
local function initialized(self)
    self._initialized = true
end

---销毁child
---@overload fun()
local function clear(self)
    --scene
    local scene_name = self.scene_name
    local child = self._child
    if child == true then

    elseif child then
        if scene_name ~= nil then
            LuaHelper.UnloadScene(scene_name)
        else
            LuaHelper.DelayDestroy(child)
        end
    end

    local unload = self.unload
    if unload then
        unload(self)
    end
    -- Logger.Log(string.format("clear ,scene_name=%s,self._child=%s,self._context=%s",self.scene_name,self._child,self._context));
    self._child = nil
    self._context = nil
    self._initialized = false
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
    return string.format("asset=%s,is_scene=%s,has_child=%s,context=%s ", self.key or self.scene_name, self:is_scene(), self:has_child(),self._context)
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
---@field set_child fun(bindable_object:BindableObject)
---@field _vm_base VMBase
---@field set_context any
---@field set_active fun(self:ViewBase,enable:boolean)
---@field has_context function 是否设置了context
---@field has_child function 是否有BindableObject
---@field clear function  销毁child
---@field key any 资源key string or function
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
    ---@type ViewBase
    local view_inst
    -- local name = ""
    local arg_is_string = type(arg) == "string"
    if arg_is_string then
        view_path = arg
    end
    if view_path == nil then
        view_inst = view_base()
    else
        view_inst = require(view_path) ---返回是当前实例
    end
    view_inst.name = string.format("%s" , arg["key"] or view_path or "")

    view_inst._vm_base = vm_base
    if arg_is_string ~= true then --- string 表示路径
        for k, v in pairs(arg) do
            -- if type(view_inst[k]) == "function" then ---函数不能被重写
                view_inst[k] = v
            -- end
        end
    end
    return view_inst
end

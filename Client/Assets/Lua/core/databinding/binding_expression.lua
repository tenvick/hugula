------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
require("core.databinding.converter")
local table = table
local type = type
local string_sub = string.sub
local string_trim = string.trim
local string_find = string.find
local string_len = string.len
local string_match = string.match
local string_format = string.format
local loadstring = loadstring or load
local tonumber = tonumber
local ipairs = ipairs
local setmetatable = setmetatable

local class = class
local weak_table = weak_table
local Converter = Converter
local BindingPathPart = BindingPathPart
local BindMode = BindMode
local BindingUtility = CS.Hugula.Databinding.BindingUtility

local splite_char = "."
local context_property = "context"
local chunk_name = "cs_binding expression"

---分割path表达式
---@overload fun(path:string)
---@param path string
---@return table
local function split_path(path)
    -- body
    local words = {}
    for w in (path .. "."):gmatch("([^.]*)%.") do
        table.insert(words, w)
    end
    return words
end

local function set_converter(self, converter)
    if converter then
        local conv = Converter[converter]
        if type(conv) == "table" then
            self.converter = conv
        end
    end
end

--- 解析path表达式
---@overload fun(path:string)
---@param path string
local function parse_path(self, path)
    local _parts = {}
    self._parts = _parts
    -- Logger.Log("parse_path,path=",path)

    if path == nil or path == "" then
        return
    end

    local last = BindingPathPart(self, ".")
    table.insert(_parts, last) --source

    local p = string_trim(path)
    if p == "." then
        return
    end

    local paths = split_path(path, ".")
    for k, v in ipairs(paths) do
        local part = string_trim(v)
        local indexer = nil
        local is_method = nil
        local method_args = nil
        ---item[i] 匹配索引
        local w, i = string_match(part, "(%w*)%[(%d+)%]")
        --匹配item[1]
        if w ~= nil and i ~= nil then
            indexer = BindingPathPart(self, tonumber(i)) --索引
            indexer.index_name = w --索引名称
            indexer.is_indexer = true
            part = w
        else
            ---匹配方法 invok(1) 1表示参数 为空表示没有
            w, i = string_match(part, "([%w|_]+)%((%d*)%)")
            --匹配 invoke(1) 1表示一个参数
            if w ~= nil then
                is_method = true ---标记为方法
                method_args = tonumber(i) ---参数个数
                part = w
            end
        end
        -- Logger.Log("parse_path.part=",part,k,v)
        if string_len(part) > 0 then
            local next = BindingPathPart(self, part)
            next.is_method = is_method
            next.method_args = method_args
            last.next_part = next
            table.insert(_parts, next) --source
            last = next
        end

        if (indexer ~= nil) then
            last.next_part = indexer
            table.insert(_parts, indexer) --source
            last = indexer
        end
    end
end

local function set_target_context(bindable_object, context)
    -- Logger.Log(string.format("set_context(%s=%s", bindable_object, context))
    if context.CollectionChanged and context.get_Item ~= nil then ---check INotifyTable:IList,INotifyPropertyChanged,INotifyCollectionChanged
        BindingUtility.SetContextByINotifyTable(bindable_object, context)
    elseif context.get_Item then ---chekc IList
        BindingUtility.SetContextByIList(bindable_object, context)
    elseif context.PropertyChanged then --check INotifyPropertyChanged
        BindingUtility.SetContextByINotifyPropertyChanged(bindable_object, context)
    else
        bindable_object[context_property] = context
    end
end

---当source改变的时候实际执行表达式
---从source的path获取值，设置到target.property
---或者将target.property 设置到source的path
---@overload fun(source:any,target:BindableObject,property:string,update_target:bool)
---@param source object
---@param target BindableObject
---@param property string
---@param update_target bool
local function apply_actual(self, source, target, property, update_target)
    local current = source
    local _parts = self._parts
    ---@class BindingPathPart
    local _part = nil
    local lens = #_parts
    local on_source_property_set = current.on_property_set

    -- Logger.Log(
    --     string_format(
    --         "apply_actual target=%s,source=%s,property=%s,update_target=%s,parts.len=%s",
    --         target,
    --         source,
    --         property,
    --         update_target,
    --         lens
    --     )
    -- )

    self._is_applied = true

    if lens == 0 then
        return
    end

    for k, part in ipairs(_parts) do
        _part = part

        if not part.is_self and k < lens then
            current = part:get_source_val(current)
        end

        -- Logger.Log(
        --     string_format(
        --         "parts target=%s,index=%s,current=%s,property=%s,_part=%s,update_target=%s",
        --         target,
        --         k,
        --         current,
        --         property,
        --         _part,
        --         update_target
        --     )
        -- )

        if part.next_part ~= nil and current then
            if current.PropertyChanged then ---监听INotifyPropertyChanged接口监听
                part:subscribe(current) ---监听属性改变
            end
        end
    end

    if update_target and current then
        -- target:_set_value(property, val, true)
        local val = nil
        if _part.is_self then
            val = current
        else
            val = _part:get_source_val(current)
        end

        local converter = self.converter
        if converter then
            val = converter.convert(val)
        end
        -- Logger.Log(
        --     string_format(
        --         "apply_actual update_target target=%s,current=%s,path=%s,val=%s",
        --         target,
        --         current,
        --         _part.path,
        --         val
        --     )
        -- )

        local format = self.format
        if format then
            -- Logger.Log("format", format, val)
            val = string_format(format, val)
        end

        -- Logger.Log(string_format("Binding target(%s):_set_value(%s=%s)", target, property, val))
        if property == context_property then ---如果是设置的context
            set_target_context(target, val)
        else
            local old = target[property]
            if old ~= val then
                target[property] = val
            else
                -- Logger.Log(string.format("%s.%s %s value is the same",target,property,old))
            end
        end
    elseif current then --updatasource
        local val = target[property]
        ---target:_get_value(property)

        local converter = self.converter
        if converter then
            val = converter.convert_back(val)
        end
        -- Logger.Log(string_format("Binding sourcel(%s).%s=%s)", source, property, val))

        if _part.is_self then
            source = val
        else
            _part:set_source_val(current, val)
        end
        ---通知source属性改变了
        if on_source_property_set then
            local path = self.path
            if _part.is_indexer then --如果是索引
                path = string_format("%s[%s]", _part.index_name, _part.path)
            end
            on_source_property_set(self, path)
        end
    end
end

--- 初始化解析source表达式并绑定
---@overload fun(target:BindableObject,property:string)
---@param target BindableObject
---@param property string
local function apply_init(self)
    local target = self._ref_target
    local _source = self._source
    -- Logger.Log(" apply_init: target=",target,_source)
    if _source ~= nil then ---解析source表达式
        local f_name = _source
        local path = self.path
        local relative_source
        local parent = target.parent
        if parent ~= nil then
            if string_sub(f_name, 1, 7) == "parent." then
                -- Logger.Log(" source.parent", parent, path)
                relative_source = parent
                path = f_name .. "." .. path
            else
                local children = parent.children
                local len = children.Count
                -- Logger.Log(" source", parent, children)
                local child = nil
                for i = 0, len - 1, 1 do
                    child = children[i]
                    -- Logger.Log(i, child.targetName)
                    if child.targetName == f_name then
                        relative_source = child
                        break
                    end
                end
            end
        end

        parse_path(self, path)

        if relative_source ~= nil then
            self._ref_source = relative_source
            self._Binding.m_Source = relative_source --
            -- Logger.Log(
            --     string_format(
            --         "apply_init path=%s local_source=%s,target=%s,property=%s",
            --         path,
            --         relative_source,
            --         target,
            --         self.property
            --     )
            -- )
            apply_actual(self, relative_source, target, self.property, true)
        end
    else
        parse_path(self, self.path)
    end
end

---
--- 设置绑定信息
---@overload fun(tab_config:table)
---@param tab_config table
local function set_binding_info(self, binding)
    self._Binding = binding
    self._ref_target = binding.target
    self.path = binding.path
    self.property = binding.propertyName --- 记录绑定的属性
    local format = binding.format
    if format ~= "" then
        self.format = format
    end
    local source = binding.source
    if source ~= "" then
        self._source = source
    end
    local converter = binding.converter
    if converter ~= "" then
        self._converter = converter
    end
    local mode = binding.mode
    if mode ~= "" then
        self._mode = mode
    end
    -- Logger.Log(string.format(" set_binding_info (path=%s,%s,%s)",binding.path,self._ref_target,binding.propertyName))
    set_converter(self, self._converter)
    apply_init(self)
end

-- ---绑定信息
-- ---@class Binding
-- ---@overload fun(binding_info:table)
-- ---@param binding_info table
-- local cs_binding =
--     class(
--     function(self, binding)
--         set_binding_info(self, binding)
--     end
-- )

--- 从缓存中获绑定目标和源
--- 一般是target更新source
--- NotifyObject 的时候调用
---@overload fun(update_target:boolean)
---@param update_target boolean --- 是否更新target目标
local function apply(self, update_target)
    -- Logger.Log("Apply", self._ref_target, "self_source",self._source)
    -- if self._source and self._is_applied then --- 如果有source并且已经生成过不需要再次绑定
    --     return
    -- end
    local _Binding = self._Binding
    -- local _weak_ref = self._weak_ref
    local target = self._ref_target --_weak_ref["target"]
    local source = _Binding.m_Source or _Binding.context --_weak_ref["source"]
    -- Logger.Log(string_format("apply target=%s,source=%s,property=%s",target,source,self.property))
    if target and source then
        apply_actual(self, source, target, self.property, update_target)
    end
end

--- 从新的值绑定目标
--- context改变的时候调用此方法
---@overload fun(context:any)
---@param context any
local function apply_context_changed(self, context, update_target)
    if self._source and self._is_applied then --- 如果有source并且已经生成过不需要再次绑定
        return
    end
    -- Logger.Log(string_format("apply_context_changed target=%s,local source=%s,context=%s,property=%s",self._ref_target,self.source,context,self.property))
    self._ref_source = context
    apply(self, update_target)
end

--- 取消值绑定目标
---@overload fun(fromBindingContextChanged:boolean)
---@param from_binding_context_changed boolean
local function unapply(self, from_binding_context_changed)
    if self._source ~= nil and from_binding_context_changed and self._is_applied then
        return
    end

    self._is_applied = false

    local weak_target, weak_source = self._ref_target, self._ref_source

    -- if (weak_source ~= nil) then
    local _parts = self._parts
    for k, part in ipairs(_parts) do
        -- Logger.Log("unapply ",self,part)
        part:unsubscribe()
        -- if not part.is_self then
        --     weak_source = part:get_source_val(weak_source)
        -- end
        -- -- Logger.Log("cs_binding.unapply",part,weak_source)
        -- if weak_source  or weak_source.PropertyChanged then
        --     part:unsubscribe()
        -- end
    end
    -- end
end

local function tostring(self)
    return string_format("Binding(target=%s,source=%s,path=%s) ", self._ref_target, self._ref_source, self.path)
end

local binding_expression =
    class(
    function(self, binding)
        set_binding_info(self, binding)
    end
)

---interface IBindingExpression 接口实现
binding_expression.Apply = apply
binding_expression.Unapply = unapply
binding_expression.__tostring = tostring
---static 方法
-- binding_expression.set_target_context = set_target_context
local binding_cls = {}
binding_cls.New = function(binding)
    local tab = binding_expression(binding)
    return tab
end
binding_cls.set_target_context = set_target_context
---绑定信息
---@class BindingExpression
---@overload fun(binding_info:table):Binding
---@param binding_info table
---@return BindingExpression
---@field Apply function
---@field Unapply function
BindingExpression = binding_cls

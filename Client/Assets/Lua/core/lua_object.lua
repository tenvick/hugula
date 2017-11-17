------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--  luaObject base of object
--  author pu
------------------------------------------------
local components = {}
local UPDATECOMPONENTS = UPDATECOMPONENTS

local function load_component(name)
    if components[name] == nil then
        components[name] = require("components/"..name)
    end
    return components[name]
end

local function get_component_name(path)
   local _,b,name= string.find(path,"[%.,%/]*(%a+_*%a+)$")
   return name
end

local function add_global_update_comp(fn)
    table.insert(UPDATECOMPONENTS,fn)
end

local function remove_global_update_comp(fn)
    local len=#UPDATECOMPONENTS
    local delIdx
    for i=1,len do
        if UPDATECOMPONENTS[i]==fn then
            delIdx =i
            break
        end
    end

    if delIdx~=nil then table.remove(UPDATECOMPONENTS,delIdx) end
end

LuaObject=class(function(self,name) 
    self.name = name or "lua_object"
	self.components = {}
	self.updatecomponents = {}
    self.active = true
    self.parent = nil
    self.is_disposed = false
end)

function LuaObject:add_component(arg)
    local name = get_component_name(arg)
    local cmp = nil 
	if self.components[name] then
        -- print("component "..name.." already exists!")
        return self.components[name]
    end
    cmp = load_component(arg)
    assert(cmp, "component ".. name .. " does not exist!")

    local loadedcmp = cmp(self)
    self.components[name] = loadedcmp
    loadedcmp.name = name
    if loadedcmp.start then loadedcmp:start() end

    if loadedcmp.on_update then
    	if not self.updatecomponents then self.updatecomponents={} end
    	self.updatecomponents[name]=loadedcmp
        add_global_update_comp(loadedcmp)
    end

    return loadedcmp
 end

 function LuaObject:remove_component(name)
	local cmp=self.components[name]
	if cmp then remove_global_update_comp(cmd)
        if cmp.remove then cmp:remove() end
    end 
    self.components[name] = nil
    self.updatecomponents[name] = nil
 end

 -- function LuaObject:set_active(bl)
 --    self.active = bl
 -- end

function LuaObject:dispose()
    table.clear(self.components)
    table.clear(self.updatecomponents)
    self.active=false
    self.is_disposed = true
end

--event eg:on_showed
--method_args {{method = "",args = ...},}
function LuaObject:register_event(event,method,...) --在某个事件后调用自己的某个方法
    if self.event_fun == nil then self.event_fun = {} end
    local event_call = self.event_fun[event]
    if event_call == nil then event_call = {} self.event_fun[event] = event_call end
    event_call[method] = {...}
end

function LuaObject:call_event(event) --触发注册的事件方法，调用之后此event会被remove
    if self.event_fun == nil then return end 
    if self.is_disposed then  log_warning(self," has disposed but you still call_event ",event,"\r\n" .. debug.traceback())  end
    local event_call = self.event_fun[event]
    if event_call then
        local t
        for k,v in pairs(event_call) do
            t = type(k) 
            if t == "string" then
                local fn = self[k]
                if fn then fn(self,unpack(v)) end
            elseif t == "function" then
                k(self,unpack(v))
            end
        end
    end
    self.event_fun[event] = nil
end

 function LuaObject:send_message(method,...)
    if self.is_disposed then log_warning(self," has disposed but you still call send_message ",method,"\r\n" .. debug.traceback()) end
    local cmps=self.components
    local fn
    
    fn = self[method]
    if type(fn) == "function" then fn(self,...) end --fn(self,{...})
    
    if cmps then
        for k,v in pairs(cmps) do
            fn = v[method]
           if type(fn) == "function" then fn(v,...) end --fn(v,unpack({...}))
        end
    end
   
 end

function LuaObject:__tostring()
    return string.format("luaObject.name = %s ", self.name)
end
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
end)

function LuaObject:add_component(arg)
    local name = get_component_name(arg)
    local cmp = nil 
	if self.components[name] then
        print("component "..name.." already exists!")
        return self.components[name]
    end
    cmp = load_component(arg)
    assert(cmp, "component ".. name .. " does not exist!")

    local loadedcmp = cmp(self)
    self.components[name] = loadedcmp
    loadedcmp.name = name
    if loadedcmp.start then loadedcmp:start() end

    if loadedcmp.onUpdate then
    	if not self.updatecomponents then self.updatecomponents={} end
    	self.updatecomponents[name]=loadedcmp
        add_global_update_comp(loadedcmp)
    end

    return loadedcmp
 end

 function LuaObject:remove_component(name)
	local cmp=self.components[name]
	if cmp then remove_global_update_comp(cmd)
    elseif cmp and cmp.remove then cmp:remove() 
    end 
    self.components[name] = nil
    self.updatecomponents[name] = nil
 end

 function LuaObject:set_active(bl)
    self.active = bl
 end

function LuaObject:dispose()

   -- for k,v in pairs(self.components) do
       -- fn=v[method]
       --if fn then  fn(v,unpack({...})) end
    --end
    self.components=nil
    self.updatecomponents = nil
    self.active=false
end

 function LuaObject:send_message(method,...)
    local cmps=self.components
    local fn
    for k,v in pairs(cmps) do
        fn = v[method]
       if fn then fn(v,...) end --fn(v,unpack({...}))
    end

    fn = self[method]
    if fn then fn(self,...) end --fn(self,{...})
 end

function LuaObject:__tostring()
    return string.format("luaObject.name = %s ", self.name)
end
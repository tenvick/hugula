------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
StateBase=class(function(self,itemObjects,name)
        if itemObjects then 
            self.m_itemList = itemObjects 
            table.sort(self.m_itemList,self.sortFn)
        else  
            self.m_itemList={}
        end
        self.name = name or ""
        self.willSort =false
    end)

local StateBase = StateBase

-- function StateBase:getItemCount()
--     local itemList = self.m_itemList
--     local size = 0
--     for k,v in ipairs(itemList) do
--         if v.assets~=nil then size=size+1 end
--     end
--     return size
-- end
local function sortFn(a,b) 
    return tonumber(a.priority)>tonumber(b.priority) 
end

function StateBase:addItem(obj)
    for i, v in ipairs(self.m_itemList) do
        if v == obj  then
            print("obj is exist in current state ")
            table.sort(self.m_itemList,sortFn)
           return
        end
    end
    table.insert(self.m_itemList, obj)
    self.willSort = true
end

--ºÏ≤‚ «∑Ò–Ë“™≈≈–Ú
function StateBase:checkSort()
    if self.willSort then
        table.sort(self.m_itemList,sortFn)
        self.willSort = false
    end
end

function StateBase:removeItem(obj)
    for i, v in ipairs(self.m_itemList) do
        if(v == obj) then
            table.remove(self.m_itemList,i)
            if(obj.onRemoveFromState~=nil) then
                obj:onRemoveFromState(self)
            end
        end
    end
end

function StateBase:onFocus(previousState)
    local itemList = self.m_itemList
    for k,v in ipairs(itemList) do
        v:onFocus(previousState)
    end

    self:checkSort()
end

function StateBase:onBlur(newState)
    local itemobj = nil
    for i=#self.m_itemList,1,-1 do
        itemobj=self.m_itemList[i]
        itemobj:onBlur(newState)
    end

 end

function StateBase:clear( ... )
    for k,v in ipairs(self.m_itemList) do
        v:clear()
    end
end

function StateBase:onEvent(funName,sender,arg)
    local fn = nil
    for k,v in ipairs(self.m_itemList) do
        fn = v[funName]
        if v.active and fn then 
            if fn(v,sender,arg) then break end
        end
    end

    self:checkSort()
end

--function StateBase:onClick(sender,arg)
--    for k,v in ipairs(self.m_itemList) do
--        if v.active and v.onClick then 
--            if v:onClick(sender,arg) then break end
--        end
--    end
--end

--function StateBase:onPress(sender,arg)
--    for k,v in ipairs(self.m_itemList) do
--        if v.active and v.onPress then 
--            if v:onPress(sender,arg) then break end
--        end
--    end
--end

--function StateBase:onDrag(sender,arg)
--    for k,v in ipairs(self.m_itemList) do
--        if v.active and v.onDrag then 
--            if v:onDrag(sender,arg) then break end
--        end
--    end
--end

--function StateBase:onDrop(sender,arg)
--    for k,v in ipairs(self.m_itemList) do
--        if v.active and v.onDrop then 
--            if v:onDrop(sender,arg) then break end
--        end
--    end
--end

--function StateBase:onDouble(sender,arg)
--    for k,v in ipairs(self.m_itemList) do
--        if v.active and v.onDouble then 
--            if v:onDouble(sender,arg) then break end
--        end
--    end
--end

--function StateBase:onCustomer(sender,arg)
--    for k,v in ipairs(self.m_itemList) do
--        if v.active and v.onCustomer then 
--            if v:onCustomer(sender,arg) then break end
--        end
--    end
--end

function StateBase:__tostring()
    local str = ""
    for k,v in ipairs(self.m_itemList) do 
        str =str..tostring(v).." " 
    end
    return string.format("StateBase.name = %s ,child = %s ", self.name,str)
end

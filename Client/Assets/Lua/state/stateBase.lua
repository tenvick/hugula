------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------

local function sortFn(a,b) 
    return tonumber(a.priority)>tonumber(b.priority) 
end

StateBase=class(function(self,itemObjects,name)
        if itemObjects then 
            self.m_itemList = itemObjects 
            table.sort(self.m_itemList,sortFn)
        else  
            self.m_itemList={}
        end
        self.name = name or ""
        self.willSort =false
        self.method = nil
    end)

local StateBase = StateBase

function StateBase:isAllLoaded()
    local itemList = self.m_itemList
    for k,v in ipairs(itemList) do
        if v.assets and #v.assets>0 and v.assetsLoaded == false then return false end
    end
    return true
end


function StateBase:addItem(obj)
    for i, v in ipairs(self.m_itemList) do
        if v == obj  then
             print(tostring(obj).." is exist in current state "..self.name)
            table.sort(self.m_itemList,sortFn)
           return
        end
    end
    table.insert(self.m_itemList, obj)
    self.willSort = true
end

function StateBase:check_sort()
    if self.will_sort then
        table.sort(self._item_list,sortFn)
        self.will_sort = false
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

    self:check_sort()
end

function StateBase:onBlur(newState)
    local itemobj = nil
    for i=#self.m_itemList,1,-1 do
        itemobj=self.m_itemList[i]
        if itemobj then  itemobj:onBlur(newState) end
    end

 end

function StateBase:clear( ... )
    for k,v in ipairs(self.m_itemList) do
        v:clear()
    end
end

function StateBase:onEvent(funName,...)
    local fn = nil
    for k,v in ipairs(self.m_itemList) do
        fn = v[funName]
        if v.active and fn then 
            if fn(v,...) then break end
        end
    end

    self:check_sort()
end

function StateBase:__tostring()
    local len = 0
    if self.m_itemList then len = #self.m_itemList end
    return string.format("StateBase.name = %s ,child = %d ", self.name,len)
end

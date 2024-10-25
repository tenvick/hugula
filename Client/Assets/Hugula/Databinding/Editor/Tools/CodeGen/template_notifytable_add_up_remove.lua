function {name}:add_{property}(data) {data_path}:Add(data) end
function {name}:update_{property}(data)
    local index = {data_path}:FindIndex(function(i, v) return v.eid == data.eid end)
    if index ~= nil then {data_path}:set_Item(index, data) end
end
function {name}:remove_{property}(data)
    local index = {data_path}:FindIndex(function(i, v) return v.eid == data.eid end)
    if index ~= nil then {data_path}:RemoveAt(index) end
end
function {name}:clear_{property}() {data_path}:Clear() end
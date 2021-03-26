
function {name}:add_{0}(data)
    {name}.{0}:Add(data)
end

function {name}:update_{0}(data)
    local index ={name}.{0}:FindIndex(
        function(i, v)
            return v.id == data.id
        end
    )
    if index then
       {name}.{0}:set_Item(index, data) --更新数据
    end
end

function {name}:remove_{0}(data)
    local index =
        {name}.{0}:FindIndex(
        function(i, v)
            return v.id == data.id
        end
    )
    if index ~= nil then
        {name}.{0}:RemoveAt(index)
    end
end
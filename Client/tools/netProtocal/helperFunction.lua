function trimLine(line)
    assert(line ~= nil)
    local first = string.find(line, '%S')
    if not first then first = 1 end
    local last = string.find(string.reverse(line), '%S')
    if not last then last = 1 end
    last = #line - last + 1
    if(first > last) then
        return nil
    end
    return string.sub(line, first, last)
end

function isUselessLine(line)
    if(line == nil or string.sub(line,1,1) == '#') then
        return true
    end
    return false
end

function getNextValidLine()
    local nextLine = io.read()
    while(nextLine ) do
        local trimedLine = trimLine(nextLine)
        if(trimedLine and not isUselessLine(trimedLine)) and trimedLine~="\r" then
            return trimedLine
        else
            nextLine = io.read()
        end
    end
    return nil
end


function isEndStruct(line)
    local f=string.find(line,"===")
    return  f~=nil
end

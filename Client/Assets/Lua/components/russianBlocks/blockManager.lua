local Input=UnityEngine.Input
local Vector3=UnityEngine.Vector3
local KeyCode=UnityEngine.KeyCode
local Quaternion = UnityEngine.Quaternion
local LuaHelper=LuaHelper
local minRowNumber=1
local block7={ --center for rota offset
{{1,1},{1,1},center={0.5,0.5},top={0,1}}, --
{{1,1,0},{0,1,0},{0,1,0},center={1,1},top={-1,1}},
{{0,1,1},{0,1,0},{0,1,0},center={1,1},top={-1,1}},
{{1,0,0},{1,1,0},{0,1,0},center={1,1},top={-1,1}},
{{0,0,1},{0,1,1},{0,1,0},center={1,1},top={-1,1}},
{{0,1,0},{1,1,1},{0,0,0},center={1,1},top={-1,1}},
{{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0},center={1.5,1.5},top={-1,1}},
}

local block

local map = {}
-- local debugMap = {}
local BlockManager = class(function(self,luaObj )
	self.luaObj = luaObj
	self.enable = false
	self.width = 12
	self.height = 18
	self.score=0
	self.leave=1
end)
----------------------------private---------------------


----------------------------class--------------------------
function BlockManager:map()
	return map
end

-- function BlockManager:debugMap()
-- 	return debugMap
-- end

function BlockManager:start()
	self.tile = 35
	self.blockDropSpeed = 0.01
	self.speed= 1
	map={}
	-- debugMap ={}
	minRowNumber = self.height
	for y=1,self.height+2 do
		for x=1,self.width do
			if(map[y]==nil) then map[y] = {} end
			if y <= self.height then map[y][x]=false
			else map[y][x]=true end

			-- --for debug
			-- if(debugMap[y]==nil) then debugMap[y] = {} end
			-- if y <= self.height then debugMap[y][x]=false 
			-- else debugMap[y][x]=true end
		end
	end
	self.score = 0
	block = self.luaObj.components.block
	--uiBlock = self.luaObj.components.uiBlock
	--if uiBlock then uiBlock.enable=true end
end

function BlockManager:gameOver()

	--self.luaObj.components.block.enable = false
	print("gameOver ")
	block = self.luaObj.components.block
	block.enable = false
	block:gameover()
	block:setEndScore(self.score)
	block:setLeave(self.leave)
	block:endGame()
	-- uiBlock = self.luaObj.components.uiBlock
	-- uiBlock:setState(3)
	-- uiBlock:setScore(self.score)
	-- uiBlock.enable=false
	--self:start()
	if self.luaObj.parent~=nil then self.luaObj.parent:sendMessage("gameOver",self)end
end

function BlockManager:gameStart()
	self:start()
	if block.enable == false then block.enable =true end
	block:begin()
end

function BlockManager:getBlocksData()
	return block7
end

--get width and height
function BlockManager:getRect()
	return self.width*self.tile,self.height*self.tile
end

function BlockManager:checkDelete(data)
	local size = #data
	local sizeY,sizeX= size+data.y,size+data.x
	local y,x,row,item,rowlen,delelen
	local dels
	local delrow,delRowCount={},0

	for y=data.y,sizeY do
		if y <= self.height then
			row = map[y]
			rowlen=#row
			dels = {}
			for i=1,rowlen do 
				item =row[i] 
				if type(item) == "userdata" then
					table.insert(dels,item)
				else 
					dels = {}
					break
				end
			end

			delelen=#dels

			if delelen>=rowlen then
				delRowCount=delRowCount+1 
				for j=1,rowlen do 
					delrow[tostring(y)]=y
					item =dels[j] LuaHelper.Destroy(item) 
					row[j]=false
				end
			end
		end
	end

	if minRowNumber >= data.y then minRowNumber = data.y end

	--move map data
	--if delRowCount == 0 then return end
	self.score=self.score+delRowCount*10
	local block =self.luaObj.components.block
	block:setScore(self.score/10,10,self.score)
	local getStartY = block:getStartPoint().y -self.tile/2
	local downCount,currRow,moveToRow,rowlen,item=0,nil,nil,0,nil
	for y=sizeY,data.y,-1 do
		if y<=self.height then
			if y == delrow[tostring(y)] then downCount=downCount+1 --如果等于删除行向下移动一行
			elseif downCount>=1 then
				print(string.format("move y%s to %s =",y,y+downCount))
				currRow=map[y]
				moveToRow=map[y+downCount]
				rowlen=#currRow
				for i=1,rowlen do
					item =currRow[i]
					moveToRow[i]=item
					if type(item) == "userdata" then
						p=item.transform.localPosition
						p.y= getStartY -(y+downCount-1)*self.tile--p.y-downCount*self.tile
						--p.y = p.y-downCount*self.tile
						item.transform.localPosition = p
						local key = "map_"..tostring(y+downCount).."_"..tostring(i)
						item.name = key
					end
					currRow[i]=false
				end
			end
		end
	end

	if downCount>0 then
		for y=data.y-1,downCount-1,-1 do
			if y<=self.height and y>=1 then
				currRow=map[y]
				moveToRow=map[y+downCount]
				rowlen=#currRow
				for i=1,rowlen do
					item =currRow[i]
					moveToRow[i]=item
					if type(item) == "userdata" then
						p=item.transform.localPosition
						--p.y=p.y-downCount*self.tile
						p.y= getStartY -(y+downCount-1)*self.tile
						item.transform.localPosition = p
						local key = "map_"..tostring(y+downCount).."_"..tostring(i)
						item.name = key
					end
					currRow[i]=false
				end
			end
		end			
	end

	-- --for debug
	-- for y=sizeY,minRowNumber,-1 do
	-- 	for x=1,self.width do
	-- 		if map[y] then
	-- 			if type(map[y][x])=="userdata" or map[y][x]==true  then
	-- 		 		debugMap[y][x].transform.localRotation=Quaternion.Euler(90,0,90)
	-- 		 	else
	-- 		 		debugMap[y][x].transform.localRotation=Quaternion.Euler(0,0,90)
	-- 			end
	-- 		end
	-- 	end
	-- end

end

local rx = 1
-- block can rota
function BlockManager:fill(data,blockDic)
	 local size = #data
	 local mx,my,key = 1,1,""
	 local row,datarow = nil,nil
	 for y=1,size do
	 	my=data.y+y-1
	 	row=map[my]
	 	datarow=data[y]
	 	for x=1,size do
	 		mx=data.x+x-1
	 		if row and datarow[x]==1 then-- 列超出界限
	 			key = "block_"..tostring(y).."_"..tostring(x) --string.format("block_%s_%s",y,x)
	 			local item  = blockDic[key]
	 			row[mx] = item
	 			--debugMap[my][mx].transform.localRotation=Quaternion.Euler(90,0,90*rx)
	 			if item ==nil then 
	 				print(tojson(blockDic))
	 				print(key.."is not exist !")
	 				print(tojson(data))
	 				print(tojson(row))
	 			end
	 			key = "map_"..tostring(my).."_"..tostring(mx)--string.format("map_%d_%d",my,mx)
	 			item.name = key
	 		end
	 	end
	 end
	 rx=rx+1
end

--check if move
function BlockManager:check(data,posx,posy)
	 local size = #data
	 local mx,my = 1,1
	 local row,datarow
	 -- printTable(data)
	 for y=1,size do
	 	my=math.floor(posy+y-1)
	 	row=map[my]
	 	datarow=data[y]
	 	if y==1 then
	 		-- print(string.format("check map.y=%s ,data.y=%s",my,y))
	 	end
	 	-- print("posy = "..tostring(my).." "..tojson(row))
	 	-- print(" y = "..tostring(y).. "  = "..tojson(datarow))
	 	for x=1,size do
	 		mx=math.floor(posx+x-1)
	 		if row == nil then-- 列超出界限
	 			if  datarow[x]==1 then 	return false end
 			elseif row[mx] == nil and datarow[x]==1 then  --行超出界限不能有方块
 				return false
 			elseif datarow[x]==1 and row[mx]~=false  then  --界限内有方块的地方不能有其他东西
 				return false  
	 		end
	 	end
	 end
	 return true
end

function BlockManager:__tostring()
    return string.format("BlockManager.name = %s ", self.name)
end

return BlockManager
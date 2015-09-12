
Proxy = {}
local Net=Net
local NetChat=NetChat
local Msg=Msg
local NetProtocalPaser = NetProtocalPaser
local getValue=getValue
local Proxy=Proxy
Proxy.tables = {}
Proxy.errorTables = {}
Proxy.sendDelay = {}

function Proxy:binding(api,fun)
	if self.tables[api.Code]==nil then self.tables[api.Code]={} end
	local msgTable=self.tables[api.Code]
	table.insert(msgTable,fun)
end

--只绑定一个全局响应函数
function Proxy:bindingOne(api,fun)
	if self.tables[api.Code]==nil then self.tables[api.Code]={} end
	local msgTable=self.tables[api.Code]
	while #msgTable>0 do table.remove(msgTable,1)  end
	table.insert(msgTable,fun)
end

function Proxy:unbinding(api,fun)
	local msgTable=self.tables[api.Code]
	if msgTable then
		local len=#msgTable local funitem local reindex
		for i=1,len do	funitem=msgTable[i]	if funitem==fun then reindex=i end		end
		if reindex then table.remove(msgTable,reindex) end --msgTable[msgTable]=nil
	end
end

function Proxy:distribute(msgType,data)
	if msgType == NetAPIList.gs_bad.Code then
		local errorfun = Proxy.errorTables[data.errno..""]
		if errorfun then --如果有错误处理函数就直接调用
			errorfun(data)
		else --否则弹出提示框
			local tips = getValue("g_notify_"..data.errno)
			showTips(tips)
		end	
	elseif msgType == NetAPIList.gs_good.Code then
		self:callHandle(data.commad,data)
	end

	self:callHandle(msgType,data)

end

function Proxy:callHandle(msgType,data)
	local funTable = self.tables[msgType]
	if funTable then
		local len = #funTable local funitem		
		for i=1,len do	
			funitem=funTable[i]	
			if funitem then  
				funitem(data) 
			end		
		end
	end
end

function Proxy:bindingError(code,fun)
	self.errorTables[code..""] = fun
end

function Proxy:unbindingError(code,fun)
	self.errorTables[code..""] = nil
end

 -- function Proxy:errorCodeBack(code)
 -- 	local fun = self.errorTables[code..""]
 -- 	if fun ~= nil then	fun(code) end
 -- end

--发送消息到服务端
function Proxy:send(api,content)
	local msg=Msg()

       	msg:set_Type(api.Code)
	NetProtocalPaser:formatMessage(msg,api.Code,content) 
	Net:Send(msg)
	    
end
--发送消息到服务端
function Proxy:send(api,content)
	local msg=Msg()
	-- print(msg)
	msg:set_Type(api.Code)
	NetProtocalPaser:formatMessage(msg,api.Code,content) 
	Net:Send(msg)
end

--关闭
function Proxy:close()
	Net:Close()
end

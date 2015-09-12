require("game.server.serverMsgHelperr")
local hall = LuaItemManager:getItemObject("hall")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper

local Net=Net
local Proxy=Proxy
local NetMsgHelper = NetMsgHelper
local NetAPIList = NetAPIList

local SystemInfo = UnityEngine.SystemInfo
local ServerMsgHelperr = ServerMsgHelperr
local NGUIText = NGUIText
local MasterServer = UnityEngine.MasterServer
hall.assets=
{
  Asset("Hall.u3d",{"Main","Create"})
}

---------------local table
local svrListCtb,myListCtb --服务器列表
local chatInput,chatInputTrigger,chatArea
local asset
local StateMain = {"Main"}
local StateCreate = {"Create"}
local clients --用户列表
------------------------local fun

--有新用户请求登陆过来
local function onRoomList(msg)
  -- printTable(msg)
  -- if clients==nil then clients ={} end
  -- table.insert(clients,msg)
  myListCtb.data = msg.list
  myListCtb:Refresh()
end

--服务端发送过来的seesionID
local function login_session(msg)
    Model.session=msg.session
    Model.clientName = SystemInfo.deviceName
    -- print("服务端发送过来的seesionID"..msg.session)
    local msg = NetMsgHelper:makept_login_req(Model.clientName,Model.session)
    Proxy:send(NetAPIList.login_req,msg) --发送登陆消息
    hall:toList()
end


local function createHall( ... )
  delay(ServerMsgHelperr.createServer,0.2,nil)
end

local function closeHall()
  clients ={}
  ServerMsgHelperr:closeServer()
  Proxy:close()
  myListCtb.data = clients
  myListCtb:Refresh()
end

local function onChatInput(gameObject, mInput, target)
  local text = NGUIText.StripSymbols(mInput.value)
  hall:chat(text)
  mInput.value = "" 
  mInput.isSelected = false
end

--接受聊天消息
local function chat_rec(msg)
  local text = msg.name..":"..msg.context
  chatArea:Add(text)
end
 
 --发送聊天消息
function hall:chat(msg)
   local msg = NetMsgHelper:makept_chat_send(Model.clientName,msg,Model.session)
    Proxy:send(NetAPIList.chat_send,msg) 
end

--返回大厅
function hall:toHall( ... )
    asset:showState(StateMain)
    hall.isMain = true
    svrListCtb.data = {}
    svrListCtb:Refresh()
    hall.getServerList()
    closeHall()
end

function hall:toList()
  hall.isMain = false
  asset:showState(StateCreate)
end

function hall.getServerList( )
   MasterServer.RequestHostList("Hugula")
   -- print(MasterServer.ipAddress..":"..MasterServer.port)
  local len = MasterServer.PollHostList().Length
  local hostList = {}
  local data = svrListCtb.data
  if data == nil then data={} end
  local needUpdate = false
  if  len~= 0 then
        local hostData = MasterServer.PollHostList()
        local i=0
        local tb ={}
        local item = nil
        while i < hostData.Length do
            item = hostData[i]
            i=i+1
            table.insert(hostList,item)
        local  exist = false
           for k,v in ipairs(data) do
              if v.comment == item.comment then
                  exist = true
                  break
              end
           end
           if exist==false then needUpdate = true end
        end
    end
    MasterServer.ClearHostList()

    if needUpdate then
      svrListCtb.data = hostList
      svrListCtb:Refresh()
    end

    if hall.isMain then
      delay(hall.getServerList,1,nil)
    end
 end 
------------------------------------
function hall:onAssetsLoad(items)
    asset = self.assets[1]
    local mainMono = LuaHelper.GetComponent(asset.items["Main"],"ReferGameObjects") 
    svrListCtb =mainMono.monos[0]
    local createMono =  LuaHelper.GetComponent(asset.items["Create"],"ReferGameObjects") 
    local monos = createMono.monos
    myListCtb,chatInput,chatArea,chatInputTrigger = monos[0],monos[1],monos[2],monos[3]
    chatInputTrigger.luaFn=onChatInput
    myListCtb.onItemRender=function(referScipt,index,itemdata)
      if(itemdata) then
          referScipt.name="Card"..tostring(itemdata.session)
          referScipt.userObject = itemdata
          local mono = referScipt.monos
          mono[1].text = itemdata.name
          referScipt.gameObject:SetActive(true)
      end
  end

  svrListCtb.onItemRender=function(referScipt,index,itemdata)
      if(itemdata) then
          referScipt.name="SvrIp"..tostring(index)
          referScipt.userObject = itemdata
          local mono = referScipt.monos
          mono[1].text = itemdata.gameName
          referScipt.gameObject:SetActive(true)
      end
  end

end


function hall:onShowed( )
  self:toHall()
end

-- function hall:onBlur( ... )
--     self:clear()
-- end

function hall:onClick(obj,arg)
	local cmd =obj.name
	if cmd == "BtnCreate" then
    self:toList()
    createHall()
  elseif cmd =="HallClose" then
    self:toHall()
  elseif cmd == "HallStart" then
    print("begin war!")
  else
    local id = string.match(cmd,"SvrIp(%d+)")
    if id~=nil then
        local refScr = LuaHelper.GetComponent(obj,"ReferGameObjects") 
        print("connect to :"..refScr.userObject.comment)
        ServerMsgHelperr:connect(refScr.userObject.comment)
    end
	end
 -- print("hello onclick "..obj.name)
end


function hall:initialize( ... )
  Proxy:bindingOne(NetAPIList.room_list,onRoomList) --有用户加入
  Proxy:bindingOne(NetAPIList.login_session,login_session) --登陆成功
  Proxy:bindingOne(NetAPIList.chat_send,chat_rec) --收到聊天消息
end
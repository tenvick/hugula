require("core.unity3d")
require("core.loader")
json=require("lib/json")
require("const.importClass")
require("net.netMsgHelper")
require("net.netAPIList")
require("net.netProtocalPaser")
require("net.proxy")
require("const.requires")
require("registerItemObjects")
require("registerState")
require("uiInput")

print("hello lan")
require("netGame")

local Proxy=Proxy
local NetMsgHelper = NetMsgHelper
local NetAPIList = NetAPIList
local proxy = Proxy
local Net=Net
local LuaHelper = LuaHelper

local function gmReceive(msg)
	printTable(msg)
end


-----------------------------UI----------------
local root =LuaHelper.Find("ServerRoot")
local Refer = LuaHelper.GetComponent(root,"ReferGameObjects")
local camackTable = Refer.monos[0]
local lable = Refer.monos[1]
camackTable.onItemRender=function(referScipt,index,itemdata)
		if itemdata  then
			local mono = referScipt.monos
			mono[0].text = itemdata.gameName..itemdata.comment 
			referScipt.gameObject:SetActive(true)
		end
	end

camackTable.onPreRender=function(referScipt,index,dataItem)
	referScipt.name="Pre"..tostring(index)	
	referScipt.gameObject:SetActive(false)
end

local function log(txt)
	lable.text = txt
end
-- camackTable.data = {"hostData","h1"}
-- camackTable:Refresh()

proxy:binding(NetAPIList.gm_cmd_req,gmReceive)

local MasterServer=luanet.UnityEngine.MasterServer
 
 log("MasterServer.ClearHostList()")
 -- MasterServer.ClearHostList()
 -- MasterServer.RequestHostList("Hugula")

local hostList  = {}
 local function getServerList( )
 	 MasterServer.RequestHostList("Hugula")

 	local len = MasterServer.PollHostList().Length
 	 log("MasterServer.Length()"..len..os.time())

 	if  len~= 0 then
            local hostData = MasterServer.PollHostList()
            local i=0
            local tb ={}
            while i < hostData.Length do
                log("Game name: " ..hostData[i].gameName)
                i=i+1
                table.insert(hostList,hostData[i])
            end

           MasterServer.ClearHostList()
    end
camackTable.data = hostList
camackTable:Refresh()
     delay(getServerList,1,nil)
 end 

 delay(getServerList,1,nil)
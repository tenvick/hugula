print("testScrollTable.lua")
require("core.loader")
json=require("lib/json")
--

local datas={}
for i=0,1000 do
	local it ={}
	it.name="name "..i
	it.title ="title"..i
	table.insert(datas,it)
end

local function on_rootscroll_loaded(req)

	local root = LuaHelper.Instantiate(req.data)
	-- local root = LuaHelper.Find("RootScroll")
scrollRectTable = LuaHelper.GetComponentInChildren(root,ScrollRectTable)

scrollRectTable.onItemRender=function(scrollRectItem,index,dataItem)
	scrollRectItem.gameObject:SetActive(true)
	local monos = scrollRectItem.monos
	monos[0].text = dataItem.title
	monos[1].text = dataItem.name
	scrollRectItem.name = dataItem.name
	local btn = monos[3]
	btn.onClick:RemoveListener(onBtnClick)
	btn.onClick:AddListener(onBtnClick)
	if index==3 then
		btn.name = "click me!"
		monos[1].text = "click me!"
	elseif index ==100 or index ==90 then
		btn.name = "click me back!"
		monos[1].text = "click me back!"
	else
		btn.name = "click "..index
	end
end

scrollRectTable.data= datas
scrollRectTable:Refresh(-1,-1)
end


Loader:getResource(CUtils.GetAssetFullPath("rootscroll.u3d"),on_rootscroll_loaded)

--click
function onBtnClick()
	local name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name
	print(name)
	if name == "click me!" then
		scrollRectTable:scrollTo(93)
	elseif name == "click me back!" then
		scrollRectTable:scrollTo(0)
	end
end


local a = delay(function( ... )
	print("delya me ")
end,3)

stopDelay(a)
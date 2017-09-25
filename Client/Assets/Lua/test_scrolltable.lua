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
scroll_rect_table = LuaHelper.GetComponentInChildren(root,ScrollRectTable)
-- scroll_rect_table.columns = 0
-- scroll_rect_table.direction = 0

scroll_rect_table.onItemRender=function(scroll_rect_item,index,dataItem)
	scroll_rect_item.gameObject:SetActive(true)
	local monos = scroll_rect_item.monos
	scroll_rect_item:Get(1).text = dataItem.title
	scroll_rect_item:Get(2).text = dataItem.name
	scroll_rect_item.name = dataItem.name
	local btn = monos[4]
	btn.onClick:RemoveListener(onBtnClick)
	btn.onClick:AddListener(onBtnClick)
	if index==3 then
		btn.name = "click me!"
		monos[2].text = "ScrollTo 93"
	elseif index==4 then
		btn.name = "delete me!"
		monos[2].text = "点我删除!"
	elseif index ==100 or index ==90 then
		btn.name = "click me back!"
		monos[2].text = "点我返回 0 !"
	else
		btn.name = "click "..index
	end
end

scroll_rect_table.data= datas
scroll_rect_table:Refresh(-1,-1)
end


Loader:get_resource(CUtils.GetAssetFullPath("rootscroll.u3d"),on_rootscroll_loaded)

--click
function onBtnClick()
	local name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name
	print(name)
	if name == "click me!" then
		scroll_rect_table:ScrollTo(93)
	elseif name == "delete me!" then
		local i = scroll_rect_table:RemoveDataAt(3)
		scroll_rect_table:Refresh(scroll_rect_table.headIndex,-1)
		print("scroll_rect_table:removeDataAt"..tostring(i))
	elseif name == "click me back!" then
		scroll_rect_table:ScrollTo(0)
	end
end


local a = delay(function( ... )
	print("delya me ")
end,3)

stop_delay(a)

print("new")
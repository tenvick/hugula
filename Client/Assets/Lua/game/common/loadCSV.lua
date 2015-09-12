------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local CUtils=CUtils
local FileHelper=toluacs.FileHelper   --luanet.import_type("FileHelper")
local Loader = Loader
local Model = Model
local json = json
local split = split

local url_Unit="Unit"
local url_Skill = "Skill"
local url_Buff = "Buff"
local url_LevelData = "LevelData"
local url_GoodsData = "Goods"
local url_Guide = "Guide"

local lineCount
local rowNum
local sheetName
function json.onDecodeError(message, text, location, etc)
    print(string.format("json.onDecodeError %s line:%d,row:%d %s ",sheetName,lineCount,rowNum,text))
    print(location)
end
--parse key value to luaTable
local function decodeToLua(text)
	local datas={}
	local lines = split(text,"\n")
	local names = lines[2]
	local columNames = split(names,";")
	local CNLen = #columNames

	lineCount=#lines-1
	for i=3,lineCount do
		line = lines[i]
		local temp = split(line,";")
		local tempData = {}
		local tempItem 
		for i=1,CNLen do
			tempItem =  temp[i]
            rowNum = i
			if tempItem then 
				tempItem = string.gsub(tempItem,'\\n','\n')
				local f = string.sub(tempItem,1,1)
				if f=="[" or f=="{" then tempItem = json:decode(tempItem)
				elseif string.match(tempItem,"^%d*%.?%d*$") then tempItem=tonumber(tempItem) end					
			end
			tempData[columNames[i]] =tempItem
		end
		datas[temp[1]] = tempData
	end
	return datas
end

local function decodeUnit(data)
	Model.units=data
end

local function decodeSkill(data)
	Model.skills=data
end

local function decodeBuff(data)
	Model.buff=data
end

local function decodeChapterData(data)
	Model.chapterData = data
end

local function decodeItemData(data)
	Model.itemData = data
end

local function decodeGoodComp(data)
	Model.goodComp = data
end

local function decodeHeroStren(data)
	Model.heroStren = data
end

local function decodeTeamBuff(data )
	Model.teamBuff = data
end

local function decodeDiscrete(data)
	Model.discreteData = data
end

local function decodeStoryDlg( data )
	Model.storyDlg = data
end

local function decodeTaskData(data)
	Model.taskData = data
end

local function decodeGuide(data)
    Model.guid = data
end

local function decodeActive(data)
	Model.activityConfig = data;
end

local function decoderZipTxt(name,context)
    sheetName = name
	local data=decodeToLua(context)
	if name == url_Unit then decodeUnit(data)
	elseif name ==url_Skill then decodeSkill(data)
	elseif name ==url_Buff then decodeBuff(data)
	-- elseif name == url_chapterData then decodeChapterData(data)
	-- elseif name == url_itemData then decodeItemData(data)
	-- elseif name == url_goodComp then decodeGoodComp(data)
	-- elseif name == url_strenth then decodeHeroStren(data)
	-- elseif name == url_teamBuff then decodeTeamBuff(data)
	-- elseif name == url_discrete then decodeDiscrete(data)
	-- elseif name == url_taskData then decodeTaskData(data)
	-- elseif name == url_storyDlg then decodeStoryDlg(data)
	-- elseif name == url_Guide then decodeGuide(data)
	-- elseif name == url_Active then decodeActive(data)
	end
	print(name.."  decoded     ")
end


local function loadComp(req)
	FileHelper.UnpackConfigAssetBundleFn(req.data.assetBundle,decoderZipTxt)
    disposeWWW(req.data)
end

local function loadConfigZip()
	local url=CUtils.GetFileFullPath(CUtils.GetAssetPath("config.tz"))
	Loader:getResource(url,loadComp,false)
end

loadConfigZip()
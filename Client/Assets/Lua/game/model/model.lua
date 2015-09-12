------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
Model={}
local Model = Model
--config data
Model.hasConnection = false --是否已经连接过
Model.guideTaskid = 200
Model.units=nil --Unit Config
function Model.getUnit(id)
	return Model.units[id..""]
end

--skill
Model.skills={}
function Model.getSkill(id)
	return Model.skills[id..""]
end

Model.buff = {}
function Model.getBuff(id)
	return Model.buff[id..""]
end

Model.session = 0 --sessionID
Model.isServer = false --是否服务器
Model.clientName = "" --名称
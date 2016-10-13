------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
Model={}
local Model = Model
--config data
Model.has_connection = false --是否已经连接过
Model.guide_taskid = 200
Model.units=nil --Unit Config
function Model.getUnit(id)
	return Model.units[id..""]
end

Model.session = 0 --sessionID
Model.isServer = false --是否服务器
Model.clientName = "" --名称
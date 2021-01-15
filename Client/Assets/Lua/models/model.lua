------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------


local model = {}
-- local Model = Model
--config data
model.has_connection = false --是否已经连接过
model.guide_taskid = 200
model.units=nil --Unit Config
function model.getUnit(id)
	return model.units[id..""]
end

model.session = 0 --sessionID
model.isServer = false --是否服务器
model.clientName = "" --名称

return model
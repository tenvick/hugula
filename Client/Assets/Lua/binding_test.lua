local require = require
require("common.requires")
local VMState = VMState
local VMGroup = VMGroup



local binding_demo = VMState:get_member("binding_demo")
function set_context(bindableObject)
    bindableObject["context"] = binding_demo
end
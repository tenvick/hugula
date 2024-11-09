local require = require
require("common.requires")
local VMState = VMState
local VMGroup = VMGroup
local set_target_context = BindingExpression.set_target_context




local binding_demo = VMState:get_member("binding_demo")
function set_context(bindableObject)
    set_target_context(bindableObject, binding_demo)
end
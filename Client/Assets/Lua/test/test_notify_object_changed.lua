require("common.requires")
-- local notify = NotifyTable()


-- -- -- 数据双向绑定示例
local notify = NotifyObject()
notify.goods = {name = "pen", color = {"red", "yellow(原始属性)", "blue"}}

local function on_property_changed(sender,perpertyname)
    Logger.Log("on_property_changed",sender,perpertyname)
end
notify:add_PropertyChanged(on_property_changed)
-- local t1 = Binder.Object()
-- t1:set_binding("text", Binding({path = "goods.color[2]",mode="twoway"}))--绑定到数组
-- t1:set_context(notify)
-- Logger.Log(t1.text)

notify.goods = {name = "pencil", color = {"samll", "middle(属性goods改变)", "big"}} --一级属性goods改变
notify:OnPropertyChanged("goods")

notify:SetProperty("hello","hello every one")

Logger.Log(typeof(CS.UnityEngine.ParticleSystem))

Logger.Log(typeof(CS.Hugula.Databinding.INotifyPropertyChanged))
Logger.Log(typeof(CS.Hugula.Databinding.INotifyTable))
Logger.Log(cast(notify,typeof(CS.Hugula.Databinding.INotifyPropertyChanged)))

-- Logger.Log(t1.text)

-- notify.goods.color[2] = "super middle(属性color改变)" --二级属性color改变
-- notify:property_changed("color")
-- Logger.Log(t1.text)

-- t1.text = "reverse middle 反向绑定 通过text设置notify.goods.color[2]" --反向绑定

-- Logger.Log(notify.goods.color[2])



-- t1:set_binding(BindableProperty.text_property, Binding({path = "value",source=s1}))--绑定到数组
-- s1:set_binding(BindableProperty.value_property, Binding({path = "[1]"}))
-- s1.context = {0.56}
-- Logger.Log(s1.value)
-- s1.context = {0.256}
-- Logger.Log(s1.value)
-- s1.value = 0.8
-- Logger.Log(t1.text)
-- Logger.Log(t1.context.goods.color[2]) --双向绑定
-- t1:set_binding(BindableProperty.text_property,Binding("{path='[2]',format='%s aaaa'}"))
-- t1.context={"cat","dog","fish"}
-- Logger.Log(t1.text)
-- t1.text = "set my dog"
-- Logger.Log(t1.context[2]) --双向绑定
-- t1:set_binding(BindableProperty.text_property,Binding({path="."}))
-- t1.context = "hello boy"

-- Logger.Log(t1.text)

-- s1.value = 0.5
-- s1:set_value(0.88)
-- print(s1:get_value())

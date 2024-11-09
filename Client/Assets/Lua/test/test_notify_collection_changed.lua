require("general.requires")

local notify = NotifyTable()

notify:PropertyChanged(
    "+",
    function(source, arg)
        print("_property_changed:", arg)
    end
)

notify:CollectionChanged(
    "+",
    function(source, arg)
        print(
            string.format(
                "_collection_changed:Action=%s,NewStartingIndex:%s,NewItems:%s,OldItems:%s,OldStartingIndex:%s",
                arg.Action,
                arg.NewStartingIndex,
                arg.NewItems,
                arg.OldItems,
                arg.OldStartingIndex
            )
        )
    end
)

print("Add('hello')", table.concat(notify.items, ","))
notify:Add("hello")
assert("hello" == table.concat(notify.items, ","), "insert_item error ")
local range = {"welcome", "to", "hugula", "this", "is", "notify", "table", "demo"}
print("insert_range('welcome',...)", table.concat(notify.items, ","))
notify:InsertRange(range)
assert("hello,welcome,to,hugula,this,is,notify", "insert_range is error")
print("insert_range(2,'index2',...)", table.concat(notify.items, ","))
notify:InsertRange(2, {"index2"})

print("remove_at(2)", table.concat(notify.items, ","))
notify:RemoveAt(2)
assert(
    "hello,welcome,to,hugula,this,is,notify,table,demo" == table.concat(notify.items, ","),
    "notify:RemoveAt(2) error"
)

print("remove_range('to','demo')", table.concat(notify.items, ","))
notify:RemoveRange({"to", "demo"})
assert("hello,welcome,hugula,this,is,notify,table" == table.concat(notify.items, ","), "notify:RemoveAt(2) error")

print("replace_range('替换1','替换2')", table.concat(notify.items, ","))
notify:ReplaceRange({"替换1", "替换2"}, 3)
assert("hello,welcome,hugula,替换1,替换2,notify,table" == table.concat(notify.items, ","), "notify:RemoveAt(2) error")

print("insert_item(2,'to')", table.concat(notify.items, ","))
notify:Insert(2, "to")
assert("hello,welcome,to,hugula,替换1,替换2,notify,table" == table.concat(notify.items, ","), "notify:RemoveAt(2) error")

print("notify:move_item(4,5)", table.concat(notify.items, ","))
notify:MoveItem(4, 5)
assert("hello,welcome,to,hugula,替换2,替换1,notify,table" == table.concat(notify.items, ","), "notify:RemoveAt(2) error")

print("notify:RemoveAt(4)", table.concat(notify.items, ","))
notify:RemoveAt(4)
assert("hello,welcome,to,hugula,替换1,notify,table" == table.concat(notify.items, ","), "notify:RemoveAt(2) error")

print("notify:set_item(4,'this')", table.concat(notify.items, ","))
notify:set_Item(4, "this")
assert("hello,welcome,to,hugula,this,notify,table" == table.concat(notify.items, ","), "notify:RemoveAt(2) error")

--   notify
print(table.concat(notify.items, ","))

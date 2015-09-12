--===================================决策节点====================================================

-------------------------------------条件节点-----------------------------
ConditionNode(luafunction) ConditionNode(Condition)
--条件等待节点
ConditionWaitNode(luafunction) ConditionWaitNode(Condition)
--按一定时间执行的条件节点
DeltaConditionNode(float delta, Condition condition) DeltaConditionNode(float delta, Condition condition)

-------------------------------------行动节点-------------------------------------
ActionNode((System.Action<BTInput, BTOutput> action) --返回true
ActionNotNode((System.Action<BTInput, BTOutput> action) --返回false
--间隔时间执行的行动节点
DeltaActionNode(float delta,System.Action<BTInput, BTOutput> action)
-- 事件行动节点
ActionEventNode(System.Action<BTInput,BTOutput> action)

-------------------------------------等待节点------------------------------------- 
WaitNode(float duration, float variation)--（等待指定时间后返回成功）

-------------------------------------装饰节点-------------------------------------
DecoratorNotNode() -- 反转结果

-------------------------------------选择节点------------------------------------- [从begin到end依次执行子节点]
SelectorNode() --(遇到true返回True)

-------------------------------------顺序节点------------------------------------- [从begin到end依次执行子节点]
SequenceNode() --(遇到false返回false)
SequenceTrueNode() --(遇到false返回true,否则循环完成后返回true)

-------------------------------------并行节点------------------------------------- [同时执行所有子节点]
ParallelNode() --平行执行它的所有Child Node,遇到False则返回False，全True才返回True。
ParallelNodeAny() --平行执行它的所有Child Node,遇到False则返回False，有True返回True
ParallelSelectorNode() --遇到True则返回True，全False才返回False
ParallelFallOnAllNode()	--所有False才返回False，否则返回True

-------------------------------------事件节点-------------------------------------
EventNode(int eventType) --事件触发的时候执行，只能有一个子节点
TriggerNode(int eventType) --触发事件节点，运行到当前节点时候会触发eventType事件 返回成功

-------------------------------------循环节点-------------------------------------
LoopNode(int count) --The Loop node allows to execute one child a multiple number of times n (if n is not specified then it's considered to be infinite) or until the child FAILS its execution
LoopUntilSuccessNode() --The LoopUntilSuccess node allows to execute one child until it SUCCEEDS. A maximum number of attempts can be specified. If no maximum number of attempts is specified or if it's set to <= 0, then this node will try to run its child over and over again until the child succeeds.

-------------------------------------压制失败-------------------------------------
SuppressFailure() --只能有一个子节点，总是返回true


--===================================输入域输出===================================--
input.target RoleActor --输入目标角色
input.position Vector3 --输入目标位置
input.skill int 	--输入技能
input.stopDistance	--移动停止距离
input.isDragging --是否拖动状态

output.leafNode --当前叶节点
output.eventData --传输数据


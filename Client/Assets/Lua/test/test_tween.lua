-- require("core.unity3d")
-- require("core.loader")
-- json=require("lib/json")


Vector3 = UnityEngine.Vector3
local cube=LuaHelper.Find("Cube")
-- LeanTween.rotateAround(cube, Vector3.up, 359.999, 100.2):setEase( LeanTweenType.easeShake ):setRepeat(30)
-- LeanTween.rotateAround( cube, Vector3.right, 1, 0.25):setEase( LeanTweenType.easeShake ):setLoopClamp():setRepeat(2):setDelay(0.05);

local cam=LuaHelper.Find("Sphere")
local camera=LuaHelper.Find("Main Camera")
LeanTween.rotateAround(cam, Vector3.up, 359.999, 10.0 ):setEase( LeanTweenType.easeShake ):setDelay(5):setUseEstimatedTime(true)

local obj=LuaHelper.Find("Camer1Path")
local camera1Path=LuaHelper.GetComponent(obj,"ReferGameObjects")
local refers = camera1Path.monos
function oncomplete( paths )
	print("on complete ")
	print(paths[6])
end


local paths={}--Vector3[4]
local len = camera1Path.Length
for i = 1,len do
	local v = camera1Path:Get(i)
	local vct =v.transform.position+cube.transform.position
	table.insert(paths,vct)
end

print(#paths)
--moveSpline
LeanTween.moveSpline(camera,paths, 6):setOrientToPath(true):setEase(LeanTweenType.easeInQuart):setDelay(1):setOnComplete(oncomplete):setOnCompleteParam(paths) --setOrientToPath(true):

print("new")
 -- LeanTween.drawBezierPath(paths[0],paths[1],paths[2],paths[3])
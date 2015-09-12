require("core.unity3d")
require("core.loader")
json=require("lib/json")

local learntween=luanet.LeanTween
learntween=luanet.LTBezier
learntween=luanet.LTBezierPath
learntween=luanet.LTDescr
learntween=luanet.LTSpline
local LTBezierPath = luanet.LTBezierPath
local Vector3Helper= luanet.Vector3Helper

local LeanTween = toluacs.LeanTween
local LeanTweenType=luanet.LeanTweenType

local gameObject=LuaHelper.Find("Cube")
-- LeanTween.rotateAround(gameObject, Vector3.up, 1, 0.2):setEase( LeanTweenType.easeShake ):setLoopClamp():setRepeat(3)
-- LeanTween.rotateAround( gameObject, Vector3.right, 1, 0.25):setEase( LeanTweenType.easeShake ):setLoopClamp():setRepeat(2):setDelay(0.05);


-- * LTBezierPath ltPath = new LTBezierPath( new Vector3[] { new Vector3(0f,0f,0f),new Vector3(1f,0f,0f), new Vector3(1f,0f,0f), new Vector3(1f,1f,0f)} );<br><br>
-- * LeanTween.move(lt, ltPath.vec3, 4.0f).setOrientToPath(true).setDelay(1f).setEase(LeanTweenType.easeInOutQuad); // animate <br>
-- * Vector3 pt = ltPath.point( 0.6f ); // retrieve a point along the path
local cam=LuaHelper.Find("Sphere")
local camera=LuaHelper.Find("Main Camera")
-- LeanTween.rotateAround( cam, Vector3.up, 360.0, 1.0 ):setDelay(1.5):setUseEstimatedTime(true)

local obj=LuaHelper.Find("Camer1Path")
local camera1Path=LuaHelper.GetComponent(obj,"ReferGameObjects")

function oncomplete( paths )
	print("on complete ")
	print(paths[0])
end


local paths=Vector3[4]
for i=0,3 do
	paths[i]= Vector3Helper.Add(camera1Path.refers[i].transform.position,gameObject.transform.position)
end
--moveSpline
LeanTween.move(camera,paths, 2):setOrientToPath(false):setEase(LeanTweenType.easeInQuart):setDelay(1):setOnComplete(oncomplete):setOnCompleteParam(paths) --setOrientToPath(true):

 -- LeanTween.drawBezierPath(paths[0],paths[1],paths[2],paths[3])
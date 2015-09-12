using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_Cloth : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.Cloth o;
			o=new UnityEngine.Cloth();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ClearTransformMotion(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			self.ClearTransformMotion();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetEnabledFading(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==2){
				UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				self.SetEnabledFading(a1);
				return 0;
			}
			else if(argc==3){
				UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				self.SetEnabledFading(a1,a2);
				return 0;
			}
			LuaDLL.luaL_error(l,"No matched override function to call");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sleepThreshold(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.sleepThreshold);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sleepThreshold(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.sleepThreshold=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bendingStiffness(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.bendingStiffness);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_bendingStiffness(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.bendingStiffness=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_stretchingStiffness(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.stretchingStiffness);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_stretchingStiffness(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.stretchingStiffness=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_damping(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.damping);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_damping(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.damping=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_externalAcceleration(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.externalAcceleration);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_externalAcceleration(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.externalAcceleration=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_randomAcceleration(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.randomAcceleration);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_randomAcceleration(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.randomAcceleration=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useGravity(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.useGravity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useGravity(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useGravity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enabled(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.enabled);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enabled(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.enabled=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_vertices(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.vertices);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normals(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.normals);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_friction(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.friction);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_friction(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.friction=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_collisionMassScale(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.collisionMassScale);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_collisionMassScale(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.collisionMassScale=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useContinuousCollision(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.useContinuousCollision);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useContinuousCollision(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.useContinuousCollision=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useVirtualParticles(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.useVirtualParticles);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useVirtualParticles(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.useVirtualParticles=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_coefficients(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.coefficients);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_coefficients(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			UnityEngine.ClothSkinningCoefficient[] v;
			checkType(l,2,out v);
			self.coefficients=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_worldVelocityScale(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.worldVelocityScale);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_worldVelocityScale(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.worldVelocityScale=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_worldAccelerationScale(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.worldAccelerationScale);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_worldAccelerationScale(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.worldAccelerationScale=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_solverFrequency(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.solverFrequency);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_solverFrequency(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.solverFrequency=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_capsuleColliders(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.capsuleColliders);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_capsuleColliders(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			UnityEngine.CapsuleCollider[] v;
			checkType(l,2,out v);
			self.capsuleColliders=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sphereColliders(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			pushValue(l,self.sphereColliders);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sphereColliders(IntPtr l) {
		try {
			UnityEngine.Cloth self=(UnityEngine.Cloth)checkSelf(l);
			UnityEngine.ClothSphereColliderPair[] v;
			checkType(l,2,out v);
			self.sphereColliders=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Cloth");
		addMember(l,ClearTransformMotion);
		addMember(l,SetEnabledFading);
		addMember(l,"sleepThreshold",get_sleepThreshold,set_sleepThreshold,true);
		addMember(l,"bendingStiffness",get_bendingStiffness,set_bendingStiffness,true);
		addMember(l,"stretchingStiffness",get_stretchingStiffness,set_stretchingStiffness,true);
		addMember(l,"damping",get_damping,set_damping,true);
		addMember(l,"externalAcceleration",get_externalAcceleration,set_externalAcceleration,true);
		addMember(l,"randomAcceleration",get_randomAcceleration,set_randomAcceleration,true);
		addMember(l,"useGravity",get_useGravity,set_useGravity,true);
		addMember(l,"enabled",get_enabled,set_enabled,true);
		addMember(l,"vertices",get_vertices,null,true);
		addMember(l,"normals",get_normals,null,true);
		addMember(l,"friction",get_friction,set_friction,true);
		addMember(l,"collisionMassScale",get_collisionMassScale,set_collisionMassScale,true);
		addMember(l,"useContinuousCollision",get_useContinuousCollision,set_useContinuousCollision,true);
		addMember(l,"useVirtualParticles",get_useVirtualParticles,set_useVirtualParticles,true);
		addMember(l,"coefficients",get_coefficients,set_coefficients,true);
		addMember(l,"worldVelocityScale",get_worldVelocityScale,set_worldVelocityScale,true);
		addMember(l,"worldAccelerationScale",get_worldAccelerationScale,set_worldAccelerationScale,true);
		addMember(l,"solverFrequency",get_solverFrequency,set_solverFrequency,true);
		addMember(l,"capsuleColliders",get_capsuleColliders,set_capsuleColliders,true);
		addMember(l,"sphereColliders",get_sphereColliders,set_sphereColliders,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.Cloth),typeof(UnityEngine.Component));
	}
}

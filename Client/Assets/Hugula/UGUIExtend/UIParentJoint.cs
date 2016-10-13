// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;

[SLua.CustomLuaClass]
[ExecuteInEditMode]
public class UIParentJoint : MonoBehaviour {

	//索引
	public int index = 0;

	public void AddChild(Transform trans,int order)
	{
		trans.SetParent(this.transform,false);
	}
	
}

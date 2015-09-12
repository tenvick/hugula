// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections.Generic;
[SLua.CustomLuaClass]
public class ReferGameObjects : MonoBehaviour {
	
	public List<GameObject> refers=new List<GameObject>();

    public List<Behaviour> monos = new List<Behaviour>();
	
	public object userObject;
	
	public bool userBool;
	
	public int userInt;
	
	public float userFloat;
	
	public string userString;
}

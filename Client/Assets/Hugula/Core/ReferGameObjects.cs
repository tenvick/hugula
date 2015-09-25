// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
[SLua.CustomLuaClass]
public class ReferGameObjects : MonoBehaviour {
	
	public GameObject[] refers ;//=new List<GameObject>();

	public Behaviour[] monos;// = new List<Behaviour>();
	
	public object userObject;
	
	public bool userBool;
	
	public int userInt;
	
	public float userFloat;
	
	public string userString;
}

// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using ICSharpCode.SharpZipLib.Zip;
using System.IO;
/// <summary>
/// 
/// </summary>
public class Begin : MonoBehaviour {
	
	public bool editorDebug=false;

    public string enterLua = "main";

    private LHighway multipleLoader;

    void Awake()
    {
        multipleLoader = LHighway.instance;
    }

	// Use this for initialization
	void Start () 
	{
        LuaBegin();
	}

	#region init
	
	void LuaBegin()
    {
        //Debug.Log("LuaBegin");
		PLua luab=this.gameObject.GetComponent<PLua>();
		if(luab==null)
		{
            PLua.isDebug = editorDebug;
            PLua.enterLua = this.enterLua;
            PLua p=gameObject.AddComponent<PLua>();
        }
        else if (luab.enabled == false)
        {
            luab.enabled = true;
        }
	
	}
	#endregion

    #region protected

    #endregion
}

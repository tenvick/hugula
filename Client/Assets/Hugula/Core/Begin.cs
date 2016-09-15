// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Hugula
{
    /// <summary>
    /// 开始脚本，入口
    /// </summary>
    public class Begin : MonoBehaviour
    {

        public string enterLua = "main";

    //private LHighway multipleLoader;

    void Awake()
    {
        //multipleLoader = LHighway.instance;
    }

	// Use this for initialization
	void Start () 
	{
        LuaBegin();
	}

	#region init
	
  void LuaBegin()
        {
			PLua.enterLua = this.enterLua;
			if(PLua.instance!=null)
			{
				#if UNITY_EDITOR
				Debug.Log ("Lua Begin " + this.enterLua);
				#endif
			}
        }

	#endregion

        #region protected
        #endregion
    }
}

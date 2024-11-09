﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
public class TestEnterLua : MonoBehaviour
{
    public string enterLua;
    // Start is called before the first frame update
    public TextAsset luaAsset;
    void Awake()
    {
        Hugula.EnterLua.luaenv = new LuaEnv();
        Hugula.EnterLua.luaenv.AddLoader((ref string name,ref int length) =>
        {
            string name1 = name.Replace('.', '/');
            string path = Application.dataPath + "/Lua/" + name1 + ".lua";
            var str = File.ReadAllBytes(path);
            length = str.Length;
            return str;
        });

        Hugula.EnterLua.luaenv.DoString("require('" + enterLua + "')");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F5))
        {
            // package.loaded["common.protocol"] = nil
            string luastr = $"   package.loaded[\"{enterLua}\"] = nil; require(\"{enterLua}\") ; print(\"refresh:{enterLua}\");     ";
            // Debug.Log(luastr);
            Hugula.EnterLua.luaenv.DoString(luastr);

        }
    }
}
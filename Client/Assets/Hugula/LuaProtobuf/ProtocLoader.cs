using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Hugula.Framework;

namespace Hugula
{
    [XLua.LuaCallCSharp]
    public class ProtocLoader : Singleton<ProtocLoader>, IDisposable
    {
        // public const string PROTO_SUFFIX = ".proto";
        public ProtocLoader()
        {

        }

        public string Load(string name)
        {
            string str = string.Empty;
#if UNITY_EDITOR
            string path = Application.dataPath + "/Lua/proto/" + name;
            if (File.Exists(path) && Hugula.EnterLua.isDebug)
            {
                str = File.ReadAllText(path); //LuaState.CleanUTF8Bom(
            }
            else
            {
                str = LoadResource(name);
            }
#else
            str =  LoadResource(name);
#endif

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(str))
            {
                if (Hugula.EnterLua.isDebug)
                    Debug.LogErrorFormat("protobuf ({0}) path={1} not exists.", name, path);
                else
                    Debug.LogErrorFormat("the protobuf(Assets/Proto/Resources/proto/{0}) did't exists.", name);
            }

#endif

            return str;
        }

        string LoadResource(string name)
        {
            var textAsset = (TextAsset)Resources.Load("proto/" + name);
            if (textAsset != null)
            {
                var ret = textAsset.text;
                Resources.UnloadAsset(textAsset);
                return ret;
            }
            else
                return null;
        }
    }
}

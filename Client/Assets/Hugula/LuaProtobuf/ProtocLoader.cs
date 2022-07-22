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

        public override void  Reset()
        {

        }

        public string Load(string name)
        {
            string str = string.Empty;
#if UNITY_EDITOR
            string path = Application.dataPath + "/proto/" + name;
            if (!ResLoader.Ready || Hugula.EnterLua.isDebug)
            {
                str = File.ReadAllText(path); //LuaState.CleanUTF8Bom(
            }
            else
            {
                // name = name.Replace(".", "+");
                str = LoadResource(name);
            }
#else
            // name = name.Replace(".", "+");
            str =  LoadResource(name);
#endif

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(str))
            {
                if (Hugula.EnterLua.isDebug)
                    Debug.LogErrorFormat("protobuf ({0}) path={1} not exists.", name, path);
                else
                    Debug.LogErrorFormat("the protobuf(Assets/proto/{0}) did't exists.", name);
            }

#endif

            return str;
        }

        string LoadResource(string name)
        {
            // var textAsset = (TextAsset)Resources.Load("proto/" + name);
            var textAsset = ResLoader.LoadAsset<TextAsset>(name);
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

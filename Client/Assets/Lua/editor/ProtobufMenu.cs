#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using XLua;

namespace HugulaEditor
{

    public class ProtobufMenu
    {
        [MenuItem("Hugula/Lua Protobuf/1.Generate Lua Api List", false, 101)]
        [MenuItem("Assets/Lua Protobuf/1.Generate Lua Api List", false, 201)]
        public static void GenerateApiList()
        {
            luaEnv.DoString("require('editor.build_protobuf')");
            DisposeLua();
        }

        [MenuItem("Hugula/Lua Protobuf/2.Generate Lua protoc_init.lua", false, 102)]
        [MenuItem("Assets/Lua Protobuf/2.Generate Lua protoc_init.lua", false, 202)]
        public static void GenerateProtocInit()
        {
            luaEnv.DoString("require('editor.build_protobuf_init')");
            DisposeLua();
        }



        private static LuaEnv m_LuaEnv;
        public static LuaEnv luaEnv
        {
            get
            {
                if (m_LuaEnv == null)
                {
                    m_LuaEnv = new LuaEnv();
                    m_LuaEnv.AddLoader(Loader);
                }
                return m_LuaEnv;
            }
        }

        public static void DisposeLua()
        {
            if (m_LuaEnv != null) m_LuaEnv.Dispose();
            m_LuaEnv = null;
        }
        /// <summary>
        ///  loader
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static byte[] Loader(ref string name)
        {
            byte[] str = null;
            string name1 = name.Replace('.', '/');
            string path = Application.dataPath + "/Lua/" + name1 + ".lua";

            if (File.Exists(path))
            {
                str = File.ReadAllBytes(path); //LuaState.CleanUTF8Bom(
            }
            else
            {
                Debug.LogErrorFormat("the file({0},{1}) did't exist. ", name, path);
            }

            return str;
        }
    }
}

#endif
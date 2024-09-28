using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Hugula;
using HugulaEditor;
using System.Buffers;
using System.Linq;
using System.IO;
using System;

namespace Tests
{
    public class LuaByteCodeTest 
    {
        // A Test behaves as an ordinary method
        [Test]
        public void LuaByteCodeTestSimplePasses()
        {
            // Use the Assert class to test conditions
            var luaName = "begin.lua";
            var xfile = "begin.bytes";          

            
            var LUA_ROOT_PATH = Application.dataPath + "/Lua/";
            var LUA_OUT_PATH = Application.dataPath + "/lua_bundle/";

            if(!Directory.Exists(LUA_OUT_PATH)) Directory.CreateDirectory(LUA_OUT_PATH);

            string source = LUA_ROOT_PATH + luaName;
            string dest = LUA_OUT_PATH + xfile;
            LuaEditorTool.LuaJitCmd(source, dest,false);

            var merge = File.ReadAllBytes(dest);
            var source64 = File.ReadAllBytes(LUA_OUT_PATH + xfile +".64");

            int length = 0;
            LuaByteCode.Patch64Bytecode(merge, out length);

            var decode64 = new byte[length];
            Buffer.BlockCopy(merge, 0, decode64, 0, length);

            bool areEqual1 = source64.SequenceEqual(decode64);
            Debug.Log($"areEqual1={areEqual1}  length={length} source64={source64.Length} decode64={decode64.Length}");

            var source32 = File.ReadAllBytes(LUA_OUT_PATH + xfile +".32");
            LuaByteCode.Patch32Bytecode(merge, out length);

            var decode32 = new byte[length];
            Buffer.BlockCopy(merge, 0, decode32, 0, length);

            bool areEqual2 = source32.SequenceEqual(decode32);
            Debug.Log($"areEqual2={areEqual1}  length={length} source32={source32.Length} decode32={decode32.Length}");

            Assert.IsTrue(areEqual1 && areEqual2);

        }

    }
}

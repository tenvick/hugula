using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LuaInterface;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace SLua
{

    public partial class LuaObject
    {
        static public bool checkArray(IntPtr l, int p, out byte[] v)
        {
            LuaDLL.luaL_checktype(l, p, LuaTypes.LUA_TSTRING);
            int strLen;
            IntPtr str = LuaDLL.luaS_tolstring32(l, p, out strLen);
            if (strLen > 0)
            {
                v = new byte[strLen];
                Marshal.Copy(str, v, 0, strLen);
            }
            else
            {
                v = null;
            }
            return true;
        }

        static public bool checkType(IntPtr l, int p, out byte[] v)
        {
            LuaDLL.luaL_checktype(l, p, LuaTypes.LUA_TSTRING);

            int strLen;
            IntPtr str = LuaDLL.luaS_tolstring32(l, p, out strLen);
            if (strLen > 0)
            {
                v = new byte[strLen];
                Marshal.Copy(str, v, 0, strLen);
            }
            else
            {
                v = null;
            }
            return true;
        }

        public static void pushValue(IntPtr l, byte[] buffer)
        {
			if (buffer != null)
				LuaDLLWrapper.luaS_pushlstring (l, buffer, buffer.Length);
			else
				LuaDLL.lua_pushnil (l);
        }

        public static void checkValueType(IntPtr l, int p, out LayerMask lm)
        {
            int v;
            checkType(l, p, out v);
            lm = v;
        }
    }

    //public partial class LuaState : IDisposable
    //{
    //    public LuaFunction loadString(string str)
    //    {
    //        return loadString(str, "chunkname");
    //    }

    //    public LuaFunction loadString(string str, string chunkname)
    //    {
    //        byte[] bytes = Encoding.UTF8.GetBytes(str);

    //        object obj;
    //        if (loadBuffer(bytes, chunkname, out obj))
    //            return obj as LuaFunction;
    //        return null; ;
    //    }

    //    public bool loadBuffer(byte[] bytes, string fn, out object ret)
    //    {
    //        ret = null;
    //        int errfunc = LuaObject.pushTry(L);
    //        if (LuaDLL.luaL_loadbuffer(L, bytes, bytes.Length, fn) == 0)
    //        {
    //            LuaDLL.lua_remove(L, errfunc); // pop error function
    //            ret = topObjects(errfunc - 1);
    //            return true;
    //        }
    //        string err = LuaDLL.lua_tostring(L, -1);
    //        LuaDLL.lua_pop(L, 2);
    //        throw new Exception(err);
    //    }
    //}

    public partial class LuaFunction : LuaVar
    {
        #region extends

        public object call(int a1)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            if (innerCall(1, error))
            {
                return state.topObjects(error - 1);
            }


            return null;
        }

        public object call(float a1)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            if (innerCall(1, error))
            {
                return state.topObjects(error - 1);
            }


            return null;
        }

        public object call(Vector2 a1)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            if (innerCall(1, error))
            {
                return state.topObjects(error - 1);
            }


            return null;
        }

        public object call(Vector3 a1)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            if (innerCall(1, error))
            {
                return state.topObjects(error - 1);
            }


            return null;
        }

        public object call(string a1)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            if (innerCall(1, error))
            {
                return state.topObjects(error - 1);
            }


            return null;
        }

        public object call(object a1, Vector2 a2)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushVar(state.L, a1);
            LuaObject.pushValue(state.L, a2);
            if (innerCall(2, error))
            {
                return state.topObjects(error - 1);
            }
            return null;
        }

        public object call(object a1, Vector3 a2)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushVar(state.L, a1);
            LuaObject.pushValue(state.L, a2);
            if (innerCall(2, error))
            {
                return state.topObjects(error - 1);
            }
            return null;
        }

        public object call(object a1, bool a2)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushVar(state.L, a1);
            LuaObject.pushValue(state.L, a2);
            if (innerCall(2, error))
            {
                return state.topObjects(error - 1);
            }
            return null;
        }

        public object call(string a1, object a2)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            LuaObject.pushVar(state.L, a2);
            if (innerCall(2, error))
            {
                return state.topObjects(error - 1);
            }
            return null;
        }

        public object call(int a1, UnityEngine.Object a2)
        {
            int error = LuaObject.pushTry(state.L);

            LuaObject.pushValue(state.L, a1);
            LuaObject.pushVar(state.L, a2);
            if (innerCall(2, error))
            {
                return state.topObjects(error - 1);
            }
            return null;
        }
        #endregion
    }
}
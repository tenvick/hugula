using UnityEngine;
using System.Collections;

namespace Hugula.Loader
{

    [SLua.CustomLuaClass]
    public class LoadingEventArg
    {
        //public int number;//current loading number
        public object target;
        public int total;
        public int current;
        public float progress;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;

namespace Hugula.Framework
{
    public class LuaNotifyComponent : LuaComponent
    {
        // Start is called before the first frame update
        void Start()
        {
            var notify = EnterLua.luaenv.Global.GetInPath<INotifyPropertyChanged>(luaPath);
            if (notify != null)
            {
                luaViewModel = notify;
                container.context = luaViewModel;
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarningFormat(" invalid return value in LuaComponent({0}) ", luaPath);
            }
#endif
        }


    }
}

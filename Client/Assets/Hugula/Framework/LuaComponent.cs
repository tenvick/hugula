using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;

namespace Hugula.Framework
{
    public class LuaComponent : MonoBehaviour
    {
        [Tooltip("require lua的相对路径")]
        public string luaPath;
        [Tooltip("BindableObject相当于view")]
        public BindableObject container;

        public object luaViewModel;

        // Start is called before the first frame update
        void Start()
        {
            var vm = EnterLua.luaenv.DoString("require('" + luaPath + "')()", luaPath);
            if (vm != null)
            {
                luaViewModel = vm[0];
                container.context = luaViewModel;
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarningFormat(" invalid return value in LuaComponent({0}) ", luaPath);
            }
#endif
        }

        void OnDestroy()
        {
            container.ClearBinding();
            container.context = null;
            luaViewModel = null;
        }

    }
}

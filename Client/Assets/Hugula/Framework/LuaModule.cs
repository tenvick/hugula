using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;

namespace Hugula.Framework
{
    //将lua viewmodel以模块的形式挂接在GameObject上
    public class LuaModule : MonoBehaviour
    {
        [Tooltip("vm_config.lua中配置的name,viewmodel层")]
        public string vmConfigName;
        [Tooltip("BindableObject相当于view")]
        public BindableObject container;

        // Start is called before the first frame update
        void Start()
        {
            var vm = EnterLua.luaenv.Global.GetInPath<INotifyPropertyChanged>("VMgenerate." + vmConfigName);
            container.context = vm;
        }

    }
}

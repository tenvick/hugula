using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;
using Hugula.Mvvm;

namespace Hugula.Framework
{
    //将lua viewmodel以模块的形式挂接在GameObject上
    public class LuaModule : MonoBehaviour
    {
        internal const string PUSH_FUN_NAME = "on_push";

        [Tooltip("vm_config.lua中配置的name,viewmodel层")]
        public string vmConfigName;
        [Tooltip("BindableObject相当于view")]
        public BindableObject container;

        // Start is called before the first frame update
        private bool m_IsInited = false;

        void Awake()
        {
            VMStateHelper.instance?.CallFunc(vmConfigName, PUSH_FUN_NAME, null);//
        }

        void Start()
        {
            // var vm = EnterLua.luaenv.Global.GetInPath<INotifyPropertyChanged>("VMgenerate." + vmConfigName);
            // container.context = vm;
            VMStateHelper.instance?.InitViewmodel(vmConfigName, container);
            m_IsInited = true;
        }

        private void OnEnable()
        {
            if (m_IsInited)
                VMStateHelper.instance?.Active(vmConfigName, null);
        }

        private void OnDisable()
        {
            VMStateHelper.instance?.Dactive(vmConfigName);
        }

        private void OnDestroy()
        {
            VMStateHelper.instance?.DestroyViewmodel(vmConfigName);

        }

    }
}

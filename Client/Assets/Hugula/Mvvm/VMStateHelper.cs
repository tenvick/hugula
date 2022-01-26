using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Framework;
using System;

namespace Hugula.Mvvm
{
    [XLua.LuaCallCSharp]
    public class VMStateHelper : Singleton<VMStateHelper>, IDisposable
    {
        private IVMState m_VMState;
        private const string VM_NAME = "VMState";
        public VMStateHelper()
        {
            m_VMState = EnterLua.luaenv?.Global.GetInPath<IVMState>(VM_NAME);
        }

        public override void Reset()
        {
            m_VMState = null;
        }
        public override void Dispose()
        {
            base.Dispose();
            m_VMState = null;
        }

        public object GetMemeber(string vm_name, string mebmer_name)
        {
            return m_VMState.get_member(vm_name, mebmer_name);
        }

        public void CallFunc(string vm_name, string fun_name, object arg)
        {
            m_VMState.call_func(vm_name, fun_name, arg);
        }

        public void PushItem(string vm_name, object arg)
        {
            m_VMState?.push_item(vm_name, arg);
        }

        public void PopupItem(string vm_name)
        {
            m_VMState?.popup_item(vm_name);
        }

        public void PushGroup(string vm_name, object arg)
        {
            m_VMState?.push(vm_name, arg);
        }

        public void Active(string vm_name, object arg)
        {
            m_VMState?.active(vm_name, arg);
        }

        public void Dactive(string vm_name)
        {
            m_VMState?.deactive(vm_name);
        }

        public void InitViewmodel(string vm_name, Hugula.Databinding.BindableObject bindalbeObject)
        {
            m_VMState?.init_viewmodel(vm_name, bindalbeObject);
        }

        public void DestroyViewmodel(string vm_name)
        {
            m_VMState?.destroy_viewmodel(vm_name);
        }

        public void Back()
        {
            m_VMState?.back();
        }

        [XLua.CSharpCallLua]
        public interface IVMState
        {
            object get_member(string vm_name, string member_name);
            void call_func(string vm_name, string fun_name, object arg);
            void push_item(string vm_name, object arg);
            void push(string vm_group_name, object arg);
            void popup_item(string vm_name);
            void active(string vm_name, object arg);
            void deactive(string vm_name);
            /// <summary>
            /// 用于luamodule初始化
            /// </summary>
            /// <param name="vm_name"></param>
            /// <param name="container"></param>
            void init_viewmodel(string vm_name, object container);
            /// <summary>
            /// 销毁viewmodel
            /// </summary>
            /// <param name="vm_name"></param>
            void destroy_viewmodel(string vm_name);
            void back();
        }
    }
}

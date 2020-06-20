using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Framework;
using System;

namespace Hugula.Mvvm
{
    public class VMStateHelper : Singleton<VMStateHelper>, IDisposable
    {
        private IVMState m_VMState;
        private const string VM_NAME = "VMState";
        public VMStateHelper()
        {
            m_VMState = EnterLua.luaenv?.Global.GetInPath<IVMState>(VM_NAME);
        }

        public override void Dispose()
        {
            base.Dispose();
            m_VMState = null;
        }

        public XLua.LuaBase GetMemeber(string vm_name, string mebmer_name)
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

        public void Back()
        {
            m_VMState?.back();
        }

        [XLua.CSharpCallLua]
        public interface IVMState
        {
            XLua.LuaBase get_member(string vm_name, string member_name);
            void call_func(string vm_name, string fun_name, object arg);
            void push_item(string vm_name, object arg);
            void push(string vm_group_name, object arg);
            void popup_item(string vm_name);
            void back();
        }
    }
}

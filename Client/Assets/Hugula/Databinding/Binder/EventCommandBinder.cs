// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace Hugula.Databinding.Binder {
    [LuaCallCSharp]
    ///<summary>
    /// C# event 装换为 执行ICommand或者 exute
    ///<summary>
    public class EventCommandBinder : MonoBehaviour {
        public ICommand command { get; set; }
        public object parameter{get;set;}
        public void ExcuteCommand () {
            if (command != null && command.CanExecute (parameter))
                command.Execute (parameter);
        }
    }
}
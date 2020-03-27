// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {
    ///<summary>
    /// C# event 装换为 执行ICommand或者 exute
    ///<summary>
    public class EventExecuteBinder : MonoBehaviour {
        public object parameter {get;set;}
   
        public IExecute execute { get; set; }
        public void Excute () {
            if (execute != null)
                execute.Execute (parameter);
        }

        void OnDestroy()
        {
            execute = null;
            parameter = null;
        }

    }
}
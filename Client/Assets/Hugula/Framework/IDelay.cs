using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Framework {
    public interface IDelayDeactive:IDelayCancel {
        void DelayDeactive ();
    }

    public interface IDelayDestory:IDelayCancel {
        void DelayDestory ();

        System.Action onDelayCompleted{get;set;}
    }

    public interface IDelayCancel
    {
        void CancelDelay();
    }
}
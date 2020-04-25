using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Framework {
    public interface IDelayDeactive {
        void DelayDeactive ();
    }

    public interface IDelayDestory {
        void DelayDestory ();

        System.Action onComplete{get;set;}
    }

    public interface IDelayCancel
    {
        void CancelDelay();
    }
}
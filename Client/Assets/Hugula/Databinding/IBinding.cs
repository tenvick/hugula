using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding {

    public interface IBinding {
        void UpdateTarget ();
        void Apply (object context, bool invoke = true);
        void Unapply ();
        void UpdateSource ();
    }
}
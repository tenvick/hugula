using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;

namespace Hugula.UIComponents {

    public class DelayDeactiveBehaviour : MonoBehaviour, IDelayDeactive, IDelayDestory {
        public float delayTime = 1;
        public System.Action onComplete { get; set; }
        public void DelayDeactive () {
            StartCoroutine (Deactive ());
        }

        IEnumerator Deactive () {
            yield return new WaitForSeconds (delayTime);
            this.gameObject.SetActive (false);
        }

        public void DelayDestory () {
            StartCoroutine (Destory ());
        }

        IEnumerator Destory () {
            yield return new WaitForSeconds (delayTime);
            if (onComplete != null) onComplete ();
            GameObject.Destroy (this.gameObject);
        }
    }
}
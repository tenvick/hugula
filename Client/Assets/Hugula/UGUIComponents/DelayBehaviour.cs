using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Framework;

namespace Hugula.UIComponents
{

    public class DelayDeactiveBehaviour : MonoBehaviour, IDelayDeactive, IDelayDestory
    {
        public float delayTime = 1;
        public System.Action onDelayCompleted { get; set; }
        public void DelayDeactive()
        {
            StartCoroutine(Deactive());
        }

        IEnumerator Deactive()
        {
            yield return new WaitForSeconds(delayTime);
            this.gameObject.SetActive(false);
        }

        public void DelayDestory()
        {
            StartCoroutine(Destory());
        }

        IEnumerator Destory()
        {
            yield return new WaitForSeconds(delayTime);
            GameObject.Destroy(this.gameObject);
            if (onDelayCompleted != null) onDelayCompleted();
        }

        public void CancelDelay()
        {
            StopAllCoroutines();
        }
    }
}
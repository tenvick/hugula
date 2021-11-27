using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;

namespace Hugula.UI 
{

    public class UISubContainer : MonoBehaviour
    {
        // [Tooltip("挂接的容器名")]
        // public string uiSubContainerName;
        // Start is called before the first frame update
        private void Awake()
        {
            // if (string.IsNullOrEmpty(uiSubContainerName))
            //     uiSubContainerName = this.name;
            UISubManager.instance?.AddContainer(this.name, this.GetComponent<Transform>());
        }

        private void Start()
        {
            UISubManager.instance?.AddContainer(this.name, this.GetComponent<Transform>());
        }

        void OnDestroy()
        {
            UISubManager.instance?.RemoveContainer(name);
        }
    }

    public class UISubManager : Singleton<UISubManager>, IDisposable
    {
        public UISubManager()
        {

        }

        public override void Reset()
        {
            mContainers.Clear();
        }

        Dictionary<string, Transform> mContainers = new Dictionary<string, Transform>();
        public void AddContainer(string name, Transform transform)
        {
            mContainers.Remove(name);
            mContainers.Add(name, transform);
        }

        public void RemoveContainer(string name)
        {
            mContainers.Remove(name);
        }

        public Transform GetContainer(string name)
        {
            Transform transform;
            if (mContainers.TryGetValue(name, out transform))
                return transform;
            else
                return null;
        }
    }
}
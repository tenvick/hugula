using System.Collections;
using Hugula.Databinding;
using UnityEngine;
using Hugula.Mvvm;

namespace Hugula.UI
{
    public class PopupItemWhenMoving : BindableObject
    {
        [Tooltip("vm_config 配置的模块名")]
        [SerializeField] string vmName;
        public Transform followTrans { get { return null; } set {  Follow(); } }

        bool check;
        Vector3 pos;
        Transform m_Transform;

        void Start()
        {
            m_Transform = transform;
        }

        // Update is called once per frame
        void Update()
        {
            if(check && pos.y- m_Transform.position.y >0.5)
            {
                check = false;
                VMStateHelper.instance.PopupItem(vmName); //取消提示
            }
        }

        void Follow()
        {
            pos = m_Transform.position;
            check = true;
        }
    }
}
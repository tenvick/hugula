using System.Collections;
using System.Collections.Generic;
using Hugula.Converter;
using Hugula.Databinding;
using UnityEngine;

namespace Hugula.UI
{
    [ExecuteInEditMode]
    public class FollowTransform : BindableObject
    {
        [SerializeField]
        Transform m_FollowTrans;
        public Transform followTrans
        {
            get { return m_FollowTrans; }
            set
            {
                m_FollowTrans = value;
                isFollowing = true;
                Follow();
            }
        }

        [Tooltip(" followKey 用到的 Convert")]
        [SerializeField] string m_Convert;
        string m_FollowKey;
        public string followKey
        {
            get { return m_FollowKey; }
            set
            {
                m_FollowKey = value;
                if (string.IsNullOrEmpty(m_FollowKey))
                {
                    m_FollowTrans = null;
                    return;
                }
                m_FollowTrans = null;
                isFollowing = true;
                Follow();
            }
        }

        public string gObjname
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        public bool once;

        public Vector3 offset;
        public Vector3 Offset
        {
            get { return offset; }
            set
            {
                offset = value;
            }
        }

        public bool rotationX = false;
        public bool rotationY = false;
        public bool rotationZ = false;
        /// <summary>
        /// 标记follow状态
        /// </summary>
        bool isFollowing = true;
        // enum Axis
        // {
        //     X, Y, Z
        // }

        // Update is called once per frame
        void Update()
        {
            Follow();
        }

        private void OnEnable()
        {
            Follow();
        }

        void Follow()
        {
            if (!isFollowing) return;
            if (followTrans == null && !string.IsNullOrEmpty(m_FollowKey)) //&& ((delayFrame > 0 && beginFrame + delayFrame <= Time.frameCount) || delayFrame <= 0))
            {
                var convert = ValueConverterRegister.instance.Get(m_Convert);
                if (convert is IValueConverter)
                {
                    var value = ((IValueConverter)convert).Convert(m_FollowKey, null);
                    if (value is Transform)
                    {
                        m_FollowTrans = (Transform)value;
                    }
                }
            }
            else if (followTrans != null)
            {
                var pos = followTrans.position;
                // pos.y = transform.position.y;
                this.transform.position = pos;
                this.transform.localPosition += offset;
                if (rotationX || rotationY || rotationZ)
                {
                    var rotation = transform.rotation.eulerAngles;
                    var followRota = followTrans.rotation.eulerAngles;
                    if (rotationX)
                        rotation.x = followRota.x;
                    if (rotationY)
                        rotation.y = followRota.y;
                    if (rotationZ)
                        rotation.z = followRota.z;
                    // if (rotation) transform.rotation = followTrans.rotation;
                    transform.rotation = Quaternion.Euler(rotation);
                }
                if (once)
                {
                    followTrans = null;
                    isFollowing = false;
                }
            }
        }

        protected override void OnDestroy()
        {
            followTrans = null;
        }
    }
}

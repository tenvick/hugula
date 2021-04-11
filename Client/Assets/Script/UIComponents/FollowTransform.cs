using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;

namespace Hugula.UI
{
    [ExecuteInEditMode]
    public class FollowTransform : BindableObject
    {
        [SerializeField]
        Transform m_FollowTrans;
        public Transform followTrans { get { return m_FollowTrans; } set { m_FollowTrans = value; Follow(); } }

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

        public bool rotationX = false;
        public bool rotationY = false;
        public bool rotationZ = false;

        // enum Axis
        // {
        //     X, Y, Z
        // }

        // Update is called once per frame
        void Update()
        {
            Follow();
        }

        void Follow()
        {
            if (followTrans != null)
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
                if (once) followTrans = null;
            }
        }

        protected override void OnDestroy()
        {
            followTrans = null;
        }
    }
}

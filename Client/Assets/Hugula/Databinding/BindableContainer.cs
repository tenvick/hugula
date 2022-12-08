using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Hugula.Databinding
{
    [LuaCallCSharp]
    public class BindableContainer : BindableObject, ICollectionBinder
    {
        protected bool m_CheckEnable = false;
        private bool m_NeedDelayBinding = false;

        /// <summary>
        /// 当前gameobject.selfActive为true的时候才绑定children
        /// </summary>
        [Tooltip("当前gameobject.selfActive为true的时候才绑定children")]
        [SerializeField] bool m_BindingOnEnable = true;
        ///<summary>
        /// 绑定的对象
        ///<summary>
        [HideInInspector]
        [BindableObjectAttribute]
        public List<BindableObject> children = new List<BindableObject>();

        #region mono单脚本引用新增加

        private XLua.LuaTable m_LuaMonos;
        public XLua.LuaTable luaMonos
        {
            get
            {
                if (m_LuaMonos == null)
                {
                    m_LuaMonos = EnterLua.luaenv.NewTable();
                    string key =string.Empty; 
                    for (int i = 0; i < monos.Count; i++)
                    {
                        key = names[i];
                        m_LuaMonos.Set<string,Object>(key,monos[i]);
                    }
                }

                return m_LuaMonos;
            }
        }

        [HideInInspector]
        public List<string> names = new List<string>();
        ///<summary>
        /// 绑定的对象
        ///<summary>
        [HideInInspector]
        public List<Object> monos = new List<Object>();

        /// <summary>
        /// monos的长度
        /// </summary>
        public int monoLength
        {
            get
            {
                if (monos != null)
                    return monos.Count;
                else
                    return 0;
            }
        }

        private System.Action<object, object> m_OnContextChanged;
        public System.Action<object, object> onContextChanged
        {
            get { return m_OnContextChanged; }
            set
            {
                m_OnContextChanged = value;
            }
        }
        #endregion

        public void AddChild(BindableObject child)
        {
            children.Add(child);
        }

        public IList<BindableObject> GetChildren()
        {
            return children;
        }

        private void CheckDoOnBindingContextChanged()
        {
            if (m_CheckEnable)
                OnBindingContextChanged();
            else
            {
                m_NeedDelayBinding = true;
#if UNITY_EDITOR
                Debug.LogWarningFormat("dont awake CheckDoOnBindingContextChanged({0}) frame={1} ", this, Time.frameCount);
#endif
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (m_CheckEnable || !m_BindingOnEnable)
            {
                base.OnBindingContextChanged();
                BindableObject child;
                for (int i = 0; i < children.Count; i++)
                // foreach (var child in children)
                {
                    child = children[i];
                    if (child)
                        child.SetInheritedContext(context, true);
                    else
                    {
                        Debug.LogErrorFormat("OnBindingContextChanged({0}) children index({1})  is null ", Hugula.Utils.CUtils.GetGameObjectFullPath(this.gameObject), i);
                    }
                }

                if (m_OnContextChanged != null)
                {
                    m_OnContextChanged(luaMonos, context);
                }
            }
            else
                m_NeedDelayBinding = true;
        }

        public override void ClearBinding()
        {
            base.ClearBinding();
            foreach (var child in children)
                child?.ClearBinding();
            children.Clear();
        }

        //Unapply binding listenler
        public override void Unapply()
        {
            base.Unapply();
            foreach (var child in children)
                child.Unapply();
        }

        #region mono

        protected override void Awake()
        {
            base.Awake();
            m_CheckEnable = true;
        }

        protected void OnEnable()
        {
            if (m_NeedDelayBinding)
            {
                m_NeedDelayBinding = false;
                OnBindingContextChanged();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(m_LuaMonos!=null)
            {
                m_LuaMonos.Dispose();
            }
            m_LuaMonos = null;
        }
        #endregion
    }
}
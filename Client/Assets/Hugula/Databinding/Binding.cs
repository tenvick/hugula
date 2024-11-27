// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding
{

    public enum BindingMode
    {
        OneWay,
        TwoWay,
        OneWayToSource,
    }

    [System.SerializableAttribute]
    public sealed class Binding : IDisposable, IBinding
    {
        internal const string SelfPath = ".";

        #region  序列化属性
        ///<summary>
        /// The type of the property
        ///</summary>
        // public Type returnType;

        ///<summary>
        /// The name of the BindableProperty
        ///</summary>
        public string propertyName;

        public string path;
        // public string format;
        public string converter;
        public BindingMode mode;

        [PopUpComponentsAttribute]
        public UnityEngine.Object source;

        /// <summary>
        /// The target object 弱引用
        /// </summary>
        [PopUpComponentsAttribute]
        public UnityEngine.Object target;
        // {
        //     get
        //     {
        //         if (_weakTarget != null && _weakTarget.TryGetTarget(out var target))
        //             return target;
        //         return null;
        //     }
        //     set
        //     {
        //         if (_weakTarget == null)
        //             _weakTarget = new WeakReference<UnityEngine.Object>(value);
        //         else
        //             _weakTarget.SetTarget(value);
        //     }
        // }
        // WeakReference<UnityEngine.Object> _weakTarget;

        public object convert;

        /// <summary>
        /// The next binding in the chain
        /// </summary>
        internal Binding next;

        #endregion

        #region  绑定
      
        // WeakReference<object> _bindingContext;

        /// <summary>
        /// The binding context object 弱引用
        /// </summary>
        public object bindingContext;
        // {
        //     get
        //     {
        //         if (_bindingContext != null && _bindingContext.TryGetTarget(out var context))
        //             return context;
        //         return null;
        //     }
        //     set
        //     {
        //         if (_bindingContext == null)
        //             _bindingContext = new WeakReference<object>(value);
        //         else
        //             _bindingContext.SetTarget(value);
        //     }
        // }

        protected BindingPathPart m_LastPart;
        
        /// <summary>
        ///是否已经绑定过
        /// </summary>
        public bool isBound
        {
            get
            {
                return bindingContext != null;
            }
        }

        protected bool m_IsApplied = false;
        /// <summary>
        /// 是否已经处理过
        /// </summary>
        protected bool m_IsDisposed = false;//{ get; private set; }//销毁标记
        internal bool m_IsProcessed = false; // 新增的标志位

        #endregion

#if LUA_PROFILER_DEBUG
        string ProfilerName = "";
        string GetProfilerName()
        {
            if (string.IsNullOrEmpty(ProfilerName))
                ProfilerName = string.Format("Binding(path={0},prop={1},tar={2})", this.path, this.propertyName, this.target);
            return ProfilerName;
        }
#endif

        public Binding()
        {
            m_IsDisposed = false;
            m_IsProcessed = false;
        }

        public Binding(string path, UnityEngine.Object target, string propertyName, BindingMode mode) : this(path, target, propertyName, mode, "")
        {
            m_IsDisposed = false;
            m_IsProcessed = false;
        }

        public Binding(string path, UnityEngine.Object target, string propertyName, BindingMode mode, string converter)
        {
            m_IsDisposed = false;
            this.path = path;
            this.target = target;
            this.propertyName = propertyName;
            this.mode = mode;
            // this.format = format;
            this.converter = converter;
        }

        #region  表达式与寻值
        //更新目标
        public void UpdateTarget()
        {
            if (!isBound || m_IsDisposed || target == null)
            {
#if UNITY_EDITOR
                var str = string.Empty;
                if (target is Component isi)
                {
                    str = CUtils.GetGameObjectFullPath(isi.gameObject);
                }
                else
                {
                    str = target?.ToString();
                }
                Debug.LogErrorFormat("UpdateTarget(target={0},perpertyName={1},path={2},m_IsDisposed={3}) , Apply(context={4}). \r\n lua:{4} ", str, propertyName, path, m_IsDisposed, bindingContext, Hugula.EnterLua.LuaTraceback());
#endif
                return;
            }

            if (m_LastPart != null && m_LastPart.source != null)
                ExpressionUtility.SetTargetInvoke(m_LastPart.source, target, propertyName, m_LastPart);
            else
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Binding.UpdateTarget Apply() target={0},  propertName={1},context={2},lastPart={3} ", target, propertyName, bindingContext, m_LastPart);
#endif
                Apply(bindingContext);
            }
        }

        //更新源
        public void UpdateSource()
        {
            if (m_IsDisposed || !isBound || target == null)
            {
#if UNITY_EDITOR
                var str = string.Empty;
                if (target is Component isi)
                {
                    str = CUtils.GetGameObjectFullPath(isi.gameObject);
                }
                else
                {
                    str = target?.ToString();
                }
                Debug.LogErrorFormat("UpdateTarget(target={0},perpertyName={1},path={2},m_IsDisposed={3}) , Apply(context={4}). \r\n lua:{4} ", str, propertyName, path, m_IsDisposed, bindingContext, Hugula.EnterLua.LuaTraceback());
#endif
                return;
            }

#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample($"{GetProfilerName()}.UpdateSource.SetSourceInvoke");
#endif
            if (m_LastPart != null && m_LastPart.source != null)
                ExpressionUtility.SetSourceInvoke(m_LastPart.source, m_LastPart);
            else
            {
#if UNIYT_EDITOR
                Debug.LogWarningFormat("Binding.UpdateSource full source {0}", this.path);
#endif
                BindingPathPart part = null;
                object m_Current = bindingContext;
                PropertyChangedEventHandlerEvent propChanged = null;
                bool isSelf = false;
                int count = parts.Count;

                for (var i = 0; i < count; i++)
                {
                    part = m_Parts[i];
                    m_LastPart = part;
                    isSelf = part.isSelf;

                    if (!isSelf && (mode == BindingMode.OneWay || mode == BindingMode.TwoWay))// 监听source
                    {
                        if (m_Current is INotifyPropertyChanged inpc)
                        {
                            part.source = m_Current;
                            part.Subscribe(inpc.PropertyChanged);
                        }
                        else if (m_Current is XLua.LuaTable inc && inc.TryGet<string, PropertyChangedEventHandlerEvent>("PropertyChanged", out propChanged))
                        {
                            part.source = inc;
                            part.Subscribe(propChanged);
                        }
                    }

                    if (!isSelf && m_Current != null && i < count - 1)
                    {
                        part.TryGetValue(m_Current, out m_Current); //
                    }

                    if (!isSelf && m_Current == null && part.nextPart == null)
                        break;

                }

                ExpressionUtility.SetSourceInvoke(m_Current, part);

            }
        }

        /// <summary>
        /// 绑定目标 context 改变后调用方向 target.property = context.path
        /// </summary>
        /// <param name="context"></param>
        public void Apply(object context)
        {
            if (m_IsDisposed)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Binding(target={0},perpertyName={1},path={2}) is Disposed , Apply(context={3}). \r\n lua:{4} ", target, propertyName, path, context, Hugula.EnterLua.LuaTraceback());
#endif
                return;
            }

            if (!m_IsApplied)
            {
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample($"{GetProfilerName()}.ParsePath");
#endif
                ParsePath();
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                m_IsApplied = true;
            }
            else if (!string.IsNullOrEmpty(converter) && convert == null)
            {
                //refresh converter
                convert = ValueConverterRegister.instance?.Get(converter);
            }

            if (isBound) //
            {
                Unapply();
            }

            bindingContext = context;
            if (source) bindingContext = source;

            if (bindingContext == null)
            {
                convert = null; //需要解绑
                return;
            }

            object m_Current = bindingContext;

            bool needsGetter = (mode == BindingMode.TwoWay || mode == BindingMode.OneWay);
            BindingPathPart part = null;

#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample($"{GetProfilerName()}.path TryGetValue");
#endif
            PropertyChangedEventHandlerEvent propChanged = null;
            int count = parts.Count;
            bool isSelf = false;

            for (var i = 0; i < count; i++)
            {
                part = m_Parts[i];
                m_LastPart = part;
                isSelf = part.isSelf;

                if (!isSelf && (mode == BindingMode.OneWay || mode == BindingMode.TwoWay))// 监听source
                {
                    if (m_Current is INotifyPropertyChanged inpc)
                    {
                        part.source = m_Current;
                        part.Subscribe(inpc.PropertyChanged);
                    }
                    else if (m_Current is XLua.LuaTable inc && inc.TryGet<string, PropertyChangedEventHandlerEvent>("PropertyChanged", out propChanged))
                    {
                        part.source = inc;
                        part.Subscribe(propChanged);
                    }
                }

                if (!isSelf && m_Current != null && i < count - 1)
                {
                    part.TryGetValue(m_Current, out m_Current); //
                }

                if (!isSelf && m_Current == null && part.nextPart == null)
                    break;

            }

#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample($"{GetProfilerName()}.Getter_Setter");
#endif
            if (needsGetter)
            {
                ExpressionUtility.SetTargetInvoke(m_Current, target, propertyName, part);
            }

#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif

        }

        /// <summary>
        /// 是否能设置target的值
        /// </summary>
        /// <param name="fromTarget"></param>
        /// <returns></returns>
        internal bool NeedsGetter(bool fromTarget = false)
        {
            return (mode == BindingMode.TwoWay && !fromTarget) || mode == BindingMode.OneWay;
        }

        internal void OnSourceChanged(BindingPathPart currPart, bool fromTarget = false)
        {

            BindingPathPart part = currPart;
            object m_Current = part.source;
            bool isLast2 = false;
            PropertyChangedEventHandlerEvent propChanged = null;
            bool isSelf = false;

            while (part != null && m_Current != null)
            {
                m_LastPart = part;
                isLast2 = part.nextPart != null;
                isSelf = part.isSelf;

                if (currPart != part && (mode == BindingMode.OneWay || mode == BindingMode.TwoWay))// currPart已经监听过了
                {
                    if (m_Current is INotifyPropertyChanged inpc)
                    {
                        part.source = m_Current; // 有PropertyChanged的对象才需要缓存source
                        part.Subscribe(inpc.PropertyChanged);
                    }
                    else if (m_Current is XLua.LuaTable inc && inc.TryGet<string, PropertyChangedEventHandlerEvent>("PropertyChanged", out propChanged))
                    {
                        part.source = inc;// 有PropertyChanged的对象才需要缓存source
                        part.Subscribe(propChanged);
                    }
                }

                if (!isSelf && m_Current != null && isLast2)
                {
                    part.TryGetValue(m_Current, out m_Current); //
                }

                if (!isSelf && m_Current == null)
                    break;

                if (part.nextPart != null)
                    part = part.nextPart;
                else
                    part = null;
            }

            if (m_LastPart != null && m_Current != null)
                m_LastPart.source = m_Current;//

            UpdateTarget();
        }

        //解绑目标
        public void Unapply()
        {
            if (m_Parts == null) return;
            int count = m_Parts.Count;
            for (var i = 0; i < count; i++)
            {
                BindingPathPart part = m_Parts[i];
                part?.Unsubscribe();
            }

        }

        static readonly char[] ExpressionSplit = new[] { '.' };

        //解析的path路径
        List<BindingPathPart> m_Parts;// new List<BindingPathPart>();


        public List<BindingPathPart> parts
        {
            get
            {
                if (m_Parts == null)
                    m_Parts = m_PoolBindingPathPart.Get();
                return m_Parts;
            }
        }
        public void ParsePath()
        {
            if (string.IsNullOrEmpty(path))
            {
#if UNITY_EDITOR
                Debug.LogError("Binding path is null or empty");
#endif
                return;
            }

            string p = path.Trim();

            if (!string.IsNullOrEmpty(converter))
                convert = ValueConverterRegister.instance?.Get(converter);

            parts.Clear();
            BindingPathPart last = null;

            if (p[0] == ExpressionSplit[0])
            {
                if (p.Length == 1)
                {
                    last = BindingPathPart.Get(); //new BindingPathPart(this, SelfPath);
                    last.Initialize(this, SelfPath);
                    parts.Add(last);
                    return;
                }

                p = p.Substring(1);
            }

            string[] pathParts = p.Split(ExpressionSplit);
            for (var i = 0; i < pathParts.Length; i++)
            {
                string part = pathParts[i].Trim();

                if (part == string.Empty)
                    throw new FormatException("Path contains an empty part:" + this.propertyName);

                BindingPathPart indexer = null;
                //索引解析
                int lbIndex = part.IndexOf('[');
                if (lbIndex != -1)
                {
                    int rbIndex = part.LastIndexOf(']');
                    if (rbIndex == -1)
                        throw new FormatException("Indexer did not contain closing [");

                    int argLength = rbIndex - lbIndex - 1;
                    if (argLength == 0)
                        throw new FormatException("Indexer did not contain arguments");

                    string argString = part.Substring(lbIndex + 1, argLength);
                    indexer = BindingPathPart.Get(); //new BindingPathPart(this, argString, true);
                    indexer.Initialize(this, argString, true);
                    part = part.Substring(0, lbIndex);
                    part = part.Trim();
                    indexer.indexerName = part;
                }

                //方法解析
                lbIndex = part.IndexOf('(');
                if (lbIndex != -1)
                {
                    int rbIndex = part.LastIndexOf(')');
                    if (rbIndex == -1)
                        throw new FormatException("Method did not contain closing (");

                    // int argLength = rbIndex - lbIndex - 1;
                    string argString = part.Substring(0, lbIndex);
                    var next = BindingPathPart.Get(); //new BindingPathPart(this, argString);
                    next.Initialize(this, argString, false, true);

                    if (last != null) last.nextPart = next;
                    parts.Add(next);
                    last = next;
                }
                else if (part.Length > 0)
                {
                    var next = BindingPathPart.Get(); //new BindingPathPart(this, part);
                    next.Initialize(this, part);
                    if (last != null) last.nextPart = next;
                    parts.Add(next);
                    last = next;
                }

                if (indexer != null)
                {
                    if (last != null) last.nextPart = indexer;
                    parts.Add(indexer);
                    last = indexer;
                }
            }
        }

        #endregion

        public Binding Clone()
        {
            Binding clone = new Binding();
            clone.path = path;
            clone.propertyName = propertyName;
            clone.converter = converter;
            clone.mode = mode;
            clone.source = source;
            clone.target = target;
            return clone;
        }

        public void Dispose()
        {
            m_IsDisposed = true; //标记销毁
            m_IsApplied = false;
            m_IsProcessed = false;
            convert = null;
            target = null;
            source = null;
            m_LastPart = null;
            bindingContext = null;

            if (m_Parts != null)
            {
                for (var i = 0; i < m_Parts.Count; i++)
                {
                    BindingPathPart part = m_Parts[i];
                    part?.ReleaseToPool();
                }
                m_PoolBindingPathPart.Release(m_Parts);
            }
            m_Parts = null;
            next = null;
        }



        public override string ToString()
        {
            return string.Format("Binding(target={2},path={0},property={1},mode={3})", this.path, this.propertyName, this.target, this.mode);
        }

        #region List pool
        const int capacity = 2048;
        const int initial = 1024;

        private static ObjectPool<List<BindingPathPart>> m_PoolBindingPathPart = new ObjectPool<List<BindingPathPart>>(null, DefaultRelease, capacity, initial);

        private static void DefaultRelease(List<BindingPathPart> toRelease)
        {
            toRelease.Clear();
        }

        #endregion


    }

}
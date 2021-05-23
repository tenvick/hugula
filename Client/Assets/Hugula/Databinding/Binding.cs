// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Hugula.Databinding
{

    public enum BindingMode
    {
        OneWay,
        TwoWay,
        OneWayToSource,
    }

    [System.SerializableAttribute]
    public class Binding : IDisposable, IBinding
    {
        internal const string SelfPath = ".";

        #region  序列化属性
        ///<summary>
        /// The type of the property
        ///</summary>
        public Type returnType;

        ///<summary>
        /// The name of the BindableProperty
        ///</summary>
        public string propertyName;

        public string path;
        public string format;
        public string converter;
        public BindingMode mode;

        [PopUpComponentsAttribute]
        public UnityEngine.Object source;
        public object target;

        public object convert;

        #endregion

        #region  绑定
        //是否已经绑定过
        public bool isBound
        {
            get
            {
                return m_Context != null;
            }
        }

        //引用的上下文
        protected object m_Context;
        protected BindingPathPart m_LastPart;

        protected bool m_IsApplied = false;

        #endregion

        bool m_IsDisposed = false;//销毁标记
#if LUA_PROFILER_DEBUG
        string ProfilerName = "";
        string GetProfilerName()
        {
            if (string.IsNullOrEmpty(ProfilerName))
                ProfilerName = string.Format("Binding(path={0},prop={1},tar={2})", this.path, this.propertyName, this.target);
            return ProfilerName;
        }
        //UnityEngine.Profiling.Profiler.BeginSample();
#endif

        public Binding()
        {

        }

        public Binding(string path, object target, string propertyName, BindingMode mode) : this(path, target, propertyName, mode, "", "")
        {

        }

        public Binding(string path, object target, string propertyName, BindingMode mode, string format, string converter)
        {
            this.path = path;
            this.target = target;
            this.propertyName = propertyName;
            this.mode = mode;
            this.format = format;
            this.converter = converter;
        }

        #region  表达式与寻值
        //更新目标
        public void UpdateTarget()
        {
            //set target value
            if (!isBound || m_IsDisposed)
            {
                Debug.LogErrorFormat("invalide source {0}", this.path);
                return;
            }
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample("Binding.UpdateTarget");
#endif
            ExpressionUtility.UpdateTargetValue(target, propertyName, m_Context, m_LastPart, format, convert);
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        //更新源
        public void UpdateSource()
        {
            if (!isBound || m_IsDisposed)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("invalide source {0}.{1}", this.m_Context, this.path);
#endif
                return;
            }
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample("Binding.UpdateSource");
#endif
            ExpressionUtility.UpdateSourceValue(target, propertyName, m_Context, m_LastPart, format, convert);
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        private object m_ApplyContext;
        public void ApplyContext(object context)
        {
#if UNITY_EDITOR
            if (m_ApplyContext != null) //跳过？
            {
                Debug.LogWarningFormat("Binding(perpertyName={0}).ApplyContext(context={1})  m_ApplyContext is't null  ", propertyName, context);
            }
#endif
            m_ApplyContext = context;
        }

        //绑定目标
        public void Apply()
        {
            var context = m_ApplyContext;
            m_ApplyContext = null;
            if (m_IsDisposed)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Binding(target={0},perpertyName={1},path={2}) is Disposed , Apply(context={3}).  ", target, propertyName, path, context);
#endif
                return;
            }

            if (!m_IsApplied)
            {
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample("Binding.ParsePath");
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
                convert = ValueConverterRegister.instance.Get(converter);
            }


            if (isBound)
            {
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample("Binding.Unapply");
#endif
                Unapply();
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }

            object bindingContext = context;
            if (source) bindingContext = source;

            m_Context = bindingContext;

            if (m_Context == null)
            {
                convert = null; //需要解绑

                return;
            }

            object m_Current = bindingContext;

#if false
            ExpressionUtility.ApplyByLua (this, m_Current);
#else
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample("Binding.Part.TryGetValue");
#endif
            bool needSubscribe = this.needSubscribe;
            BindingPathPart part = null;
            for (var i = 0; i < m_Parts.Count; i++)
            {
                part = m_Parts[i];
                part.SetSource(m_Current); //
                if (!part.isSelf && m_Current != null)
                {

                    if (i < m_Parts.Count - 1)
                        part.TryGetValue(needSubscribe && part.nextPart != null, out m_Current); //lua NofityObject对象通过此方法在BindingExpression.get_property中订阅
                }

                if (!part.isSelf && m_Current == null)
                    break;

                if (part.nextPart != null && needSubscribe)
                {
                    if (m_Current is INotifyPropertyChanged)
                    {
                        part.Subscribe((INotifyPropertyChanged)m_Current);
                    }
                }
            }

            SetLastPart();

#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif
            //if ( invoke)
            // {
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample("Binding.InitValue");
#endif
            //初始化值
            InitValue();
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            // }
#endif
        }

        //解绑目标
        public void Unapply()
        {
            for (var i = 0; i < m_Parts.Count - 1; i++)
            {
                BindingPathPart part = m_Parts[i];
                part.Unsubscribe();
            }
        }

        public bool needSubscribe
        {
            get
            {
                return mode == BindingMode.OneWay || mode == BindingMode.TwoWay;
            }
        }

        public void SetLastPart()
        {
            m_LastPart = m_Parts[m_Parts.Count - 1];
        }

        internal void OnSourceChanged(BindingPathPart lastPart)
        {
            bool needSubscribe = mode == BindingMode.OneWay || mode == BindingMode.TwoWay;
            BindingPathPart part = lastPart.nextPart;
            object m_Current = part.source;
            while (part != null)
            {
                part.SetSource(m_Current); //
                m_LastPart = part;
                if (!part.isSelf && m_Current != null)
                {

                    if (part.nextPart != null)
                        part.TryGetValue(needSubscribe && part.nextPart != null, out m_Current);
                }

                // UnityEngine.Debug.LogFormat("OnSourceChanged current={0},property={1},m_Path={2},parts.Count={3},part={4},part.isSelf={5}", m_Current, propertyName, path, m_Parts.Count, part, part.isSelf);
                if (!part.isSelf && m_Current == null)
                    break;

                if (part.nextPart != null && needSubscribe)
                {
                    if (m_Current is INotifyPropertyChanged)
                    {
                        // UnityEngine.Debug.LogFormat("current = {0}", m_Current);
                        part.Subscribe((INotifyPropertyChanged)m_Current);
                    }
                }

                if (part.nextPart != null)
                    part = part.nextPart;
                else
                    part = null;
            }

            UpdateTarget();
        }

        void InitValue()
        {
            switch (mode)
            {
                case BindingMode.OneWay:
                    UpdateTarget();
                    break;

                case BindingMode.TwoWay:
                    UpdateTarget();
                    break;

                case BindingMode.OneWayToSource:
                    UpdateSource();
                    break;

                default:
                    Debug.LogErrorFormat("Invalid mode {0}", mode);
                    break;
            }
        }

        static readonly char[] ExpressionSplit = new[] { '.' };

        //解析的path路径
        readonly List<BindingPathPart> m_Parts = new List<BindingPathPart>();

        public List<BindingPathPart> parts
        {
            get
            {
                return m_Parts;
            }
        }
        public void ParsePath()
        {

            string p = path.Trim();

            if (!string.IsNullOrEmpty(converter))
                convert = ValueConverterRegister.instance.Get(converter);

            m_Parts.Clear();
            var last = new BindingPathPart(this, SelfPath);
            m_Parts.Add(last);

            if (p[0] == ExpressionSplit[0])
            {
                if (p.Length == 1)
                {
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
                    indexer = new BindingPathPart(this, argString, true);
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

                    int argLength = rbIndex - lbIndex - 1;

                    string argString = part.Substring(0, lbIndex);
                    var next = new BindingPathPart(this, argString);

                    // if (argLength >= 1) {
                    //     next.isSetter = true;
                    // } else {
                    next.isMethod = true;
                    // }

                    last.nextPart = next;
                    m_Parts.Add(next);
                    last = next;
                }
                else if (part.Length > 0)
                {
                    var next = new BindingPathPart(this, part);
                    last.nextPart = next;
                    m_Parts.Add(next);
                    last = next;
                }

                if (indexer != null)
                {
                    last.nextPart = indexer;
                    m_Parts.Add(indexer);
                    last = indexer;
                }
            }
        }

        #endregion

        public void Dispose()
        {
            for (var i = 0; i < m_Parts.Count - 1; i++)
            {
                BindingPathPart part = m_Parts[i];
                part.Dispose();
            }
            m_IsDisposed = true; //标记销毁
            m_IsApplied = false;
            convert = null;
            target = null;
            source = null;
            m_ApplyContext = null;
            m_Context = null;
            m_LastPart = null;
            m_Parts.Clear();
        }

    }

}
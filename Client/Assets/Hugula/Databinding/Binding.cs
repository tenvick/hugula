// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
#define USE_LIST_HANDLE_EVENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Hugula.Utils;
using XLua;

namespace Hugula.Databinding
{
    public enum BindingMode
    {
        OneWay,
        TwoWay,
        OneWayToSource,
        OneTime,
        // Default
    }

    [System.SerializableAttribute]
    public sealed class Binding : IDisposable, IBinding //gc alloc 128 B
    {
        internal const string SelfPath = ".";

        #region  序列化属性

        ///<summary>
        /// The name of the BindableProperty
        ///</summary>
        public string propertyName;
#if UNITY_EDITOR
        /// <summary>
        /// The path to the property  ,
        /// normal path is like 'name' or 'name.age' ,\n 
        /// getter path is like '&(VM.bag.on_render)' global  &(.on_render) local context , 直接获取路径的值
        /// setter path is like '$(VM.bag.on_render)' global or '$(.on_render)'  local ，直接调用函数默认参数为(source,target,path,propertyName)
        /// </summary>
        public string path;
#endif
        public string converter;
        public BindingMode mode;

        [PopUpComponentsAttribute]
        public UnityEngine.Object source;

        /// <summary>
        /// The target object 弱引用
        /// </summary>
        [PopUpComponentsAttribute]
        public UnityEngine.Object target;

        [SerializeField]
        BindingPathPartConfig[] partConfigs;
        /// <summary>
        /// The next binding in the chain
        /// </summary>

        #endregion
        internal Binding next;

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
        Action<object, string> m_ChangeHandler;

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

        public bool isSelf
        {
            get
            {
                if (partConfigs != null && partConfigs.Length > 0)
                    return partConfigs[0].isSelf;
                return false;
            }
        }

        /// <summary>
        /// 是否已经处理过
        /// </summary>
        private bool m_IsDisposed = false;//{ get; private set; }//销毁标记
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
            m_ChangeHandler = PropertyChanged;
        }

        public Binding(string path, UnityEngine.Object target, string propertyName, BindingMode mode) : this(path, target, propertyName, mode, "")
        {

        }

        public Binding(string path, UnityEngine.Object target, string propertyName, BindingMode mode, string converter)
        {
            m_IsDisposed = false;
            m_IsProcessed = false;
#if UNITY_EDITOR
            this.path = path;
            ParsePathToConfig(path);
#endif
            this.target = target;
            this.propertyName = propertyName;
            this.mode = mode;
            this.converter = converter;
            m_ChangeHandler = PropertyChanged;
        }

        #region  表达式与寻值

        /// <summary>
        /// 更新源 context 改变后调用方向 context.path = target.property
        /// </summary>
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

            ApplyCore(bindingContext, target, propertyName, true);
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

            var hasBinding = bindingContext != null;

            if (hasBinding && ReferenceEquals(source, bindingContext)) //已经绑定过并且是source 不需要继续执行
            {
                return;
            }
            else if (hasBinding) //已经绑定过
            {
                Unapply();
            }

            if (source)
                bindingContext = source;
            else
                bindingContext = context;

            if (bindingContext == null)
            {
                return;
            }

            ApplyCore(bindingContext, target, propertyName);
        }

        /// <summary>
        /// 更新目标或者源
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="target"></param>
        /// <param name="property"></param>
        /// <param name="fromTarget"></param>
        internal void ApplyCore(object sourceObject, object target, string property, bool fromTarget = false)
        {
            if ((mode == BindingMode.OneWay || mode == BindingMode.OneTime) && fromTarget) //
                return;

            object m_Current = sourceObject;

            bool needsGetter = (mode == BindingMode.TwoWay && !fromTarget) || mode == BindingMode.OneWay || mode == BindingMode.OneTime; ;
            bool needsSetter = !needsGetter && ((mode == BindingMode.TwoWay && fromTarget) || mode == BindingMode.OneWayToSource);
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample($"{GetProfilerName()}.path TryGetValue");
#endif
            BindingPathPartConfig part = new BindingPathPartConfig();
            PropertyChangedEventHandlerEvent propChanged = null;
            int count = partConfigs.Length;
            bool isSelf = false;

            for (var i = 0; i < count; i++)
            {
                part = partConfigs[i];
                isSelf = part.isSelf;

                if (!isSelf && (mode == BindingMode.OneWay || mode == BindingMode.TwoWay))// 监听source
                {
                    if (m_Current is INotifyPropertyChanged inpc)
                    {
                        Subscribe(inpc.PropertyChanged, ref part);
                        partConfigs[i] = part;
                    }
                    else if (m_Current is XLua.LuaTable inc && inc.TryGet<string, PropertyChangedEventHandlerEvent>("PropertyChanged", out propChanged))
                    {
                        Subscribe(propChanged, ref part);
                        partConfigs[i] = part;
                    }
                }

                if (!isSelf && m_Current != null && i < count - 1)
                {
                    TryGetValue(m_Current, ref part, out m_Current);
                }

                if (!isSelf && m_Current == null && i == count - 1) //last one
                    break;
            }
#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample($"{GetProfilerName()}.Getter_Setter");
#endif

            if (needsGetter)
            {
                SetTargetValue(m_Current, part);
            }
            else if (needsSetter && m_Current != null)
            {
                ExpressionUtility.SetSourceInvoke(m_Current, part, this);
            }

#if LUA_PROFILER_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 监听属性变化
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="part"></param>
        internal void Subscribe(PropertyChangedEventHandlerEvent handler, ref BindingPathPartConfig part)
        {
            if (ReferenceEquals(handler, part.m_NotifyPropertyChanged))
                return;

            Unsubscribe(ref part);

            var propertName = part.path;
            handler.Add(m_ChangeHandler, propertName);
            part.m_NotifyPropertyChanged = handler;
        }

        /// <summary>
        /// 解绑属性变化
        /// </summary>
        /// <param name="part"></param>
        internal void Unsubscribe(ref BindingPathPartConfig part)
        {
            if (part.m_NotifyPropertyChanged == null) return;

            var propertName = part.path;
            part.m_NotifyPropertyChanged.Remove(m_ChangeHandler, propertName);
            part.m_NotifyPropertyChanged = null;//
        }

        /// <summary>
        /// 属性变化处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal void PropertyChanged(object sender, string propertyName)
        {
#if !USE_LIST_HANDLE_EVENT

            if (!string.IsNullOrEmpty(propertyName))
            {
                bool hasPart = false;

                for (var i = 0; i < partConfigs.Length; i++)
                {
                    var part = partConfigs[i];
                    if (propertyName == part.path)
                    {
                        hasPart = true;
                        break;
                    }
                }

                if (!hasPart)
                {
                    return;
                }
            }
#endif
            //changed 更新目标
            ApplyCore(bindingContext, target, propertyName);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal bool TryGetValue(object source, ref BindingPathPartConfig part, out object value)
        {
            value = source;
            XLua.LuaTable lua = source as XLua.LuaTable;
            bool isLua = (lua != null);
            if (part.isExpGetter)
            {
                if (part.isGlobal)
                    value = EnterLua.luaenv?.Global.GetInPath<object>(part.path);
                else if (isLua)
                    value = lua.GetInPath<object>(part.path);
                else
                    return false;

                return true;
            }
            else if (part.isExpSetter)
            {
                LuaFunction func = null;
                if (part.isGlobal)
                    func = EnterLua.luaenv?.Global.GetInPath<LuaFunction>(part.path);
                else if (isLua)
                    func = lua.GetInPath<LuaFunction>(part.path);
                else
                    return false;

                if (func != null)
                {
                    value = func.Func<object, object, string, string, object>(source,target,string.Empty,propertyName);
                    return true;
                }
                return false;
            }
            else if (isLua)
                lua.TryGet<string, object>(part.path, out value);
            else
                value = ExpressionUtility.GetSourceInvoke(value, part.path, part.isMethod, part.isIndexer);

            return true;
        }

        internal bool SetTargetValue(object source, BindingPathPartConfig part)
        {
            object value = source;

            XLua.LuaTable lua = value as XLua.LuaTable;
            bool isLua = (lua != null);
            if (part.isExpGetter)
            {
                if (part.isGlobal)
                    value = EnterLua.luaenv?.Global.GetInPath<object>(part.path);
                else if (isLua)
                    value = lua.GetInPath<object>(part.path);

                part.isSelf = true;
            }
            else if (part.isExpSetter)
            {
                LuaFunction func = null;
                if (part.isGlobal)
                    func = EnterLua.luaenv?.Global.GetInPath<LuaFunction>(part.path);
                else if (isLua)
                    func = lua.GetInPath<LuaFunction>(part.path);


                if (func != null)
                {
                    value = func.Func<object, object, string, string, object>(source,target,string.Empty,propertyName);
                    if (value == null)
                        return true;
                }
            }

            ExpressionUtility.SetTargetInvoke(value, target, propertyName, ref part, converter);
            return true;
        }

        internal bool CheckInvoke(string propertyName)
        {
            if (partConfigs == null || partConfigs.Length == 0) return false;
            for (var i = 0; i < partConfigs.Length; i++)
            {
                var part = partConfigs[i];
                if (propertyName == part.path)
                {
                    return true;
                }
            }
            return false;
        }

        //解绑目标
        public void Unapply()
        {
            if (partConfigs == null) return;
            for (var i = 0; i < partConfigs.Length; i++)
            {
                Unsubscribe(ref partConfigs[i]);
            }
        }

#if UNITY_EDITOR
        public void ParsePathToConfig(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            string p = path.Trim();

            if (p[0] == ExpressionSplit[0])
            {
                if (p.Length == 1)
                {
                    var BindingPathPartConfig = new BindingPathPartConfig { path = SelfPath, isIndexer = false, isExpSetter = false, isSelf = true };
                    partConfigs = new BindingPathPartConfig[] { BindingPathPartConfig };
                    return;
                }

                p = p.Substring(1);
            }

            // 使用List来存储解析结果，避免数组索引问题
            List<BindingPathPartConfig> resultParts = new List<BindingPathPartConfig>();

            List<string> tokens = ParseTokensWithSpecialExpressions(p);

            for (var i = 0; i < tokens.Count; i++)
            {
                string part = tokens[i].Trim();

                if (part == string.Empty)
                    throw new FormatException("Path contains an empty part:" + this.propertyName);

                BindingPathPartConfig indexer = default;

                //索引解析
                int lbIndex = part.IndexOf('['); // e.g. color[1]
                if (lbIndex != -1)
                {
                    int rbIndex = part.LastIndexOf(']');
                    if (rbIndex == -1)
                        throw new FormatException("Indexer did not contain closing ]");

                    int argLength = rbIndex - lbIndex - 1;
                    if (argLength == 0)
                        throw new FormatException("Indexer did not contain arguments");

                    string argString = part.Substring(lbIndex + 1, argLength);
                    indexer = new BindingPathPartConfig { path = argString, isIndexer = true, isExpSetter = false, isSelf = false };
                    part = part.Substring(0, lbIndex);
                    part = part.Trim();
                }

                //特殊表达式或方法调用判断
                int methodStartIndex = part.IndexOf('(');
                if (methodStartIndex != -1)
                {
                    int rbIndex = part.LastIndexOf(')');
                    if (rbIndex == -1)
                        throw new FormatException("Method did not contain closing )");

                    // 判断是否为特殊表达式 $() 或 &()
                    var isExpSetter = part.StartsWith("$(");
                    var isExpGetter = part.StartsWith("&(");
                    if ((isExpSetter || isExpGetter) && part.EndsWith(")"))
                    {
                        // 整个part作为特殊表达式，需要再次解析
                        var isGlobal = part[2] != ExpressionSplit[0]; // 不以 $(.  &(. 开头为local
                        int argBgIndex = isGlobal ? 2 : 3;
                        int len = isGlobal ? part.Length - 3 : part.Length - 4;
                        var argString = part.Substring(argBgIndex, len);
                        var next = new BindingPathPartConfig { path = argString, isGlobal = isGlobal, isExpSetter = isExpSetter, isExpGetter = isExpGetter };
                        resultParts.Add(next);
                    }
                    else
                    {
                        // 普通方法调用
                        string argString = part.Substring(0, methodStartIndex);
                        var next = new BindingPathPartConfig { path = argString, isMethod = true };
                        resultParts.Add(next);
                    }
                }
                else if (part.Length > 0)
                {
                    var next = new BindingPathPartConfig { path = part, isExpSetter = false };
                    resultParts.Add(next);
                }

                // 如果有indexer，需要额外追加一个part
                if (indexer.isIndexer)
                {
                    // 追加索引器part
                    resultParts.Add(indexer);
                }
            }

            // 循环结束后将List转换为数组
            partConfigs = resultParts.ToArray();
        }


        /// <summary>
        /// 将输入字符串解析成Token列表, 支持识别$()与&()的特殊块，这些块内的内容不进行'.'拆分
        /// </summary>
        private List<string> ParseTokensWithSpecialExpressions(string input)
        {
            List<string> tokens = new List<string>();
            int length = input.Length;
            int start = 0;
            bool inSpecialBlock = false;
            bool isDollarBlock = false; // 标识当前是否是$(...)块
            bool isAmpBlock = false;    // 标识当前是否是&(...)块

            // 我们使用一个手动解析器从头到尾扫描字符串:
            // 常规模式下遇到'.'就切分，
            // 遇到'$('或'&('进入特殊块模式，直到遇到匹配的')'再结束。
            // 在特殊块模式下不对'.'进行拆分。
            for (int i = 0; i < length; i++)
            {
                // 如果目前不在特殊块中，检查是否开始$()或&()块
                if (!inSpecialBlock)
                {
                    if (i < length - 1 && input[i] == '$' && input[i + 1] == '(')
                    {
                        // 先把前面累积的普通Token切分出来
                        if (i > start)
                        {
                            string normalSegment = input.Substring(start, i - start).Trim();
                            if (!string.IsNullOrEmpty(normalSegment))
                                tokens.AddRange(normalSegment.Split(ExpressionSplit, StringSplitOptions.RemoveEmptyEntries));
                        }
                        inSpecialBlock = true;
                        isDollarBlock = true;
                        isAmpBlock = false;
                        i++; // 跳过'('的位置，现在i指向'('字符
                        start = i + 1; // start从'('的下一个字符开始
                        continue;
                    }

                    if (i < length - 1 && input[i] == '&' && input[i + 1] == '(')
                    {
                        if (i > start)
                        {
                            string normalSegment = input.Substring(start, i - start).Trim();
                            if (!string.IsNullOrEmpty(normalSegment))
                                tokens.AddRange(normalSegment.Split(ExpressionSplit, StringSplitOptions.RemoveEmptyEntries));
                        }
                        inSpecialBlock = true;
                        isDollarBlock = false;
                        isAmpBlock = true;
                        i++; // 跳过'('的位置，现在i指向'('字符
                        start = i + 1; // start从'('的下一个字符开始
                        continue;
                    }


                    // 普通模式下遇到'.'需要切分Token
                    if (input[i] == '.')
                    {
                        // 切分之前的内容作为一个token
                        if (i > start)
                        {
                            string normalSegment = input.Substring(start, i - start).Trim();
                            if (!string.IsNullOrEmpty(normalSegment))
                                tokens.Add(normalSegment);
                        }
                        start = i + 1;
                    }
                }
                else
                {
                    // 在特殊块内，只有遇到')'才会结束块
                    if (input[i] == ')')
                    {
                        // 收集特殊块的内容
                        string blockContent = input.Substring(start, i - start).Trim();
                        // 将特殊块整体作为一个token
                        // 并在token中恢复前缀比如"$(" 或 "&("
                        string prefix = isDollarBlock ? "$(" : "&(";
                        string fullToken = prefix + blockContent + ")";
                        tokens.Add(fullToken);

                        inSpecialBlock = false;
                        isDollarBlock = false;
                        isAmpBlock = false;
                        start = i + 1; // 块结束后更新start
                    }
                }
            }

            // 如果最后还有普通文本未切分完毕（且未在特殊块中）
            if (!inSpecialBlock && start < length)
            {
                string normalSegment = input.Substring(start).Trim();
                if (!string.IsNullOrEmpty(normalSegment))
                {
                    tokens.AddRange(normalSegment.Split(ExpressionSplit, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            // 如果inSpecialBlock还为true，说明有块未闭合
            if (inSpecialBlock)
                throw new FormatException("Special expression block not closed with ')'");

            return tokens;
        }

#endif

        static readonly char[] ExpressionSplit = new[] { '.' };

        #endregion

        public Binding Clone()
        {
            Binding clone = new Binding();
#if UNITY_EDITOR
            clone.path = path;
#endif
            if (partConfigs != null)
            {
                clone.partConfigs = new BindingPathPartConfig[partConfigs.Length];
                Array.Copy(partConfigs, clone.partConfigs, partConfigs.Length); // 深拷贝数组
            }
            clone.propertyName = propertyName;
            clone.converter = converter;
            clone.mode = mode;
            clone.source = source;
            clone.target = target;
            return clone;
        }

        public void Dispose()
        {
            Unapply();
            m_IsDisposed = true; //标记销毁
            m_IsProcessed = false;
            target = null;
            source = null;
            bindingContext = null;
            next = null;
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            return $"Binding(context={bindingContext},target={target},path={path},property={propertyName},mode={mode})";
        }
#endif
    }

}
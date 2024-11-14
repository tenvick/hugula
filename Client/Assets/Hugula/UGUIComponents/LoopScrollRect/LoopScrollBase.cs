using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// [ExecuteInEditMode]
namespace Hugula.UIComponents
{
    [XLua.LuaCallCSharp]
    public abstract class LoopScrollBase : ScrollRect, ILoopSelect //: UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Tooltip("-1 auto columns")]
        [SerializeField]
        int m_Columns = -1; //显示的列数

        int m_RealColumns = 0;
        /// <summary>
        /// 显示列数 排版方式 </br>
        /// Columns == 0 显示一行 </br>
        /// Columns == 1 显示一列 </br>
        /// Columns = n n>1  按照格子方式显示n列的数据 </br>
        /// </summary>
        public int columns { get { return m_RealColumns; } set { m_RealColumns = value; } } //显示列数 
        int m_PageSize = 1;

        /// <summary>
        /// 项目池的大小 </br>
        /// 自动计算
        /// </summary>
        public int pageSize
        {
            get
            {
                return m_PageSize;
            }
        }

        [SerializeField]
        Vector2 m_ItemSize = Vector2.zero;
        Vector2 m_RuntimeItemSize = Vector2.zero;

        /// <summary>
        /// ItemClone的默认size 当不为0的时候强制使用当前size
        /// </summary>
        public Vector2 itemSize { get { return m_RuntimeItemSize; } set { m_RuntimeItemSize = value; } } //默认size

        [Tooltip("padding left=x,top=y,right=z,bottom=w")]
        [SerializeField]
        Vector4 m_Padding = Vector4.zero;
        /// <summary>
        /// 间距</br>
        /// </summary>
        public Vector4 padding { get { return m_Padding; } set { m_Padding = value; m_HorizontalPadding = float.NaN; m_VerticalPadding = float.NaN; } } //间隔

        [SerializeField]
        bool m_RemoveEasing = true;
        /// <summary>
        /// 次渲染的项目数量</br>
        /// </summary>
        public bool removeEasing { get { return m_RemoveEasing; } set { m_RemoveEasing = value; } } //每次渲染item数

        // [SerializeField]
        [PopUpComponentsAttribute]
        [SerializeField]
        BindableObject[] m_Templates;
        /// <summary>
        /// clone的原始项模板</br>
        /// </summary>
        public BindableObject[] templates { get { return m_Templates; } set { m_Templates = value; } } //clone的项目


        [Tooltip("默认选中索引")]
        [SerializeField]
        int m_SelectedIndex = -1;
        /// <summary>
        /// 通过OnSelected方法 选中项目的索引</br>
        /// </summary>
        public int selectedIndex
        {
            get { return m_SelectedIndex; }
            set
            {
                // m_SelectedIndex = value;
                TriggerStyleBySelectedIndex(value);
            }
        }

        int m_LastSelectedIndex = -1;
        /// <summary>
        /// 通过OnSelected方法 选中项目的索引</br>
        /// </summary>
        public int lastSelectedIndex { get { return m_LastSelectedIndex; } }

        ILoopSelectStyle m_SelecteStyle;
        /// <summary>
        /// 设置选中样式
        /// </summary>
        protected ILoopSelectStyle selectStyle
        {
            get { return m_SelecteStyle; }
            set
            {
                var last = m_SelecteStyle;
                if (last != null && last != value) last.CancelStyle(); //取消上一个选中

                m_SelecteStyle = value;
                if (m_SelecteStyle != null)
                {
                    m_SelecteStyle.SelectedStyle();
                }
            }
        }
        /// <summary>
        /// 当item被clone时候调用
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.OnInstantiateFunction = (object o, int idx) => //o是ItemSource(Clone) ,idx是数据索引
        ///         {
        ///             var gobj = (Transform)o;
        ///             gobj.name = "item" + idx.ToString();
        ///             var btn = gobj.GetComponentInChildren<Button>();
        ///             btn.OnClick.AddListener(() =>
        ///              {
        ///                    print("Click "+mData.ToString());
        ///               });
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<object, object, int> onInstantiated { get; set; } //渲染

        /// <summary>
        /// 获取模板类型
        /// </summary>
        public Func<object, int, int> onGetItemTemplateType { get; set; } //获取模板类型


        /// <summary>
        /// 填充数据显示项目
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.onItemRender = (object o, int idx) => //o是ItemSource(Clone) ,idx是数据索引
        ///         {
        ///             var gobj = (Transform)o;
        ///             gobj.name = "item" + idx.ToString();
        ///             Text txt = gobj.GetComponentInChildren<Text>();
        ///             string test = testData[idx];
        ///             txt.text = test;
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<object, object, int> onItemRender { get; set; } //渲染

        /// <summary>
        /// 选中某个项
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.onSelected = (object o, int idx,int last idx) => //o是ItemSource(Clone) ,idx是数据索引
        ///         {
        ///             var gobj = (Transform)o;
        ///             gobj.name = "item" + idx.ToString();
        ///             Text txt = gobj.GetComponentInChildren<Text>();
        ///             string test = testData[idx];
        ///             txt.text = test;
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<object, object, int, int> onSelected { get; set; }

        /// <summary>
        /// 选中命令
        /// </summary>
        /// <example>
        /// <code>
        public ICommand itemCommand { get; set; }

        public object itemParameter { get; set; }

        [System.NonSerialized] private RectTransform m_Rect;
        internal RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        /// <summary>
        /// 内容的最大范围
        /// </summary>
        protected Rect m_ContentRect = new Rect();
        /// <summary>
        /// 可视范围
        /// </summary>
        protected Rect m_ViewPointRect = new Rect();
        [SerializeField]
        protected Vector2 m_ContentLocalStart;

        protected override void Awake()
        {
            base.Awake();
            m_HorizontalPadding = float.NaN;
            m_VerticalPadding = float.NaN;
            m_LastSelectedIndex = m_SelectedIndex;
            m_HeadDataIndex = 0;
            m_FootDataIndex = 0;
            Init();
        }

        protected override void LateUpdate()
        {
            UpdateViewPointBounds();
            ScrollLoopItem();

            if (renderQueue.Count > 0)
            {
                for (int i = 0; i < this.m_PageSize; i++)
                {
                    QueueToRender();
                }
            }

            base.LateUpdate(); //... CanvasUpdateRegistry.PerformUpdate() 75.23 ms

        }

        protected override void OnDestroy()
        {
            onItemRender = null;
            onInstantiated = null;
            content = null;
            selectStyle = null;

            foreach (var loopItem in m_Pages)
            {
                if (loopItem.item != null)
                {
                    GameObject.Destroy(loopItem.item.gameObject);
                }
                loopItem.item = null;
            }
            m_Pages.Clear();
         
            m_Pool?.Dispose();
            m_Pool = null;
            base.OnDestroy();
        }

        /// <summary>
        /// >Set the horizontal or vertical scroll position as a value between 0 and 1, with 0 being at the left or at the bottom.
        /// </summary>
        /// <param name="value">The position to set, between 0 and 1.</param>
        /// <param name="axis">The axis to set: 0 for horizontal, 1 for vertical.</param>
        protected override void SetNormalizedPosition(float value, int axis)
        {
            base.SetNormalizedPosition(value, axis);
            m_ScrollBarFrame = Time.frameCount;
        }

        private int m_ScrollBarFrame;
        protected bool scrollBarDragging
        {
            get
            {
                return m_ScrollBarFrame == Time.frameCount;
            }
        }

        #region  数据操作相关
        /// <summary>
        /// 对数据源的引用
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.sourceData = testData; //方便获取数据源
        ///     }
        /// }
        /// </code>
        /// </example>
        public object sourceData; //原始数据引用

        /// <summary>
        /// 所有回调函数的第一个自定义参数
        /// </summary>
        public object parameter { get; set; }

        // /// <summary>
        // /// 绑定上下文 用于lua binding
        // /// </summary>
        // public object context { get; set; }

        private int m_DataLength;

        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.DataLength = testData.Count;
        ///     }
        /// }
        /// </code>
        /// </example>
        public int dataLength
        {
            get
            {
                return m_DataLength;
            }
            set
            {
                m_DataLength = value;
                ClearItems();
                CalcBounds();
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //向位置0插入一条数据,后刷新显示
        ///         myScrollRect.InsertAt(0);
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void InsertAt(int index, int count = 1)
        {
            m_DataLength += count; //长度+1
            CalcBounds();
            UpdateBegin(index, m_PageSize);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //删除位置0的数据,会触发OnRemoveFunction代理方法
        ///         myScrollRect.RemoveAt(0); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void RemoveAt(int index, int count = 1)
        {
            if (index < 0 || index >= dataLength || count <= 0) return;

            m_DataLength -= count;
            // 移动数据 将当前index后面的数据向前移动cout个位置
            int min = int.MaxValue, max = int.MinValue;
            int remBeginIdx = int.MinValue, remEndIdx = int.MinValue;
            LoopItem repItem = null;
            int index1 = 0, nextIdx = 0;

            LoopItem item1;
            for (int i = 0; i < m_Pages.Count; i++)
            {
                item1 = m_Pages[i];
                index1 = item1.index;
                nextIdx = index1 + count;
                if (index1 != -1)
                    min = Math.Min(index1, min);

                max = Math.Max(index1, max);

                if (index1 >= index) //需要移动下面的项目来填充
                {
                    if (remBeginIdx == int.MinValue)
                    {
                        remBeginIdx = index1;
                        remEndIdx = index1;
                    }

                    if (m_SelectedIndex == index1)
                    {
                        // selectedIndex = -1;
                        // selectedIndex = index;
                    }

                    repItem = GetValideLoopItemAt(nextIdx);
                    if (repItem != null)
                    {
                        if (repItem.onlyPosDirty == false)
                        {
                            ReplaceLoopItemAt(index1, nextIdx);
                            remEndIdx = nextIdx;
                        }
                    }
                    else
                    {
                        if (remBeginIdx > index) //如果第一个没有被替换 刷新到开头
                        {
                            ReplaceLoopItemAt(remEndIdx, index);
                        }
                        item1.index = -1; //强制刷新
                        item1.onlyPosDirty = true;
                        LayOut(item1, index1 + 1); //刷新到下一个位置
                    }
                }
            }

            int idx = index;
            if (min == int.MaxValue) min = 0;
            if (idx < min) idx = min;
            int end_idx = idx + m_PageSize;
            int movIdx = 0;
            if (columns != 0) movIdx = min % columns; //如果顶部没有对齐。
            int max_idx = min + m_PageSize - 1 + movIdx;
            if (end_idx > max_idx) end_idx = max_idx;

            int range_begin = idx > min ? idx : min; //取大
            int range_end = end_idx;
            if (max >= dataLength)
            { //有数据删除
                range_end = max;
            }
            else
            {
                if (range_end >= dataLength) range_end = dataLength - 1;
            }

            for (int i = range_begin; i <= range_end; i++)
                renderQueue.Enqueue(i);

            CalcBounds();
        }

        /// <summary>
        /// 立即刷新索引处的项目
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //如果索引0在显示范围内就立马刷新
        ///         myScrollRect.UpdateAt(0); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void UpdateAt(int idx)
        {

            if (idx >= 0 && idx < dataLength)
            {
                var item = GetLoopItemAt(idx);
                if (item.index == idx)
                    RenderItem(item, idx);
            }

        }

        /// <summary>
        /// 从指定位置开始刷新直到显示范围尾
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //从索引3位置到PageSize结束的所有项目。
        ///         myScrollRect.UpdateBegin(3); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void UpdateBegin(int idx, int count = 0)
        {
            int min = int.MaxValue, max = int.MinValue;
            foreach (var item1 in m_Pages)
            {
                // item1.onlyPosDirty = false;
                if (item1.index != -1)
                    min = Math.Min(item1.index, min);
                max = Math.Max(item1.index, max);
            }
            if (count == 0) count = m_PageSize;
            if (min == int.MaxValue) min = 0;
            if (idx < min) idx = min;
            int end_idx = idx + count;
            int movIdx = 0;
            if (columns != 0) movIdx = min % columns; //如果顶部没有对齐。
            int max_idx = min + m_PageSize - 1 + movIdx;
            if (end_idx > max_idx) end_idx = max_idx;

            int range_begin = idx > min ? idx : min; //取大
            int range_end = end_idx;
            if (max >= dataLength)
            { //有数据删除
                range_end = max;
            }
            else
            {
                if (range_end >= dataLength) range_end = dataLength - 1;
            }

            for (int i = range_begin; i <= range_end; i++)
                renderQueue.Enqueue(i);

        }

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //显示数据
        ///         myScrollRect.Refresh(); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void Refresh() //开始渲染
        {
            StopMovement();
            // int min = int.MaxValue, max = int.MinValue;
            // foreach (var item1 in m_Pages)
            // {
            //     if (item1.index != -1)
            //     {
            //         min = Math.Min(item1.index, min);
            //         max = Math.Max(item1.index, max);
            //     }
            // }

            UpdateBegin(0);
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //清理数据
        ///         myScrollRect.Clear(); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void Clear()
        {
            StopMovement();
            ClearItems();
            if (content && content != rectTransform)
            {
                content.anchoredPosition = Vector2.zero;  //m_ContentLocalStart;
            }
            m_DataLength = 0;
            m_HeadDataIndex = 0;
            m_FootDataIndex = 0;
        }

        public class SelectArg
        {
            public int selectedIndex;
            public int lastSelectedIndex;
        }
        private SelectArg defaultArg = new SelectArg();

        protected bool CanSelect(int index, out object parameter) //选中
        {
            parameter = null;
            object para = null;
            if (itemParameter == null)
            {
                defaultArg.selectedIndex = index;
                defaultArg.lastSelectedIndex = m_SelectedIndex;
                para = defaultArg;
            }
            else
                para = itemParameter;


            if (itemCommand != null)
            {
                if (itemCommand.CanExecute(para)) //check can select
                {
                    parameter = para;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 选中，渲染出来才能选中
        /// </summary>
        /// <param name="loopItem"></param>
        public void OnSelect(LoopItem loopItem) //选中
        {
            // if (onSelected != null) onSelected(this.parameter, loopItem.item, m_SelectedIndex, m_LastSelectedIndex);
            var loopSelectStyle = loopItem.loopSelectStyle;
            var can = CanSelect(loopItem.index, out object para);

            if (can && loopSelectStyle != null)
            {
                if (loopItem.index != m_SelectedIndex) m_LastSelectedIndex = m_SelectedIndex; //如果是TriggerStyleBySelectedIndex的时候标记的索引
                m_SelectedIndex = loopItem.index;
                selectStyle = loopSelectStyle;
                if (itemParameter == null)
                {
                    defaultArg.selectedIndex = m_SelectedIndex;
                    defaultArg.lastSelectedIndex = m_LastSelectedIndex;
                    para = defaultArg;
                }
                if (para != null) itemCommand?.Execute(para);
            }
        }

        protected void TriggerStyleBySelectedIndex(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < dataLength)
            {
                //find exist
                var item = GetValideLoopItemAt(selectedIndex);
                if (item != null && item.loopSelectStyle != null)
                {
                    OnSelect(item);
                }
                else
                {
                    var can = CanSelect(selectedIndex, out object para);
                    if (can)
                    {
                        m_SelectedIndex = selectedIndex; //标记
                    }
                }
            }
            else
            {
                m_SelectedIndex = -1;
                selectStyle = null;
            }
        }

        #endregion

        #region 模板项目

        protected Hugula.Utils.GameObjectPool<BindableObject> m_Pool;
        void OnPoolGet(BindableObject comp)
        {
            comp.gameObject?.SetActive(true);
        }

        void OnPoolRealse(BindableObject comp)
        {
            comp.gameObject?.SetActive(false);
        }

        void InitTemplatePool()
        {
            if (m_Pool == null)
            {
                m_Pool = new Hugula.Utils.GameObjectPool<BindableObject>(OnPoolGet, OnPoolRealse);
            }

            if (templates != null && m_Pool.countSource < templates.Length)
            {
                for (int i = 0; i < templates.Length; i++)
                    m_Pool.Add(i, templates[i]);
            }
        }

        protected List<LoopItem> m_Pages = new List<LoopItem>();
        protected int m_HeadDataIndex;
        protected int m_FootDataIndex;

        protected void Init()
        {
            if (m_ItemSize.x > 0 && m_ItemSize.y > 0)
            {
                SetItemSize(m_ItemSize.x, 0);
                SetItemSize(m_ItemSize.y, 1);
            }
            else if (templates != null && templates.Length > 0)
            {
                var rect = templates[0].GetComponent<RectTransform>().rect;
                SetItemSize(Mathf.Abs(rect.width), 0);
                SetItemSize(Mathf.Abs(rect.height), 1);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"LoopScroll{this} Init templates is null path:{Hugula.Utils.CUtils.GetGameObjectFullPath(this.gameObject)}");
            }
#endif

            InitColumns();
            InitPages();
            InitTemplatePool();
        }

        void InitColumns()
        {

            var vSize = viewRect.rect; //  content.rect;
            if (m_Columns < 0) //auto columns
                m_RealColumns = Mathf.RoundToInt(Mathf.Abs(vSize.width) / (itemSize.x + this.horizontalPadding));
            else
                m_RealColumns = m_Columns;

            if (columns == 0)
            {
                m_PageSize = Mathf.CeilToInt(Mathf.Abs(vSize.width) / (itemSize.x + this.horizontalPadding)) + 1;
            }
            else
            {
                m_PageSize = columns * (Mathf.CeilToInt(Mathf.Abs(vSize.height) / (itemSize.y + this.verticalPadding)) + 1);
            }
        }

        void InitPages()
        {
            for (int i = m_Pages.Count; i < m_PageSize; i++)
            {
                m_Pages.Add(new LoopItem());
            }
        }

        /// <summary>
        /// 获取当前idx的loopItem
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        protected LoopItem GetLoopItemAt(int idx)
        {
            int i = idx % m_PageSize;
            return m_Pages[i];
        }

        /// <summary>
        /// 获取验证index的loopItem
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        protected LoopItem GetValideLoopItemAt(int idx)
        {
            var i = idx % m_PageSize;
            var tmp = m_Pages[i];
            if (tmp.index == idx)
                return tmp;
            return null;
        }

        /// <summary>
        /// 替换指定位置的项目 idx的item与idx2的item交换
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        /// <returns></returns>
        protected bool ReplaceLoopItemAt(int idx1, int idx2)
        {
            int i = idx1 % m_PageSize;
            int j = idx2 % m_PageSize;
            if (i == j) return false;

            var idx1Item = m_Pages[i];
            var idx2Item = m_Pages[j];

            idx1Item.onlyPosDirty = true;
            var templateId = idx1Item.templateType;
            var temp = idx1Item.item;
            var tmptrans = idx1Item.transform;
            var tmpstyle = idx1Item.loopSelectStyle;
            var tmpstyle2 = idx2Item.loopSelectStyle;

            idx1Item.item = idx2Item.item;
            idx1Item.transform = idx2Item.transform;
            idx1Item.templateType = idx2Item.templateType;
            if (tmpstyle != null)
                tmpstyle.InitSytle(idx2Item, this);

            idx2Item.item = temp;
            idx2Item.transform = tmptrans;
            idx2Item.templateType = templateId;
            if (tmpstyle2 != null)
                tmpstyle2.InitSytle(idx1Item, this);

            // if(m_SelectedIndex == idx1)
            //     m_SelecteStyle = tmpstyle2;
            // else if(m_SelectedIndex == idx2)
            //     m_SelecteStyle = tmpstyle;

            idx2Item.onlyPosDirty = true;

            return true;
        }

        public Component GetItemAt(int idx)
        {
            if (idx < 0) return null;
            var loopItem = GetLoopItemAt(idx);
            if (loopItem.index == idx)
                return loopItem.item;

            return null;
        }

        #endregion

        #region 渲染与布局
        private float m_HorizontalPadding = float.NaN;
        /// <summary>
        /// 水平间距
        /// </summary>
        protected float horizontalPadding
        {
            get
            {
                if (m_HorizontalPadding.Equals(float.NaN))
                    m_HorizontalPadding = padding.x + padding.z;
                return m_HorizontalPadding;
            }
        }
        private float m_VerticalPadding = float.NaN;

        /// <summary>
        /// 垂直间距
        /// </summary>
        protected float verticalPadding
        {
            get
            {
                if (m_VerticalPadding.Equals(float.NaN))
                    m_VerticalPadding = padding.y + padding.w;
                return m_VerticalPadding;
            }
        }

        protected Queue<int> renderQueue = new Queue<int>(); //渲染队列

        protected void QueueToRender()
        {
            if (renderQueue.Count > 0)
            {
                int idx = renderQueue.Dequeue();
                var item = GetLoopItemAt(idx);
                RenderItem(item, idx);
            }
        }
        Component OnCreateOrGetItem(int idx, int templateId, LoopItem loopItem, RectTransform content)
        {
            Component item = loopItem.item;
            if (item == null || loopItem.templateType != templateId)
            {
                //还对象到缓存池
                m_Pool.Release(loopItem.templateType, (BindableObject)item);
                bool isNew = false;
                item = m_Pool.Get(templateId, content, out isNew); //创建或者从缓存中获取

                loopItem.templateType = templateId;
                loopItem.item = item;
                loopItem.transform = item.GetComponent<RectTransform>();
                InitItemStyle(loopItem);

                if (isNew && onInstantiated != null)
                {
                    onInstantiated(this.parameter, item, idx);
                }

            }

            return item;
        }

        protected void RenderItem(LoopItem loopItem, int idx)
        {
            // bool dispatchOnSelectedEvent = false;
            var oldIdx = loopItem.index;
            loopItem.index = idx;
            if (idx < dataLength)
            {
                var templateId = 0;
                if (onGetItemTemplateType != null) templateId = onGetItemTemplateType(this.parameter, idx);
                var item = OnCreateOrGetItem(idx, templateId, loopItem, content); //
#if UNITY_EDITOR
                item.name = string.Format("{0}-{1}", idx, templateId);
#endif              
            }

            var gObj = loopItem.item.gameObject;
            if (idx >= dataLength)
            {
                gObj.SetActive(false);
                return;
            }
            else if (!gObj.activeSelf)
            {
                gObj.SetActive(true);
            }

            //keep selected
            if (m_SelecteStyle != null)
            {
                if (m_SelecteStyle.loopItem.index == m_SelectedIndex)
                    m_SelecteStyle.SelectedStyle();
                else
                    m_SelecteStyle.CancelStyle();
            }

            if (oldIdx != idx || !loopItem.onlyPosDirty) //索引变化或者刷新数据
            {
                if (onItemRender != null) onItemRender(this.parameter, loopItem.item, loopItem.index); //填充内容
                if (!(oldIdx == -1 && loopItem.onlyPosDirty)) //如果是删除项目不刷新位置
                {
                    LayOut(loopItem, loopItem.index);
                    loopItem.onlyPosDirty = false;
                }
            }

            //check select
            if (m_SelectedIndex == idx && m_SelecteStyle != loopItem.loopSelectStyle)
            {
                OnSelect(loopItem);
            }

        }

        protected void UpdateViewPointBounds()
        {
            Vector2 localp = content.anchoredPosition;
            var cpos = localp + m_ContentLocalStart; //开始位置
            m_ViewPointRect = viewRect.rect; //
            m_ViewPointRect.height = -Mathf.Abs(m_ViewPointRect.height);
            m_ViewPointRect.x = cpos.x;
            m_ViewPointRect.y = -cpos.y;
        }

        protected void InitItemStyle(LoopItem loopItem)
        {
            var loopItemSelect = loopItem.item?.GetComponent<ILoopSelectStyle>();
            if (loopItemSelect != null)
            {
                loopItemSelect.InitSytle(loopItem, this);
            }
        }

        ///<summary>
        /// 内容滚动
        ///</summary>
        protected abstract void ScrollLoopItem();

        ///<summary>
        /// 计算内容范围
        ///</summary>
        protected abstract void CalcBounds();

        ///<summary>
        /// 布局
        ///</summary>
        protected abstract void LayOut(LoopItem loopItem, int index);

        protected void SetItemSize(float size, int axis)
        {
            m_RuntimeItemSize[axis] = size;
        }
        #endregion

        #region  tool
        protected void ClearItems()
        {
            if (content && content != rectTransform) content.anchoredPosition = Vector2.zero;// m_ContentLocalStart;
            foreach (var item in m_Pages)
            {
                if (item.transform)
                {
                    item.transform.gameObject.SetActive(false);
                    item.transform.GetComponent<BindableObject>()?.Unapply();
                }
                item.index = -1;
            }

            m_SelectedIndex = -1;
            m_LastSelectedIndex = -1;
            selectStyle = null;
        }

        #endregion

        #region  drag
        protected bool m_IsOnDrag = false;
        protected Vector3 m_DragVelocity;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_IsOnDrag = true;
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            m_DragVelocity = content.anchoredPosition - m_ContentStartPosition;

            m_IsOnDrag = false;
            base.OnEndDrag(eventData);


            //滚动到
        }
        #endregion
    }

#if UNITY_EDITOR

    public static class EditorGUITools
    {

        private static Texture2D backgroundTexture = new Texture2D(1, 1);
        private static GUIStyle _staticRectStyle = new GUIStyle { normal = new GUIStyleState { background = backgroundTexture } };

        public static void DrawRect(Rect position, Color color, GUIContent content = null)
        {
            if (backgroundTexture == null) backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();

            if (_staticRectStyle == null)
            {
                _staticRectStyle = new GUIStyle { normal = new GUIStyleState { background = backgroundTexture } };
            }
            _staticRectStyle.normal.background = backgroundTexture;
            GUI.Box(position, content ?? GUIContent.none, _staticRectStyle);
        }

        public static void LayoutBox(Color color, GUIContent content = null)
        {
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUILayout.Box(content ?? GUIContent.none, _staticRectStyle);
            GUI.backgroundColor = backgroundColor;
        }
    }

#endif
}
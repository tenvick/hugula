using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.UIComponents {
    public class LoopItemSelect : MonoBehaviour {
        Button clickBtn;
        LoopItem loopItem;
        ILoopSelect loopScrollBase;

        public void OnSelect () {
            if (loopScrollBase != null)
                loopScrollBase.OnSelect (loopItem);
        }

        public void InitLoopItem (LoopItem loopItem, ILoopSelect loopScrollBase) {
            this.loopItem = loopItem;
            this.loopScrollBase = loopScrollBase;

        }

        void Start () {
            clickBtn = GetComponent<Button> ();
            if (clickBtn) clickBtn.onClick.AddListener (OnSelect);
        }

        void OnDestroy () {
            if (clickBtn)
                clickBtn.onClick.RemoveListener (OnSelect);
            clickBtn = null;
            loopItem = null;
            loopScrollBase = null;
        }
    }

    public class LoopItem {
        public int templateType = 0; //当前项的模板类型
        public int index = -1; //对应data的索引
        public Component item; //clone的模板项
        public RectTransform transform; //
        public Rect rect; //当前位置

    }

    public class LoopVerticalItem : LoopItem {
        public Vector2 bound; //高度范围
        public void SetPos (float y) {
            bound.x = y;
        }

        public bool isDirty { set; get; }

        public void SetHeight (float height) {
            bound.y = height;
            isDirty = true;
        }

        public float yMin {
            get {
                return bound.x;
            }
        }

        public float yMax {
            get {
                return bound.x + bound.y;
            }
        }
    }

    public interface ILoopSelect {
        void OnSelect (LoopItem loopItem);
    }
}
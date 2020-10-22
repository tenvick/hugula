using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.UI;
using Hugula;

namespace Hugula.Databinding.Binder
{

    [RequireComponent(typeof(Image))]
    public class ImageBinder : MaskableGraphicBinder<Image>
    {

        // public const string ImageProperty = "Image";
        // public const string ColorProperty = "color";

        public bool setNativeSize = false;

        private Sprite addressSprite ;

        #region 新增属性
        private string m_spriteName;

        public string spriteName
        {
            get
            {
                return m_spriteName;
            }
            set
            {
                if (!string.Equals(value, m_spriteName))
                {
                    UnloadSprite();

                    LoadSprite(value);
                    m_spriteName = value;
                }
            }
        }

        #endregion

        #region  protected method
        void LoadSprite(string spriteName)
        {
            if (target && !string.IsNullOrEmpty(spriteName))
            {
                target.enabled = false;
                ResLoader.LoadAssetAsync<Sprite>(spriteName, OnSpriteCompleted, null);
            }
        }

        void OnSpriteCompleted(Sprite sprite, object arg)
        {
            addressSprite = sprite;
            target.sprite = sprite;
            target.enabled = true;
            if (setNativeSize)
                target.SetNativeSize();
        }

        void UnloadSprite()
        {
            if (addressSprite!= null)
            {
                ResLoader.Release(addressSprite);
            }
            addressSprite = null; 
        }

        #endregion

        #region  重写属性
        public Sprite sprite
        {
            get { return target.sprite; }
            set
            {
                target.sprite = value;
                OnPropertyChanged();
            }
        }

        public Sprite overrideSprite
        {
            get { return target.overrideSprite; }
            set
            {
                target.overrideSprite = value;
                OnPropertyChanged();
            }
        }

        public Image.Type type
        {
            get { return target.type; }
            set
            {
                target.type = value;
                OnPropertyChanged();
            }
        }

        public bool preserveAspect
        {
            get { return target.preserveAspect; }
            set
            {
                target.preserveAspect = value;
                OnPropertyChanged();
            }
        }

        public bool fillCenter
        {
            get { return target.fillCenter; }
            set
            {
                target.fillCenter = value;
                OnPropertyChanged();
            }
        }

        public Image.FillMethod fillMethod
        {
            get { return target.fillMethod; }
            set
            {
                target.fillMethod = value;
                OnPropertyChanged();
            }
        }

        public float fillAmount
        {
            get { return target.fillAmount; }
            set
            {
                target.fillAmount = value;
                OnPropertyChanged();
            }
        }

        public bool fillClockwise
        {
            get { return target.fillClockwise; }
            set
            {
                target.fillClockwise = value;
                OnPropertyChanged();
            }
        }

        public int fillOrigin
        {
            get { return target.fillOrigin; }
            set
            {
                target.fillOrigin = value;
                OnPropertyChanged();
            }
        }

        public float eventAlphaThreshold
        {
            get { return target.alphaHitTestMinimumThreshold; }
            set
            {
                target.alphaHitTestMinimumThreshold = value;
                OnPropertyChanged();
            }
        }

        public float alphaHitTestMinimumThreshold
        {
            get { return target.alphaHitTestMinimumThreshold; }
            set
            {
                target.alphaHitTestMinimumThreshold = value;
                OnPropertyChanged();
            }
        }

        public bool useSpriteMesh
        {
            get { return target.useSpriteMesh; }
            set
            {
                target.useSpriteMesh = value;
                OnPropertyChanged();
            }
        }

        public Texture mainTexture
        {
            get { return target.mainTexture; }
        }

        public bool hasBorder
        {
            get { return target.hasBorder; }
        }

        public float pixelsPerUnit
        {
            get { return target.pixelsPerUnit; }
        }

        public Material material
        {
            get { return target.material; }
            set
            {
                target.material = value;
                OnPropertyChanged();
            }
        }
        #endregion

        protected override void OnDestroy()
        {
            UnloadSprite();
            base.OnDestroy();
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [RequireComponent(typeof(Image))]
    public class ImageBinder : MaskableGraphicBinder<Image>
    {

        // public const string ImageProperty = "Image";
        // public const string ColorProperty = "color";

        public bool setNativeSize = false;

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
                UnloadSprite(m_spriteName);
                if (!string.Equals(value, m_spriteName))
                {
                    m_spriteName = value;
                    LoadSprite(value);
                }
            }
        }
        #endregion

        #region  protected method
        void LoadSprite(string spriteName)
        {
            if (target)
            {
                target.enabled = false;
                //load altas
                var altasBundle = Atlas.AtlasMappingManager.GetSpriteBundle(spriteName); // find altas
                if (altasBundle != null)
                {
                    ResourcesLoader.LoadAssetAsync(altasBundle + Common.CHECK_ASSETBUNDLE_SUFFIX, altasBundle, typeof(Atlas.AtlasAsset), OnAltasCompleted, null, spriteName);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarningFormat("can't find {0}'s mapping in Assets/Config/atlas_mapping_root.asset", spriteName);
                }
#endif
            }
        }

        void OnAltasCompleted(object data, object arg)
        {
            if (target && data is Atlas.AtlasAsset)
            {
                var altas = (Atlas.AtlasAsset)data;
                var sprite = altas.GetSprite(arg.ToString());
                target.sprite = sprite;
                target.enabled = true;
                if (setNativeSize)
                    target.SetNativeSize();
            }
        }

        void UnloadSprite(string spriteName)
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                var altasBundle = Atlas.AtlasMappingManager.GetSpriteBundle(spriteName); // find altas
                if (altasBundle != null)
                {
                    CacheManager.Subtract(altasBundle + Common.CHECK_ASSETBUNDLE_SUFFIX);
                }
            }
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
            UnloadSprite(m_spriteName);
            base.OnDestroy();
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class ImageBinder : MaskableGraphicBinder {

        // public const string ImageProperty = "Image";
        // public const string ColorProperty = "color";
        private Image m_target;
        protected Image image {
            get {

                if (m_target == null)
                    m_target = GetTarget<Image> ();
                return m_target;
            }
            set {
                m_target = value;
            }
        }

        public bool setNativeSize = false;

        #region 新增属性
        private string m_spriteName;

        public string spriteName {
            get {
                return m_spriteName;
            }
            set {
                UnloadSprite (m_spriteName);
                if (!string.Equals (value, m_spriteName)) {
                    m_spriteName = value;
                    LoadSprite (value);
                }
            }
        }
        #endregion

        #region  protected method
        void LoadSprite (string spriteName) {
            if (image) {
                image.enabled = false;
                //load altas
                var altasBundle = Atlas.AtlasMappingManager.GetSpriteBundle (spriteName); // find altas
                if (altasBundle != null) {
                    ResourcesLoader.LoadAssetAsync (altasBundle + Common.CHECK_ASSETBUNDLE_SUFFIX, altasBundle, typeof (Atlas.AtlasAsset), OnAltasCompleted, null, spriteName);
                }
#if UNITY_EDITOR
                else {
                    Debug.LogWarningFormat ("can't find {0}'s mapping in Assets/Config/atlas_mapping_root.asset", spriteName);
                }
#endif
            }
        }

        void OnAltasCompleted (object data, object arg) {
            if (image && data is Atlas.AtlasAsset) {
                var altas = (Atlas.AtlasAsset) data;
                var sprite = altas.GetSprite (arg.ToString ());
                image.sprite = sprite;
                image.enabled = true;
                if (setNativeSize)
                    image.SetNativeSize ();
            }
        }

        void UnloadSprite (string spriteName) {
            if (!string.IsNullOrEmpty (spriteName)) {
                var altasBundle = Atlas.AtlasMappingManager.GetSpriteBundle (spriteName); // find altas
                if (altasBundle != null) {
                    CacheManager.Subtract (altasBundle + Common.CHECK_ASSETBUNDLE_SUFFIX);
                }
            }
        }

        #endregion

        #region  重写属性
        public Sprite sprite {
            get { return image.sprite; }
            set {
                image.sprite = value;
                OnPropertyChanged ();
            }
        }

        public Sprite overrideSprite {
            get { return image.overrideSprite; }
            set {
                image.overrideSprite = value;
                OnPropertyChanged ();
            }
        }

        public Image.Type type {
            get { return image.type; }
            set {
                image.type = value;
                OnPropertyChanged ();
            }
        }

        public bool preserveAspect {
            get { return image.preserveAspect; }
            set {
                image.preserveAspect = value;
                OnPropertyChanged ();
            }
        }

        public bool fillCenter {
            get { return image.fillCenter; }
            set {
                image.fillCenter = value;
                OnPropertyChanged ();
            }
        }

        public Image.FillMethod fillMethod {
            get { return image.fillMethod; }
            set {
                image.fillMethod = value;
                OnPropertyChanged ();
            }
        }

        public float fillAmount {
            get { return image.fillAmount; }
            set {
                image.fillAmount = value;
                OnPropertyChanged ();
            }
        }

        public bool fillClockwise {
            get { return image.fillClockwise; }
            set {
                image.fillClockwise = value;
                OnPropertyChanged ();
            }
        }

        public int fillOrigin {
            get { return image.fillOrigin; }
            set {
                image.fillOrigin = value;
                OnPropertyChanged ();
            }
        }

        public float eventAlphaThreshold {
            get { return image.alphaHitTestMinimumThreshold; }
            set {
                image.alphaHitTestMinimumThreshold = value;
                OnPropertyChanged ();
            }
        }

        public float alphaHitTestMinimumThreshold {
            get { return image.alphaHitTestMinimumThreshold; }
            set {
                image.alphaHitTestMinimumThreshold = value;
                OnPropertyChanged ();
            }
        }

        public bool useSpriteMesh {
            get { return image.useSpriteMesh; }
            set {
                image.useSpriteMesh = value;
                OnPropertyChanged ();
            }
        }

        public Texture mainTexture {
            get { return image.mainTexture; }
        }

        public bool hasBorder {
            get { return image.hasBorder; }
        }

        public float pixelsPerUnit {
            get { return image.pixelsPerUnit; }
        }

        public Material material {
            get { return image.material; }
            set {
                image.material = value;
                OnPropertyChanged ();
            }
        }
        #endregion

        protected override void OnDestroy () {
            UnloadSprite (m_spriteName);
            image = null;
            base.OnDestroy ();
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class ImageBinder : MaskableGraphicBinder {

        // public const string ImageProperty = "Image";
        // public const string ColorProperty = "color";
        protected Image m_Image;

        #region  重写属性
        public Sprite sprite {
            get { return m_Image.sprite; }
            set {
                m_Image.sprite = value;
                OnPropertyChanged ();
            }
        }

        public Sprite overrideSprite {
            get { return m_Image.overrideSprite; }
            set {
                m_Image.overrideSprite = value;
                OnPropertyChanged ();
            }
        }

        public Image.Type type {
            get { return m_Image.type; }
            set {
                m_Image.type = value;
                OnPropertyChanged ();
            }
        }

        public bool preserveAspect {
            get { return m_Image.preserveAspect; }
            set {
                m_Image.preserveAspect = value;
                OnPropertyChanged ();
            }
        }

        public bool fillCenter {
            get { return m_Image.fillCenter; }
            set {
                m_Image.fillCenter = value;
                OnPropertyChanged ();
            }
        }

        public Image.FillMethod fillMethod {
            get { return m_Image.fillMethod; }
            set {
                m_Image.fillMethod = value;
                OnPropertyChanged ();
            }
        }

        public float fillAmount {
            get { return m_Image.fillAmount; }
            set {
                m_Image.fillAmount = value;
                OnPropertyChanged ();
            }
        }

        public bool fillClockwise {
            get { return m_Image.fillClockwise; }
            set {
                m_Image.fillClockwise = value;
                OnPropertyChanged ();
            }
        }

        public int fillOrigin {
            get { return m_Image.fillOrigin; }
            set {
                m_Image.fillOrigin = value;
                OnPropertyChanged ();
            }
        }

        public float eventAlphaThreshold {
            get { return m_Image.eventAlphaThreshold; }
            set {
                m_Image.eventAlphaThreshold = value;
                OnPropertyChanged ();
            }
        }

        public float alphaHitTestMinimumThreshold {
            get { return m_Image.alphaHitTestMinimumThreshold; }
            set {
                m_Image.alphaHitTestMinimumThreshold = value;
                OnPropertyChanged ();
            }
        }

        public bool useSpriteMesh {
            get { return m_Image.useSpriteMesh; }
            set {
                m_Image.useSpriteMesh = value;
                OnPropertyChanged ();
            }
        }

        public Texture mainTexture {
            get { return m_Image.mainTexture; }
        }

        public bool hasBorder {
            get { return m_Image.hasBorder; }
        }

        public float pixelsPerUnit {
            get { return m_Image.pixelsPerUnit; }
        }

        public Material material {
            get { return m_Image.material; }
            set {
                m_Image.material = value;
                OnPropertyChanged ();
            }
        }
        #endregion

        protected virtual void Awake () {
            base.Awake();
            m_Image = GetTarget<Image> ();
        }

    }
}
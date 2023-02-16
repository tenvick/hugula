using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using TMPro;

namespace PSDUINewImporter
{
    public sealed class TextComponentImport : BaseComponentImport<TMPro.TextMeshProUGUI>
    {
        Dictionary<string, string> m_FontAliasName = new Dictionary<string, string>(){
            //{"PingFang-SC-Bold","ArialMT"},
            //{"MicrosoftJhengHeiBold","ArialMT"},
            //{"Dutch801BT-Roman","ArialMT"},
            //{"SourceHanSansCN-Medium","ArialMT"},
            //{"DIN-Medium","ArialMT"}
            };

        public const string NormalTag = "Normal";
        public const string PressedTag = "Pressed";
        public const string DisabledTag = "Disabled";
        public const string Highlighted = "Highlighted";

        public TextComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index, Layer layer, TextMeshProUGUI target, GameObject parent, int posSizeLayerIndex)
        {
            var myText = target.GetComponent<TextMeshProUGUI>();
            // layer.target = myText;
            RectTransform rectTransform = myText.GetComponent<RectTransform>();
            PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
            myText.raycastTarget = false; //取消射线检测

            SetText(myText, layer);

            SetColor(myText,layer);
            float size;
            if (float.TryParse(layer.arguments[2], out size))
            {
                myText.fontSize = (int)size;
            }

            //设置字体,注意unity中的字体名需要和导出的xml中的一致
            string fontFolder;

            //if (layer.TagContains("Static")) //.arguments[1].ToLower().Contains("static"))
            //{
            //    fontFolder = PSDImporterConst.FONT_STATIC_FOLDER;
            //}
            //else
            {
                fontFolder = PSDImporterConst.FONT_FOLDER;
            }

            var fontName = GetFontName(layer.arguments[1]);

            string fontFullName = Path.Combine(fontFolder, fontName + PSDImporterConst.FONT_ASSET_SUFIX);
            // Debug.Log("font name ; " + fontFullName);
            var font = AssetDatabase.LoadAssetAtPath(fontFullName, typeof(TMP_FontAsset)) as TMP_FontAsset;
            if (font == null)
            {
                Debug.LogError($"Load font failed : {fontFullName} ");
            }
            else
            {
                myText.font = font;
            }

            myText.fontMaterial.DisableKeyword("UNDERLAY_ON");
            //ps的size在unity里面太小，文本会显示不出来,暂时选择溢出
            myText.overflowMode = TextOverflowModes.Overflow;
            myText.enableWordWrapping = false;
            //设置对齐
            if (layer.arguments.Length >= 5)
                myText.alignment = ParseAlignmentPS2UGUI(layer.arguments[4]);
            else
            {
                // 默认居中
                myText.alignment = TextAlignmentOptions.Center;
            }

            #region 
            string name = "";


            if (!string.IsNullOrEmpty(layer.outline))
            {
                name += "-o" + layer.outline;
            }
            if (!string.IsNullOrEmpty(layer.shadow))
            {
                if (!string.IsNullOrEmpty(layer.outline))
                {
                    EditorUtility.DisplayDialog("FBI WARN", layer.name + " :不支持同时投影和描边", "OK");
                }
                else
                {
                    name += "-s" + layer.shadow;
                }
            }

            //outerGlow 外发光
            if (!string.IsNullOrEmpty(layer.outerGlow))
            {
                name += "-og" + layer.outerGlow;
            }
            Material material;
            string path = "";
            if (!string.IsNullOrEmpty(name))
            {
                path = string.Format("{0}{1}{2}.mat", PSDImporterConst.FONT_FOLDER, myText.font.name, name);
                var fileInfo = new FileInfo(path);
                if (!fileInfo.Exists)
                {
                    if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
                    material = new Material(myText.fontMaterial);
                    myText.fontMaterial = material;
                    AssetDatabase.CreateAsset(material, path);
                    AssetDatabase.Refresh();
                }
            }

            // outerGlow
            if (!string.IsNullOrEmpty(layer.outerGlow))
            {
                material = AssetDatabase.LoadAssetAtPath<Material>(path);
                myText.fontMaterial = material;

                var _temp = layer.outerGlow.Split('_');
                // 第一位颜色
                Color effectColor;
                if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + _temp[0]), out effectColor))
                {
                    //myText.outlineColor = effectColor;
                    //myText.outlineWidth = s / 50;

                    material.EnableKeyword("GLOW_ON");
                    //var color = effectColor * (0.45f * 191f / 255);
                    //color.a = 1f;
                    material.SetColor("_GlowColor", new Color(Mathf.Pow(effectColor.r, 1 / 0.45f), Mathf.Pow(effectColor.g, 1 / 0.45f), Mathf.Pow(effectColor.b, 1 / 0.45f), 1));
                    material.SetFloat("_GlowOffset", 0);
                    material.SetFloat("_GlowInner", 0);
                    material.SetFloat("_GlowOuter", 0.5f);
                    material.SetFloat("_GlowPower", 0.15f);

                    //AssetDatabase.CreateAsset(material, path);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }


            // OutLine
            if (!string.IsNullOrEmpty(layer.outline))
            {
                material = AssetDatabase.LoadAssetAtPath<Material>(path);
                myText.fontMaterial = material;

                var _temp = layer.outline.Split('_');
                // 第一位颜色
                Color effectColor;
                if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + _temp[1]), out effectColor))
                {
                    var s = float.Parse(_temp[0]);

                    //myText.outlineColor = effectColor;
                    //myText.outlineWidth = s / 50;

                    material.EnableKeyword("UNDERLAY_ON");
                    //var color = effectColor * (0.45f * 191f / 255);
                    //color.a = 1f;
                    material.SetColor("_UnderlayColor", new Color(Mathf.Pow(effectColor.r, 1 / 0.45f), Mathf.Pow(effectColor.g, 1 / 0.45f), Mathf.Pow(effectColor.b, 1 / 0.45f), 1));
                    material.SetFloat("_UnderlayDilate", (s / 5f) * (30f / myText.fontSize));
                    material.SetFloat("_UnderlayOffsetX", 0);
                    material.SetFloat("_UnderlayOffsetY", 0);
                    material.SetFloat("_UnderlaySoftness", 0);

                    //AssetDatabase.CreateAsset(material, path);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }

            if (!string.IsNullOrEmpty(layer.gradient))
            {
                //Debug.LogError(layer.gradient);
                var _temp = layer.gradient.Split(',');

                Color color1;
                UnityEngine.ColorUtility.TryParseHtmlString(("#" + _temp[0]), out color1);
                Color color2;
                UnityEngine.ColorUtility.TryParseHtmlString(("#" + _temp[1]), out color2);
                VertexGradient vertexGradient = new VertexGradient(color2, color2, color1, color1);
                myText.colorGradient = vertexGradient;
                myText.enableVertexGradient = true;
                myText.color = Color.white;
            }

            // Shadow
            if (!string.IsNullOrEmpty(layer.shadow))
            {
                if (!string.IsNullOrEmpty(layer.outline))
                {
                    return;
                }

                material = AssetDatabase.LoadAssetAtPath<Material>(path);
                myText.fontMaterial = material;

                ;
                var _temp = layer.shadow.Split('_');
                // 第一位颜色
                Color effectColor;
                if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + _temp[1]), out effectColor))
                {
                    var s = float.Parse(_temp[0]);
                    var a = float.Parse(_temp[2]);
                    var angel = float.Parse(_temp[3]);
                    var b = float.Parse(_temp[4]);
                    //myText.outlineColor = effectColor;
                    //myText.outlineWidth = s / 50;

                    material.EnableKeyword("UNDERLAY_ON");
                    //var color = effectColor * (0.45f * 191f / 255);
                    //color.a = 1f;
                    material.SetColor("_UnderlayColor", new Color(Mathf.Pow(effectColor.r, 1 / 0.45f), Mathf.Pow(effectColor.g, 1 / 0.45f), Mathf.Pow(effectColor.b, 1 / 0.45f), a / 100f));
                    material.SetFloat("_UnderlayOffsetX", -Mathf.Cos(angel * Mathf.PI / 180) * s / 10f);
                    material.SetFloat("_UnderlayOffsetY", -Mathf.Sin(angel * Mathf.PI / 180) * s / 10f);
                    material.SetFloat("_UnderlaySoftness", b / 20f);
                    material.SetFloat("_UnderlayDilate", 0.2f);
                    //AssetDatabase.CreateAsset(material, path);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }

            }

            #endregion

            // rectTransform.sizeDelta = new Vector2(layer.size.width * 1.2f, layer.size.height * 1.1f);
            SetRectTransformSize(rectTransform, layer.size, 1.2f);
            SetRectTransformPosition(rectTransform, layer.position);
            // UnityEngine.Debug.LogFormat("name = {0},size={1},position={2} ",layer.name, layer.size,layer.position);
        }

        protected override void CheckAddBinder(Layer layer, TextMeshProUGUI text)
        {
            if (layer.TagContains(PSDImportUtility.DynamicTag))
            {
                var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.TextMeshProUGUIBinder>(text.gameObject);
                if (binder != null)
                {
                    var textBinding = binder.GetBinding("text");
                    if (textBinding == null)
                    {
                        textBinding = new Hugula.Databinding.Binding();
                        textBinding.propertyName = "text";
                        binder.AddBinding(textBinding);
                    }
                    textBinding.path = layer.name;
                }
            }

        }

        /// <summary>
        /// ps的对齐转换到ugui，暂时只做水平的对齐
        /// </summary>
        /// <param name="justification"></param>
        /// <returns></returns>
        public TextAlignmentOptions ParseAlignmentPS2UGUI(string justification)
        {
            var defaut = TextAlignmentOptions.Center;
            if (string.IsNullOrEmpty(justification))
            {
                return defaut;
            }

            string[] temp = justification.Split('.');
            if (temp.Length != 2)
            {
                Debug.LogWarning("ps exported justification is error !");
                return defaut;
            }
            Justification justi = (Justification)System.Enum.Parse(typeof(Justification), temp[1]);
            int index = (int)justi;
            //转化成TextAlignment
            TextAlignmentOptions[] textAlignmentOptions =
            {
                TextAlignmentOptions.TopLeft, TextAlignmentOptions.Top | TextAlignmentOptions.Center,
                TextAlignmentOptions.TopRight, TextAlignmentOptions.Left | TextAlignmentOptions.Center,
                TextAlignmentOptions.Center, TextAlignmentOptions.Right | TextAlignmentOptions.Center,
                TextAlignmentOptions.BottomLeft
            };
            //defaut = (TextAnchor)System.Enum.ToObject(typeof(TextAnchor), index);
            defaut = textAlignmentOptions[index];
            return defaut;
        }

        //ps的对齐方式
        public enum Justification
        {
            CENTERJUSTIFIED = 0,
            LEFTJUSTIFIED = 1,
            RIGHTJUSTIFIED = 2,
            LEFT = 3,
            CENTER = 4,
            RIGHT = 5,
            FULLYJUSTIFIED = 6,
        }

        protected string GetFontName(string name)
        {
            if (m_FontAliasName.TryGetValue(name, out var newName))
            {
                return newName;
            }
            return name;
        }

        public static void SetText(TMPro.TextMeshProUGUI target, Layer layer)
        {
            target.text = layer.arguments[3];
        }

        public static void SetColor(TMPro.TextMeshProUGUI myText, Layer layer)
        {
            Color color;
            if (!myText.enableVertexGradient)//没有渐变，才需要设置颜色
            {
                if (UnityEngine.ColorUtility.TryParseHtmlString(("#" + layer.arguments[0]), out color))
                {
                    if (layer.opacity > -1)
                    {
                        color.a = layer.opacity / 100f;
                        // Debug.Log("Opacity:" + color.a);
                    }

                    myText.color = color;
                }
                else
                {
                    Debug.Log(layer.arguments[0]);
                }
            }
        }
    }
}
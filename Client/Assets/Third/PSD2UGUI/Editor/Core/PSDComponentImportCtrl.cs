using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace PSDUINewImporter
{
    public class PSDComponentImportCtrl
    {
        private PSDUI psdUI;

        public PSDComponentImportCtrl(string xmlFilePath)
        {
            ShowTips("导入PSD","加载配置",0.1f);
            PSDImporterConst.LoadConfig();
            PSDDirectoryUtility.ClearCache();
            ShowTips("导入PSD", $"加载{xmlFilePath}", 0.2f);
            InitDataAndPath(xmlFilePath);
            InitAliasType();
            ShowTips("导入PSD", $"InitCanvas", 0.3f);
            InitCanvas();
            ShowTips("导入PSD", $"LoadLayers", 0.5f);

            if(PSDImporterConst.PSDUI_SETTING_TEXTRUE)
            {
                LoadLayers();
            }

            ClearTips();
        }

        Dictionary<string, IComponentImport> m_LayerImport = new Dictionary<string, IComponentImport>();
        Dictionary<string, string> m_AliasType = new Dictionary<string, string>();

        public IComponentImport GetLayerImport(string layerType)
        {
            IComponentImport layerImport = null;
            if (m_AliasType.TryGetValue(layerType.ToLower(), out var alias))
            {
                layerType = alias;
            }

            if (!m_LayerImport.TryGetValue(layerType, out layerImport))
            {
                layerImport = PSDUtils.CreateComponentImport(layerType, this);
                m_LayerImport.Add(layerType, layerImport);
            }
            return layerImport;
        }

        public string GetFullImportType(string importType)
        {
            if (m_AliasType.TryGetValue(importType.ToLower(), out var alias))
            {
                importType =  alias;
            }
           return importType;
        }

        public void DrawLayer(int index, Layer layer, GameObject target, GameObject parent, bool autoPosition = true, bool autoSize = true)
        {
            GetLayerImport(layer.importType).DrawLayer(index, layer, target, parent, autoPosition, autoSize);
        }

        public void DrawLayers(Layer[] layers, GameObject target, GameObject parent)
        {
            if (layers != null)
            {
                int realyIdx = -1;
                for (int layerIndex = layers.Length - 1; layerIndex >= 0; layerIndex--)
                {
                    if (PSDImportUtility.NeedDraw(layers[layerIndex]))
                        DrawLayer(++realyIdx, layers[layerIndex], null, parent);
                }
            }
        }

        public bool CompareLayerType(string aliasType, string compType)
        {
            if (m_AliasType.TryGetValue(aliasType.ToLower(), out var alias))
            {
                aliasType = alias;
            }
            return aliasType == compType;
        }

        private void InitDataAndPath(string xmlFilePath)
        {
            psdUI = (PSDUI)PSDImportUtility.DeserializeXml(xmlFilePath, typeof(PSDUI));
            Debug.Log(psdUI.psdSize.width + "=====psdSize======" + psdUI.psdSize.height);
            if (psdUI == null)
            {
                Debug.Log("The file " + xmlFilePath + " wasn't able to generate a PSDUI.");
                return;
            }
#if UNITY_5_2
            if (EditorApplication.SaveCurrentSceneIfUserWantsTo () == false) { return; }
#elif UNITY_5_3
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo () == false) { return; }
#endif
            PSDImportUtility.baseFilename = Path.GetFileNameWithoutExtension(xmlFilePath);
            //if(xmlFilePath.StartsWith(Application.dataPath))
            //    PSDImportUtility.baseDirectory = "Assets/" + Path.GetDirectoryName(xmlFilePath.Remove(0, Application.dataPath.Length + 1)) + "/";
            //else
                PSDImportUtility.baseDirectory = PSDImporterConst.Globle_BASE_FOLDER;
        }

        private void InitAliasType()
        {
            m_AliasType.Add("bc", "BindableContainer");
            m_AliasType.Add("bt", "Button");
            m_AliasType.Add("txt", "Text");
            m_AliasType.Add("img", "Image");
            m_AliasType.Add("lsv", "LoopScrollView");
            m_AliasType.Add("input", "InputField");
            m_AliasType.Add("toggle", "Toggle");
            m_AliasType.Add("ckb", "Toggle");
            m_AliasType.Add("chekbox", "Toggle");
            m_AliasType.Add("cus","Customer");

        }

        private void InitCanvas()
        {
#if UNITY_5_2
            EditorApplication.NewScene ();
#elif UNITY_5_3
            EditorSceneManager.NewScene (NewSceneSetup.DefaultGameObjects);
#endif
            Canvas temp = AssetDatabase.LoadAssetAtPath(PSDImporterConst.ASSET_PATH_CANVAS, typeof(Canvas)) as Canvas;
            PSDImportUtility.canvas = GameObject.Instantiate(temp) as Canvas;
            PSDImportUtility.canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.CanvasScaler scaler = PSDImportUtility.canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;
            scaler.referenceResolution = new Vector2(psdUI.psdSize.width, psdUI.psdSize.height);

            // find 
            var _eventSystem = Object.FindObjectOfType<EventSystem>();

            if (_eventSystem != null)
            {
                PSDImportUtility.eventSys = _eventSystem.gameObject;
            }
            else
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(PSDImporterConst.ASSET_PATH_EVENTSYSTEM, typeof(GameObject)) as GameObject;

                PSDImportUtility.eventSys = GameObject.Instantiate(go) as GameObject;
            }
        }

        private void LoadLayers()
        {
            Layer layer = null;
            var len = psdUI.layers.Length;
            for (int layerIndex = 0; layerIndex < len; layerIndex++)
            {
                layer = psdUI.layers[layerIndex];
                ImportLayer(layer, PSDImportUtility.baseDirectory);
            }
        }

        public void BeginDrawUILayers()
        {
            var selectedTransform = Selection.activeTransform;
            RectTransform obj;
            if (selectedTransform is RectTransform && selectedTransform.name == PSDImportUtility.baseFilename)
            {
                obj = (RectTransform)selectedTransform;
            }
            else
            {
                obj = PSDImportUtility.LoadAndInstant<RectTransform>(PSDImporterConst.ASSET_PATH_EMPTY, PSDImportUtility.baseFilename, PSDImportUtility.canvas.gameObject);
                
                obj.pivot = Vector2.one*.5f;
                obj.offsetMin = Vector2.zero;
                obj.offsetMax = Vector2.zero;
                obj.anchorMin = Vector2.one*.5f;
                obj.anchorMax = Vector2.one*.5f;
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(psdUI.psdSize.width,psdUI.psdSize.height);
            }


            int realyIdx = -1;
            ShowTips("DrawUILayers","开始绘制ui，稍等片刻",1);
            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                var layer = psdUI.layers[layerIndex];
                if (PSDImportUtility.NeedDraw(layer))
                    DrawLayer(++realyIdx, layer, null, obj.gameObject);
            }
            AssetDatabase.Refresh();
            ClearTips();
        }

        //--------------------------------------------------------------------------
        // private methods,按texture或image的要求导入图片到unity可加载的状态
        //-------------------------------------------------------------------------

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="baseDirectory"></param>
        private void ImportLayer(Layer layer, string baseDirectory)
        {
            if (CompareLayerType(layer.type, ComponentType.Image))
            {
                string texturePathName = PSDDirectoryUtility.FindFileInDirectory(PSDImportUtility.baseDirectory, layer.name + PSDImporterConst.PNG_SUFFIX);// PSDImportUtility.baseDirectory + layer.name + PSDImporterConst.PNG_SUFFIX;
                Debug.Log(texturePathName);
                ShowTips("ImportLayer设置Image图片", $"{layer.name}=>{texturePathName}", 1);
                if (!string.IsNullOrEmpty(texturePathName))
                {
                    // modify the importer settings
                    TextureImporter textureImporter = AssetImporter.GetAtPath(texturePathName) as TextureImporter;

                    if (textureImporter != null && !layer.TagContains("Texture")) //Texture类型不设置属性
                    {
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.mipmapEnabled = false; //默认关闭mipmap
                                                               // textureImporter.sRGBTexture = false;
                                                               // if(image.imageSource == ImageSource.Global)
                                                               // {
                                                               //     textureImporter.spritePackingTag = PSDImporterConst.Globle_FOLDER_NAME;
                                                               // }
                                                               // else
                                                               // {
                                                               //     textureImporter.spritePackingTag = PSDImportUtility.baseFilename;
                                                               // }

                        textureImporter.maxTextureSize = 2048;

                        // if (image.miniType == ImageType.SliceImage)  //slice才需要设置border,可能需要根据实际修改border值
                        if (layer.TagContains("9S"))
                        {
                            setSpriteBorder(textureImporter, layer.arguments);
                            //textureImporter.spriteBorder = new Vector4(3, 3, 3, 3);   // Set Default Slice type  UnityEngine.UI.Image's border to Vector4 (3, 3, 3, 3)
                        }

                        AssetDatabase.WriteImportSettingsIfDirty(texturePathName);
                        AssetDatabase.ImportAsset(texturePathName);
                    }
                }
            }
            else if (CompareLayerType(layer.type, ComponentType.RawImage)) // ComponentType.RawImage == " ")
            {
                //string texturePathName = PSDDirectoryUtility.FindFileInDirectory(PSDImportUtility.baseDirectory, layer.name + PSDImporterConst.PNG_SUFFIX);// PSDImportUtility.baseDirectory + layer.name + PSDImporterConst.PNG_SUFFIX;
                //Debug.Log(texturePathName);
                //ShowTips("ImportLayer设置RawImage图片", $"{layer.name}=>{texturePathName}", 1);
                //if (!string.IsNullOrEmpty(texturePathName))
                //{
                //    // modify the importer settings
                //    TextureImporter textureImporter = AssetImporter.GetAtPath(texturePathName) as TextureImporter;

                //    if (textureImporter != null ) //Texture类型不设置属性
                //    {
                //        textureImporter.textureType = TextureImporterType.Sprite;
                //        textureImporter.spriteImportMode = SpriteImportMode.Single;
                //        textureImporter.mipmapEnabled = false; //默认关闭mipmap
                //        textureImporter.maxTextureSize = 2048;
                     
                //        AssetDatabase.WriteImportSettingsIfDirty(texturePathName);
                //        AssetDatabase.ImportAsset(texturePathName);
                //    }
                //}
            }
            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    ImportLayer(layer.layers[layerIndex], PSDImportUtility.baseDirectory);
                }
            }
        }
        //设置九宫格
        void setSpriteBorder(TextureImporter textureImporter, string[] args)
        {
            textureImporter.spriteBorder = new Vector4(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
        }


        void ShowTips(string title,string info,float progress)
        {
            EditorUtility.DisplayProgressBar(title, info, progress);
        }

        void ClearTips()
        {
            EditorUtility.ClearProgressBar();
        }

    }
}
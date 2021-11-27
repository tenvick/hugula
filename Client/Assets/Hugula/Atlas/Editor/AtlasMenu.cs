using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hugula;
using Hugula.Atlas;
using Hugula.Utils;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace HugulaEditor
{
    public class AtlasConfig
    {
        //存放全局Atlas mapping root的目录
        public const string ATLAS_MAPPING_ROOT = @"Assets\Config";
    }

    public class AtlasMenu
    {

        [MenuItem("Assets/Atlas/1.Create(Refresh) Atlas Asset from Folder", true, 101)]
        private static bool ValidateCreateAtlasByFolder()
        {
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
                {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Hugula/Atlas/1.Create(Refresh) Atlas Asset from Folder", false, 101)]
        [MenuItem("Assets/Atlas/1.Create(Refresh) Atlas Asset from Folder")]
        public static void CreateAtlasByFolder()
        {
            // string path = string.Empty;
            StringBuilder sb = new StringBuilder();
            string tagName = string.Empty;

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                var assPath = AssetDatabase.GetAssetPath(obj);
                var objName = obj.name.ToLower();
                // 如果是文件夹
                if (Directory.Exists(assPath))
                {

                    var ragName = objName + "_atlas.spriteatlas";
                    string atlasPath = Path.Combine(assPath, ragName);
                    tagName = objName + "_atlas";

                    if (!File.Exists(atlasPath))
                    {

                        SpriteAtlas atlas = new SpriteAtlas();
                        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
                        {
                            enableRotation = false,
                            enableTightPacking = false,
                            padding = 4,
                        };
                        SpriteAtlasTextureSettings textureSet = new SpriteAtlasTextureSettings()
                        {
                            readable = false,
                            generateMipMaps = false,
                            sRGB = true,
                            filterMode = FilterMode.Bilinear,
                        };
                        atlas.SetTextureSettings(textureSet);
                        atlas.SetPackingSettings(packSetting);
                        atlas.Add(new[] { obj });
                        AssetDatabase.CreateAsset(atlas, atlasPath);

                        sb.Append("Crate atlas Asset :");
                        sb.Append(atlasPath);
                        sb.Append("\r\n");
                    }
                    else
                    {
                        sb.Append("Atlas asset already exists :");
                        sb.Append(atlasPath);
                        sb.Append("\r\n");
                    }
                }
            }

            // var selection = Selection.objects;

            // foreach (Object s in selection)
            // {
            //     if (s is DefaultAsset && (path = AssetDatabase.GetAssetPath(s)) != null && Directory.Exists(path))
            //     {

            //     }
            // }

            sb.AppendLine("\r\nall completed");
            Debug.Log(sb.ToString());
            EditorUtils.WriteToTmpFile(tagName + ".txt", sb.ToString());
        }

        // [MenuItem("Hugula/Atlas/1.Create(Refresh) Atlas Asset", false, 101)]
        // [MenuItem("Assets/Atlas/1.Create(Refresh) Atlas Asset", false, 101)]
        public static void CreateAtlasAsset()
        {
            var selection = Selection.objects;
            string path = string.Empty;

            StringBuilder sb = new StringBuilder();
            string tagName = string.Empty;
            foreach (Object s in selection)
            {
                if (s is DefaultAsset && (path = AssetDatabase.GetAssetPath(s)) != null && Directory.Exists(path))
                {
                    var ragName = s.name.ToLower() + "_atlas.spriteatlas";
                    string atlas_path = Path.Combine(path, ragName);
                    tagName = s.name.ToLower() + "_atlas";

                    sb.Append("Crate atlas Asset :");
                    sb.Append(ragName);
                    sb.Append("\r\n");

                    var allchildren = EditorUtils.getAllChildFiles(path, @"\.meta$|\.manifest$|\.DS_Store$|\.u$", null, false);
                    int count = 0;
                    List<int> names = new List<int>();
                    List<Sprite> allSprites = new List<Sprite>();
                    foreach (var f in allchildren)
                    {
                        count++;
                        TextureImporter ti = AssetImporter.GetAtPath(f) as TextureImporter;
                        if (ti != null)
                        {
                            if (ti.textureType != TextureImporterType.Sprite) ti.textureType = TextureImporterType.Sprite;
                            Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(f);
                            foreach (var item in objs)
                            {
                                if (item is Sprite)
                                {
                                    sb.AppendLine(item.name);
                                    names.Add(UnityEngine.Animator.StringToHash(item.name));
                                    allSprites.Add((Sprite)item);
                                }
                            }
                            // ti.spritePackingTag = tagName;
                            // ti.assetBundleName = tagName + Common.CHECK_ASSETBUNDLE_SUFFIX;
                            EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + count + " / " + allchildren.Count + ")", count / allchildren.Count);
                        }
                        else
                            Debug.LogWarningFormat("{0} is not Texture ", f);
                    }
                    EditorUtility.ClearProgressBar();
                    //生成或者替换资源
                    var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlas_path);
                    if (atlas == null)
                    {
                        atlas = new SpriteAtlas();
                        AssetDatabase.CreateAsset(atlas, atlas_path);
                        SpriteAtlasPackingSettings packSet = new SpriteAtlasPackingSettings()
                        {
                            blockOffset = 1,
                            enableRotation = false,
                            enableTightPacking = false,
                            padding = 2,
                        };
                        atlas.SetPackingSettings(packSet);

                        SpriteAtlasTextureSettings textureSet = new SpriteAtlasTextureSettings()
                        {
                            readable = false,
                            generateMipMaps = false,
                            sRGB = true,
                            filterMode = FilterMode.Bilinear,
                        };
                        atlas.SetTextureSettings(textureSet);
                    }

                    SpriteAtlasExtensions.Add(atlas, allSprites.ToArray());
                    EditorUtility.SetDirty(atlas);

                    var groupAssets = new List<string>();
                    groupAssets.Add(atlas_path);
                    var atlasGroup = AASEditorUtility.FindGroup(tagName, AASEditorUtility.DefaltGroupSchema[0]);
                    AASEditorUtility.SetGroupAddress(AASEditorUtility.LoadAASSetting(), atlasGroup, groupAssets);
                    sb.AppendFormat("build {0} success  count = {1} ", ragName, names.Count);
                    AssetDatabase.SaveAssets();

                }
            }

            sb.AppendLine("\r\nall completed");
            Debug.Log(sb.ToString());
            EditorUtils.WriteToTmpFile(tagName + ".txt", sb.ToString());
        }

        [MenuItem("Hugula/Atlas/2.Generate All Atlas Mapping", false, 103)]
        [MenuItem("Assets/Atlas/2.Generate All Atlas Mapping")]
        public static void GenerateAllAtlasMapping()
        {
            StringBuilder sb = new StringBuilder();
            List<int> allSprites = new List<int>();
            List<string> atlasNames = new List<string>();

            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
              p.EndsWith(".spriteatlas")
            ).ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                var o = AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteAtlas>(files[i]);
                var ti = AssetImporter.GetAtPath(files[i]);
                var key = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                if (o != null)
                {
                    // var assetBundleName = ti.assetBundleName;
                    EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + i + " / " + files.Length + ")", (float)i / (float)files.Length);

                    var sps = new UnityEngine.Sprite[o.spriteCount];

                    int len = o.GetSprites(sps);
                    for (int j = 0; j < len; j++)
                    {
                        string name = sps[j].name.Replace("(Clone)", "");
                        int id = UnityEngine.Animator.StringToHash(name);
                        allSprites.Add(id);
                        atlasNames.Add(key);
                        sb.AppendFormat("{0}({1})   {2}\r\n", name, id, key);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            string mapping_root_name = Hugula.Atlas.AtlasManager.ATLAS_MAPPING_ROOT_NAME;
            string atlas_path = Path.Combine(AtlasConfig.ATLAS_MAPPING_ROOT, mapping_root_name + ".asset");
            //生成或者替换资源
            var atlas = AssetDatabase.LoadAssetAtPath<MappingAsset>(atlas_path);
            if (atlas == null)
            {
                atlas = MappingAsset.CreateInstance<MappingAsset>();
                AssetDatabase.CreateAsset(atlas, atlas_path);
            }

            atlas.names = allSprites.ToArray();
            atlas.keys = atlasNames.ToArray();
            EditorUtility.SetDirty(atlas);

            //aas 
            var groupAssets = new List<string>();
            groupAssets.Add(atlas_path);
            var atlasGroup = AASEditorUtility.FindGroup(mapping_root_name, AASEditorUtility.DefaltGroupSchema[0]);
            AASEditorUtility.SetGroupAddress(AASEditorUtility.LoadAASSetting(), atlasGroup, groupAssets);
            AssetDatabase.SaveAssets();
            Debug.LogFormat(" save {0}  count = {1} ", atlas_path, atlasNames.Count);
            EditorUtils.WriteToTmpFile(mapping_root_name + ".txt", sb.ToString());

        }

    }
}
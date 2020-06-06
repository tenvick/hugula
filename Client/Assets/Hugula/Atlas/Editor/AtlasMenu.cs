using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using Hugula.Atlas;
using Hugula.Utils;
using Hugula;

namespace HugulaEditor
{
    public class AtlasConfig
    {
        //存放全局Atlas mapping root的目录
        public const string ATLAS_MAPPING_ROOT = @"Assets\Config";
    }

    public class AtlasMenu
    {
        [MenuItem("Hugula/Atlas/1.Create(Refresh) Atlas Asset", false, 101)]
        [MenuItem("Assets/Atlas/1.Create(Refresh) Atlas Asset", false, 101)]
        public static void CreateAtlasAsset()
        {
            var selection = Selection.objects;
            string path = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (Object s in selection)
            {
                if (s is DefaultAsset && (path = AssetDatabase.GetAssetPath(s)) != null && Directory.Exists(path))
                {
                    var ragName = s.name.ToLower() + "_atlas.asset";
                    string atlas_path = Path.Combine(path, ragName);
                    var tagName = s.name.ToLower() + "_atlas";

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
                                    names.Add(LuaHelper.StringToHash(item.name));
                                    allSprites.Add((Sprite)item);
                                }
                            }
                            ti.spritePackingTag = tagName;
                            ti.assetBundleName = tagName + Common.CHECK_ASSETBUNDLE_SUFFIX;
                            EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + count + " / " + allchildren.Count + ")", count / allchildren.Count);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    //生成或者替换资源
                    AtlasAsset atlas = AssetDatabase.LoadAssetAtPath<AtlasAsset>(atlas_path);
                    if (atlas == null)
                    {
                        atlas = AtlasAsset.CreateInstance<AtlasAsset>();
                        AssetDatabase.CreateAsset(atlas, atlas_path);
                    }

                    atlas.names = names;
                    atlas.sprites = allSprites;
                    EditorUtility.SetDirty(atlas);
                    var import = AssetImporter.GetAtPath(atlas_path);
                    import.assetBundleName = Path.GetFileNameWithoutExtension(ragName) + Common.CHECK_ASSETBUNDLE_SUFFIX;
                    sb.AppendFormat("build {0} success  count = {1} ", ragName, names.Count);
                    AssetDatabase.SaveAssets();

                }
            }

            sb.AppendLine("\r\nall completed");
            Debug.Log(sb.ToString());
        }

        [MenuItem("Hugula/Atlas/2.Generate All Atlas Mapping", false, 103)]
        [MenuItem("Assets/Atlas/2.Generate All Atlas Mapping", false, 103)]
        public static void GenerateAllAtlasMapping()
        {
            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
                p.EndsWith(".asset")
            ).ToArray();

            List<int> allSprites = new List<int>();
            List<string> atlasNames = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                var o = AssetDatabase.LoadAssetAtPath<AtlasAsset>(files[i]);
                var ti = AssetImporter.GetAtPath(files[i]);
                if (o != null)
                {
                    var assetBundleName = ti.assetBundleName;
                    EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + i + " / " + files.Length + ")", (float)i / (float)files.Length);
                    for (int j = 0; j < o.names.Count; j++)
                    {
                        allSprites.Add(o.names[j]);
                        atlasNames.Add(assetBundleName);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            string atlas_path = Path.Combine(AtlasConfig.ATLAS_MAPPING_ROOT, AtlasManager.ATLAS_MAPPING_ROOT_NAME + ".asset");
            //生成或者替换资源
            AssetBundleMappingAsset atlas = AssetDatabase.LoadAssetAtPath<AssetBundleMappingAsset>(atlas_path);
            if (atlas == null)
            {
                atlas = AtlasAsset.CreateInstance<AssetBundleMappingAsset>();
                AssetDatabase.CreateAsset(atlas, atlas_path);
            }

            atlas.assetNames = allSprites.ToArray();
            atlas.abNames = atlasNames.ToArray();
            EditorUtility.SetDirty(atlas);
            var import = AssetImporter.GetAtPath(atlas_path);
            import.assetBundleName = AtlasManager.ATLAS_MAPPING_ROOT_NAME + Common.CHECK_ASSETBUNDLE_SUFFIX;
            AssetDatabase.SaveAssets();
            Debug.LogFormat(" save {0}  count = {1} ", atlas_path, atlasNames.Count);
        }

    }
}
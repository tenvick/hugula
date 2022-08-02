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
using System.Linq;

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
                        var textureImporterFormat = new TextureImporterPlatformSettings ();
                        textureImporterFormat.format = TextureImporterFormat.ASTC_6x6;
                        textureImporterFormat.maxTextureSize = 2048;
                        textureImporterFormat.overridden = true;
                        textureImporterFormat.crunchedCompression = true;
                        textureImporterFormat.androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality16Bit;

                        atlas.SetTextureSettings(textureSet);
                        atlas.SetPackingSettings(packSetting);
                        atlas.Add(new[] { obj });

                        atlas.SetPlatformSettings(textureImporterFormat);

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

                    AddToAddress(atlasPath,tagName);
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
           UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(HugulaEditor.ResUpdate.BuildConfig.BuildTarget);

            StringBuilder sb = new StringBuilder();
            List<int> allSprites = new List<int>();
            List<string> atlasNames = new List<string>();
            List<int> atlasIndexs = new List<int>();

            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
              p.EndsWith(".spriteatlas")
            ).ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                var o = AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteAtlas>(files[i]);
                var key = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                if (o != null)
                {
                    EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + i + " / " + files.Length + ")", (float)i / (float)files.Length);
                    var sps = new UnityEngine.Sprite[o.spriteCount];

                    int len = o.GetSprites(sps);
                    for (int j = 0; j < len; j++)
                    {
                        string name = sps[j].name.Replace("(Clone)", "");
                        int id = UnityEngine.Animator.StringToHash(name);
                        allSprites.Add(id);
                        var idx = atlasNames.IndexOf(key);
                        if(idx<0) 
                        {
                            atlasNames.Add(key);
                            idx = atlasNames.Count -1;
                        }
                        atlasIndexs.Add(idx);
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
            atlas.keys = atlasIndexs.ToArray();
            atlas.sourceKeys = atlasNames.ToArray();
            EditorUtility.SetDirty(atlas);

            //aas 
            var atlasGroup = AASEditorUtility.FindGroup(mapping_root_name, AASEditorUtility.DefaltGroupSchema[0]);
            var setting = AASEditorUtility.LoadAASSetting();
            var guid = AssetDatabase.AssetPathToGUID(atlas_path); //获得GUID
            var entry = setting.CreateOrMoveEntry(guid, atlasGroup); //通过GUID创建entry
            entry.SetAddress(mapping_root_name);
            AssetDatabase.SaveAssets();
            Debug.LogFormat(" save {0}  count = {1} ", atlas_path, atlasNames.Count);
            EditorUtils.WriteToTmpFile(mapping_root_name + ".txt", sb.ToString());

        }

        private static void AddToAddress(string assetPath,string groupName)
        {
            var setting = AASEditorUtility.LoadAASSetting();
            var groupSchama = AASEditorUtility.DefaltGroupSchema[0];
            var group = AASEditorUtility.FindGroup(groupName, groupSchama);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var altasEntry = group.GetAssetEntry(guid);
            if(altasEntry == null)
                altasEntry =  setting.CreateOrMoveEntry(guid, group); //通过GUID创建entry
            altasEntry.SetAddress(System.IO.Path.GetFileNameWithoutExtension(assetPath));

        }
    }
}
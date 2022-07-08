using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;
using Hugula.Utils;
using Hugula.Audio;
using Hugula;

namespace HugulaEditor
{
    public static class AudioMenu
    {
        [MenuItem("Hugula/Audio/1.Create(Refresh) Default AudioClip Asset ", false, 101)]
        [MenuItem("Assets/Audio/1.Create(Refresh) Default AudioClip Asset", false, 101)]
        public static void CreateAudioClipAsset()
        {
            var selection = Selection.objects;
            string path = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (Object s in selection)
            {
                if (s is DefaultAsset && (path = AssetDatabase.GetAssetPath(s)) != null && Directory.Exists(path))
                {
                    var ragName = s.name.ToLower() + "_audio.asset";
                    // var tagName = s.name.ToLower() + "_atlas";

                    string res_path = Path.Combine(path, ragName);

                    sb.Append("Crate audioclip asset :");
                    sb.Append(ragName);
                    sb.Append("\r\n");

                    var allchildren = EditorUtils.getAllChildFiles(path, @"\.meta$|\.manifest$|\.DS_Store$|\.u$", null, false);
                    List<int> names = new List<int>();
                    List<AudioClip> audioClips = new List<AudioClip>();

                    for (int i = 0; i < allchildren.Count; i++)
                    {
                        var itemPath = allchildren[i];
                        // var ti = AssetImporter.GetAtPath(itemPath);
                        var aclip = AssetDatabase.LoadAssetAtPath<AudioClip>(itemPath);
                        if (aclip)
                        {
                            audioClips.Add(aclip);
                            names.Add(LuaHelper.StringToHash(aclip.name));
                            // if (ti != null)
                            // {
                            //     ti.assetBundleName = tagName + Common.CHECK_ASSETBUNDLE_SUFFIX;
                            // }
                        }

                        EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + (i + 1) + " / " + allchildren.Count + ")", (i + 1) / allchildren.Count);
                    }

                    EditorUtility.ClearProgressBar();

                    //生成或者替换资源
                    var audioAsset = AssetDatabase.LoadAssetAtPath<AudioClipAsset>(res_path);
                    if (audioAsset == null)
                    {
                        audioAsset = AudioClipAsset.CreateInstance<AudioClipAsset>();
                        AssetDatabase.CreateAsset(audioAsset, res_path);
                    }

                    audioAsset.audioClips = audioClips.ToArray();
                    audioAsset.audioNames = names.ToArray();
                    EditorUtility.SetDirty(audioAsset);

                    var groupAssets = new List<string>();
                    groupAssets.Add(res_path);
                    var atlasGroup = AASEditorUtility.FindGroup(AudioClipAsset.DEFALUT_SOUND_ASSET_NAME, AASEditorUtility.DefaltGroupSchema[0]);
                    AASEditorUtility.SetGroupAddress(AASEditorUtility.LoadAASSetting(), atlasGroup, groupAssets);
                    sb.AppendFormat("build {0} success  count = {1} ", ragName, names.Count);
                    AssetDatabase.SaveAssets();

                }
            }

            sb.AppendLine("\r\nall completed");
            Debug.Log(sb.ToString());
        }

        /**

        [MenuItem("Hugula/Audio/2.Generate AudioClip Asset Mapping", false, 103)]
        [MenuItem("Assets/Audio/2.Generate AudioClip Asset Mapping", false, 103)]
        public static void GenerateAllAtlasMapping()
        {

            var files = AssetDatabase.FindAssets("t:AudioClipAsset");

            List<int> allAudioclip = new List<int>();
            List<string> abNames = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(files[i]);
                var o = AssetDatabase.LoadAssetAtPath<AudioClipAsset>(path);
                var ti = AssetImporter.GetAtPath(path);
                if (o != null)
                {
                    var assetBundleName = ti.assetBundleName;
                    EditorUtility.DisplayProgressBar("Processing...", "生成中... (" + i + " / " + files.Length + ")", (float)i / (float)files.Length);
                    for (int j = 0; j < o.audioNames.Length; j++)
                    {
                        allAudioclip.Add(o.audioNames[j]);
                        abNames.Add(assetBundleName);//o.name
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            // string atlas_path = Path.Combine(AtlasConfig.ATLAS_MAPPING_ROOT, AudioClipMappingManager.MAPPING_ROOT_NAME + ".asset");
            // //生成或者替换资源
            // AssetBundleMappingAsset atlas = AssetDatabase.LoadAssetAtPath<AssetBundleMappingAsset>(atlas_path);
            // if (atlas == null)
            // {
            //     atlas = AssetBundleMappingAsset.CreateInstance<AssetBundleMappingAsset>();
            //     AssetDatabase.CreateAsset(atlas, atlas_path);
            // }

            // atlas.assetNames = allAudioclip.ToArray();
            // atlas.abNames = abNames.ToArray();
            // EditorUtility.SetDirty(atlas);
            // var import = AssetImporter.GetAtPath(atlas_path);
            // import.assetBundleName = AudioClipMappingManager.MAPPING_ROOT_NAME + Common.CHECK_ASSETBUNDLE_SUFFIX;
            // AssetDatabase.SaveAssets();
            // Debug.LogFormat(" save {0}  count = {1} ", atlas_path, abNames.Count);
        }

        **/

    }
}

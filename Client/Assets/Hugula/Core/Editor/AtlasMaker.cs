using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.IO;
public class AtlasMaker  {
    static List<string> usingAtlasNames;
    static List<Atlas> usingAtlas;

    [MenuItem("AssetBundles/Create Or Update Atlas", priority = -1)]
    public static void CreateAtlas()
    {
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);  

        string[] allAssetsPath=AssetDatabase.GetAllAssetPaths();
        usingAtlasNames = new List<string>();
        usingAtlas = new List<Atlas>();
        
        for (int i = 0; i < allAssetsPath.Length; i++)
        {
            string path=allAssetsPath[i];
            if (path.IndexOf("/Resources/") != -1) continue;//TODO 暂时防止Resources目录下的物体被打包

            Sprite s=AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (s != null)
            {
                TextureImporter ti=(TextureImporter) AssetImporter.GetAtPath(path);
                if (!string.IsNullOrEmpty(ti.spritePackingTag))
                {
                    PushSprite(s, ti.spritePackingTag, ti.assetBundleName);
                }
            }
        }


        //检查===
        for (int i = 0; i < usingAtlas.Count; i++)
        {
            Atlas atlas = usingAtlas[i];

            Debug.Log("Generated "+ EditorAtlasUtilites.atlasStorePath+ usingAtlasNames[i] + ".prefab");
            for (int j = 0; j < atlas.confuseIndex.Count; j++)
                Debug.LogWarning("!same sprite name in the atlas: " + atlas.names[atlas.confuseIndex[j]]);

            if (atlas.bundleNames.Count > 1)
            {
                for (int j = 0; j < atlas.bundleNames.Count; j++)
                    Debug.LogWarning("!sprite group in different bundle :" + atlas.bundleNames[j]);
            }
            EditorUtility.SetDirty(atlas);
        }
    }
    static void PushSprite(Sprite sprite, string tag, string bundleName)
    {
        Atlas atlas;
        int index;
        index = usingAtlasNames.IndexOf(tag);

        if (index != -1)
        {
            atlas = usingAtlas[index];
        }
        else
        {
            atlas = EditorAtlasUtilites.GetAtlas(tag);
            atlas.names = new List<string>();
            atlas.sprites = new List<Sprite>();
            atlas.confuseIndex = new List<int>();
            atlas.bundleNames = new List<string>();
            usingAtlasNames.Add(tag);
            usingAtlas.Add(atlas);
            index = usingAtlas.Count - 1;
        }

        index = atlas.names.IndexOf(sprite.name);
        if (index != -1)
            atlas.confuseIndex.Add(index);
        index = atlas.bundleNames.IndexOf(bundleName);
        if (index == -1)
            atlas.bundleNames.Add(bundleName);

        atlas.names.Add(sprite.name);
        atlas.sprites.Add(sprite);
    }

   
}

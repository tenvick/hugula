using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Atlas : MonoBehaviour {
    public List<string> names;
    public List<Sprite> sprites;

    public Sprite GetSpriteByName(string name)
    {
        int index = names.IndexOf(name);
        if (index == -1)
            return null;
        else
            return sprites[index];
    }

#if UNITY_EDITOR
    
    //重名的文件提示
    [HideInInspector]
    public List<int> confuseIndex;
    //打包的资源名称
    [HideInInspector]
    public List<string> bundleNames;
    void Awake()
    {
        hideFlags = HideFlags.NotEditable;
    }
#endif

   
}


#if UNITY_EDITOR
    public class EditorAtlasUtilites
    {
        public static string atlasStorePath = "Assets/CustomerResource/Export/Tetris/";
        public static Atlas GetAtlas(Sprite s)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(s);
            UnityEditor.TextureImporter ti = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
            if (!string.IsNullOrEmpty(ti.spritePackingTag))
                return GetAtlas(ti.spritePackingTag);
            return null;
        }

        public static Atlas GetAtlas(string tag)
        {
            string name = tag + "_atlas";
            string storePath = System.IO.Path.Combine(atlasStorePath, name + ".prefab");
            Atlas atlas;
            GameObject o = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(storePath);
            if (o == null)
            {
                o = new GameObject();
                atlas = o.AddComponent<Atlas>();
                UnityEditor.PrefabUtility.CreatePrefab(storePath, o);
                GameObject.DestroyImmediate(o);
                o = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(storePath);
            }
            atlas = o.GetComponent<Atlas>();
            atlas.hideFlags = HideFlags.NotEditable;

            return atlas;
        }
    }
#endif
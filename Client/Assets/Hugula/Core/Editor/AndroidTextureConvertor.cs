// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AndroidTextureConvertor
{

    [MenuItem("Hugula/", false, 20)]
    static void Breaker() { }

     //[MenuItem("Hugula/AndroidAtlasConver ", false, 21)]
     //[MenuItem("Assets/Hugula_AndroidAtlasConver", false, 21)]
    static void ConvertUIAtlas()
    {
        /**
        Object selPrefab = Selection.activeObject;

        string path = AssetDatabase.GetAssetPath(selPrefab);
        path = path.Replace("Assets/", "");
        path = Application.dataPath + "/" + path;

        string prefabName = CUtils.GetURLFullFileName(path);
        string folderPath = path.Replace(prefabName, "");

        string androidFolderPath = folderPath + "android/";
        if (!Directory.Exists(androidFolderPath)) Directory.CreateDirectory(androidFolderPath);
        string proAndroidFolderPath = androidFolderPath.Replace(Application.dataPath, "Assets");

        GameObject obj = Object.Instantiate(selPrefab) as GameObject;
        obj.name = obj.name.Replace("(Clone)", "");

        UIAtlas at = obj.GetComponent<UIAtlas>();
        if (at != null)
        {
            Material mat = at.spriteMaterial;

            string atlasPrefabPath = proAndroidFolderPath + at.name + ".prefab";
            GameObject newAtlasPrefab = PrefabUtility.CreatePrefab(atlasPrefabPath, obj);
            GameObject.DestroyImmediate(obj);

            Material newMat = Material.Instantiate(mat) as Material;
            newMat.name = newMat.name.Replace("(Clone)", "");

            string matPath = proAndroidFolderPath + newMat.name + ".mat";
            AssetDatabase.CreateAsset(newMat, matPath);
            newAtlasPrefab.GetComponent<UIAtlas>().spriteMaterial = newMat;

            Texture2D t = mat.mainTexture as Texture2D;
            string texturePath = AssetDatabase.GetAssetPath(t);

            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            textureImporter.textureType = TextureImporterType.Advanced;
            textureImporter.isReadable = true;
            textureImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

            //Debug.Log("propertives updated");

            Color32[] cols = t.GetPixels32();
            Color[] cCols = new Color[cols.Length];
            Color[] aCols = new Color[cols.Length];
            int id = -1;
            foreach (Color32 c in cols)
            {
                id++;

                cCols.SetValue(new Color((float)c.r / 255, (float)c.g / 255, (float)c.b / 255), id);
                aCols.SetValue(new Color((float)c.a / 255, (float)c.a / 255, (float)c.a / 255), id);
            }

            Texture2D colorTexture = new Texture2D(t.width, t.height);
            colorTexture.SetPixels(cCols);
            colorTexture.Apply();

            Texture2D maskTexture = new Texture2D(t.width, t.height);
            maskTexture.SetPixels(aCols);
            maskTexture.Apply();

            //Debug.Log("pixels done");

            //----------------------------------
            string cTexPath = androidFolderPath + t.name + "_c.png";
            string aTexPath = androidFolderPath + t.name + "_a.png";

            byte[] byt = colorTexture.EncodeToPNG();
            File.WriteAllBytes(cTexPath, byt);

            byt = maskTexture.EncodeToPNG();
            File.WriteAllBytes(aTexPath, byt);

            //Debug.Log("-----------------------texture saved");
            cTexPath = cTexPath.Replace(Application.dataPath, "Assets");
            aTexPath = aTexPath.Replace(Application.dataPath, "Assets");

            AssetDatabase.ImportAsset(cTexPath);
            AssetDatabase.ImportAsset(aTexPath);

            Texture cTex = AssetDatabase.LoadAssetAtPath(cTexPath, typeof(Texture)) as Texture;
            Texture aTex = AssetDatabase.LoadAssetAtPath(aTexPath, typeof(Texture)) as Texture;

            newMat.shader = Shader.Find("Unlit/ExtraAlpha Colored");
            newMat.mainTexture = cTex;
            newMat.SetTexture("_AlphaMap", aTex);

            string[] paths = new string[] { cTexPath, aTexPath };
            foreach (string p in paths)
            {
                TextureImporter tImporter = AssetImporter.GetAtPath(p) as TextureImporter;
                tImporter.textureType = TextureImporterType.Advanced;
                tImporter.mipmapEnabled = false;
				tImporter.maxTextureSize = 2048;
				tImporter.isReadable = false;
				
				//AssetDatabase.ImportAsset(p, ImportAssetOptions.ForceUpdate);
            }
			

            textureImporter.isReadable = false;
            //AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

//			at.spriteMaterial = newMat;
        }
         * 
         * **/
    }

    //----------------------------------------------------------------------------------------------------------------

   // [MenuItem("AndroidTool/ConvertObject3D")]
    static void ConvertObject3D()
    {
        Object selPrefab = Selection.activeObject;

        string path = AssetDatabase.GetAssetPath(selPrefab);
        path = path.Replace("Assets/", "");
        path = Application.dataPath + "/" + path;

        string prefabName = CUtils.GetURLFullFileName(path);
        string folderPath = path.Replace(prefabName, "");

        string androidFolderPath = folderPath + "android/";
        string androidMatFolder = androidFolderPath + "materials/";
        string androidTexFolder = androidFolderPath + "textures/";

        if (!Directory.Exists(androidFolderPath)) Directory.CreateDirectory(androidFolderPath);
        if (!Directory.Exists(androidMatFolder)) Directory.CreateDirectory(androidMatFolder);
        if (!Directory.Exists(androidTexFolder)) Directory.CreateDirectory(androidTexFolder);

        objPrefabToAndroid(selPrefab, androidFolderPath, androidMatFolder, androidTexFolder);
    }

   // [MenuItem("AndroidTool/ConvertObject3DFolder")]
    static void ConvertObject3DFolder()
    {
        Object selPrefab = Selection.activeObject;

        string path = AssetDatabase.GetAssetPath(selPrefab);
        path = path.Replace("Assets/", "");
        path = Application.dataPath + "/" + path;

        string prefabName = CUtils.GetURLFullFileName(path);
        if (Selection.activeObject is GameObject)
            path = path.Replace(prefabName, "");
        else path += "/";

        string androidFolderPath = path + "android/";
        string androidMatFolder = androidFolderPath + "materials/";
        string androidTexFolder = androidFolderPath + "textures/";

        if (!Directory.Exists(androidFolderPath)) Directory.CreateDirectory(androidFolderPath);
        if (!Directory.Exists(androidMatFolder)) Directory.CreateDirectory(androidMatFolder);
        if (!Directory.Exists(androidTexFolder)) Directory.CreateDirectory(androidTexFolder);

        EditorUtility.DisplayProgressBar("transforming:", "", 0);
        string[] fileEntries = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        int len = fileEntries.Length;
        for (int i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("transforming:", (i + 1).ToString() + "/" + len.ToString(), (float)(i + 1) / len);

            string file = fileEntries[i];
            file = file.Replace("\\", "/");
            file = file.Replace(Application.dataPath, "Assets");

            Object obj = AssetDatabase.LoadMainAssetAtPath(file);
            objPrefabToAndroid(obj, androidFolderPath, androidMatFolder, androidTexFolder);
        }
        EditorUtility.ClearProgressBar();
    }

    static void objPrefabToAndroid(Object prefab, string androidFolderPath, string androidMatFolder, string androidTexFolder)
    {
        GameObject newObj = Object.Instantiate(prefab) as GameObject;
        newObj.name = newObj.name.Replace("(Clone)", "");

        string newPrePath = androidFolderPath + newObj.name+".prefab";
        newPrePath = newPrePath.Replace(Application.dataPath, "Assets");

        GameObject newPrefab = PrefabUtility.CreatePrefab(newPrePath, newObj);
        GameObject.DestroyImmediate(newObj);

        GameObject test = Object.Instantiate(newPrefab) as GameObject;

        //get all children transforms
        Transform[] TMs = test.GetComponentsInChildren<Transform>();

        Dictionary<string, Material> mats = new Dictionary<string, Material>();
        foreach (Transform t in TMs)
        {
            GameObject obj = t.gameObject;

            //-----------------通过Renderer查找材质
            Renderer ren = obj.GetComponent<MeshRenderer>();
            if (ren == null) ren = obj.GetComponent<SkinnedMeshRenderer>();

            if (ren != null)
            {
                Material[] submats = ren.sharedMaterials;
                foreach (Material mat in submats)
                {
                    if (mat.shader.name == "Unlit/Transparent" || mat.shader.name == "Unlit/Premultiplied Colored" || mat.shader.name == "Transparent/colorTint" || mat.shader.name == "FXMaker/Mask Alpha Blended Tint")
                    {
                        //Debug.Log("obj:" + obj.name);
                        if (!mats.ContainsKey(mat.name) && !File.Exists(androidMatFolder+mat.name+".mat"))
                            mats[mat.name] = Material.Instantiate(mat) as Material;
                    }
                }
            }
        }

        Dictionary<string, Texture2D> texs = new Dictionary<string, Texture2D>();
        string matPath = androidMatFolder.Replace(Application.dataPath, "Assets");
        foreach (Material mat in mats.Values)
        {
            mat.name = mat.name.Replace("(Clone)", "");

            AssetDatabase.CreateAsset(mat, matPath + mat.name + ".mat");
            texs[mat.mainTexture.name] = mat.mainTexture as Texture2D;

            if (mat.shader.name == "FXMaker/Mask Alpha Blended Tint")
            {
                Texture2D maskTex = mat.GetTexture("_Mask") as Texture2D;
                texs[maskTex.name] = maskTex;

                mat.shader = Shader.Find("Transparent/extraAlphaMask");
                mat.SetTexture("_Mask", maskTex);
            }
            else if (mat.shader.name == "FXMaker/Mask Alpha Blended Tint")
            {
                mat.shader = Shader.Find("Transparent/extraAlphaTint");
            }
            else
            {
                mat.shader = Shader.Find("Transparent/extraAlpha");
            }
        }

        foreach (Transform t in TMs)
        {
            GameObject obj = t.gameObject;
			
			Debug.Log("start reset mat:"+obj.name);
			
            Renderer ren = obj.GetComponent<MeshRenderer>();
            if (ren == null) ren = obj.GetComponent<SkinnedMeshRenderer>();

            if (ren != null)
            {
                Material[] submats = ren.sharedMaterials;

                for (int i = 0; i < submats.Length; i++)
                {
                    if (mats.ContainsKey(submats[i].name))
					{
							submats[i] = mats[submats[i].name];
					}else
					{
						if(File.Exists(androidMatFolder+submats[i].name+".mat"))
						{
							string localMatPath = androidMatFolder+submats[i].name+".mat";
							localMatPath = localMatPath.Replace(Application.dataPath,"Assets");
							
							Debug.Log(obj.name+" loadMatAtPath:"+localMatPath);
							submats[i] = AssetDatabase.LoadAssetAtPath(localMatPath,typeof(Material)) as Material;
						}
					}
                }
				
                ren.sharedMaterials = submats;
            }
        }

        newPrefab = PrefabUtility.CreatePrefab(newPrePath, test);
        GameObject.DestroyImmediate(test);

        foreach (Texture2D t in texs.Values)
        {
            //Debug.Log("texture:" + t.name);
			
			string cTexPath = androidTexFolder + t.name + "_c.png";
            string aTexPath = androidTexFolder + t.name + "_a.png";
			
			if(!File.Exists(cTexPath) && !File.Exists(aTexPath))
			{
				string texturePath = AssetDatabase.GetAssetPath(t);
	            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
	
	            textureImporter.textureType = TextureImporterType.Advanced;
	            textureImporter.isReadable = true;
	            textureImporter.mipmapEnabled = false;
	            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
	
	            //Debug.Log("propertives updated");
	
	            Color32[] cols = t.GetPixels32();
	            Color[] cCols = new Color[cols.Length];
	            Color[] aCols = new Color[cols.Length];
	            int id = -1;
	            foreach (Color32 c in cols)
	            {
	                id++;
	
	                cCols.SetValue(new Color((float)c.r / 255, (float)c.g / 255, (float)c.b / 255), id);
	                aCols.SetValue(new Color((float)c.a / 255, (float)c.a / 255, (float)c.a / 255), id);
	            }
	
	            Texture2D colorTexture = new Texture2D(t.width, t.height);
	            colorTexture.SetPixels(cCols);
	            colorTexture.Apply();
	
	            Texture2D maskTexture = new Texture2D(t.width, t.height);
	            maskTexture.SetPixels(aCols);
	            maskTexture.Apply();
	
	            // Debug.Log("pixels done");
	
	            byte[] byt = colorTexture.EncodeToPNG();
	            File.WriteAllBytes(cTexPath, byt);
	
	            byt = maskTexture.EncodeToPNG();
	            File.WriteAllBytes(aTexPath, byt);
	
	            textureImporter.isReadable = false;
	            //Debug.Log("-----------------------texture saved");
	            cTexPath = cTexPath.Replace(Application.dataPath, "Assets");
	            aTexPath = aTexPath.Replace(Application.dataPath, "Assets");
	
	            AssetDatabase.ImportAsset(cTexPath);
	            AssetDatabase.ImportAsset(aTexPath);
			}
            
        }

        foreach (Material mat in mats.Values)
        {
            string oldTexName = mat.mainTexture.name;

            string cTexPath = androidTexFolder + oldTexName + "_c.png";
            string aTexPath = androidTexFolder + oldTexName + "_a.png";

            cTexPath = cTexPath.Replace(Application.dataPath, "Assets");
            aTexPath = aTexPath.Replace(Application.dataPath, "Assets");

            Texture cTex = AssetDatabase.LoadAssetAtPath(cTexPath, typeof(Texture)) as Texture;
            Texture aTex = AssetDatabase.LoadAssetAtPath(aTexPath, typeof(Texture)) as Texture;
            //----------------

            mat.mainTexture = cTex;
            mat.SetTexture("_AlphaMap", aTex);
            //----------------

            List<string> paths = new List<string>();
            paths.Add(cTexPath);
            paths.Add(aTexPath);

            if (mat.shader.name == "Transparent/extraAlphaMask")
            {
                string maskTexName = mat.GetTexture("_Mask").name;

                string dTexPath = androidTexFolder + maskTexName + "_c.png";
                File.Delete(dTexPath);
                string metaPath = dTexPath + ".meta";
                if (File.Exists(metaPath)) File.Delete(metaPath);

                string mTexPath = androidTexFolder + maskTexName + "_a.png";
                mTexPath = mTexPath.Replace(Application.dataPath, "Assets");

                Texture mTex = AssetDatabase.LoadAssetAtPath(mTexPath, typeof(Texture)) as Texture;
                mat.SetTexture("_Mask", mTex);

                paths.Add(mTexPath);
            }

            foreach (string p in paths)
            {
                TextureImporter tImporter = AssetImporter.GetAtPath(p) as TextureImporter;
                tImporter.textureType = TextureImporterType.Advanced;
                tImporter.mipmapEnabled = false;
            }
        }
        System.GC.Collect();
    }



}

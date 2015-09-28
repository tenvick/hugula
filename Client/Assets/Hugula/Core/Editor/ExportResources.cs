// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class ExportResources{
	
	#region public p
	
	public const string OutLuaPath="/Tmp/"+Common.LUACFOLDER+"/";
	public static string outConfigPath=Application.streamingAssetsPath+"/config.tz";
	//public const string zipPassword="hugula@pl@";
	public static string outAndroidZipt4f=Application.streamingAssetsPath+"/data.zip";
#if Nlua 
    #if UNITY_EDITOR_OSX
	    public static string luacPath=Application.dataPath+"/../tools/luaTools/luac";
    #else
        public static string luacPath = Application.dataPath + "/../tools/luaTools/win/luac.exe";
    #endif
#else
	#if UNITY_EDITOR_OSX && UNITY_IPHONE
	public static string luacPath=Application.dataPath+"/../tools/luaTools/lua5.1.4c";
	#elif UNITY_EDITOR_OSX 
	public static string luacPath=Application.dataPath+"/../tools/luaTools/luajit";
	#else
    public static string luacPath = Application.dataPath + "/../tools/luaTools/win/luajit.exe";
    #endif
#endif
	#endregion

	#region 

    //[MenuItem("Hugula AES/GenerateKey", false, 12)]
	public static void GenerateKey() 
	{ 
		using (System.Security.Cryptography.RijndaelManaged myRijndael = new System.Security.Cryptography.RijndaelManaged()) {
			
			myRijndael.GenerateKey ();
			byte[] Key=myRijndael.Key;

			KeyVData KeyScri=ScriptableObject.CreateInstance<KeyVData>();
			KeyScri.KEY=Key;
			AssetDatabase.CreateAsset(KeyScri,"Assets/Config/I81.asset");

			Debug.Log("key Generate "+Key.Length);

		}
	}

    //[MenuItem("Hugula AES/GenerateIV", false, 13)]
	public static void GenerateIV() 
	{ 
		using (System.Security.Cryptography.RijndaelManaged myRijndael = new System.Security.Cryptography.RijndaelManaged()) {
			
			myRijndael.GenerateIV ();
			byte[]  IV=myRijndael.IV;

			KeyVData KeyScri=ScriptableObject.CreateInstance<KeyVData>();
			KeyScri.IV=IV;
			AssetDatabase.CreateAsset(KeyScri,"Assets/Config/K81.asset");
			Debug.Log("IV Generate "+IV.Length);
		}
	}

	static byte[] GetKey()
	{
		KeyVData kv = (KeyVData)AssetDatabase.LoadAssetAtPath("Assets/Config/I81.asset",typeof(KeyVData));
		return kv.KEY;
	}

	static byte[] GetIV()
	{
		KeyVData kv = (KeyVData)AssetDatabase.LoadAssetAtPath("Assets/Config/K81.asset",typeof(KeyVData));
		return kv.IV;
	}

	#endregion

    #region export
   
	public static void exportLua()
	{
		checkLuaExportPath();

        string  path= Application.dataPath+"/Lua/"; //AssetDatabase.GetAssetPath(obj).Replace("Assets","");
		
        List<string> files =getAllChildFiles(path);// Directory.GetFiles(Application.dataPath + path);
		
		IList<string> childrens = new List<string>();
		
		foreach (string file in files)
         {
             if (file.EndsWith("lua")) 
			 {
                 childrens.Add(file);
             }
         }
        Debug.Log("luajit path = "+luacPath);
		string crypName="",fileName="",outfilePath="",arg="";
		System.Text.StringBuilder sb=new System.Text.StringBuilder();
		//refresh directory
		DirectoryDelete(Application.dataPath + OutLuaPath);
		CheckDirectory(Application.dataPath+OutLuaPath);
		//CheckDirectory(Application.dataPath.Replace("Assets","")+outPath);

		List<string> exportNames = new List<string> ();
		 foreach (string filePath in childrens)
         {
			fileName=CUtils.GetURLFileName(filePath);
			crypName = filePath.Replace(path,"").Replace(".lua","."+Common.LUA_LC_SUFFIX).Replace("\\","_").Replace("/","_");
			outfilePath=Application.dataPath+OutLuaPath+crypName;
			exportNames.Add("Assets"+OutLuaPath+crypName);
			sb.Append(fileName);
			sb.Append("=");
			sb.Append(crypName);
			sb.Append("\n");

#if Nlua || UNITY_IPHONE 
			arg="-o "+outfilePath+" "+filePath;// luac 
			File.Copy(filePath, outfilePath, true);// source code copy
#else
            arg = "-b " + filePath + " " + outfilePath; //for jit
            //Debug.Log(arg);
            //System.Diagnostics.Process.Start(luacPath, arg);//jit 
            File.Copy(filePath, outfilePath, true);// source code copy
#endif
		 }
		 Debug.Log("lua:"+path+"files="+files.Count+" completed");
         System.Threading.Thread.Sleep(1000);
         AssetDatabase.Refresh();

		//u5 打包
		string outPath= ExportAssetBundles.GetOutPath();
		CheckDirectory(Application.dataPath.Replace("Assets","")+outPath);
		ExportAssetBundles.BuildABs (exportNames.ToArray(),"Assets"+OutLuaPath ,"luaout.bytes",BuildAssetBundleOptions.CompleteAssets);

		//Encrypt
		string tarName = Application.dataPath + OutLuaPath + "luaout.bytes";
		string realOutPath=ExportAssetBundles.GetOutPath()+"/font.u3d";
		byte[] by = File.ReadAllBytes (tarName);
		byte[] Encrypt = CryptographHelper.Encrypt (by, GetKey (), GetIV());
		File.WriteAllBytes (realOutPath,Encrypt);
		Debug.Log(realOutPath + " export");

	}

	public static void exportConfig()
	{
		string  path= Application.dataPath+"/Config"; //AssetDatabase.GetAssetPath(obj).Replace("Assets","");

        List<string> files = getAllChildFiles(path+"/", "csv");
        string assetPath = "Assets/Config";
        List<string> res = new List<string>();

        foreach (string name in files)
        {
            string abPath = assetPath + name.Replace(path, "");
            Debug.Log(abPath);
			res.Add(abPath);
        }

        string cname = "font1.u3d";
        string outPath= ExportAssetBundles.GetOutPath();
        string tarName = outPath +"/"+ cname;
		CheckDirectory(Application.dataPath.Replace("Assets","")+outPath);

		ExportAssetBundles.BuildABs (res.ToArray (), outPath, cname, BuildAssetBundleOptions.CompleteAssets);
		Debug.Log(tarName + " export");
	}

    public static void exportLanguage()
    {
        string assetPath = "Assets/Lan/";
        string path = Application.dataPath + "/Lan/"; //AssetDatabase.GetAssetPath(obj).Replace("Assets","");
        List<string> files = getAllChildFiles(path,"csv");

		string outPath = ExportAssetBundles.GetOutPath();
		CheckDirectory(Application.dataPath.Replace("Assets","")+outPath);
		CheckDirectory(Application.dataPath.Replace("Assets","")+outPath+"/Lan");

        foreach (string name in files)
        {
            string abPath = assetPath + name.Replace(path, "");
//            Debug.Log(abPath);
			string tarPath = outPath+"/Lan";
			string abName = CUtils.GetURLFileName(name)+"."+Common.LANGUAGE_SUFFIX;
			ExportAssetBundles.BuildABs(new string[]{abPath},tarPath,abName,BuildAssetBundleOptions.CompleteAssets);
			Debug.Log(tarPath+abName +" export");

        }
    }

 
    //[MenuItem("Hugula/build for publish ", false, 16)]
	public static void exportPublish()
	{
		exportLua();
		
		exportConfig();

        exportLanguage();

        autoVerAdd();

	}
	
	#endregion
	
	#region private
    /// <summary>
    /// 资源版本号自动加一
    /// </summary>
    private static void autoVerAdd()
    {
        string curr = "";
        string path=Application.streamingAssetsPath + "/Ver.t";

         using (FileStream fs = File.Open(path,FileMode.OpenOrCreate,FileAccess.Read))
        {
            StreamReader sr = new StreamReader(fs);
             curr = sr.ReadToEnd();
             Debug.Log("current ver is " + curr);
             if (curr=="")curr="0";
             curr = (int.Parse(curr)+1).ToString();
        }

         using (StreamWriter sr = new StreamWriter(path, false))
         {
             Debug.Log("new ver is " + curr);
             sr.WriteLine(curr);
         }
    }

    private static void CheckDirectory(string fullPath)
    {
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    private static void checkLuaChildDirectory(string fullpath)
    {
        DirectoryInfo info = Directory.GetParent(fullpath);
        string Dir = info.FullName; 
        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
        }
    }
	
	private static void checkLuaExportPath()
	{
		string dircAssert=Application.dataPath+OutLuaPath;
		if(!Directory.Exists(dircAssert))
		{
			Directory.CreateDirectory(dircAssert);
		}
	}
	
	public static List<string> getAllChildFiles(string path,string suffix="lua",List<string> files=null)
	{
		if(files==null)files=new List<string>();
		addFiles(path,suffix,files);
		string[] dires=Directory.GetDirectories(path);
		foreach(string dirp in dires)
		{
//            Debug.Log(dirp);
			getAllChildFiles(dirp,suffix,files);
		}
		return files;
	}
	
    public static void addFiles(string direPath,string suffix,List<string> files)
	{
		string[] fileMys=Directory.GetFiles(direPath);
		foreach(string f in fileMys)
		{
			if(f.ToLower().EndsWith(suffix.ToLower())) 
			{
				files.Add(f);
			}
		}
	}

	public static void DirectoryDelete (string path)
	{
		DirectoryInfo di = new DirectoryInfo(path);
		di.Delete(true);
	}
	#endregion
}

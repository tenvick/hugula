// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.IO;
[SLua.CustomLuaClass]
public class CUtils {
	
	/// <summary>
	/// Gets the Suffix of the URL file.
	/// </summary>
	/// <returns>
	/// The URL file type.
	/// </returns>
	/// <param name='url'>
	/// url.
	/// </param>
	public static string GetURLFileSuffix(string url)
	{
		if (string.IsNullOrEmpty (url))	return string.Empty;
		int last=url.LastIndexOf(".");
		int end=url.IndexOf("?");
		if(end==-1)
			end=url.Length;
		else
		{
			last=url.IndexOf(".",0,end);
		}
//		Debug.Log(string.Format("last={0},end={1}",last,end));
		string cut=url.Substring(last,end-last).Replace(".","");
		return cut;
	}
	
	/// <summary>
	/// Gets the name of the URL file.
	/// </summary>
	/// <returns>
	/// The URL file name.
	/// </returns>
	/// <param name='url'>
	/// URL.
	/// </param>
	public static string GetURLFileName(string url)
	{
		if (string.IsNullOrEmpty (url))	return string.Empty;
		string re = "";
    	int len = url.Length - 1;
		char[] arr=url.ToCharArray();
	    while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
			len = len - 1;
    	//int sub = (url.Length - 1) - len;
		//int end=url.Length-sub;
			
		re = url.Substring(len+1);
		int last=re.LastIndexOf(".");
        if (last == -1) last = re.Length;
		string cut=re.Substring(0,last);
		return cut;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string GetKeyURLFileName(string url)
    {
        //Debug.Log(url);
		if (string.IsNullOrEmpty (url))	return string.Empty;
        string re = "";
        int len = url.Length - 1;
        char[] arr = url.ToCharArray();
        while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
            len = len - 1;
        //int sub = (url.Length - 1) - len;
        //int end=url.Length-sub;

        re = url.Substring(len + 1);
        int last = re.LastIndexOf(".");
        if (last == -1) last = re.Length;
        string cut = re.Substring(0, last);
        cut = cut.Replace('.', '_');
        return cut;
    }
	
	public static string GetURLFullFileName(string url)
	{
		if (string.IsNullOrEmpty (url))	return string.Empty;
		string re = "";
    	int len = url.Length - 1;
		char[] arr=url.ToCharArray();
	    while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
			len = len - 1;
		
		re = url.Substring(len+1);
		return re;
	}
	
	/// <summary>
	/// Gets the file full path for www
	/// form Application.dataPath
	/// </summary>
	/// <returns>
	/// The file full path.
	/// </returns>
	/// <param name='absolutePath'>
	/// Absolute path.
	/// </param>
	public static string GetFileFullPath(string absolutePath)
	{
		string path="";
		
		path=Application.persistentDataPath+"/"+absolutePath;
		currPersistentExist=File.Exists(path);
		if(!currPersistentExist)
			path=Application.streamingAssetsPath+"/"+absolutePath;
		
		if (path.IndexOf("://")==-1)
		{
			path="file://"+path;
		}
		
		return path;
	}

    /// <summary>
    /// get assetBunld full path
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public static string GetAssetFullPath(string assetPath)
    {
        string path = GetFileFullPath(GetAssetPath(assetPath));
        return path;
    }
	
	/// <summary>
	/// Gets the file full path.
	/// </summary>
	/// <returns>
	/// The file full path.
	/// </returns>
	/// <param name='absolutePath'>
	/// Absolute path.
	/// </param>
	public static string GetFileFullPathNoProtocol(string absolutePath)
	{
		string path=Application.persistentDataPath+"/"+absolutePath;
		currPersistentExist=File.Exists(path);
		if(!currPersistentExist)
			path=Application.streamingAssetsPath+"/"+absolutePath;
		
		return path;
	}
	
	public static string GetDirectoryFullPathNoProtocol(string absolutePath)
	{
		string path=Application.persistentDataPath+"/"+absolutePath;
		currPersistentExist=Directory.Exists(path);
		if(!currPersistentExist)
			path=Application.streamingAssetsPath+"/"+absolutePath;
		
		return path;
	}
	
	public static string dataPath
	{
		get{
#if UNITY_EDITOR
			return Application.streamingAssetsPath;
#else
			return Application.persistentDataPath;		
#endif
		}
	}
	
	
	public static string GetAssetPath(string name)
	{
		string Platform="";
#if UNITY_IOS
			Platform="iOS";
#elif UNITY_ANDROID
            Platform ="Android";
#elif UNITY_WEBPLAYER
            Platform ="WebPlayer";
#elif UNITY_WP8
			Platform="WP8Player";
#elif UNITY_METRO
            Platform = "MetroPlayer";
#elif UNITY_OSX || UNITY_STANDALONE_OSX
		Platform = "StandaloneOSXIntel";
#else
        Platform = "StandaloneWindows";
#endif
            string path = Path.Combine(Platform, name);  //System.String.Format("{0}/{1}", Platform, name);
        return path;
	}


    public static  string GetPlatformFolderForAssetBundles()
	{
#if UNITY_IOS
			return "iOS";
#elif UNITY_ANDROID
        return "Android";
#elif UNITY_WEBPLAYER
            return "WebPlayer";
#elif UNITY_WP8
			return "WP8Player";
#elif UNITY_METRO
            return "MetroPlayer";
#elif UNITY_OSX 
		return "OSX";
#elif UNITY_STANDALONE_OSX
		return  "StandaloneOSXIntel";
#else
            return "Windows";
#endif

	}
	
	public static bool currPersistentExist=false;

	public static void Collect()
	{
		System.GC.Collect();
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    static public void Execute(IList<System.Action> list)
    {
        if (list != null)
        {
            for (int i = 0; i < list.Count; )
            {
                System.Action del = list[i];

                if (del != null)
                {
                    del();

                    if (i >= list.Count) break;
                    if (list[i] != del) continue;
                }
                ++i;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    static public void Execute(BetterList<System.Action> list)
    {
        if (list != null)
        {
            for (int i = 0; i < list.size; )
            {
                System.Action del = list[i];
                if (del != null)
                {
                    del();

                    if (i >= list.size) break;
                    if (list[i] != del) continue;
                }
                ++i;
            }
        }
    }
}

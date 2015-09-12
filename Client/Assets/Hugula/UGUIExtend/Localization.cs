//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Localization manager is able to parse localization information from text assets.
/// Using it is simple: text = Localization.Get(key), or just add a UILocalize script to your labels.
/// You can switch the language by using Localization.language = "French", for example.
/// This will attempt to load the file called "French.txt" in the Resources folder,
/// or a column "French" from the Localization.csv file in the Resources folder.
/// If going down the TXT language file route, it's expected that the file is full of key = value pairs, like so:
/// 
/// LABEL1 = Hello
/// LABEL2 = Music
/// Info = Localization Example
/// 
/// In the case of the CSV file, the first column should be the "KEY". Other columns
/// should be your localized text values, such as "French" for the first row:
/// 
/// KEY,English,French
/// LABEL1,Hello,Bonjour
/// LABEL2,Music,Musique
/// Info,"Localization Example","Par exemple la localisation"
/// </summary>
[SLua.CustomLuaClass]
public static class Localization
{
	/// <summary>
	/// Whether the localization dictionary has been loaded.
	/// </summary>
 
	static public bool localizationHasBeenSet = false;

	// Loaded languages, if any
	static string[] mLanguages = null;

	// Key = Value dictionary (mulit language)
	static Dictionary<string, string[]> mOldDictionary = new Dictionary<string, string[]>();

    // Key = Values dictionary (single languages)
	static Dictionary<string, string> mDictionary = new Dictionary<string, string>();

	// Index of the selected language within the multi-language dictionary
	static int mLanguageIndex = -1;

	// Currently selected language
	static string mLanguage;

	/// <summary>
	/// Localization dictionary. Dictionary key is the localization key. Dictionary value is the list of localized values (columns in the CSV file).
	/// Be very careful editing this via code, and be sure to set the "KEY" to the list of languages.
	/// </summary>

    static public Dictionary<string, string> dictionary
    {
        get
        {

            if (!localizationHasBeenSet)
            {
#if UNITY_EDITOR
               LoadDictionaryInEditor(Application.systemLanguage.ToString());
#endif
            }
            return mDictionary;
        }
        set
        {
            localizationHasBeenSet = (value != null);
            //mDictionary = value;
        }
    }

	/// <summary>
	/// List of loaded languages. Available if a single Localization.csv file was used.
	/// </summary>

	static public string[] knownLanguages
	{
		get
		{
            //if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", "English"));
			return mLanguages;
		}
	}

	/// <summary>
	/// Name of the currently active language.
	/// </summary>

	static public string language
	{
		get
		{
			if (string.IsNullOrEmpty(mLanguage))
			{
				string[] lan = knownLanguages;
				mLanguage = PlayerPrefs.GetString("Language", lan != null ? lan[0] : "English");
				LoadAndSelect(mLanguage);
			}
			return mLanguage;
		}
		set
		{
			if (mLanguage != value)
			{
				mLanguage = value;
				LoadAndSelect(value);
			}
		}
	}

	/// <summary>
	/// Load the specified localization dictionary.
	/// </summary>

	static bool LoadDictionary (string value)
	{
        LHighway loader = LHighway.instance;
        string fileName = Common.LANGUAGE_FLODER + "/" + value.ToLower() + "." + Common.LANGUAGE_SUFFIX;
        //fileName = CUtils.GetAssetFullPath(fileName);
        //string exsi = CUtils.GetFileFullPathNoProtocol(fileName);
        string url = "";
        //if (!System.IO.File.Exists(exsi))
        //{
        //    url = CUtils.GetFileFullPath(Common.LANGUAGE_FLODER + "/english." + Common.LANGUAGE_SUFFIX);
        //    mLanguage = "English";
        //}
        //else
        {
            url = CUtils.GetAssetFullPath(fileName);
        }

        CRequest req = new CRequest(url, "", "TextAsset");
        req.OnComplete += delegate(CRequest req1)
        {
            //WWW www =((WWW)req1.data);
            TextAsset main = req1.data as TextAsset; //www.assetBundle.mainAsset as TextAsset;
            byte[] txt = main.bytes;
#if UNITY_EDITOR
            Debug.Log(mLanguage + " is loaded "+txt.Length);
#endif
            if (txt != null) Load(txt);
            SelectLanguage(mLanguage);
            req1.assetBundle.Unload(true);
            //www.Dispose();
            //www = null;
            localizationHasBeenSet = true;
        };

        req.OnEnd += delegate(CRequest req1)
        {
            url = CUtils.GetFileFullPath(Common.LANGUAGE_FLODER + "/english." + Common.LANGUAGE_SUFFIX);
            mLanguage = "English";
            req1.url = url;
            loader.LoadReq(req1);
        };
        loader.LoadReq(req);
        return false;
	}

#if UNITY_EDITOR
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static void LoadDictionaryInEditor(string value)
    {
        string fileName =  "Lan/" + value.ToLower() + ".csv" ;
      //  string exsi = CUtils.GetFileFullPathNoProtocol(fileName);
        string url = "";
        //if (!System.IO.File.Exists(exsi))
        //{
        //    url = "Assets/Lan/english.csv";//CUtils.GetFileFullPath();
        //    mLanguage = "English";
        //}
        //else
        {
            url = "Assets/" + fileName; // CUtils.GetFileFullPath();
        }

        Debug.Log(url);
        Object m = UnityEditor.AssetDatabase.LoadAssetAtPath(url,typeof(TextAsset));
        
        TextAsset main =m  as TextAsset;
        if (main)
        {
            byte[] txt = main.bytes;
            mLanguage = value;
           // Debug.Log(mLanguage + "   Editor is loaded " + txt.Length);
            //Debug.Log(System.Text.Encoding.UTF8.GetString(txt));
            if (txt != null) Load(txt);
            SelectLanguage(mLanguage);
        }
        localizationHasBeenSet = true;
    }
#endif

 	/// <summary>
	/// Load the specified language.
	/// </summary>

	static bool LoadAndSelect (string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
            LoadDictionary(value);
            return true;
		}
		return false;
	}

    static public void Load(byte[] asset)
    {
        mDictionary.Clear();
        ByteReader reader = new ByteReader(asset);
        mDictionary = reader.ReadDictionary();
    }

	/// <summary>
	/// Select the specified language from the previously loaded CSV file.
	/// </summary>

	static bool SelectLanguage (string language)
	{
        if (mDictionary.Count == 0) return false;

        mLanguage = language;
        PlayerPrefs.SetString("Language", mLanguage);
        //UIRoot.Broadcast("OnLocalize");

		return true;
	}

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	static public string Get (string key)
	{
		// Ensure we have a language to work with
        string val;
#if UNITY_IPHONE || UNITY_ANDROID
        //key + " Mobile"
        if (mDictionary.TryGetValue(key, out val)) return val;
#endif

#if UNITY_EDITOR
        if (mDictionary.TryGetValue(key, out val)) return val;
        Debug.LogWarning("Localization key not found: '" + key + "'");
        return key;
#else
		return (mDictionary.TryGetValue(key, out val)) ? val : key;
#endif

    }

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	[System.Obsolete("Use Localization.Get instead")]
	static public string Localize (string key) { return Get(key); }

	/// <summary>
	/// Returns whether the specified key is present in the localization dictionary.
	/// </summary>

	static public bool Exists (string key)
	{
		return mDictionary.ContainsKey(key);
	}
}

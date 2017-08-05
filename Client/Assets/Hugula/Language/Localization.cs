//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------
using System.Collections.Generic;
using System.Collections;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;

namespace Hugula
{

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
        [SLua.DoNotToLuaAttribute]
        static public Dictionary<string, string> dictionary
        {
            get
            {

                if (!localizationHasBeenSet)
                {
#if UNITY_EDITOR
                    LoadDictionaryInEditor(PlayerPrefs.GetString("Language", Application.systemLanguage.ToString()));
#endif
                }
                return mDictionary;
            }
            set
            {
                localizationHasBeenSet = (value != null);
                // mDictionary = value;
            }
        }

        // /// <summary>
        // /// List of loaded languages. Available if a single Localization.csv file was used.
        // /// </summary>

        // static public string[] knownLanguages {
        //     get {
        //         if (!localizationHasBeenSet) LoadDictionary (PlayerPrefs.GetString ("Language",SystemLanguage.English.ToString()));
        //         return mLanguages;
        //     }
        // }

        /// <summary>
        /// Name of the currently active language.
        /// </summary>

        static public string language
        {
            get
            {
                if (string.IsNullOrEmpty(mLanguage))
                {
                    mLanguage = PlayerPrefs.GetString("Language", SystemLanguage.English.ToString());
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
        /// set  currently language.
        /// </summary>
        static public void SetLanguage(SystemLanguage lan)
        {
            language = lan.ToString();
        }

        /// <summary>
        /// Load the specified localization dictionary.
        /// </summary>
        static bool LoadDictionary(string value)
        {
            if (value.Equals(SystemLanguage.ChineseSimplified.ToString()))
                value = SystemLanguage.Chinese.ToString();

            string assetName = Common.LANGUAGE_PREFIX + value.ToLower();
            string abName = CUtils.GetRightFileName(assetName + Common.CHECK_ASSETBUNDLE_SUFFIX);

            CRequest req = CRequest.Get();
            req.relativeUrl = abName;
            req.assetName = assetName;
            req.assetType = typeof(BytesAsset);
            req.async = false;
            var uri = new UriGroup();
            uri.Add(CUtils.GetRealPersistentDataPath(),true);
            uri.Add(CUtils.GetRealStreamingAssetsPath());
            req.uris = uri;

            req.OnComplete += delegate (CRequest req1)
            {
                BytesAsset main = req1.data as BytesAsset; //www.assetBundle.mainAsset as TextAsset;
                byte[] txt = main.bytes;
#if UNITY_EDITOR
                Debug.Log(mLanguage + " is loaded " + txt.Length+" "+Time.frameCount);
#endif
                if (txt != null) Load(txt);
                SelectLanguage(mLanguage);
                CacheManager.Unload(req1.keyHashCode);
                localizationHasBeenSet = true;
            };

            req.OnEnd += delegate (CRequest req1)
            {
                if (!value.ToLower().Equals(SystemLanguage.English.ToString().ToLower()))
                    language = SystemLanguage.English.ToString();
            };

            ResourcesLoader.LoadAsset(req);
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
            string url = "Assets/Config/Lan/" + Common.LANGUAGE_PREFIX + value.ToLower() + ".csv";
            Debug.Log(url);
            Object m = UnityEditor.AssetDatabase.LoadAssetAtPath(url, typeof(BytesAsset));

            BytesAsset main = m as BytesAsset;
            if (main)
            {
                byte[] txt = main.bytes;
                // mLanguage = value;
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

        static bool LoadAndSelect(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
#if UNITY_EDITOR
                if (ManifestManager.SimulateAssetBundleInEditor)
                    LoadDictionaryInEditor(value);
                else
                    LoadDictionary(value);
#else
                LoadDictionary (value);
#endif
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

        static bool SelectLanguage(string language)
        {
            if (mDictionary.Count == 0) return false;

            mLanguage = language;
            PlayerPrefs.SetString("Language", mLanguage);
            return true;
        }

        /// <summary>
        /// Localize the specified value.
        /// </summary>

        static public string Get(string key)
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
            return (mDictionary.TryGetValue (key, out val)) ? val : key;
#endif

        }

        /// <summary>
        /// Returns whether the specified key is present in the localization dictionary.
        /// </summary>

        static public bool Exists(string key)
        {
            return mDictionary.ContainsKey(key);
        }
    }

    public class WaitForLanguageHasBeenSet:IEnumerator
    {
        private float timeOut=0.1f;
        private System.DateTime begin;
        public WaitForLanguageHasBeenSet()
        {
            begin = System.DateTime.Now;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            var dt = System.DateTime.Now - begin;
            if (dt.TotalMinutes>=timeOut) return false;
            return !Localization.localizationHasBeenSet;
        }

        public void Reset()
        {
           
        }

    }
}
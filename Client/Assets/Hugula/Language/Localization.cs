//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------
using System.Collections.Generic;
using System.Collections;
using Hugula;
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

    public static class Localization
    {
        /// <summary>
        /// Whether the localization dictionary has been loaded.
        /// </summary>

        static public bool localizationHasBeenSet = false;

        // Loaded languages, if any
        static string[] mLanguages = null;

        public const string KEY_LAN = "m_cs_Language";

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
                    LoadDictionary(PlayerPrefs.GetString(KEY_LAN, Application.systemLanguage.ToString()));
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
        //         if (!localizationHasBeenSet) LoadDictionary (PlayerPrefs.GetString (KEY_LAN,SystemLanguage.English.ToString()));
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
                    mLanguage = PlayerPrefs.GetString(KEY_LAN, SystemLanguage.English.ToString());
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
            // language = lan.ToString();
            if ((int)lan <= 0)
                PlayerPrefs.DeleteKey(KEY_LAN);
            else
                SelectLanguage(lan.ToString());
        }

        /// <summary>
        /// Load the specified localization dictionary.
        /// </summary>
        static void LoadDictionary(string value)
        {
            if (value.Equals(SystemLanguage.ChineseSimplified.ToString()))
                value = SystemLanguage.Chinese.ToString();

            string assetName = Common.LANGUAGE_PREFIX + value.ToLower();

            var main = Resources.Load<TextAsset>(assetName);
            if (main != null)
            {
                byte[] txt = main.bytes;
#if UNITY_EDITOR
                Debug.Log(mLanguage + " is loaded " + txt.Length + " " + Time.frameCount);
#endif
                if (txt != null) Load(txt);
                localizationHasBeenSet = true;
            }
            else
            {
                if (!value.ToLower().Equals(SystemLanguage.English.ToString().ToLower()))
                    language = SystemLanguage.English.ToString();
            }
        }

        /// <summary>
        /// Load the specified language.
        /// </summary>

        static bool LoadAndSelect(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                LoadDictionary (value);
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
#if !HUGULA_RELEASE
            Debug.Log("SelectLanguage;" + language);
#endif
            PlayerPrefs.SetString(KEY_LAN, language);
            return true;
        }

        /// <summary>
        /// Localize the specified value.
        /// </summary>

        static public string Get(string key)
        {
            // Ensure we have a language to work with
            string val;

#if UNITY_EDITOR
            if (mDictionary.TryGetValue(key, out val)) return val;
            Debug.LogWarning("Localization key not found: '" + key + "'");
            return key;
#else
            return (mDictionary.TryGetValue (key, out val)) ? val : key;
#endif

        }

        static public string GetFormat(string key, params object[] args)
        {
            string val = Get(key);
            val = string.Format(val, args);
            return val;
        }

        /// <summary>
        /// Returns whether the specified key is present in the localization dictionary.
        /// </summary>

        static public bool Exists(string key)
        {
            return mDictionary.ContainsKey(key);
        }
    }

    public class WaitForLanguageHasBeenSet : IEnumerator
    {
        private float timeOut = 0.1f;
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
            if (dt.TotalMinutes >= timeOut) return false;
            return !Localization.localizationHasBeenSet;
        }

        public void Reset()
        {

        }

    }
}
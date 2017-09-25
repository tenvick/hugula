using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Hugula.Editor
{

    public static class HugulaPlayerPrefs
    {
        private const string TXT_PATH = "Assets/Hugula/Config/playerprefs.txt";
        private static Dictionary<string, string> m_kvData;// = new Dictionary<string,string>();
        private const string version = "0.1";
        private const string ver_key = "HugulaPlayerPrefs_version";
        private static void Initialize()
        {
            if (m_kvData == null)
            {
                m_kvData = new Dictionary<string, string>();
                // m_kvData[ver_key] = version;
                // version
                if (File.Exists(TXT_PATH))
                {
                    using (StreamReader file = new StreamReader(TXT_PATH.Replace("//", "/")))
                    {
                        string item;
                        string[] sp;
                        string key, val;
                        while ((item = file.ReadLine()) != null)
                        {
                            sp = item.Split(':');
                            if (sp.Length >= 2)
                            {
                                key = sp[0].Trim();
                                val = sp[1].Trim();
                                m_kvData[key] = val;
                            }
                        }
                    }
                }
            }
        }

        private static void SaveData()
        {
            // if (!File.Exists(TXT_PATH)) File.Create(TXT_PATH);
            using (StreamWriter file = new StreamWriter(TXT_PATH.Replace("//", "/"),false))
            {
                foreach (var kv in m_kvData)
                {
                    Debug.Log(string.Format("{0}:{1}",kv.Key,kv.Value));
                    file.WriteLine(string.Format("{0}:{1}", kv.Key, kv.Value));
                }
            }
        }

        public static string GetString(string key, string defaultvalue = "")
        {
            Initialize();
            string outVal;
            if (m_kvData.TryGetValue(key, out outVal))
                return outVal;
            else
                return defaultvalue;
        }

        public static void DeleteKey(string key)
        {
            Initialize();
            m_kvData.Remove(key);
            SaveData();
        }

        public static void SetString(string key, string value)
        {
            Initialize();
            m_kvData[key] = value;
            SaveData();
        }

    }

}

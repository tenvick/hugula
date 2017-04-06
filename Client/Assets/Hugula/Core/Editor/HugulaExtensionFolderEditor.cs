using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using UnityEngine;


namespace Hugula.Editor {

    /// <summary>
    /// 
    /// </summary>
    public class HugulaExtensionFolderEditor {

        public static string SettingPath {
            get {
                string re = Path.Combine (EditorCommon.ConfigPath, EditorCommon.ExtensionFolder);
                return re;
            }
        } //"Assets/Hugula/Config/SettingHugula.txt";

        private static ExtensionFolder _instance = null;

        public static ExtensionFolder instance {
            get {
                if (_instance == null) {
                    _instance = new ExtensionFolder ();
                    LoadDatabase(_instance);
                }
                return _instance;
            }

        }

        public static void AddExtendsPath (string path) {
            if (!instance.AssetLabels.Contains (path)) {
                instance.AssetLabels.Add (path);
                SaveSettingData (instance);
            }
        }

        public static void RemoveExtendsPath (string path) {
            int index = instance.AssetLabels.IndexOf (path);
            if (index >= 0) {
                instance.AssetLabels.RemoveAt (index);
                SaveSettingData (instance);
            }
        }

        
        public static void AddExtendsFile (string name) {
            if (!instance.ExtensionFiles.Contains (name)) {
                instance.ExtensionFiles.Add (name);
                SaveSettingData (instance);
            }
        }

        public static void RemoveExtendsFile (string name) {
            int index = instance.ExtensionFiles.IndexOf (name);
            if (index >= 0) {
                instance.ExtensionFiles.RemoveAt (index);
                SaveSettingData (instance);
            }
        }

        public static bool ContainsExtendsPath (string path) {
            int index = instance.AssetLabels.IndexOf (path);
            return index >= 0;
        }

        public static string GetLabelsByPath (string abPath) {
            string folder = null;
            var allLabels = instance.AssetLabels;

            foreach (var labelPath in allLabels) {
                if (abPath.StartsWith (labelPath + "/")) {
                    folder = CUtils.GetAssetName (labelPath).ToLower ();
                }
            }
            return folder;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private static void SaveSettingData (ExtensionFolder _instance) {
            if (SettingPath != null)
            {
                Debug.Log("Saving database " + SettingPath);
                StreamWriter file = new StreamWriter(SettingPath.Replace ("//", "/"));
                file.WriteLine(string.Format("version: {0}", _instance.version));
                file.WriteLine("assetLabels:");
                writeStringList(file, _instance.AssetLabels);
                file.WriteLine("extensionFiles:");
                writeStringList(file, _instance.ExtensionFiles);
                file.Close();
            }
        }

        private static void LoadDatabase (ExtensionFolder _instance) {
            List<string> list = new List<string> ();
            if (!File.Exists (SettingPath)) {
                FileHelper.CheckCreateFilePathDirectory (SettingPath);
                File.Create (SettingPath);
            }

            using (StreamReader file = new StreamReader (SettingPath.Replace ("//", "/"))) {
                string str;
                while ((str = file.ReadLine ()) != null) {
                    char[] separator = new char[] { ':' };
                    string[] strArray = str.Split (separator);
                    string str2 = strArray[0].Trim ();
                    string s = strArray[1].Trim ();
                    if (str2 == "version") {
                        _instance.version = int.Parse (s);
                    } else {
                        if (str2 == "assetLabels") {
                            _instance.AssetLabels = readStringList (file);
                            continue;
                        }
                        if (str2 == "extensionFiles") {
                            _instance.ExtensionFiles = readStringList (file);
                            continue;
                        }
                    }
                }
            }

        }
        private static List<string> readStringList (StreamReader file) {
            List<string> list = new List<string> ();
            while (file.Peek () == 0x2d) {
                string item = file.ReadLine ().Remove (0, 1).Trim ();
                list.Add (item);
            }
            return list;
        }

        private static void writeStringList(StreamWriter file, List<string> list)
        {
            foreach (string str in list)
            {
                file.WriteLine("- " + str);
            }
        }
    }

    /// <summary>
    /// 本地设置
    /// </summary>
    public class ExtensionFolder {
        /// <summary>
        /// 文件夹列表
        /// </summary>
        public List<string> AssetLabels = new List<string> ();

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<string> ExtensionFiles = new List<string> ();

        public int version = 0;

    }
}
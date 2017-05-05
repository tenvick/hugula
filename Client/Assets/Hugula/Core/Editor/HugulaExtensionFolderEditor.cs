using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using UnityEngine;
using UnityEditor;

namespace Hugula.Editor
{

    /// <summary>
    /// 
    /// </summary>
    public class HugulaExtensionFolderEditor
    {

        public static string SettingPath
        {
            get
            {
                string re = Path.Combine(EditorCommon.ConfigPath, EditorCommon.ExtensionFolder);
                return re;
            }
        } //"Assets/Hugula/Config/SettingHugula.txt";

        private static ExtensionFolder _instance = null;

        public static ExtensionFolder instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExtensionFolder();
                    LoadDatabase(_instance);
                }
                return _instance;
            }

            set
            {
                _instance = null;
            }

        }

        public static void AddExtendsPath(string path)
        {
            if (!instance.AssetLabels.Contains(path))
            {
                instance.AssetLabels.Add(path);
                Debug.LogFormat(" add extends path ({0}) success",path);
                SaveSettingData(instance);
            }
        }

        public static void RemoveExtendsPath(string path)
        {
            int index = instance.AssetLabels.IndexOf(path);
            if (index >= 0)
            {
                instance.AssetLabels.RemoveAt(index);
                Debug.LogFormat(" remove extends path ({0}) success",path);
                SaveSettingData(instance);
            }
        }


        public static void AddExtendsFile(string name)
        {
            if (!instance.ExtensionFiles.Contains(name))
            {
                instance.ExtensionFiles.Add(name);
                Debug.LogFormat(" add extension file ({0}) success",name);
                SaveSettingData(instance);
            }
        }

        public static void RemoveExtendsFile(string name)
        {
            int index = instance.ExtensionFiles.IndexOf(name);
            if (index >= 0)
            {
                instance.ExtensionFiles.RemoveAt(index);
                Debug.LogFormat(" remove extension file ({0}) success",name);
                SaveSettingData(instance);
            }
        }

        public static void AddZipFile(List<string> names)
        {
            CheckDeleteZipFile(names);
            if (names.Count > 0)
            {
                instance.ZipFiles.Add(names);
                SaveSettingData(instance);
            }

        }

        public static void RemoveZipFile(List<string> names)
        {
            if (CheckDeleteZipFile(names))
                SaveSettingData(instance);
        }

        public static bool CheckDeleteZipFile(List<string> names)
        {
            var zfs = instance.ZipFiles;
            int dindex;
            bool needSave = false;
            List<string> item;
            string n = "";
            for (int j = 0; j < names.Count;)
            {
                n = names[j];
                if (instance.ExtensionFiles.Contains(n))
                {
                    Debug.LogWarningFormat(" {0} has added to Extension Files", n);
                    names.RemoveAt(j);
                    continue;
                }
                else
                {
                    j++;
                }

                for (int i = 0; i < zfs.Count;)
                {
                    item = zfs[i];
                    dindex = item.IndexOf(n);
                    if (dindex >= 0)
                    {
                        item.RemoveAt(dindex);
                        needSave = true;
                    }

                    if (item.Count == 0)
                    {
                        zfs.RemoveAt(i);
                        needSave = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return needSave;
        }

        public static bool ContainsZipFile(string name)
        {
            var zfs = instance.ZipFiles;
            foreach (var li in zfs)
            {
                if (li.Contains(name))
                    return true;
            }
            return false;
        }

        public static bool ContainsExtendsPath(string path)
        {
            var allLabels = instance.AssetLabels;

            foreach (var labelPath in allLabels)
            {
                if (path.Equals(labelPath) || path.StartsWith(labelPath + "/"))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetLabelsByPath(string abPath)
        {
            string folder = null;
            var allLabels = instance.AssetLabels;

            foreach (var labelPath in allLabels)
            {
                if (abPath.StartsWith(labelPath + "/"))
                {
                    folder = CUtils.GetAssetName(labelPath).ToLower();
                }
            }
            return folder;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private static void SaveSettingData(ExtensionFolder _instance)
        {
            if (SettingPath != null)
            {
                Debug.Log("Saving database " + SettingPath);
                StreamWriter file = new StreamWriter(SettingPath.Replace("//", "/"));
                file.WriteLine(string.Format("version: {0}", _instance.version));
                file.WriteLine("assetLabels:");
                writeStringList(file, _instance.AssetLabels);
                file.WriteLine("extensionFiles:");
                writeStringList(file, _instance.ExtensionFiles);
                file.WriteLine("zipFiles:");
                writeStringListList(file, _instance.ZipFiles);
                file.Close();
            }
        }

        private static void LoadDatabase(ExtensionFolder _instance)
        {
            List<string> list = new List<string>();
            if (!File.Exists(SettingPath))
            {
                FileHelper.CheckCreateFilePathDirectory(SettingPath);
                File.Create(SettingPath);
            }

            using (StreamReader file = new StreamReader(SettingPath.Replace("//", "/")))
            {
                string str;
                while ((str = file.ReadLine()) != null)
                {
                    char[] separator = new char[] { ':' };
                    string[] strArray = str.Split(separator);
                    string str2 = strArray[0].Trim();
                    string s = strArray[1].Trim();
                    if (str2 == "version")
                    {
                        _instance.version = int.Parse(s);
                    }
                    else
                    {
                        if (str2 == "assetLabels")
                        {
                            _instance.AssetLabels = readStringList(file);
                            continue;
                        }
                        if (str2 == "extensionFiles")
                        {
                            _instance.ExtensionFiles = readStringList(file);
                            continue;
                        }
                        if (str2 == "zipFiles")
                        {
                            _instance.ZipFiles = readStringListList(file);
                            continue;
                        }
                    }
                }
            }

        }
        private static List<string> readStringList(StreamReader file)
        {
            List<string> list = new List<string>();
            while (file.Peek() == 0x2d)
            {
                string item = file.ReadLine().Remove(0, 1).Trim();
                list.Add(item);
            }
            return list;
        }

        private static List<List<string>> readStringListList(StreamReader file)
        {
            List<List<string>> list = new List<List<string>>();
            List<string> childrenList;
            while (file.Peek() == 0x2d)
            {
                string item = file.ReadLine().Remove(0, 1).Trim();
                childrenList = new List<string>();
                var strs = item.Split(',');
                foreach (var s in strs)
                    childrenList.Add(s);
                list.Add(childrenList);
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

        private static void writeStringListList(StreamWriter file, List<List<string>> list)
        {
            string str = "";
            string sp = "";
            foreach (var strList in list)
            {
                str = "";
                sp = "";
                foreach (var s in strList)
                {
                    str += sp + s;
                    sp = ",";
                }
                file.WriteLine("- " + str);
            }
        }

        [MenuItem("Hugula/Refresh Extension Editor Setting")]
        public static void Open()
        {
            _instance = null;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(SettingPath, typeof(TextAsset));
        }


    }

    /// <summary>
    /// 本地设置
    /// </summary>
    public class ExtensionFolder
    {
        /// <summary>
        /// 外部加载文件夹列表
        /// </summary>
        public List<string> AssetLabels = new List<string>();

        /// <summary>
        /// 外部加载文件列表
        /// </summary>
        public List<string> ExtensionFiles = new List<string>();

        /// <summary>
        ///  zip 文件列表
        /// </summary>
        public List<List<string>> ZipFiles = new List<List<string>>();

        public int version = 1;

    }
}
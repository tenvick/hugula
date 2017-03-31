using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Hugula.Utils;


namespace Hugula.Editor
{

/// <summary>
/// 
/// </summary>
public class HugulaExtensionFolderEditor
{

    public static string SettingPath 
    {
        get{
            string re = Path.Combine(EditorCommon.ConfigPath,EditorCommon.ExtensionFolder);
            return re;
        }
    }  //"Assets/Hugula/Config/SettingHugula.txt";

    private static ExtensionFolder _instance = null;

    private static ExtensionFolder instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ExtensionFolder();
                List<string> list = new List<string>();
                if (!File.Exists(SettingPath))
                {  
                    FileHelper.CheckCreateFilePathDirectory(SettingPath);
                    File.Create(SettingPath);
                }
                using (StreamReader file = new StreamReader(SettingPath.Replace("//", "/")))
                {
                    string item;
                    while ((item = file.ReadLine())!=null)
                    {
                        list.Add(item.Trim());
                    }
                }
                Debug.Log("HugulaSetting.instance" + list.Count);
                _instance.AssetLabels = list;
            }
            return _instance;
        }

    }

    public static void AddExtendsPath(string path)
    {
        if (!instance.AssetLabels.Contains(path))
        {
            instance.AssetLabels.Add(path);
            SaveSettingData();
        }
    }

    public static void RemoveExtendsPath(string path)
    {
        int index = instance.AssetLabels.IndexOf(path);
        if (index>=0)
        {
            instance.AssetLabels.RemoveAt(index);
            SaveSettingData();
        }
    }

    public static bool ContainsExtendsPath(string path)
    {
        int index = instance.AssetLabels.IndexOf(path);
        return index >= 0;
    }

    public static string GetLabelsByPath(string abPath)
    {
        string folder = null;
        var allLabels = instance.AssetLabels;

		foreach (var labelPath in allLabels)
        {
            if (abPath.StartsWith(labelPath+"/"))
            {
                folder = CUtils.GetAssetName(labelPath).ToLower();
            }
        }
        return folder;
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    private static void SaveSettingData()
    {
        List<string> list = instance.AssetLabels;
        using (StreamWriter file = new StreamWriter(SettingPath.Replace("//", "/"),false))
        {
            foreach (var s in list)
            {
                Debug.Log(s);
                file.WriteLine(s);
            }

        }
    }
    //private static List<string> readStringList(StreamReader file)
    //{
    //    List<string> list = new List<string>();
    //    while (file.Peek() == 0x2d)
    //    {
    //        string item = file.ReadLine().Remove(0, 1).Trim();
    //        list.Add(item);
    //    }
    //    return list;
    //}
}


/// <summary>
/// 本地设置
/// </summary>
public class ExtensionFolder
{
    /// <summary>
    /// 列表
    /// </summary>
    public List<string> AssetLabels = new List<string>();

}}
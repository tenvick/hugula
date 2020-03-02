using System.Collections;
using System.IO;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Hugula.Editor
{
    public class HugulaEditorSetting
    {
        //设置assetbundle name忽略的后缀
        public string[] abNameIgnoreSuffix;

        public string[] luaIgnorePattern;

        //指定平台压缩zip
        public ZipPlatform zipPlatform = ZipPlatform.None;

        public bool IsIgnoreLua(string luaPath)
        {
            if(luaIgnorePattern==null || luaIgnorePattern.Length==0) return false;
            
            foreach(var p in luaIgnorePattern)
            {
                if(Regex.IsMatch(luaPath,p))
                {
                    Debug.LogWarningFormat("Ignore Lua {0} is match pattern {1}",luaPath,p);
                    return true;
                }
            }

            return false;
        }

        //获取替换后的ab名字
        public string GetAssetBundleNameByReplaceIgnore(string abName)
        {
            if (abNameIgnoreSuffix != null)
            {
                foreach (var s in abNameIgnoreSuffix)
                {
                    if (abName.EndsWith(s))
                    {
                        abName = abName.Substring(0, abName.Length - s.Length);
                        break;
                    }
                }
            }
            return abName;
        }

        public static string SettingPath
        {
            get
            {
                string re = Path.Combine(EditorCommon.ConfigPath, EditorCommon.SettingFile);
                return re;
            }
        }
        private static HugulaEditorSetting _instance = null;

        public static HugulaEditorSetting instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HugulaEditorSetting();
                    if (!File.Exists(SettingPath))
                    {
                        FileHelper.CheckCreateFilePathDirectory(SettingPath);
                        File.Create(SettingPath);
                    }
                    Debug.Log("Loading database " + SettingPath);
                    using (StreamReader file = new StreamReader(SettingPath.Replace("//", "/")))
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

                                if (key == "abNameIgnoreSuffix")
                                {
                                    _instance.abNameIgnoreSuffix = val.Split(',');
                                }
                                else if(key == "luaIgnorePattern")
                                {
                                    _instance.luaIgnorePattern = val.Split(',');
                                }
                                else if (key == "zipPlatform")
                                {
                                    string[] vals = val.Split(',');
                                    ZipPlatform zp = ZipPlatform.None;
                                    foreach (var v in vals)
                                    {
                                        var zpv = (ZipPlatform)System.Enum.Parse(typeof(ZipPlatform), v, true);
                                        if (zp == ZipPlatform.None)
                                            zp = zpv;
                                        else
                                            zp = zp | zpv;

                                    }

                                    _instance.zipPlatform = zp;
                                }
                            }
                        }
                    }

                }
                return _instance;
            }
        }

        [MenuItem("Hugula/Refresh EditorSetting")]
        public static void Open()
        {
            var a = instance;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(SettingPath, typeof(TextAsset));
            _instance = null;
        }

    }


}
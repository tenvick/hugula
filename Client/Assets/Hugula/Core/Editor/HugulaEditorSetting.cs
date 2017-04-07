using System.Collections;
using System.IO;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;

namespace Hugula.Editor {
    public class HugulaEditorSetting {

        

        //设置assetbundle name忽略的后缀
        public string[] abNameIgnoreSuffix;

        //备份资源方式
        /// 0  /res ver.txt
        /// 1  /v{d}/res /v{d} md5.u
        /// 2 both 
        public CopyResType backupResType = CopyResType.OnlyNewest;

        //获取替换后的ab名字
        public string GetAssetBundleNameByReplaceIgnore (string abName) {
            if (abNameIgnoreSuffix != null) {
                foreach (var s in abNameIgnoreSuffix) {
                    if(abName.EndsWith(s))
                    {
                        abName = abName.Substring(0,abName.Length-s.Length);
                        break;
                    }
                }
            }
            return abName;
        }

        public static string SettingPath {
            get {
                string re = Path.Combine (EditorCommon.ConfigPath, EditorCommon.SettingFile);
                return re;
            }
        }
        private static HugulaEditorSetting _instance = null;

        public static HugulaEditorSetting instance {
            get {
                if (_instance == null) {
                    _instance = new HugulaEditorSetting ();
                    if (!File.Exists (SettingPath)) {
                        FileHelper.CheckCreateFilePathDirectory (SettingPath);
                        File.Create (SettingPath);
                    }
                    Debug.Log ("Loading database " + SettingPath);
                    using (StreamReader file = new StreamReader (SettingPath.Replace ("//", "/"))) {
                        string item;
                        string[] sp;
                        string key, val;
                        while ((item = file.ReadLine ()) != null) {
                            sp = item.Split (':');
                            if (sp.Length >= 2) {
                                key = sp[0].Trim ();
                                val = sp[1].Trim ();

                                if (key == "abNameIgnoreSuffix") {
                                    _instance.abNameIgnoreSuffix = val.Split (',');
                                }else if(key == "backupResType")
                                {
                                    CopyResType re = (CopyResType)System.Enum.Parse(typeof(CopyResType),val);
                                    _instance.backupResType = re;
                                }
                            }
                        }
                    }

                }
                return _instance;
            }
        }

        [MenuItem ("Hugula/Refresh EditorSetting")]
        public static void Open () {
            var a = instance;
            Selection.activeObject = AssetDatabase.LoadAssetAtPath (SettingPath, typeof (TextAsset));
            _instance = null;
        }

    }

    /// <summary>
    /// 更新包资源导出结构类型
    /// </summary>
    public enum CopyResType
    {
        /// <summary>
        /// 仅保持最新包res和最新版本文件 ver.txt
        /// </summary>
        OnlyNewest,
        /// <summary>
        /// v{d}/res 放资源 和版本文件
        /// </summary>
        VerResFolder,
        /// <summary>
        /// Res放所有资源 v{d} 只放版本文件
        /// </summary>
        ResVerFolder
    }
}
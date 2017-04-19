using System.Collections;
using System.IO;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;

namespace Hugula.Editor {
    public class HugulaEditorSetting {

        

        //设置assetbundle name忽略的后缀
        public string[] abNameIgnoreSuffix;

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
                                }
                                // else if(key == "backupResType")
                                // {
                                //     CopyResType re = (CopyResType)System.Enum.Parse(typeof(CopyResType),val);
                                //     _instance.backupResType = re;
                                // }
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

   
}
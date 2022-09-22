using System.Collections.Generic;
using System.IO;
using Hugula;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
namespace HugulaEditor
{
    //编辑器工具类
    public static class EditorUtils
    {

        public const string streamingPath = "Assets/StreamingAssets"; //打包assetbundle输出目录。
        public const string TmpPath = "Tmp/";
        // public const string HugulaFolder = "HugulaFolder";

        //获取url的assetbundle name 
        static public string GetAssetBundleName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            int idxEnd = url.IndexOf('?');
            int idxBegin = url.IndexOf(CUtils.platformFloder); // 

            if (idxBegin == -1)
            {
                idxBegin = url.LastIndexOf("/") + 1;
                int idx1 = url.LastIndexOf("\\") + 1;
                if (idx1 > idxBegin) idxBegin = idx1;
            }
            else
                idxBegin = idxBegin + CUtils.platformFloder.Length + 1;

            if (idxBegin >= url.Length) idxBegin = 0;
            if (idxEnd == -1) idxEnd = url.Length;

            string re = url.Substring(idxBegin, idxEnd - idxBegin);
            return re;
        }


        static public string GetGameObjectPathInScene(Transform obj, string path)
        {
            if (obj.parent == null)
            {
                if (string.IsNullOrEmpty(path)) path = obj.name;
                return path; //  +/+path
            }
            else
            {
                string re = string.Format("{0}/{1}", obj.parent.name, obj.name);
                return GetGameObjectPathInScene(obj.transform.parent, re);
            }
        }

        public static string GetAssetTmpPath()
        {
            return Path.Combine(Application.dataPath, TmpPath);
        }

        public static string GetProjectTempPath()
        {
            return Path.Combine(Application.dataPath.Replace("Assets", ""), "Tmp/hugula");
        }

        public static string GetOutPutPath()
        {
            return Path.Combine(streamingPath, CUtils.GetAssetPath(""));
        }

        /// <summary>
        /// 向tmp目录写入数据。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="context"></param>
        public static void WriteToTmpFile(string fileName, string context)
        {
            string tmpPath = GetAssetTmpPath();
            EditorUtils.CheckDirectory(tmpPath);
            string outPath = Path.Combine(tmpPath, fileName);
            using (StreamWriter sr = new StreamWriter(outPath, false))
            {
                sr.Write(context);
            }
            Debug.Log("write to path=" + outPath);
        }

        public static string GetAssetPath(string filePath)
        {
            string path = filePath.Replace(Application.dataPath + "/", "");
            return path; //Path.Combine("Assets",path);
        }

        public static string GetFileStreamingOutAssetsPath()
        {
            string dircAssert = Path.Combine(Application.streamingAssetsPath, CUtils.GetAssetPath(""));
            return dircAssert;
        }

        public static void DeleteStreamingOutPath()
        {
            DirectoryDelete(Application.streamingAssetsPath);
        }

        /// <summary>
        /// 检查输出目标
        /// </summary>
        public static void CheckstreamingAssetsPath()
        {
            string dircAssert = EditorUtils.GetFileStreamingOutAssetsPath();
            if (!Directory.Exists(dircAssert))
            {
                Directory.CreateDirectory(dircAssert);
            }

            Debug.Log(string.Format("current BuildTarget ={0},path = {1} ", CUtils.platform, dircAssert));
        }

        public static void CheckDirectory(string fullPath)
        {
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public static void DirectoryDelete(string path)
        {
            try
            {

                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists) di.Delete(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void DeleteFilesException(string directoryPath, string exception, bool isMatch = true)
        {
            string[] fileMys = Directory.GetFiles(directoryPath);
            foreach (string f in fileMys)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(f, exception) == isMatch)
                {
                    File.Delete(f);
                    Debug.Log($"del: {f}");
                }
            }

            string[] dires = Directory.GetDirectories(directoryPath);
            foreach (string dirp in dires)
            {
                DeleteFilesException(dirp, exception, isMatch);
            }
        }

        public static List<string> getAllChildFiles(string path, string suffix = "lua", List<string> files = null, bool isMatch = true)
        {
            if (files == null) files = new List<string>();
            if (!string.IsNullOrEmpty(path)) addFiles(path, suffix, files, isMatch);
            string[] dires = Directory.GetDirectories(path);
            foreach (string dirp in dires)
            {
                //            Debug.Log(dirp);
                getAllChildFiles(dirp, suffix, files, isMatch);
            }
            return files;
        }

        public static void addFiles(string direPath, string suffix, List<string> files, bool isMatch = true)
        {
            string[] fileMys = Directory.GetFiles(direPath);
            foreach (string f in fileMys)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(f, suffix) == isMatch)
                {
                    files.Add(f);
                }
            }
        }

        static string RES_NUMBER_KEY
        {
            get
            {
                
                var key = $"{CUtils.platform}_{Application.identifier}_{CodeVersion.CODE_VERSION}_res_number";
                // Debug.Log(key);
                return key;
            }
        }
        //获取当前资源版本号
        public static int GetResNumber()
        {
            return EditorPrefs.GetInt(RES_NUMBER_KEY, 0);
        }

        public static void SetResNumber(int resNum)
        {
            if (resNum < GetResNumber()) 
            {
                Debug.LogError($"设置的resNum:{resNum} < 原始值:{EditorPrefs.GetInt(RES_NUMBER_KEY)} 已经自动修正+1");
                resNum = GetResNumber()+1;
            }
            EditorPrefs.SetInt(RES_NUMBER_KEY, resNum);
        }

        public static void ResNumberClear()
        {
            // EditorPrefs.SetInt(RES_NUMBER_KEY, resNum);
            EditorPrefs.DeleteKey(RES_NUMBER_KEY);
        }

        //select objects contains folder file
        public static UnityEngine.Object[] SelectObjects(params System.Type[] args) //System.Type[] filter = null)
        {
            if (args.Length == 0) args = new System.Type[] { typeof(UnityEngine.Object) };

            System.Func<Object, bool> checkType = (Object s) =>
            {
                foreach (System.Type t in args)
                {
                    //  Debug.LogFormat("{0}={1},is={2}",s.GetType(),t,s.GetType().IsSubclassOf(t));
                    if (s.GetType().IsSubclassOf(t) || s.GetType().Equals(t)) return true;
                }
                return false;
            };

            List<UnityEngine.Object> re = new List<UnityEngine.Object>();
            Object[] selection = UnityEditor.Selection.objects;
            string path = string.Empty;
            foreach (Object s in selection)
            {
                if (s is DefaultAsset && (path = AssetDatabase.GetAssetPath(s)) != null && Directory.Exists(path))
                {
                    var import = AssetImporter.GetAtPath(path);
                    if (!string.IsNullOrEmpty(import.assetBundleName))
                    {
                        re.Add(s);
                    }
                    else
                    {
                        var allchildren = getAllChildFiles(path, @"\.meta$|\.manifest$|\.DS_Store$|\.u$", null, false);
                        foreach (var f in allchildren)
                        {
                            var cpath = EditorUtils.GetAssetPath(f);
                            // Debug.Log(cpath);
                            var obj = AssetDatabase.LoadAssetAtPath(cpath, typeof(Object));
                            if (checkType(obj))
                                re.Add(obj);
                        }
                    }

                }
                else if (checkType(s))
                {
                    re.Add(s);
                }
            }

            return re.ToArray();
        }

        public static void SetFieldValue(object obj, string name, object value)
        {
            System.Type tp = obj.GetType();
            var field = tp.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance);
            field.SetValue(obj, value);
        }

        public static object InvokeStatic(System.Type type ,string name,params object[] args)
        {
            var staticMethod = type.GetMethod(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static);
            return staticMethod.Invoke(null,args);
        }

        [MenuItem("Assets/Hugula/打开开始场景  %g")]
        static public void OpenBeginSence()
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath("Assets/Scenes/s_begin.unity", typeof(UnityEngine.Object)));
        }
    }

}
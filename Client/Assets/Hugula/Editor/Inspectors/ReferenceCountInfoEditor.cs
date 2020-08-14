using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text.RegularExpressions;

using Hugula;
using Hugula.Loader;
using Hugula.Utils;
namespace HugulaEditor
{
    public static class CacheManagerExtension
    {
        // public Dictionary<string, CacheData> GetCacheData(this Camera ,.aa)
    }

    [CustomEditor(typeof(ReferenceCountInfo))]
    public class ReferenceCountInfoEditor : Editor
    {
        string input;
        string str = "";
        bool showList = true;
        int findCount = -1;
        int allCount = 0;

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            EditorGUILayout.Space();
            ReferenceCountInfo temp = target as ReferenceCountInfo;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("key ", GUILayout.Width(30));
            input = EditorGUILayout.TextField(input, GUILayout.Width(150));
            if (GUILayout.Button("check", GUILayout.Width(50)))
            {
                // string mdstring = input;
                // mdstring = CUtils.GetRightFileName(input);
                findCount = FindeCountByKey(input);
            }

            EditorGUILayout.LabelField("" + findCount, GUILayout.Width(20));

            EditorGUILayout.EndHorizontal();
            // EditorGUILayout.LabelField("filter file name ", GUILayout.Width(150));
            // temp.info = EditorGUILayout.TextField(temp.info, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(60) });
            // EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button("sure", GUILayout.Width(50)))
            // {
            //     string a = temp.info;
            //     string mare = string.Empty;

            //     if (!string.IsNullOrEmpty(a))
            //     {
            //         string[] mach = a.Split(',');
            //         string md5 = string.Empty;
            //         int hash = 0;
            //         foreach (var m in mach)
            //         {
            //             md5 = CUtils.GetRightFileName(m);
            //             hash = LuaHelper.StringToHash(md5);
            //             mare += string.Format("{0}_{1};", md5, hash);
            //         }
            //     }
            //     HugulaDebug.filterNames = mare;
            //     Debug.Log(a);
            //     Debug.Log(mare);
            // }
            // EditorGUILayout.EndHorizontal();

            allCount = CacheManager.EditorCacheData.Keys.Count;
            EditorGUILayout.LabelField("show reference count list " + allCount, GUILayout.Width(180));
            showList = EditorGUILayout.Toggle(showList, GUILayout.Width(20));
            if (showList)
            {
                foreach (var k in CacheManager.EditorCacheData)
                {
                    string assetBundleName = k.Value.assetBundleName;

                    if (!string.IsNullOrEmpty(input))
                    {
                        if (Regex.Matches(assetBundleName, input).Count > 0)
                        {
                            str = string.Format(" {0} = {1}", k.Value.assetBundleName, k.Value.count);
                            GUILayout.Label(str, GUILayout.Width(500));
                        }
                    }
                    else
                    {
                        str = string.Format(" {0} = {1}", k.Value.assetBundleName, k.Value.count);
                        GUILayout.Label(str, GUILayout.Width(500));
                    }
                }
            }

            EditorGUILayout.Space();
        }

        public int FindeCountByKey(string key)
        {
            var d = CacheManager.GetCache(key);
            if (d != null)
                return d.count;

            return -1;
        }
    }
}

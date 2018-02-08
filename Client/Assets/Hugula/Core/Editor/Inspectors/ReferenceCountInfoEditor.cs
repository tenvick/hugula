using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using Hugula.Pool;
using Hugula.Loader;
using Hugula.Utils;

[CustomEditor(typeof(ReferenceCountInfo))]
public class ReferenceCountInfoEditor : Editor
{
    string input;
    bool isMd5;
    string str = "";
    bool showList = true;
    int findCount = -1;
    int allCount = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        ReferenceCountInfo temp = target as ReferenceCountInfo;

        EditorGUILayout.BeginHorizontal();

        if (isMd5)
            EditorGUILayout.LabelField("md5 key ", GUILayout.Width(30));
        else
            EditorGUILayout.LabelField("key ", GUILayout.Width(30));

        input = EditorGUILayout.TextField(input, GUILayout.Width(150));
        isMd5 = EditorGUILayout.Toggle(isMd5, GUILayout.Width(20));//(isMd5, GUILayout.Width (120));
        if (isMd5)
        {
            if (GUILayout.Button("check", GUILayout.Width(50)))
            {
                findCount = FindeCountByKey(input);
            }
        }
        else
        {
            if (GUILayout.Button("check", GUILayout.Width(50)))
            {
                string mdstring = input;
// #if HUGULA_COMMON_ASSETBUNDLE
					
// #else
                mdstring = CUtils.GetRightFileName(input);
// #endif
                findCount = FindeCountByKey(mdstring);
            }
        }

        EditorGUILayout.LabelField("" + findCount, GUILayout.Width(20));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("filter file name ", GUILayout.Width(150));

        temp.info = EditorGUILayout.TextField(temp.info, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(60) });
		EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("sure", GUILayout.Width(50)))
        {
            string a = temp.info;
            string mare = string.Empty;

            if (!string.IsNullOrEmpty(a))
            {
                string[] mach = a.Split(',');
                string md5 = string.Empty;
                int hash = 0;
                foreach (var m in mach)
                {
                    md5 = CUtils.GetRightFileName(m);
                    hash = LuaHelper.StringToHash(md5);
                    mare += string.Format("{0}_{1};",md5,hash);
                }
            }
            HugulaDebug.filterNames = mare;
            Debug.Log(a);
			Debug.Log(mare);
        }
        EditorGUILayout.EndHorizontal();

        allCount = CacheManager.caches.Keys.Count;
        EditorGUILayout.LabelField("show reference count list " + allCount, GUILayout.Width(180));
        showList = EditorGUILayout.Toggle(showList, GUILayout.Width(20));
        if (showList)
        {
            foreach (var k in CacheManager.caches)
            {
                str = string.Format(" {0} = {1}", k.Value.assetBundleKey, k.Value.count);
                GUILayout.Label(str, GUILayout.Width(500));
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

using UnityEngine;
using System.Collections;
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        ReferenceCountInfo temp = target as ReferenceCountInfo;

		EditorGUILayout.BeginHorizontal ();

		if (isMd5)
			EditorGUILayout.LabelField("md5 key ", GUILayout.Width(30));
		else
			EditorGUILayout.LabelField("key ", GUILayout.Width(30));

		input = EditorGUILayout.TextField (input, GUILayout.Width (150));
		isMd5 = EditorGUILayout.Toggle (isMd5, GUILayout.Width (20));//(isMd5, GUILayout.Width (120));
		if (isMd5) {
			if (GUILayout.Button ("check", GUILayout.Width (50))) {
				findCount = FindeCountByKey (input);
			}
		} else {
			if (GUILayout.Button ("check", GUILayout.Width (50))) {
				string mdstring = input;
				#if BUILD_COMMON_ASSETBUNDLE 
					
				#else
				mdstring  = CUtils.GetRightFileName(input);
				#endif
				findCount = FindeCountByKey (mdstring);
			}
		}

		EditorGUILayout.LabelField(""+findCount,GUILayout.Width (20));

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.LabelField("show reference count list ", GUILayout.Width(150));
		showList = EditorGUILayout.Toggle (showList, GUILayout.Width (20));
		if (showList) {
			foreach (var k in CacheManager.caches) {
				str = string.Format (" {0} = {1}", k.Value.assetBundleKey, k.Value.count);
				GUILayout.Label (str, GUILayout.Width (500));
			}
		}

        EditorGUILayout.Space();
    }

	public int FindeCountByKey(string key)
	{
		var d = CacheManager.GetCache (key);
		if (d != null)
			return d.count;

		return -1;
	}
}

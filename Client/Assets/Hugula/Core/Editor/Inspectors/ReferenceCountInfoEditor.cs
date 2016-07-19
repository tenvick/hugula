using UnityEngine;
using System.Collections;
using UnityEditor;

using Hugula.Pool;
using Hugula.Loader;

[CustomEditor(typeof(ReferenceCountInfo))]
public class ReferenceCountInfoEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Mono List", GUILayout.Width(200));
        ReferenceCountInfo temp = target as ReferenceCountInfo;
        //Undo.RecordObject(target, "F");

        if (GUILayout.Button("show ReferenceCount"))
        {
        }

        string str = "";

        foreach (var k in CacheManager.caches)
        {
            str = string.Format(" {0} = {1}",k.Value.assetBundleName,k.Value.count);
            GUILayout.Label(str, GUILayout.Width(500));
        }

        EditorGUILayout.Space();
        //EditorGUILayout.EndHorizontal();
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PrefabPoolAssist))]
public class PrefabPoolEditor : Editor
{

    public override void OnInspectorGUI()
    {
        foreach (var tar in targets)
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            PrefabPoolAssist temp = tar as PrefabPoolAssist;

            if (string.IsNullOrEmpty(temp.cacheKey) && GUILayout.Button("Fill cacheKey"))
            {
                temp.cacheKey = temp.name;
            }

            EditorGUILayout.Space();
        }
    }
}

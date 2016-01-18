using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Atlas))]
public class AtlasInspector : Editor {

    public string InputSpriteName = "";
    public Object selectTarget = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        Atlas temp = target as Atlas;

        EditorGUILayout.LabelField("Sprite Check", GUILayout.Width(200));
        EditorGUILayout.BeginHorizontal();

        InputSpriteName = GUILayout.TextField(InputSpriteName, GUILayout.Width(100));
        selectTarget = EditorGUILayout.ObjectField(selectTarget, typeof(UnityEngine.Object), true);

        if (selectTarget != null)
            InputSpriteName = selectTarget.name;

        if (InputSpriteName != "" && GUILayout.Button("check",GUILayout.Width(120)))
        {
           var spri = temp.GetSpriteByName(InputSpriteName);
           Debug.Log(InputSpriteName);
           Debug.Log(spri);
        }

        EditorGUILayout.EndHorizontal();

    }
}

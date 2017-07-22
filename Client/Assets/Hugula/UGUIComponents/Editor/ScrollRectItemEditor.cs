using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Hugula.UGUIExtend;

[CustomEditor(typeof(ScrollRectItem))]
public class ScrollRectItemEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mono List", GUILayout.Width(200));
        ScrollRectItem temp = target as ScrollRectItem;
        Undo.RecordObject(target, "F");
        if (temp.monos != null)
        {
            for (int i = 0; i < temp.monos.Length; i++)
            {
                List<Type> allowTypes = new List<Type>() { typeof(GameObject) };
                int selectIndex = 0;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));
                SetReferName(temp, i);
                UnityEngine.Object obj = GetMonos(temp, i);
                if (obj != null)
                {
                    Type currentType = obj.GetType();
                    GameObject go = null;
                    if (currentType == typeof(GameObject))
                        go = (GameObject)obj;
                    else if (currentType.IsSubclassOf(typeof(Component)))
                        go = ((Component)obj).gameObject;
                    Component[] cs = null;

                    if (go != null)
                    {
                        cs = go.GetComponents<Component>();

                        for (int j = 0; j < cs.Length; j++)
                        {
                            allowTypes.Add(cs[j].GetType());
                            if (obj == cs[j])
                                selectIndex = j + 1;
                        }
                        selectIndex = EditorGUILayout.Popup(selectIndex, ConvertTypeArrayToStringArray(allowTypes));
                        if (selectIndex == 0)
                            AddMonos(temp, i, go); //temp.monos[i] = go;
                        else
                            AddMonos(temp, i, cs[selectIndex - 1]);  //temp.monos[i] = cs[selectIndex - 1];
                    }
                    if (temp.names[i] == "")
                        temp.names[i] = obj.name;
                }

                AddMonos(temp, i, EditorGUILayout.ObjectField(GetMonos(temp, i), typeof(UnityEngine.Object), true));

                var objfind = GetMonos(temp, i);
                if (temp.names.Count > i && temp.names[i] != "" && objfind != null && temp.names[i] != objfind.name)
                {
                    if (GUILayout.Button("Auto", GUILayout.Width(50)))
                    {
                        temp.names[i] = "";
                    }
                }
                if (GUILayout.Button("Del", GUILayout.Width(40)))
                {
                    temp.names.RemoveAt(i);
                    RemoveAtMonos(temp, i);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("Add Item"))
        {
            temp.names.Add("");
            AddMonos(temp,-1,null);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        EditorUtility.SetDirty(target);
    }

    public void AddMonos(ScrollRectItem refer, int i, UnityEngine.Object obj)
    {
        List<UnityEngine.Object> monos = null;

        if (refer.monos != null)
            monos = new List<UnityEngine.Object>(refer.monos);
        else
            monos = new List<UnityEngine.Object>();

    
        //monos.Add(obj);
        if (i < 0)
        {
            monos.Add(obj);
        }
        else
        {
            while (monos.Count <= i)
                monos.Add(null);
            monos[i] = obj;
        }
        refer.monos = monos.ToArray();
    }

    public void RemoveAtMonos(ScrollRectItem refer, int index)
    {
        List<UnityEngine.Object> monos = new List<UnityEngine.Object>(refer.monos);
        monos.RemoveAt(index);
        refer.monos = monos.ToArray();
    }

    public UnityEngine.Object GetMonos(ScrollRectItem refer, int index)
    {
        if (index >= 0 && index < refer.monos.Length)
            return refer.monos[index];
        return null;
    }

    public void SetReferName(ScrollRectItem refer, int i)
    {
        while (refer.names.Count <= i)
            refer.names.Add(null);
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.xMin = 30;
        rect.width = 70;
        refer.names[i] = EditorGUI.TextField(rect, refer.names[i]);
        GUILayout.Space(70);
    }
public static  string[] ConvertTypeArrayToStringArray(List<Type> tps)
    {
        List<string> temp = new List<string>();
        for (int i = 0; i < tps.Count; i++)
        {
            string s = tps[i].ToString();
            int index = s.LastIndexOf('.');
            if (index != -1)
            {
                index += 1;
                s = s.Substring(index);
            }
            temp.Add(s);
        }
        return temp.ToArray();
    }
}

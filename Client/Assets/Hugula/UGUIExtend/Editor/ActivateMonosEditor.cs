// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
//using Entry = PropertyReferenceDrawer.Entry;

//[CustomPropertyDrawer(typeof(MonoBehaviour))]
public class ActivateMonosDrawer : PropertyDrawer
{
    const int lineHeight = 16;

    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        Undo.RecordObject(prop.serializedObject.targetObject, "Delegate Selection");

        SerializedProperty targetProp = prop.FindPropertyRelative("gameObject");
        //Debug.Log(prop.arraySize);
        //Debug.Log(prop.);

        //SerializedProperty methodProp = prop.FindPropertyRelative("mMethodName");
        //Debug.Log(prop.serializedObject.name);
        //Debug.Log(targetProp.objectReferenceValue);
        //MonoBehaviour target = targetProp.objectReferenceValue as MonoBehaviour;
        ////string methodName = methodProp.stringValue;

        //EditorGUI.indentLevel = prop.depth;
        //EditorGUI.LabelField(rect, label);
       // base.OnGUI(rect, prop, label);
    }
}

public class ActivateMonosEditor  {


}

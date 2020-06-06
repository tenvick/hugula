using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HugulaEditor.Databinding;
using Hugula.UIComponents;

namespace NijinEditor.UIComponents
{

    [CustomEditor(typeof(GroupIntSelector))]
    public class GroupIntSelectorEditor : Editor
    {
        void OnEnable()
        {

        }

        // public override void OnInspectorGUI()
        // {
        //     base.OnInspectorGUI();
        //     var tmp = target as GroupIntSelector;
        //     // BindalbeObjectUtilty.BindableObjectField(tmp, 0);
        // }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor
{
    // [CustomEditor (typeof (BindableContainer), true)]
    public static class BindableExpressionEditor
    {

        static GUIStyle BindingPropertiesStyle = new GUIStyle();
        // Dictionary<Binding, Dictionary<int,bool>> toggleDic = new Dictionary<Binding, Dictionary<int,bool>> ();
        static Dictionary<int, bool> toggleDic = new Dictionary<int, bool>();
        static string[] bindMode = new string[] { "oneway", "twoway" };
        static StringBuilder sb = new StringBuilder();

        public static void Expression(Binding binding, string targetName, int i)
        {
            bool toggle = false;
            int key = binding.GetHashCode() + i;
            toggleDic.TryGetValue(key, out toggle);

            GUILayout.Label(string.Format(".{0}=>", binding.propertyName), GUILayout.MinWidth(60));
            GUILayout.BeginHorizontal();
            if (toggle = GUILayout.Toggle(toggle, "", GUILayout.MaxWidth(20)))
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical();
                binding.path = BindalbeEditorUtilty.DrawEditorLabl("path", binding.path, GUILayout.MinWidth(100));
                binding.format = BindalbeEditorUtilty.DrawEditorLabl("format", binding.format, GUILayout.MinWidth(100));
                binding.mode = BindalbeEditorUtilty.DrawEume("mode", binding.mode, GUILayout.MinWidth(100));//DrawPopup ("mode", binding.mode, GUILayout.MinWidth (100));
                binding.converter = BindalbeEditorUtilty.DrawEditorLabl("converter", binding.converter, GUILayout.MinWidth(100));
                binding.source = BindalbeEditorUtilty.DrawPopUpComponents("source", binding.source, GUILayout.MinWidth(100));
                GUILayout.EndHorizontal();
            }
            else
            {
                sb.Clear();
                sb.Append("{");
                if (!string.IsNullOrEmpty(binding.path))
                    sb.AppendFormat("path={0},", binding.path);
                if (!string.IsNullOrEmpty(binding.format))
                    sb.AppendFormat("format={0},", binding.format);
                if (binding.mode != BindingMode.OneWay)
                    sb.AppendFormat("mode={0},", binding.mode);
                if (!string.IsNullOrEmpty(binding.converter))
                    sb.AppendFormat("converter={0},", binding.converter);
                if (binding.source)
                    sb.AppendFormat("source={0},", binding.source);
                if (sb.Length > 2) sb.Length = sb.Length - 1;
                sb.Append("}");
                EditorGUILayout.LabelField(sb.ToString(), GUILayout.MaxWidth(500));
                GUILayout.EndHorizontal();
            }

            toggleDic[key] = toggle;
        }

        public static void DrawLabl(string title, string content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(title));
            GUILayout.Label(content);
            GUILayout.EndHorizontal();
        }

    }

}
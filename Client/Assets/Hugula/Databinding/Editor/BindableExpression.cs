using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor {
    // [CustomEditor (typeof (BindableContainer), true)]
    public static class BindableExpression {

        static GUIStyle BindingPropertiesStyle = new GUIStyle ();
        // Dictionary<Binding, Dictionary<int,bool>> toggleDic = new Dictionary<Binding, Dictionary<int,bool>> ();
        static Dictionary<int, bool> toggleDic = new Dictionary<int, bool> ();
        static string[] bindMode = new string[] { "oneway", "twoway" };
        static StringBuilder sb = new StringBuilder ();

        public static void Expression (Binding binding, string targetName, int i) {
            bool toggle = false;
            int key = binding.GetHashCode () + i;
            toggleDic.TryGetValue (key, out toggle);
            // bindingDic.TryGetValue(i,out toggle);

            GUILayout.Label (string.Format (".{0}=>", binding.propertyName), GUILayout.MinWidth (60));
            GUILayout.BeginHorizontal ();
            if (toggle = GUILayout.Toggle (toggle, "", GUILayout.MaxWidth (20))) {
                GUILayout.EndHorizontal ();
                // GUILayout.BeginVertical ();
                // var expDic = StringToExpression (binding.expression);
                // string path = string.Empty;
                // string format = string.Empty;
                // string mode = string.Empty;
                // string convert = string.Empty;
                // string source = string.Empty;

                // expDic.TryGetValue ("path", out path);
                // path = DrawEditorLabl ("path", path, GUILayout.MinWidth (100));
                // expDic.TryGetValue ("format", out format);
                // format = DrawEditorLabl ("format", format, GUILayout.MinWidth (100));
                // expDic.TryGetValue ("mode", out mode);
                // mode = DrawPopup ("mode", mode, GUILayout.MinWidth (100));
                // expDic.TryGetValue ("convert", out convert);
                // convert = DrawEditorLabl ("convert", convert, GUILayout.MinWidth (100));

                // expDic.TryGetValue ("source", out source);
                // source = DrawEditorLabl ("source", source, GUILayout.MinWidth (100));
                // GUILayout.EndHorizontal ();
                // expDic["path"] = path;
                // binding.path = path;
                // expDic["format"] = format;
                // binding.format = format;
                // expDic["mode"] = mode;
                // binding.mode = mode;
                // expDic["convert"] = convert;
                // binding.converter = convert;
                // expDic["source"] = source;
                // binding.source = source;

                // string expression = ExpressionToString (expDic);

                // Debug.Log (expression);
                // binding.expression = expression;

                GUILayout.BeginVertical ();
                binding.path = DrawEditorLabl ("path", binding.path, GUILayout.MinWidth (100));
                binding.format = DrawEditorLabl ("format", binding.format, GUILayout.MinWidth (100));
                binding.mode = DrawPopup ("mode", binding.mode, GUILayout.MinWidth (100));
                binding.converter = DrawEditorLabl ("converter", binding.converter, GUILayout.MinWidth (100));
                binding.source = DrawEditorLabl ("source", binding.source, GUILayout.MinWidth (100));
                GUILayout.EndHorizontal ();

            } else {
                sb.Length = 0;
                sb.Append ("{");
                if (!string.IsNullOrEmpty (binding.path))
                    sb.AppendFormat ("path={0},", binding.path);
                if (!string.IsNullOrEmpty (binding.format))
                    sb.AppendFormat ("format={0},", binding.format);
                if (!string.IsNullOrEmpty (binding.mode))
                    sb.AppendFormat ("mode={0},", binding.mode);
                if (!string.IsNullOrEmpty (binding.converter))
                    sb.AppendFormat ("converter={0},", binding.converter);
                if (!string.IsNullOrEmpty (binding.source))
                    sb.AppendFormat ("source={0},", binding.source);
                if (sb.Length > 1) sb.Remove (sb.Length - 1, 1);
                sb.Append ("}");
                // string expression = 
                EditorGUILayout.LabelField (sb.ToString (), GUILayout.MaxWidth (500));
                GUILayout.EndHorizontal ();

            }

            toggleDic[key] = toggle;
        }

        // public static string DrawEditorObject (string title, string content) {
        //     GUILayout.BeginHorizontal ();
        //     GUILayout.Label (new GUIContent (title));
        //     content = EditorGUILayout.ObjectField(content,)
        //     GUILayout.EndHorizontal ();
        //     return content;
        // }

        public static void DrawLabl (string title, string content) {
            GUILayout.BeginHorizontal ();
            GUILayout.Label (new GUIContent (title));
            GUILayout.Label (content);
            GUILayout.EndHorizontal ();
        }

        public static string DrawEditorLabl (string title, string content, params GUILayoutOption[] options) {
            GUILayout.BeginHorizontal ();
            GUILayout.Label (new GUIContent (title), GUILayout.Width (60));
            content = GUILayout.TextField (content, options);
            GUILayout.EndHorizontal ();
            if (!string.IsNullOrEmpty (content)) content = content.Replace (",", "").Replace ("=", "");
            return content;
        }

        public static string DrawPopup (string title, string content, params GUILayoutOption[] options) {
            GUILayout.BeginHorizontal ();
            GUILayout.Label (new GUIContent (title));
            int selectIndex = System.Array.IndexOf (bindMode, content);
            if (selectIndex == -1) selectIndex = 0;
            selectIndex = EditorGUILayout.Popup (selectIndex, bindMode, options);
            GUILayout.EndHorizontal ();

            return bindMode[selectIndex];
        }

        // static System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        public static string ExpressionToString (Dictionary<string, string> dicExp) {
            sb.Clear ();
            sb.Append ("{");
            foreach (var kv in dicExp) {
                if (!string.IsNullOrEmpty (kv.Value))
                    if (!(kv.Key.Equals ("mode") && kv.Value.Equals ("oneway")))
                        sb.AppendFormat (@"{0}=""{1}"",", kv.Key, kv.Value);

            }
            if (sb.Length > 1) sb.Remove (sb.Length - 1, 1);
            sb.Append ("}");
            return sb.ToString ();
        }

        static char[] clumChar = new char[] { ',' };
        static char rowChar = '=';
        static string parttern = @"(\w+)=""(.+)"",?";
        static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex (parttern, System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.Singleline);
        public static Dictionary<string, string> StringToExpression (string expr) {
            Dictionary<string, string> dic = new Dictionary<string, string> ();
            if (string.IsNullOrEmpty (expr))
                expr = "{}";

            string content = expr.Substring (1);
            if (content.IndexOf ("}") >= 0)
                content = content.Substring (0, content.Length - 1);

            string[] colum = content.Split (clumChar);
            // string[] row;
            int idx = 0;
            string key, value;
            foreach (var item in colum) {
                idx = item.IndexOf (rowChar);
                if (idx > 1) {
                    key = item.Substring (0, idx);
                    value = item.Substring (idx + 1);
                    dic[key] = value.Replace ("\"", "");
                }
            }

            return dic;
        }
    }

    public static class BindableObjectHelper {

        static Dictionary<int, string> dicPropertyName = new Dictionary<int, string> ();
        public static void BindableObjectField (BindableObject target, int index) {
            if (target == null) return;
            int key = target.GetHashCode ();
            string propertyName;
            dicPropertyName.TryGetValue (key, out propertyName);
            EditorGUILayout.Separator ();
            // EditorGUILayout.LabelField ("B List", GUILayout.Width (200));
            EditorGUILayout.Space ();

            EditorGUILayout.BeginHorizontal ();
            var temp = target as BindableObject;
            GUILayout.Label (target.name, GUILayout.Width (100));
            EditorGUILayout.ObjectField (temp, typeof (BindableObject), GUILayout.MaxWidth (150)); //显示绑定对象
            int selectedIndex = PopupComponentsProperty (temp, propertyName, GUILayout.MaxWidth (150)); //绑定属性
            propertyName = GetSelectedPropertyByIndex (selectedIndex);
            dicPropertyName[key] = propertyName;
            if (GUILayout.Button ("+", GUILayout.MaxWidth (50))) {
                if (selectedIndex == 0) {
                    Debug.LogWarningFormat ("please choose a property to binding");
                    return;
                }
                if (temp.bindings == null) temp.bindings = new List<Binding> ();

                Binding expression = new Binding ();
                expression.propertyName = propertyName;
                temp.bindings.Add (expression);
            }
            Undo.RecordObject (target, "F");
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.Separator ();
            UnityEngine.Object objComponent;
            if (temp.bindings != null) {
                for (int i = 0; i < temp.bindings.Count; i++) {
                    EditorGUILayout.BeginHorizontal ();
                    var binding = temp.bindings[i];
                    BindableExpression.Expression (binding, temp.targetName, i);
                    //         GUILayout.Label ((i + 1).ToString (), GUILayout.Width (20));
                    //         // objComponent = temp.children[i];
                    //         // objComponent = PopupGameObjectComponents (GetbindableObjects (temp, i).target, i); //选择绑定的component type类型
                    //         // if (objComponent != null) AddbindableObjects (temp, i, objComponent); //绑定选中的类型
                    //         // //显示选中的对象
                    //         // AddbindableObjects (temp, i, EditorGUILayout.ObjectField (GetbindableObjects (temp, i).target, typeof (UnityEngine.Object), true, GUILayout.MaxWidth (80)));
                    //         // //选择可绑定属性
                    //         PopupComponentsProperty (temp, i);

                    if (GUILayout.Button ("-", GUILayout.Width (30))) {
                        RemoveAtbindableObjects (temp, i);
                    }

                    EditorGUILayout.EndHorizontal ();
                    //         //设置binding属性
                    //         // SetBindingProperties (temp, i);
                    //         EditorGUILayout.Space ();
                }
            }
            // EditorGUILayout.Space ();
            // EditorGUILayout.BeginHorizontal ();
            // EditorGUILayout.Space ();

        }

        static List<string> allowTypes = new List<string> ();
        static List<Type> allowTypeProperty = new List<Type> ();
        static List<bool> allowIsMethod = new List<bool> ();

        static void RemoveAtbindableObjects (BindableObject target, int index) {
            var binding = target.bindings;
            binding.RemoveAt (index);
        }

        static string GetSelectedPropertyByIndex (int selectIndex) {
            if (allowTypes.Count > selectIndex)
                return allowTypes[selectIndex];
            return null;
        }
        static int PopupComponentsProperty (UnityEngine.Object target, string propertyName, params GUILayoutOption[] options) {
            // var binding = target.bindingExpression[i];
            int selectIndex = 0;

            var obj = target; //temp
            Type t = obj.GetType ();
            allowTypes.Clear ();
            allowTypeProperty.Clear ();
            allowIsMethod.Clear ();
            //  add tips
            allowTypes.Add ("choose property");
            allowTypeProperty.Add (typeof (Nullable));
            // allowIsMethod.Add (false);
            //end tips

            // string Property = binding.propertyName;
            int j = 1;
            var propertes = t.GetMembers (BindingFlags.Public | BindingFlags.Instance);
            foreach (var mi in propertes) {　
                Type parameterType = null;
                string name = null;
                bool isMethod = false;

                if (mi.MemberType == MemberTypes.Property) {
                    var pi = (PropertyInfo) mi;
                    name = pi.Name;
                }

                //没必要绑定方法，可以把方法转换成属性。
                // if (mi.MemberType == MemberTypes.Method && (!mi.Name.StartsWith ("get_") && !mi.Name.StartsWith ("set_"))) {
                //     var parameters = ((MethodInfo) mi).GetParameters ();
                //     if (parameters.Length == 1) {
                //         parameterType = parameters[0].ParameterType;
                //         name = mi.Name;
                //         isMethod = true;
                //     } else if (parameters.Length == 0) {
                //         parameterType = typeof (void);
                //         name = mi.Name;
                //         isMethod = true;
                //     }
                // }

                if (!string.IsNullOrEmpty (name)) {
                    char first = name[0];
                    // if (first >= 'A' && first <= 'Z') {
                    allowTypes.Add (name);
                    allowTypeProperty.Add (parameterType);
                    // allowIsMethod.Add (isMethod);
                    if (name.Equals (propertyName)) {
                        selectIndex = j;
                    }
                    j++;
                    // }
                }
            }

            selectIndex = EditorGUILayout.Popup (selectIndex, allowTypes.ToArray (), options);
            return selectIndex;
            // if (allowTypes.Count > selectIndex) {
            //UnityEngine.UI.Button button; button.onClick UnityEvent
            // binding.propertyName = allowTypes[selectIndex];
            // binding.returnType = BindableContainerEditor.GetLuaType (allowTypeProperty[selectIndex]);
            // binding.isMethod = allowIsMethod[selectIndex];
            // Debug.LogFormat(" propertyName{0} returnType{1},isMethod={2} ",binding.propertyName,binding.returnType,binding.isMethod);
            // }
            // else
            //     Debug.LogWarningFormat (" binding ({0}) property({1}) error" + allowTypes.Count, binding.target, binding.propertyName);
        }

    }
}
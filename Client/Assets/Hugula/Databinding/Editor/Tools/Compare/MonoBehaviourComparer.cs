using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using UnityEditor.IMGUI.Controls;

namespace HugulaEditor.Databinding
{
    public class MonoBehaviourComparer : EditorWindow
    {
        private GameObject _objectA;
        private GameObject _objectB;

        private bool _showDifferences;

        [MenuItem("Window/MonoBehaviour Comparer")]
        private static void OpenWindow()
        {
            GetWindow<MonoBehaviourComparer>("MonoBehaviour Comparer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Compare MonoBehaviours", EditorStyles.boldLabel);

            _objectA = EditorGUILayout.ObjectField("Object A", _objectA, typeof(GameObject), true) as GameObject;
            _objectB = EditorGUILayout.ObjectField("Object B", _objectB, typeof(GameObject), true) as GameObject;

            if (_objectA == null || _objectB == null)
            {
                return;
            }

            if (GUILayout.Button("Compare"))
            {
                CompareMonoBehaviours(_objectA, _objectB);
                _showDifferences = true;
            }

            if (_showDifferences)
            {
                GUILayout.Label("Differences", EditorStyles.boldLabel);

                foreach (var component in _objectA.GetComponents<MonoBehaviour>())
                {
                    var otherComponent = _objectB.GetComponent(component.GetType());

                    if (otherComponent == null)
                    {
                        EditorGUILayout.LabelField(component.GetType().Name, "Object B doesn't have this component");
                    }
                    else
                    {
                        var componentSerializedObject = new SerializedObject(component);
                        var otherComponentSerializedObject = new SerializedObject(otherComponent);
                        var property = componentSerializedObject.GetIterator();
                        var otherProperty = otherComponentSerializedObject.GetIterator();

                        while (property.NextVisible(true) && otherProperty.NextVisible(true))
                        {
                            if (property.propertyType != otherProperty.propertyType)
                            {
                                continue;
                            }

                            if (property.type != otherProperty.type)
                            {
                                continue;
                            }

                            if (property.name == "m_Script" || otherProperty.name == "m_Script")
                            {
                                continue;
                            }

                            if (!SerializedProperty.EqualContents(property, otherProperty))
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.LabelField(property.displayName);
                                EditorGUILayout.PropertyField(property, true);
                                EditorGUILayout.PropertyField(otherProperty, true);
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                }
            }
        }

        private void CompareMonoBehaviours(GameObject objA, GameObject objB)
        {
            Debug.Log($"Comparing MonoBehaviours on {objA.name} and {objB.name}");

            foreach (var component in objA.GetComponents<MonoBehaviour>())
            {
                var otherComponent = objB.GetComponent(component.GetType());

                if (otherComponent == null)
                {
                    Debug.Log($"Object B doesn't have {component.GetType().Name}");
                }
                else
                {
                    var componentSerializedObject = new SerializedObject(component);
                    var otherComponentSerializedObject = new SerializedObject(otherComponent);
                    var property = componentSerializedObject.GetIterator();
                    var otherProperty = otherComponentSerializedObject.GetIterator();

                    while (property.NextVisible(true) && otherProperty.NextVisible(true))
                    {
                        if (property.propertyType != otherProperty.propertyType)
                        {
                            continue;
                        }

                        if (property.type != otherProperty.type)
                        {
                            continue;
                        }

                        if (property.name == "m_Script" || otherProperty.name == "m_Script")
                        {
                            continue;
                        }

                        if (!SerializedProperty.EqualContents(property, otherProperty))
                        {
                            Debug.Log($"{component.GetType().Name}: {property.displayName} differs");

                            var propertyValue = property.type == "PPtr<$GameObject>" ? property.objectReferenceValue?.name : property.stringValue;
                            var otherPropertyValue = otherProperty.type == "PPtr<$GameObject>" ? otherProperty.objectReferenceValue?.name : otherProperty.stringValue;

                            Debug.Log($"{objA.name}.{component.GetType().Name}.{property.displayName}: {propertyValue}");
                            Debug.Log($"{objB.name}.{component.GetType().Name}.{otherProperty.displayName}: {otherPropertyValue}");
                        }
                    }
                }
            }
        }
    }


    // 这个窗口将显示两个 `GameObject` 的选择框，以及一个 "Compare" 按钮。当用户单击 "Compare" 按钮时，将比较两个 `GameObject` 上所有 `MonoBehaviour` 的属性，并将差异以树形列表的形式显示在窗口中。

    // 在比较 `MonoBehaviour` 属性时，我们首先使用 `SerializedObject` 类来访问每个 `MonoBehaviour` 的属性。然后，我们使用 `SerializedProperty` 类来访问每个属性的值，并将其与另一个 `MonoBehaviour` 的相应属性进行比较。如果属性的值不同，则将其显示在树形列表中。

    // 注意，该示例仅比较了 `MonoBehaviour` 的序列化属性，并且假定 `GameObject` 上的所有 `MonoBehaviour` 与其在其他 `GameObject` 上的对应项具有相同的类型。如果需要比较其他类型的属性（例如 `Transform` 或 `Renderer`），则需要相应地调整代码。




}
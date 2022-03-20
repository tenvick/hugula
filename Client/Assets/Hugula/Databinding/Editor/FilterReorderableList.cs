
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
    public class FilterReorderableList : ReorderableList
    {
        public System.Func<SerializedProperty ,string,bool> onFilterFunc;
        public string searchText;
        public FilterReorderableList(IList elements, Type elementType) : base(elements, elementType)
        {

        }

        public FilterReorderableList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
        {

        }
        public FilterReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {

        }
        public FilterReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {

        }



    }

}
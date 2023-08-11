using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

class BindablePropertyDropdown : AdvancedDropdown
{
    private List<PropertyInfo> properties;
    private GenericMenu.MenuFunction2 onAddClick;
    private Object target;
    public BindablePropertyDropdown(AdvancedDropdownState state, List<PropertyInfo> properties, Object target, GenericMenu.MenuFunction2 onAddClick) : base(state)
    {
        this.target = target;
        this.onAddClick = onAddClick;
        this.properties = properties;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        onAddClick.Invoke(new object[]{properties[item.id], target});
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Properties");

        for (int i = 0; i < properties.Count; i++)
        {
            var per = properties[i];
            var item = new AdvancedDropdownItem($"{per.Name} ({per.PropertyType.Name})");
            item.id = i;    
            root.AddChild(item);
        }
        return root;
    }
}

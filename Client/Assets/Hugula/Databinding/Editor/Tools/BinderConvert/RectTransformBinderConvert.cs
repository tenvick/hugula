using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Hugula.Databinding;
using Hugula.Databinding.Binder;

namespace HugulaEditor.Databinding.ConvertLua
{


    public class RectTransformBinderConvert :  BaseBinderConvert
    {
        public override string GetSetSourceCodeByPropertyName(object obj, string target, string propertyName, string value)
        {
            // RectTransformBinder rectTransformBinder = obj as RectTransformBinder;
            // var mMultiInfo = rectTransformBinder.mMultiInfo;
            // if (propertyName == "width")
            // {
            //     return $"{target}:SetSizeWithCurrentAnchors(CS.UnityEngine.RectTransform.Axis.Horizontal, {value}*{mMultiInfo.x});";
            // }
            // else if (propertyName == "height")
            // {
            //     return $"{target}:SetSizeWithCurrentAnchors(CS.UnityEngine.RectTransform.Axis.Vertical, {value} * {mMultiInfo.y});";
            // }
            // else if (propertyName == "mulAnchoredPos")
            // {
            //     return $"{target}.anchoredPosition = {value} * CS.UnityEngine.Vector2({mMultiInfo.x},{mMultiInfo.y});";
            // }
            // else if (propertyName == "anchoredPosition")
            // {
            //     return $"{target}.anchoredPosition = {value};";
            // }
            // else if (propertyName == "anchoredPositionY")
            // {
            //     return $"{target}.anchoredPosition = CS.UnityEngine.Vector2({target}.anchoredPosition.x, {value});";
            // }
            // else if (propertyName == "anchoredPositionX")
            // {
            //     return $"{target}.anchoredPosition = CS.UnityEngine.Vector2({value}, {target}.anchoredPosition.y);";
            // }
            // else if (propertyName == "worldPos")
            // {
            //     return $"{target}.position={value};";
            // }
            // else if (propertyName == "contraActiveSelf")
            // {
            //     return $"{target}.gameObject:SetActive(not {value})";
            // }
            // else if (propertyName == "World2LocalRemoveY")
            // {
            //     return $"binder_lua_helper.RectTransformBinder_World2LocalRemoveY({target},{value}) ";
            // }
            // else if (propertyName == "rotation_v3")
            // {
            //     return $"{target}.transform.localRotation = CS.UnityEngine.Quaternion.Euler({value})";
            // }
            // else if (propertyName == "rotation_z")
            // {
            //     return $"{target}.transform.localRotation = CS.UnityEngine.Quaternion.Euler(CS.UnityEngine.Vector3.forward*{value})";
            // }
            // else if(propertyName == "world_rotation_v3")
            // {
            //     return $"{target}.transform.rotation = CS.UnityEngine.Quaternion.Euler({value})";
            // }
            // else if (propertyName == "look_at")
            // {
            //     return $"{target}.transform.forward={value}";
            // }
            // else if (propertyName == "scaleActive")
            // {
            //     return $"binder_lua_helper.RectTransformBinder_scaleActive({target},{value})";
            // }
            // else if (propertyName == "scale")
            // {
            //     return $"binder_lua_helper.RectTransformBinder_scale({target},{value})";
            // }
            // else if (propertyName == "scaleV3")
            // {
            //     return $"{target}.localScale = {value}";
            // }
            // else if (propertyName == "realtimeWorldPos")
            // {
            //     var targetPath = BindableUtility.GetFullPath((obj as Component).transform);
            //     Debug.LogError($" RectTransformBinder.realtimeWorldPos Func<Vector3> is not support yet. {targetPath}  {target} {value}");
            //     return $@" erro("" RectTransformBinder.realtimeWorldPos Func<Vector3> is not support yet. {targetPath} {target} {value}"")";
            // }
            // else if (propertyName == "realtimeLookat")
            // {
            //     var targetPath = BindableUtility.GetFullPath((obj as Component).transform);
            //     Debug.LogError($" RectTransformBinder.realtimeLookat Func<Vector3> is not support yet.  {targetPath} {target} {value}");
            //     return $@" erro("" RectTransformBinder.realtimeLookat Func<Vector3> is not support yet. {targetPath} {target} {value}"")";
            // }
            // else if (propertyName == "sizeDelta")
            // {
            //     return $"{target}.sizeDelta = {value}";
            // }
            // else if (propertyName == "anchonMin")
            // {
            //     return $"{target}.anchorMin = {value}";
            // }
            // else if (propertyName == "anchonMax")
            // {
            //     return $"{target}.anchonMax = {value}";
            // }
            // else if (propertyName == "siblingindex")
            // {
            //     return $"{target}:SetSiblingIndex({value})";
            // }
            // else
            {
                return  base.GetSetSourceCodeByPropertyName(obj, target, propertyName, value);
            }
        }

    }

}
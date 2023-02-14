using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace PSDUINewImporter
{
    public enum LayoutMode { Undefined = -1, Min = 0, Middle = 1, Max = 2, Stretch = 3 }

    public class PSDRectLayout
    {
        public static LayoutMode GetLayoutModeForAxis(
            Vector2 anchorMin,
            Vector2 anchorMax,
            int axis)
        {
            if (anchorMin[axis] == 0 && anchorMax[axis] == 0)
                return LayoutMode.Min;
            if (anchorMin[axis] == 0.5f && anchorMax[axis] == 0.5f)
                return LayoutMode.Middle;
            if (anchorMin[axis] == 1 && anchorMax[axis] == 1)
                return LayoutMode.Max;
            if (anchorMin[axis] == 0 && anchorMax[axis] == 1)
                return LayoutMode.Stretch;
            return LayoutMode.Undefined;
        }

        private static Vector3[] s_Corners = new Vector3[4];

        static Vector3 GetRectReferenceCorner(RectTransform gui, bool worldSpace)
        {
            if (worldSpace)
            {
                Transform t = gui.transform;
                gui.GetWorldCorners(s_Corners);
                if (t.parent)
                    return t.parent.InverseTransformPoint(s_Corners[0]);
                else
                    return s_Corners[0];
            }
            return (Vector3)gui.rect.min + gui.transform.localPosition;
        }

        public static void SetPivotSmart(RectTransform rect, float value, int axis, bool smart, bool parentSpace)
        {
            Vector3 cornerBefore = GetRectReferenceCorner(rect, !parentSpace);

            Vector2 rectPivot = rect.pivot;
            rectPivot[axis] = value;
            rect.pivot = rectPivot;

            if (smart)
            {
                Vector3 cornerAfter = GetRectReferenceCorner(rect, !parentSpace);
                Vector3 cornerOffset = cornerAfter - cornerBefore;
                rect.anchoredPosition -= (Vector2)cornerOffset;

                Vector3 pos = rect.transform.position;
                pos.z -= cornerOffset.z;
                rect.transform.position = pos;
            }
        }

        static float[] kPivotsForModes = new float[] { 0, 0.5f, 1, 0.5f, 0.5f }; // Only for actual modes, not for Undefined.

        static void SetLayoutModeForAxis(
            Vector2 anchorMin,Transform[] targetObjects,
            int axis, LayoutMode layoutMode,
            bool doPivot, bool doPosition, Vector2[,] defaultValues)
        {

            //var targetObjects = anchorMin.serializedObject.targetObjects;
            for (int i = 0; i < targetObjects.Length; i++)
            {
                RectTransform gui = targetObjects[i] as RectTransform;
                //Undo.RecordObject(gui, "Change Rectangle Anchors");

                if (doPosition)
                {
                    if (defaultValues != null && defaultValues.Length > i)
                    {
                        Vector2 temp;

                        temp = gui.anchorMin;
                        temp[axis] = defaultValues[i, 0][axis];
                        gui.anchorMin = temp;

                        temp = gui.anchorMax;
                        temp[axis] = defaultValues[i, 1][axis];
                        gui.anchorMax = temp;

                        temp = gui.anchoredPosition;
                        temp[axis] = defaultValues[i, 2][axis];
                        gui.anchoredPosition = temp;

                        temp = gui.sizeDelta;
                        temp[axis] = defaultValues[i, 3][axis];
                        gui.sizeDelta = temp;
                    }
                }

                if (doPivot && layoutMode != LayoutMode.Undefined)
                {
                    SetPivotSmart(gui, kPivotsForModes[(int)layoutMode], axis, true, true);
                }

                Vector2 refPosition = Vector2.zero;
                switch (layoutMode)
                {
                    case LayoutMode.Min:
                        SetAnchorSmart(gui, 0, axis, false, true, true);
                        SetAnchorSmart(gui, 0, axis, true, true, true);
                        refPosition = gui.offsetMin;
                        EditorUtility.SetDirty(gui);
                        break;
                    case LayoutMode.Middle:
                        SetAnchorSmart(gui, 0.5f, axis, false, true, true);
                        SetAnchorSmart(gui, 0.5f, axis, true, true, true);
                        refPosition = (gui.offsetMin + gui.offsetMax) * 0.5f;
                        EditorUtility.SetDirty(gui);
                        break;
                    case LayoutMode.Max:
                        SetAnchorSmart(gui, 1, axis, false, true, true);
                        SetAnchorSmart(gui, 1, axis, true, true, true);
                        refPosition = gui.offsetMax;
                        EditorUtility.SetDirty(gui);
                        break;
                    case LayoutMode.Stretch:
                        SetAnchorSmart(gui, 0, axis, false, true, true);
                        SetAnchorSmart(gui, 1, axis, true, true, true);
                        refPosition = (gui.offsetMin + gui.offsetMax) * 0.5f;
                        EditorUtility.SetDirty(gui);
                        break;
                }

                if (doPosition)
                {
                    // Handle position
                    Vector2 rectPosition = gui.anchoredPosition;
                    rectPosition[axis] -= refPosition[axis];
                    gui.anchoredPosition = rectPosition;

                    // Handle sizeDelta
                    if (layoutMode == LayoutMode.Stretch)
                    {
                        Vector2 rectSizeDelta = gui.sizeDelta;
                        rectSizeDelta[axis] = 0;
                        gui.sizeDelta = rectSizeDelta;
                    }
                }
            }
        }


        public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart)
        {
            SetAnchorSmart(rect, value, axis, isMax, smart, false, false, false);
        }

        public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart, bool enforceExactValue)
        {
            SetAnchorSmart(rect, value, axis, isMax, smart, enforceExactValue, false, false);
        }

        public static void SetAnchorSmart(RectTransform rect, float value, int axis, bool isMax, bool smart, bool enforceExactValue, bool enforceMinNoLargerThanMax, bool moveTogether)
        {
            RectTransform parent = null;
            if (rect.transform.parent == null)
            {
                smart = false;
            }
            else
            {
                parent = rect.transform.parent.GetComponent<RectTransform>();
                if (parent == null)
                    smart = false;
            }

            bool clampToParent = true;//!AnchorAllowedOutsideParent(axis, isMax ? 1 : 0);
            if (clampToParent)
                value = Mathf.Clamp01(value);
            if (enforceMinNoLargerThanMax)
            {
                if (isMax)
                    value = Mathf.Max(value, rect.anchorMin[axis]);
                else
                    value = Mathf.Min(value, rect.anchorMax[axis]);
            }

            float offsetSizePixels = 0;
            float offsetPositionPixels = 0;
            if (smart)
            {
                float oldValue = isMax ? rect.anchorMax[axis] : rect.anchorMin[axis];

                offsetSizePixels = (value - oldValue) * parent.rect.size[axis];

                // Ensure offset is in whole pixels.
                // Note: In this particular instance we want to use Mathf.Round (which rounds towards nearest even number)
                // instead of Round from this class which always rounds down.
                // This makes the position of rect more stable when their anchors are changed.
                float roundingDelta = 0;
                if (ShouldDoIntSnapping(rect))
                    roundingDelta = Mathf.Round(offsetSizePixels) - offsetSizePixels;
                offsetSizePixels += roundingDelta;

                if (!enforceExactValue)
                {
                    value += roundingDelta / parent.rect.size[axis];

                    // Snap value to whole percent if close
                    if (Mathf.Abs(Round(value * 1000) - value * 1000) < 0.1f)
                        value = Round(value * 1000) * 0.001f;

                    if (clampToParent)
                        value = Mathf.Clamp01(value);
                    if (enforceMinNoLargerThanMax)
                    {
                        if (isMax)
                            value = Mathf.Max(value, rect.anchorMin[axis]);
                        else
                            value = Mathf.Min(value, rect.anchorMax[axis]);
                    }
                }

                if (moveTogether)
                    offsetPositionPixels = offsetSizePixels;
                else
                    offsetPositionPixels = (isMax ? offsetSizePixels * rect.pivot[axis] : (offsetSizePixels * (1 - rect.pivot[axis])));
            }

            if (isMax)
            {
                Vector2 rectAnchorMax = rect.anchorMax;
                rectAnchorMax[axis] = value;
                rect.anchorMax = rectAnchorMax;

                Vector2 other = rect.anchorMin;
                //if (moveTogether)
                //    other[axis] = s_StartDragAnchorMin[axis] + rectAnchorMax[axis] - s_StartDragAnchorMax[axis];
                rect.anchorMin = other;
            }
            else
            {
                Vector2 rectAnchorMin = rect.anchorMin;
                rectAnchorMin[axis] = value;
                rect.anchorMin = rectAnchorMin;

                Vector2 other = rect.anchorMax;
                //if (moveTogether)
                //    other[axis] = s_StartDragAnchorMax[axis] + rectAnchorMin[axis] - s_StartDragAnchorMin[axis];
                rect.anchorMax = other;
            }

            if (smart)
            {
                Vector2 rectPosition = rect.anchoredPosition;
                rectPosition[axis] -= offsetPositionPixels;
                rect.anchoredPosition = rectPosition;

                if (!moveTogether)
                {
                    Vector2 rectSizeDelta = rect.sizeDelta;
                    rectSizeDelta[axis] += offsetSizePixels * (isMax ? -1 : 1);
                    rect.sizeDelta = rectSizeDelta;
                }
            }
        }

        private static bool ShouldDoIntSnapping(RectTransform rect)
        {
            Canvas canvas = rect.gameObject.GetComponentInParent<Canvas>();
            return (canvas != null && canvas.renderMode != RenderMode.WorldSpace);
        }

        static float Round(float value) { return Mathf.Floor(0.5f + value); }
        static int RoundToInt(float value) { return Mathf.FloorToInt(0.5f + value); }


    }
}
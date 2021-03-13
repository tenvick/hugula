using System;
using UnityEditor;
using UnityEngine;

namespace HugulaEditor.Databinding.Editor
{
    public class ConnectionGUI
    {
        public NodeGUI startNode;
        public NodeGUI endNode;

        private Color lineColor;
        private float lineWidth;
        private Vector3[] curvePoints;

        private static readonly Color SelectedColor = new Color(0.43f, 0.65f, 0.90f, 1.0f);

        public Color UnselectedColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return Color.gray;
                }
                else
                {
                    return Color.white;
                }
            }
        }

        public Vector3 StartPosition
        {
            get
            {
                Vector3 pos = Vector3.zero;

                if (startNode != null)
                {
                    var rect = startNode.nodeRect;
                    pos.x = rect.xMax;
                    pos.y = rect.center.y;
                }

                return pos;
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                Vector3 pos = Vector3.zero;

                if (endNode != null)
                {
                    var rect = endNode.nodeRect;
                    pos.x = rect.xMin;
                    pos.y = rect.center.y;
                }

                return pos;
            }
        }

        public bool UseAltCurve
        {
            get { return endNode.nodeRect.xMin <= startNode.nodeRect.xMax; }
        }

        public ConnectionGUI()
        {
            lineColor = UnselectedColor;
            lineWidth = GraphEditorSettings.LineWidth;
            curvePoints = new Vector3[GraphEditorSettings.CurveDivision * 2];
        }

        public void SetActive()
        {
            lineColor = SelectedColor;
            lineWidth = GraphEditorSettings.SelectedLineWidth;
        }

        public void SetInactive()
        {
            lineColor = UnselectedColor;
            lineWidth = GraphEditorSettings.LineWidth;
        }

        public void Draw()
        {
            if (startNode == null || endNode == null)
            {
                return;
            }

            var start = StartPosition;
            var end = EndPosition;

            Handles.color = lineColor;

            if (UseAltCurve)
            {
                // calculate center point
                float centerY = 0f;
                if (start.y >= end.y)
                {
                    centerY = endNode.nodeRect.yMax + GraphEditorSettings.TreeViewVerticalMargin / 2.0f;
                }
                else
                {
                    centerY = endNode.nodeRect.yMin - GraphEditorSettings.TreeViewVerticalMargin / 2.0f;
                }

                // calculate tangent offset
                var pointDistanceA = Mathf.Abs(start.y - centerY) / 5f;
                pointDistanceA = Mathf.Max(pointDistanceA, GraphEditorSettings.AltTangentLength);
                var offsetA = new Vector3(pointDistanceA, 0f, 0f);

                var pointDistanceB = Mathf.Abs(end.y - centerY) / 3f;
                pointDistanceB = Mathf.Max(pointDistanceB, GraphEditorSettings.AltTangentLength);
                var offsetB = new Vector3(pointDistanceB, 0f, 0f);

                var p1 = new Vector3(start.x, centerY, 0f);
                var p2 = new Vector3(end.x, centerY, 0f);

                // start -> p1
                var points = Handles.MakeBezierPoints(start, p1, start + offsetA, p1 + offsetA, GraphEditorSettings.CurveDivision);
                Array.Copy(points, 0, curvePoints, 0, points.Length);

                // p2 -> end
                points = Handles.MakeBezierPoints(p2, end, p2 - offsetB, end - offsetB, GraphEditorSettings.CurveDivision);
                Array.Copy(points, 0, curvePoints, GraphEditorSettings.CurveDivision, points.Length);

                // draw curve
                Handles.DrawAAPolyLine(lineWidth, curvePoints);
            }
            else
            {
                var pointDistance = (end.x - start.x) / 3.0f;
                pointDistance = Mathf.Max(pointDistance, GraphEditorSettings.TangentLength);

                var startTan = new Vector3(start.x + pointDistance, start.y, 0f);
                var endTan = new Vector3(end.x - pointDistance, end.y, 0f);

                // draw curve
                var points = Handles.MakeBezierPoints(start, end, startTan, endTan, GraphEditorSettings.CurveDivision);
                Handles.DrawAAPolyLine(lineWidth, points);
            }
        }
    }
}

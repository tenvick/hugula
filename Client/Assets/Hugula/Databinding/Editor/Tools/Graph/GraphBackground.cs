using UnityEditor;
using UnityEngine;

namespace HugulaEditor.Databinding.Editor
{
    public class GraphBackground
    {
        private const float NodeGridSize = 12.0f;
        private const float MajorGridSize = 120.0f;
        private static readonly Color GridMinorColorDark = new Color(0f, 0f, 0f, 0.18f);
        private static readonly Color GridMajorColorDark = new Color(0f, 0f, 0f, 0.28f);
        private static readonly Color GridMinorColorLight = new Color(0f, 0f, 0f, 0.10f);
        private static readonly Color GridMajorColorLight = new Color(0f, 0f, 0f, 0.15f);

        private Rect graphRegion;
        private Vector2 scrollPosition;
        private Material lineMaterial;

        private static Color gridMinorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                    return GridMinorColorDark;
                else
                    return GridMinorColorLight;
            }
        }

        private static Color gridMajorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                    return GridMajorColorDark;
                else
                    return GridMajorColorLight;
            }
        }

        private Material CreateLineMaterial()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            Material m = new Material(shader);
            m.hideFlags = HideFlags.HideAndDontSave;
            return m;
        }

        private void DrawGrid()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (lineMaterial == null)
            {
                lineMaterial = CreateLineMaterial();
            }

            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.Begin(GL.LINES);

            DrawGridLines(NodeGridSize, gridMinorColor);
            DrawGridLines(MajorGridSize, gridMajorColor);

            GL.End();
            GL.PopMatrix();
        }

        private void DrawGridLines(float gridSize, Color gridColor)
        {
            GL.Color(gridColor);
            for (float x = graphRegion.xMin - (graphRegion.xMin % gridSize) - scrollPosition.x; x < graphRegion.xMax; x += gridSize)
            {
                if (x < graphRegion.xMin)
                {
                    continue;
                }
                DrawLine(new Vector2(x, graphRegion.yMin), new Vector2(x, graphRegion.yMax));
            }
            GL.Color(gridColor);
            for (float y = graphRegion.yMin - (graphRegion.yMin % gridSize) - scrollPosition.y; y < graphRegion.yMax; y += gridSize)
            {
                if (y < graphRegion.yMin)
                {
                    continue;
                }
                DrawLine(new Vector2(graphRegion.xMin, y), new Vector2(graphRegion.xMax, y));
            }
        }

        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        public void Draw(Rect position, Vector2 scroll)
        {
            graphRegion = position;
            scrollPosition = scroll;

            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Graphs.Styles.graphBackground.Draw(position, false, false, false, false);
            }

            DrawGrid();
        }
    }
}

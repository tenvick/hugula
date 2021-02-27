using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor
{
    public enum NodeType
    {
        Style1,
        Style2,
        Style3,
    }

    public class NodeEvent
    {
        public enum EventType
        {
            None,
            Touched,
        }

        public EventType eventType;
        public NodeGUI eventSourceNode;
        public Vector2 globalMousePosition;
    }

    public class NodeGUI
    {
        public string name;
        public int windowID;
        public int depth;
        public Rect nodeRect;
        public MonoBehaviour target;

        private string extraInfo;
        private NodeType nodeType;
        private string nodeTextColor;
        private string currentNodeStyle;

        private static GUIStyle titleTextStyle;
        private static GUIStyle infoTextStyle;
        
        private static Color titleBGColor = new Color32(80, 80, 80, 255);
        private static Dictionary<NodeType, string> selectedStyleDictionary;
        private static Dictionary<NodeType, string> unselectedStyleDictionary;
        private static Dictionary<NodeType, string> nodeTitleColorDictionary;

        public static Action<NodeEvent> NodeEventHandler { get; set; }

        public string ExtraInfo
        {
            get { return extraInfo; }
            set
            {
                extraInfo = value;
                UpdateRect();
            }
        }

        public GUIStyle TitleTextStyle
        {
            get
            {
                if (titleTextStyle == null)
                {
                    titleTextStyle = new GUIStyle(EditorStyles.label);
                    titleTextStyle.richText = true;
                    titleTextStyle.fontSize = 12;
                    titleTextStyle.alignment = TextAnchor.MiddleCenter;

                    if (EditorGUIUtility.isProSkin)
                    {
                        titleTextStyle.normal.textColor = Color.white;
                    }
                    else
                    {
                        titleTextStyle.normal.textColor = Color.black;
                    }
                }

                return titleTextStyle;
            }
        }

        public GUIStyle InfoTextStyle
        {
            get
            {
                if (infoTextStyle == null)
                {
                    infoTextStyle = new GUIStyle(EditorStyles.label);
                    infoTextStyle.fontSize = 11;
                    infoTextStyle.alignment = TextAnchor.MiddleCenter;

                    if (EditorGUIUtility.isProSkin)
                    {
                        infoTextStyle.normal.textColor = Color.white;
                    }
                    else
                    {
                        infoTextStyle.normal.textColor = Color.black;
                    }
                }

                return infoTextStyle;
            }
        }

        public NodeType NodeType
        {
            get { return nodeType; }

            set
            {
                nodeType = value;

                // get style
                currentNodeStyle = unselectedStyleDictionary[nodeType];
                nodeTextColor = nodeTitleColorDictionary[nodeType];
            }
        }

        public Vector2 Position
        {
            get { return nodeRect.position; }

            set
            {
                nodeRect.position = value;
            }
        }

        public float Right
        {
            get { return nodeRect.xMax; }
        }

        public float Bottom
        {
            get { return nodeRect.yMax; }
        }

        static NodeGUI()
        {
            selectedStyleDictionary = new Dictionary<NodeType, string>()
            {
                { NodeType.Style1, "flow node 0 on"},
                { NodeType.Style2, "flow node 0 on"},
                { NodeType.Style3, "flow node 0 on"},
            };

            unselectedStyleDictionary = new Dictionary<NodeType, string>()
            {
                { NodeType.Style1, "flow node 0"},
                { NodeType.Style2, "flow node 0"},
                { NodeType.Style3, "flow node 0"},
            };

            nodeTitleColorDictionary = new Dictionary<NodeType, string>()
            {
                { NodeType.Style1, ColorUtility.ToHtmlStringRGB(new Color32(38, 156, 255, 255)) },
                { NodeType.Style2, ColorUtility.ToHtmlStringRGB(new Color32(44, 238, 122, 255)) },
                { NodeType.Style3, ColorUtility.ToHtmlStringRGB(new Color32(255, 200, 0, 255)) },
            };
        }

        public void Draw()
        {
            var movedRect = GUI.Window(windowID, nodeRect, DrawNodeWindow, string.Empty, currentNodeStyle);

            if (GraphEditorSettings.enableDrag && Event.current.button == 0)
            {
                nodeRect = movedRect;
            }
        }

        public void SetActive()
        {
            currentNodeStyle = selectedStyleDictionary[nodeType];

            // set target
            Selection.activeObject = target;
        }

        public void SetInactive()
        {
            currentNodeStyle = unselectedStyleDictionary[nodeType];
        }

        public void UpdateRect()
        {
            float height = GraphEditorSettings.NodeBaseHeight;

            if (!string.IsNullOrEmpty(extraInfo))
            {
                height += InfoTextStyle.CalcSize(new GUIContent(extraInfo)).y;
            }

            nodeRect = new Rect(0, 0, GraphEditorSettings.NodeBaseWidth, height);
        }

        private void DrawNodeWindow(int id)
        {
            HandleNodeEvent();
            DrawNode();

            GUI.DragWindow();
        }

        private void HandleNodeEvent()
        {
            switch (Event.current.rawType)
            {
                case EventType.MouseUp:
                    {
                        if (Event.current.button == 0)
                        {
                            var ev = new NodeEvent
                            {
                                eventType = NodeEvent.EventType.Touched,
                                eventSourceNode = this,
                                globalMousePosition = Event.current.mousePosition + nodeRect.position,
                            };

                            // redirect to manager
                            NodeEventHandler.Invoke(ev);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void DrawNode()
        {
            var titleHeight = TitleTextStyle.CalcSize(new GUIContent(name)).y + GraphEditorSettings.NodeTitleHeightMargin;
            var titleRect = new Rect(0, 0, nodeRect.width, titleHeight);

            var infoHeight = nodeRect.height - titleHeight;
            var infoRect = new Rect(0, titleHeight, nodeRect.width, infoHeight);


            if (!EditorGUIUtility.isProSkin)
            {
                // draw dark title BG
                GUI.color = titleBGColor;
                var rect = new Rect(2, 2, nodeRect.width - 4, titleHeight - 4);
                GUI.Box(rect, string.Empty);
                GUI.color = Color.white;
            }

            // draw title
            var text = string.Format("<b><color=#{0}>{1}</color></b>", nodeTextColor, name);
            GUI.Label(titleRect, text, TitleTextStyle);

            if (!string.IsNullOrEmpty(extraInfo))
            {
                GUI.Label(infoRect, extraInfo, InfoTextStyle);
            }
        }
    }
}

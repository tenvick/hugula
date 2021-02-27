using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        public class TreeNode
        {
            public TreeNode parent;
            public List<TreeNode> children = new List<TreeNode>();
            public List<NodeGUI> nodeList = new List<NodeGUI>();

            public int Depth()
            {
                int i = 0;
                TreeNode curr = this;
                while (curr != null && curr.parent != null)
                {
                    i++;
                    curr = curr.parent;
                }

                return i;

            }

            public int ChildrenCount()
            {
                int i = children.Count;

                foreach (var c in children)
                {
                    i += c.ChildrenCount();
                }

                return i;
            }

            public void AddChild(TreeNode child)
            {
                child.parent = this;
                children.Add(child);
            }

            public void AddNode(NodeGUI node)
            {
                nodeList.Add(node);
            }
        }

        public enum NodeTypeFilter
        {
            BindableContainer,
            Binder,
            All,
        }

        private List<NodeGUI> nodeList = new List<NodeGUI>();
        private List<ConnectionGUI> connectionList = new List<ConnectionGUI>();
        private Dictionary<NodeGUI, List<ConnectionGUI>> nodeToConnectionMap = new Dictionary<NodeGUI, List<ConnectionGUI>>();

        private GraphBackground background = new GraphBackground();

        private Rect graphRegion;
        private Vector2 scrollRectMax;
        private Vector2 scrollPos;

        private int bindableContainerCount;
        private int binderCount;
        private bool enableExtraInfo = true;

        private string searchText;
        private NodeTypeFilter searchType;
        private bool searchResultDirty;
        private int currentNodeIndex;
        private List<NodeGUI> searchResultList = new List<NodeGUI>();

        [MenuItem("Hugula/Data Binding/Data Binding Graph")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<GraphEditorWindow>("DataBinding");
            window.Show();
        }

        void OnEnable()
        {
            ResetView();
            NodeGUI.NodeEventHandler = OnNodeEvent;
        }

        void OnDisable()
        {
            NodeGUI.NodeEventHandler = null;
        }

        void OnGUI()
        {
            DrawToolbar();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawGraph();
            }

            HandleEvents();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var toolbarHeight = GUILayout.Height(GraphEditorSettings.ToolbarHeight);
                if (GUILayout.Button("Create Graph", EditorStyles.toolbarButton, GUILayout.Width(100), toolbarHeight))
                {
                    CreateGraph();
                }
                if (GUILayout.Button("Check Error", EditorStyles.toolbarButton, GUILayout.Width(100), toolbarHeight))
                {
                    CheckError();
                }
                EditorGUILayout.LabelField("Find what", GUILayout.Width(70), toolbarHeight);

                EditorGUI.BeginChangeCheck();
                {
                    searchText = EditorGUILayout.TextField(string.Empty, searchText, new GUIStyle("ToolbarSeachTextField"), GUILayout.Width(160), toolbarHeight);
                    if (GUILayout.Button("Close", "ToolbarSeachCancelButtonEmpty"))
                    {
                        // reset text
                        searchText = null;
                    }
                    //EditorGUILayout.LabelField("NodeType Filter", GUILayout.Width(100), toolbarHeight);
                    //searchType = (NodeTypeFilter)EditorGUILayout.EnumPopup(searchType, EditorStyles.toolbarPopup, GUILayout.Width(140), toolbarHeight);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    searchResultDirty = true;
                }
                if (GUILayout.Button("Find Next", EditorStyles.toolbarButton, GUILayout.Width(100), toolbarHeight))
                {
                    Find();
                }

                enableExtraInfo = GUILayout.Toggle(enableExtraInfo, "Enable Extra Info", EditorStyles.toolbarButton, GUILayout.Width(150), toolbarHeight);
                GraphEditorSettings.enableDrag = GUILayout.Toggle(GraphEditorSettings.enableDrag, "Enable Drag", EditorStyles.toolbarButton, GUILayout.Width(120), toolbarHeight);

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField(string.Format("Binder: {0}", binderCount), GUILayout.Width(100), toolbarHeight);
                EditorGUILayout.LabelField(string.Format("BindableContainer: {0}", bindableContainerCount), GUILayout.Width(150), toolbarHeight);
            }
        }

        private void DrawGraph()
        {
            // draw background
            background.Draw(graphRegion, scrollPos);

            using (var scrollScope = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollScope.scrollPosition;

                // draw connections
                connectionList.ForEach(x => x.Draw());

                // draw nodes
                BeginWindows();
                nodeList.ForEach(x => x.Draw());
                EndWindows();

                // handle GUI events
                HandleGraphGUIEvents();

                // set rect for scroll
                if (nodeList.Count > 0)
                {
                    GUILayoutUtility.GetRect(new GUIContent(string.Empty), GUIStyle.none, GUILayout.Width(scrollRectMax.x), GUILayout.Height(scrollRectMax.y));
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                var newRect = GUILayoutUtility.GetLastRect();
                if (newRect != graphRegion)
                {
                    graphRegion = newRect;
                    Repaint();
                }
            }
        }

        private void HandleEvents()
        {

        }


        private void HandleGraphGUIEvents()
        {
            var e = Event.current;

            switch (e.type)
            {
                case EventType.MouseUp:
                    {
                        if (e.button == 0)
                        {
                            SelectNode(null);
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    {
                        if (e.button == 2)
                        {
                            // middle
                            var newPos = scrollPos - e.delta;
                            scrollPos.x = Mathf.Max(newPos.x, 0f);
                            scrollPos.y = Mathf.Max(newPos.y, 0f);
                            Repaint();
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void OnNodeEvent(NodeEvent ev)
        {
            if (ev.eventType == NodeEvent.EventType.Touched)
            {
                SelectNode(ev.eventSourceNode);
            }
        }

        private void UpdateScrollRect()
        {
            if (nodeList.Count == 0)
            {
                return;
            }

            var right = nodeList.OrderByDescending(x => x.Right).Select(x => x.Right).First() + GraphEditorSettings.WindowSpan;
            var bottom = nodeList.OrderByDescending(x => x.Bottom).Select(x => x.Bottom).First() + GraphEditorSettings.WindowSpan;

            scrollRectMax = new Vector2(right, bottom);
        }

        private void SetupView(TreeNode root)
        {
            int index = 0;
            float x = GraphEditorSettings.TreeViewStartX;
            float y = GraphEditorSettings.TreeViewStartY;

            SetupLayout(root, x, ref y, ref index);

        }

        private void SetupLayout(TreeNode treeNode, float startX, ref float y, ref int index)
        {
            float step = GraphEditorSettings.NodeBaseWidth + GraphEditorSettings.TreeViewHorizontalMargin;

            foreach (var item in treeNode.nodeList)
            {
                var x = step * item.depth + startX;
                item.Position = new Vector2(x, y);

                // calculate next
                y += (item.nodeRect.height + GraphEditorSettings.TreeViewVerticalMargin);

                // add to graph
                AddNode(item);
                index++;
            }

            // iterate children
            foreach (var item in treeNode.children)
            {
                SetupLayout(item, startX, ref y, ref index);
            }
        }


        private void ResetView()
        {
            // clear current
            nodeList.Clear();
            connectionList.Clear();
            nodeToConnectionMap.Clear();
            searchResultList.Clear();

            searchText = null;
            searchType = NodeTypeFilter.All;
            searchResultDirty = true;
            currentNodeIndex = 0;

            binderCount = 0;
            bindableContainerCount = 0;
        }

        public void CheckError()
        {
            var selectedTransform = Selection.activeTransform;

            if (selectedTransform == null)
            {
                Debug.LogError("No transform is selected");
                return;
            }
            else
            {
                var transformPath = GetGameObjectPath(selectedTransform);
                Debug.LogFormat("Check DataBinding for {0}", transformPath);
            }

            var container = selectedTransform.GetComponent<Hugula.Databinding.BindableObject>();
            //todo

        }

        public void CreateGraph()
        {
            var selectedTransform = Selection.activeTransform;

            if (selectedTransform == null)
            {
                Debug.LogError("No transform is selected");
                return;
            }
            else
            {
                var transformPath = GetGameObjectPath(selectedTransform);
                Debug.Log(string.Format("Create binding graph for {0}", transformPath), selectedTransform);
            }

            ResetView();

            // build tree from selected
            var root = new TreeNode();
            var container = selectedTransform.GetComponent<Hugula.Databinding.BindableObject>();
            BuildTree(container, root);

            // get count
            bindableContainerCount = root.ChildrenCount() + 1;

            // setup view
            SetupView(root);

            UpdateScrollRect();
        }

        private void DoSearch()
        {
            // do new search
            searchResultList.Clear();

            foreach (var item in nodeList)
            {
                if (searchType != NodeTypeFilter.All)
                {
                    // filter node type
                    if ((int)item.NodeType != (int)searchType)
                    {
                        continue;
                    }
                }

                if (item.ExtraInfo == null)
                {
                    continue;
                }

                if (item.name.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0 || item.ExtraInfo.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    // add to result list
                    searchResultList.Add(item);
                }
            }

            searchResultDirty = false;
            currentNodeIndex = 0;

            if (searchResultList.Count == 0)
            {
                Debug.LogFormat("Can't find \"{0}\"", searchText);
            }
            else
            {
                Debug.LogFormat("Find {0} nodes", searchResultList.Count);
            }
        }

        private void Find()
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }

            if (searchResultDirty)
            {
                DoSearch();
            }

            if (searchResultList.Count == 0)
            {
                // find nothing
                return;
            }

            //// get node from list
            NodeGUI currentNode = searchResultList[currentNodeIndex];

            //// wrap around
            currentNodeIndex = (currentNodeIndex + 1) % searchResultList.Count;

            //Debug.LogFormat("Node {0}, transform path {1}", currentNode.name, EditorHelper.GetTransformPath(currentNode.target.transform));

            FocusNode(currentNode);
            SelectNode(currentNode);
        }

        private void BuildTree(Hugula.Databinding.BindableObject root, TreeNode treeNode)
        {
            var depth = treeNode.Depth() + 1;
            binderCount += root.GetBindings().Count;
            // 构建自身节点
            var rootNode = CreateNode(NodeType.Style1);
            rootNode.name = string.Format("{0}({1})", root.name, root.GetType().Name);
            rootNode.target = root;
            if (enableExtraInfo)
            {
                var sourceList = GetBindingSourceList(root);
                rootNode.ExtraInfo = CollectSourceInfo(sourceList);
            }
            rootNode.depth = depth - 1;
            treeNode.AddNode(rootNode);

            //父节点连接 
            var parnet = treeNode.parent;
            if (parnet != null)
            {
                var parentRoot = parnet.nodeList[0];
                var connection = CreateConnection();
                connection.startNode = parentRoot;
                connection.endNode = rootNode;
                AddConnection(connection, true);
            }

            //children
            if (root is ICollectionBinder)
            {
                var container = root as Hugula.Databinding.ICollectionBinder;
                var children = container.GetChildren();
                foreach (var item in children)
                {
                    NodeGUI node = null;
                    if (item is ICollectionBinder)
                    {
                        var childTreeNode = new TreeNode();
                        treeNode.AddChild(childTreeNode);
                        BuildTree(item, childTreeNode);
                    }
                    else if (item != null)
                    {
                        binderCount += item.GetBindings().Count;
                        node = CreateNode(NodeType.Style2);
                        if (enableExtraInfo)
                        {
                            var sourceList = GetBindingSourceList(item);
                            node.ExtraInfo = CollectSourceInfo(sourceList);
                        }

                        var connection = CreateConnection();
                        connection.startNode = rootNode;  //= FindNodeUp(treeNode, NodeType.Style1);
                        connection.endNode = node;
                        AddConnection(connection, true);

                        node.name = string.Format("{0}({1})", item.name, item.GetType().Name);
                        node.target = item;
                        node.depth = depth;

                        treeNode.AddNode(node);
                    }


                }

            }


        }


        private string GetGameObjectPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

        private int GenerateWindowID()
        {
            int id = -1;

            foreach (var item in nodeList)
            {
                if (item.windowID > id)
                {
                    id = item.windowID;
                }
            }

            return id + 1;
        }

        private void FocusNode(NodeGUI node)
        {
            var offset = node.nodeRect.center - graphRegion.center;

            var newScrollPos = new Vector2();
            newScrollPos.x = Mathf.Clamp(offset.x, 0f, scrollRectMax.x);
            newScrollPos.y = Mathf.Clamp(offset.y, 0f, scrollRectMax.y);

            scrollPos = newScrollPos;

            Repaint();
        }

        public NodeGUI CreateNode(NodeType nodeType)
        {
            var node = new NodeGUI();

            // set default value
            node.UpdateRect();
            node.NodeType = nodeType;

            return node;
        }

        public void SelectNode(NodeGUI node)
        {
            foreach (var item in nodeList)
            {
                item.SetInactive();
            }

            if (node != null)
            {
                node.SetActive();

                // select associated connection
                if (nodeToConnectionMap.ContainsKey(node))
                {
                    var list = nodeToConnectionMap[node];
                    SelectConnections(list);
                }
            }
            else
            {
                // set target
                Selection.activeObject = null;

                SelectConnection(null);
            }

            Repaint();
        }

        public void SelectConnection(ConnectionGUI connection)
        {
            foreach (var item in connectionList)
            {
                item.SetInactive();
            }

            if (connection != null)
            {
                connection.SetActive();
            }
        }

        public void SelectConnections(IEnumerable<ConnectionGUI> connections)
        {
            foreach (var item in connectionList)
            {
                item.SetInactive();
            }

            foreach (var item in connections)
            {
                item.SetActive();
            }
        }

        static System.Text.StringBuilder sb = new System.Text.StringBuilder();

        private string BindingToString(Hugula.Databinding.Binding binding)
        {
            sb.Clear();
            var property = binding.propertyName;
            var path = binding.path;
            var format = binding.format;
            BindingMode mode = binding.mode;
            var converter = binding.converter;
            var source = binding.source;
            if (!string.IsNullOrEmpty(path))
                sb.AppendFormat(".{0}=({1}) ", property, path);
            if (!string.IsNullOrEmpty(format))
                sb.AppendFormat("format({0}) ", format);
            if (mode != BindingMode.OneWay)
                sb.AppendFormat("mode({0}) ", mode);
            if (!string.IsNullOrEmpty(converter))
                sb.AppendFormat("converter({0}) ", converter);
            if (source)
            {
                //sb.AppendLine();
                sb.AppendFormat("source={0}", source.name);
            }

            return sb.ToString();
        }

        private List<string> GetBindingSourceList(Hugula.Databinding.BindableObject bindableObject)
        {
            var list = new List<string>();
            var bindings = bindableObject.GetBindings();


            foreach (var binding in bindings)
            {
                list.Add(BindingToString(binding));
            }

            return list;
        }

        private string CollectSourceInfo(List<string> sourceList)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in sourceList)
            {
                if (sb.Length != 0)
                {
                    sb.AppendLine();
                }

                sb.AppendFormat("{0}", item);
            }

            return sb.ToString();
        }

        public ConnectionGUI CreateConnection()
        {
            var connection = new ConnectionGUI();

            return connection;
        }

        public void AddNode(NodeGUI node)
        {
            node.windowID = GenerateWindowID();
            nodeList.Add(node);
        }

        public void AddConnection(ConnectionGUI connection, bool twoWay)
        {
            AddAssociation(connection.startNode, connection);

            if (twoWay)
            {
                AddAssociation(connection.endNode, connection);
            }

            connectionList.Add(connection);
        }

        private void AddAssociation(NodeGUI node, ConnectionGUI connection)
        {
            if (node == null)
            {
                return;
            }

            if (!nodeToConnectionMap.ContainsKey(node))
            {
                // add it
                nodeToConnectionMap.Add(node, new List<ConnectionGUI>());
            }

            nodeToConnectionMap[node].Add(connection);
        }


    }
}

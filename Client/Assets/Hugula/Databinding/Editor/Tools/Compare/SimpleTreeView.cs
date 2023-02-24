using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HugulaEditor.Databinding
{
    class SimpleTreeView : TreeView
    {
        public SimpleTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Reload();

            getNewSelectionOverride = getNewSelectionFunction;

        }

        public delegate void NewSelectionFunction(TreeView treeView, TreeViewItem clickedItem, bool keepMultiSelection, bool useActionKeyAsShift);

        public NewSelectionFunction onViewItemSelected;

        private List<int> getNewSelectionFunction(TreeViewItem clickedItem, bool keepMultiSelection, bool useActionKeyAsShift)
        {
            // Debug.Log($" clickedItem:{clickedItem}  keepMultiSelection:{keepMultiSelection}");
            onViewItemSelected?.Invoke(this, clickedItem, keepMultiSelection, useActionKeyAsShift);
            return new List<int>() { clickedItem.id };
        }

        GameObject m_GameObject;
        Dictionary<int, GameObject> m_CacheGameobject = new Dictionary<int, GameObject>();

        public static Texture2D m_GOIcon = EditorGUIUtility.FindTexture("GameObject Icon");

        public void SetGameObject(GameObject go)
        {
            m_GameObject = go;
            m_CacheGameobject.Clear();
            m_AllItems.Clear();
            if (go != null)
                CreateTreeViewItem(go.transform, m_AllItems, 0);
            Reload();
        }

        public GameObject GetGameObject(int instanceID)
        {
            m_CacheGameobject.TryGetValue(instanceID, out var gObj);
            return gObj;
        }


        List<TreeViewItem> m_AllItems = new List<TreeViewItem>();
        int m_ID = 0;
        void CreateTreeViewItem(Transform transform, List<TreeViewItem> allItems, int depth)
        {
            var bos = transform.GetComponents<Hugula.Databinding.BindableObject>();
            UIBehaviour[] all;
            var count = bos?.Length;
            var bcCount = transform.GetComponents<Hugula.Databinding.ICollectionBinder>()?.Length;
            var str = "";
            if (count > 0 || bcCount > 0)
                str = $"				({count}-{bcCount}) {bos[0].GetType().Name}";
            else if ((all = transform.GetComponents<UIBehaviour>()) != null && all.Length > 0)
            {
                str = $"				[{all[all.Length - 1].GetType().Name}]";
            }

            var treeViewItem = new TransformTreeViewItem(transform.GetInstanceID(), depth, transform.name + str, transform);//root;
            treeViewItem.icon = m_GOIcon;
            m_AllItems.Add(treeViewItem);
            m_CacheGameobject[transform.GetInstanceID()] = transform.gameObject;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                CreateTreeViewItem(child, allItems, depth + 1);
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            SetupParentsAndChildrenFromDepths(root, m_AllItems);
            return root;
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
			var transformTreeViewItem = item as TransformTreeViewItem;
			if(transformTreeViewItem !=null && transformTreeViewItem.transform && search.IndexOf("t:")>=0)
			{	
				search=search.Trim();
				var typeStr = search.Substring(2,search.Length-2);
				var comps = transformTreeViewItem.transform.GetComponents<MonoBehaviour>();
				foreach(var  com in comps)
				{
					if(com.GetType().Name.Contains(typeStr))
						return true;
				}			 
				return false;
			}else
            	return item.displayName.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }

    public class TransformTreeViewItem : TreeViewItem
    {
        public Transform transform;
        public TransformTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {

        }

        public TransformTreeViewItem(int id, int depth, string displayName, Transform transform) : base(id, depth, displayName)
        {
            this.transform = transform;
        }

        public TransformTreeViewItem() : base()
        {

        }
    }

}


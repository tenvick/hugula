using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Hugula.UGUIExtend;

[CustomEditor(typeof(ScrollRectTable))]
public class ScrollRectTableEditor : UnityEditor.Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Layout ", GUILayout.Width(200));
		ScrollRectTable temp = target as ScrollRectTable;

		// temp.data
		if (GUILayout.Button("Simulate Layout"))
		{
			temp.data = null;
			// temp.SetRangeSymbol(temp.pageSize);
			var item0 = temp.tileItem;
			SetPosition(temp,item0.rectTransform,0);
			var childrens1 = temp.GetComponentsInChildren<ScrollRectItem>();
			for(int i=childrens1.Length-1; i < temp.pageSize;i++)
			{
				GameObject clone = (GameObject)GameObject.Instantiate(item0.gameObject);
				clone.transform.SetParent(temp.transform, false);
			}

			List<ScrollRectItem> childrens = new List<ScrollRectItem>();
			childrens1 = temp.GetComponentsInChildren<ScrollRectItem>();
			foreach(var item in childrens1)
			{
				if(item!=item0)
					childrens.Add(item);
			}
			
			for(int i =0;i<childrens.Count;i++)
			{
				temp.SetPosition(childrens[i].transform as RectTransform,i);
			}
		}

		if (GUILayout.Button("Clear Simulate"))
		{
			var childrens = temp.GetComponentsInChildren<ScrollRectItem>();
			// temp.data = null;
			// SetRectItemPos(temp.tileItem,0);
			foreach (var item in childrens)
			{
				if(item!=temp.tileItem) GameObject.DestroyImmediate(item.gameObject);
			}
			var rectt = temp.transform as RectTransform;
			Debug.LogFormat("anchorMax={0},offsetMax={1}",rectt.anchorMax,rectt.offsetMax);
			Debug.LogFormat("anchorMin={0},offsetMin={1}",rectt.anchorMin,rectt.offsetMin);
			// temp.SetRangeSymbol(0);
			temp.moveContainer.anchoredPosition = new Vector3(0, 0, 0);
			var sizeDelta =  temp.moveContainer.sizeDelta;
			var rectTrans = temp.tileItem.rectTransform;
			if(temp.columns==0)
			{
					var dtAnchor = rectTrans.anchorMax - rectTrans.anchorMin;
				if (1 - dtAnchor.y <= 0.001 ) // 而且是高度适配 
				{
					sizeDelta.x = 1;
				}
			}else if(temp.columns==1) //如果只有一列
			{
				var dtAnchor = rectTrans.anchorMax - rectTrans.anchorMin;
				if (1 - dtAnchor.x <= 0.001 ) // 而且是宽度适配 
				{
					sizeDelta.y = 1;
				}
			}else
				sizeDelta = new Vector2(1,1);
			temp.moveContainer.sizeDelta = sizeDelta;
			//
		}

		if(GUILayout.Button("Hide Source ScorllItem"))
		{
			SetRectItemPos(temp.tileItem);
		}

	}

	static ScrollRectTableEditor()
	{
		PrefabUtility.prefabInstanceUpdated = delegate(GameObject instance) 
		{
			// Debug.Log(instance);
			
		};
	}

	public void SetPosition(ScrollRectTable table, RectTransform trans, int index)
	{
		if (trans.parent != table.transform) trans.SetParent(table.transform,false);

		var pos = trans.localPosition;
		pos.x = -10000;
		trans.localPosition = pos;

        pos = trans.localPosition;
		var rect = table.itemRect;
		if (table.columns == 0)
		{
			pos.x = (rect.width+table.padding.x) * index + table.padding.x + rect.width*.5f ;
		}
		else
		{
			int y = index / table.columns;
			int x = index % table.columns;
			if (table.direction == ScrollRectTable.Direction.Down)
				pos.y = -(rect.height+table.padding.y) * y - rect.height*.5f - table.padding.y;//+ rect.height * .5f
			else
				pos.y = (rect.height+table.padding.y) * y + rect.height*.5f + table.padding.y ;//+ rect.height * .5f;

			pos.x = (rect.width+table.padding.x)* x + table.padding.x + rect.width * .5f; 
		}
		trans.localPosition = pos;
	}

	void SetRectItemPos(ScrollRectItem item,float x=-10000)
	{
		var  pos = item.transform.localPosition;
		pos.x = x;
		item.transform.localPosition = pos;
		item.gameObject.SetActive(true);
	}
}

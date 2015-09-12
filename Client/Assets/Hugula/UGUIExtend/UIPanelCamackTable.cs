// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SLua;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/UIPanel Camack Table ")]
[SLua.CustomLuaClass]
public class UIPanelCamackTable : MonoBehaviour
{
    /**
    #region public static
    /// <summary>
    /// ²åÈëÊý¾ÝµÄÊ±ºòµ÷ÓÃ
    /// </summary>
    public static string DataInsertStr = @"return function(data,index,script)
      if script.data==nil then script.data={} end
      local lenold=#script.data
      table.insert(script.data,index,data)
  end";

    /// <summary>
    /// ÒÆ³ýÊý¾ÝµÄÊ±ºòµ÷ÓÃ
    /// </summary>
    public static string DataRemoveStr = @"return function(data,index,script)
      table.remove(data,index)
  end";

    /// <summary>
    /// Ô¤äÖÈ¾µÄÊ±ºòµ÷ÓÃ
    /// </summary>
    public static string PreRenderStr = @"return function(referScipt,index,dataItem)
    referScipt.name=""Pre""..tostring(index)  
    referScipt.gameObject:SetActive(false)
  end";
    #endregion

    #region public attribute
    public enum Direction
	{
		Down,
		Up,
	}
	public int scrollDirection{get;private set;}	
	public Direction direction = Direction.Down;
	public GameObject moveContainer;
	public ReferGameObjects  tileItem;//the template item
	public LuaFunction onItemRender;//function(tileItemClone,index,dataItem)
	public LuaFunction onPreRender;//function(tileItemClone,index,dataItem)
	public LuaFunction onDataRemove;//function(data,index,UIPanelCamackTable)
	public LuaFunction onDataInsert;//function(data,index,UIPanelCamackTable)
	
	public int pageSize=5;
	public float renderPerFrames=1;
//	public int pageCount  //
//	{ 	get;
//		private set;
//	}
	public int	recordCount  //
	{ 	get;
		private set;
	}
	
	public int columns = 0;
	public Vector2 padding = Vector2.zero;
	
	//public bool repositionNow = false;
	//public bool keepWithinPanel = false;
	public Vector3 tileSize=new Vector3(158,193,1);
	public bool keepTileBound;
	public LuaTable data{
		get{return _data;}
		set{
			if(_data!=null)
			{
				foreach(ReferGameObjects obj in this.repositionTileList)
					obj.gameObject.SetActive(false);
			}
			_data=value;
			CalcBounds();
			CalcPage();
		    this.currFirstIndex=0;
			this.lastEndIndex=0;
		}
	}
	#endregion
	
	
	#region private attribute
	LuaTable _data;//data
	public int headIndex{
		get; ////the camera position index
		private set;
	}
	public int currFirstIndex{ get;private set;}//=0;//current pageBegin data index
	int lastEndIndex=0;// last time render last data index
	int currRenderIndex=0;//current render index
	int lastHeadIndex=0;
    //int lastRecordCount=0;
	List<int> repositionIntList=new List<int>();
	List<ReferGameObjects> repositionTileList=new List<ReferGameObjects>();
	List<ReferGameObjects> preRenderList=new List<ReferGameObjects>();//
//	List<GameObject> preDeleteList=new List<GameObject>();//
//	int dir=0;
	
	Vector3 dtmove;	
	Vector3 beginPosition;
	Vector3 currPosition;
	
	UIPanel mPanel;
    //UIScrollView mDrag;
	UICamera mDragCamera;
	bool mStarted = false;
	bool foward=false;//panel true camera
	
	Rect rect;
	BoxCollider boxCollider;
	
	private GameObject beginSymbol;
	private GameObject endSymbol;
	#endregion
	
	
	#region public method
	public int getIndex(ReferGameObjects item)
	{
		int i=this.repositionTileList.IndexOf(item);
		int j=-1;
		if(i>=0)j=this.currFirstIndex+i;
		return j;
	}

	public LuaTable getDataFromIndex(int index)
	{
		return (LuaTable)data[index + 1];
	}

	public int removeChild(ReferGameObjects item)
	{
		int i=getIndex(item);
		if(i>=0)
		{
            if (onDataRemove == null) onDataRemove = PLua.instance.lua.LoadString(DataRemoveStr, "onDataRemove");
            onDataRemove.Call(this.data,i+1,this);
			this.CalcPage();
        }
		return i;
	}
	
	public int insertData(object item,int index)
	{
		if(index<0)index=0;
		if(index>=this.recordCount)index=this.recordCount;
        if (onDataInsert == null) onDataInsert = PLua.instance.lua.LoadString(DataInsertStr, "onDataInsert");
		onDataInsert.Call(item,index+1,this);
		this.CalcPage();
		return index;
	}
	
	public int removeDataAt(int index)
	{
		if(index>=0 && index<this.recordCount)
		{
            if (onDataRemove == null) onDataRemove = PLua.instance.lua.LoadString(DataRemoveStr, "onDataRemove");
            onDataRemove.Call(data,index+1,this);
			this.CalcPage();
            return index;
		}
		
		return -1;
	}
	
	public void clear()
	{
		foreach(ReferGameObjects item in repositionTileList)
		{
			item.gameObject.SetActive(false);
		}
	}
	
	public void scrollTo(int index)
	{
		Vector3 currPos=moveContainer.transform.localPosition;
		if(index<0)index=0;
		if(columns==0)
		{
			float x=index*rect.width;
			if(foward)
			{
				currPos.x=beginPosition.x+x; //ok
				currPos.y=beginPosition.y;
				currPos.z=beginPosition.z;
			}
			else
			{
				currPos.x=beginPosition.x-x;
				currPos.y=beginPosition.y;
				currPos.z=beginPosition.z;
			}
		}else if(columns>0)
		{
			float y=((int)((float)index/(float)columns))*rect.height;
			if(foward)
			{
				currPos.x=beginPosition.x;
				currPos.z=beginPosition.z;
				if(this.direction==Direction.Down)
					currPos.y=beginPosition.y-y;//pos.y=-(rect.height*y+ this.padding.y);
				else
					currPos.y=beginPosition.y+y;//pos.y=(rect.height*y+ this.padding.y);
			}
			else
			{
				currPos.x=beginPosition.x;
				currPos.y=Math.Abs(beginPosition.y-y); //ok
				currPos.z=beginPosition.z;
			}
		}
		SpringPanel.Begin(moveContainer,currPos,13f);
	}
	
	/// <summary>
	/// Refresh the form give begin data Index.
	/// </summary>
	/// <param name='begin'>
	/// Begin.
	/// </param>
	public void Refresh(int begin=-1,int end=-1)
	{
		if(!mStarted)return;
		int bg=0,ed=0;
		if(begin<0)
		{ 
			bg=0;//Debug.Log("Refresh 0 ");
			ed=this.pageSize;
			if(moveContainer!=null)
				moveContainer.transform.localPosition=this.beginPosition;
			if(mPanel!=null)
			{
				Vector4 cr = mPanel.finalClipRegion;
				cr.x = 0;
				cr.y = 0;
				if (cr.z==0)cr.z=Screen.width;
                mPanel.clipOffset = cr; 
			}
			scroll(0,true);
		}
		else
		{
		
			bg=begin;
			if (end==-1)
				 end=this.currFirstIndex+this.pageSize;
				
			refresh(bg,end);
		}
	}
	
	/// <summary>
	/// Refresh the specified item's position.
	/// </summary>
	/// <param name='item'>
	/// Item.
	/// </param>
	public void Refresh(ReferGameObjects item)
	{
		int i=getIndex(item);
		if(i>=0)
		{
			i=i+currFirstIndex;
			preRefresh(i);
		}
	}
	
	#endregion
	
	#region private method
	
	void CalcPage()
	{
        //lastRecordCount=recordCount;
		if(this._data!=null)
		{
			recordCount=this._data.Values.Count;
		}
		else
		{
			recordCount=0;
            //lastRecordCount=0;
		}
		SetRangeSymbol();
	}
	
	void SetRangeSymbol()
	{
		if(beginSymbol==null)
		{
			beginSymbol=new GameObject("begin");
			beginSymbol.AddComponent<UIWidget>();
            beginSymbol.layer = this.gameObject.layer;
			UIWidget w = beginSymbol.GetComponent<UIWidget>();
			w.SetDimensions(2,2);
		}
		if(endSymbol==null)
		{
			endSymbol=new GameObject("end");
            endSymbol.AddComponent<UIWidget>();
            endSymbol.layer = this.gameObject.layer;
			UIWidget w = endSymbol.GetComponent<UIWidget>();
			w.SetDimensions(2,2);
		}
		
		setPosition(beginSymbol.transform,0);
		setPosition(endSymbol.transform,recordCount - 1);
	}
		
	void CalcLastEndIndex()
	{
		int last=this.currFirstIndex+this.pageSize-1;
		if(last>=this.recordCount)last=recordCount-1;
		this.lastEndIndex=last;
	}
	
	void CalcBounds()
	{
		if( tileItem )
		{
			GameObject tileObj=tileItem.gameObject;
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(tileObj.transform);
			Vector3 scale = tileObj.transform.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
            if (keepTileBound)
            {
                Vector3 max = new Vector3(0.5f, 0.5f, 0f);
                b.max = Vector3.Scale(max, tileSize);
                Vector3 min = new Vector3(-0.5f, -0.5f, 0f);
                b.min = Vector3.Scale(min, tileSize);
            }
			else if(b.extents==Vector3.zero)
			{
				MeshFilter me=tileObj.GetComponent<MeshFilter>();
				if(me!=null)
				{
					b=me.sharedMesh.bounds;
					b.min = Vector3.Scale(b.min, scale);
					b.max = Vector3.Scale(b.max, scale);
				}
                if (me == null)
				{
					Vector3 max=new Vector3(0.5f,0.5f,0f);
					b.max=Vector3.Scale(max, tileSize);
					Vector3 min=new Vector3(-0.5f,-0.5f,0f);
					b.min=Vector3.Scale(min, tileSize);
				}				
			}
			
			rect=new Rect(0,0,b.size.x+this.padding.x,b.size.y+this.padding.y);
		}
	}
	
	float renderframs=0;
	void renderItem()
	{
		if(renderPerFrames<1)
		{
			renderframs+=renderPerFrames;
			if(renderframs>=1)
			{
				renderframs=0;
				render();
			}
		}else
		{
			for(int i=0;i<this.renderPerFrames;i++)
			{
				render();
			}
		}
	}
	
	void render()
	{
		if(this.preRenderList.Count>0)
		{
			ReferGameObjects item=preRenderList[0];
			currRenderIndex=this.repositionIntList[0];
			preRenderList.RemoveAt(0);
			repositionIntList.RemoveAt(0);
			if(currRenderIndex+1<=recordCount)
			{
				if(onItemRender!=null)onItemRender.Call(new object[]{item,currRenderIndex,data[currRenderIndex+1]});
			}
		}
	}
	
	void setPosition(Transform trans,int index)
	{
		Vector3 scale=trans.localScale;
		trans.parent=this.transform;
		trans.localScale=scale;
		Vector3 pos=Vector3.zero;
		if(this.columns==0)
		{
			pos.x=rect.width*index+this.padding.x;
			if(boxCollider && this.boxCollider.size.x<pos.x)
			{
				Vector3 size=boxCollider.size;
				float sizeX=pos.x+rect.width;
				if(size.x<sizeX)size.x=sizeX;				
				boxCollider.size=size;
				Vector3 center=boxCollider.center;
				center.x=size.x/2;
				boxCollider.center=center;
			}
		}
		else
		{
			int y=index/columns;
			int x=index % columns;
			if(this.direction==Direction.Down)
				pos.y=-(rect.height*y+ this.padding.y);
			else
				pos.y=(rect.height*y+ this.padding.y);
			
			pos.x=rect.width*x+this.padding.x;
			
			if(boxCollider && (this.boxCollider.size.x<pos.x || Mathf.Abs(this.boxCollider.size.y)<Mathf.Abs(pos.y)))
			{
				Vector3 size=boxCollider.size;
				float sizeX=pos.x+rect.width;
				if(size.x<sizeX)size.x=sizeX;
				if(this.direction==Direction.Down)
					size.y=pos.y-rect.height*6;
				else
					size.y=pos.y+rect.height*6;
				boxCollider.size=size;
				Vector3 center=boxCollider.center;
				center.y=size.y/2;
				center.x=size.x/4;
				boxCollider.center=center;
			}

		}
		trans.localPosition=pos;
	}
	
	void preRender(ReferGameObjects item,int index)
	{
		setPosition(item.transform,index);
        if (this.onPreRender == null) onPreRender = PLua.instance.lua.LoadString(PreRenderStr, "onPreRenderStr");
		object dataI=index+1<=recordCount?data[index+1]:null;
		onPreRender.Call(new object[]{item,index,dataI});
	}
	
	void preRefresh(int i)
	{
		if(i>=0)
		{
			int Cindex=i-this.currFirstIndex;
			if(Cindex>=0)
			{
				if(repositionTileList.Count<this.pageSize)
				{
					GameObject obj =this.tileItem.gameObject;
					GameObject clone=(GameObject)GameObject.Instantiate(obj);
					ReferGameObjects cloneRefer=clone.GetComponent<ReferGameObjects>();
					repositionTileList.Add(cloneRefer);
					Vector3 scale=clone.transform.localScale;
					clone.transform.parent=this.transform;
					clone.transform.localScale=scale;
					this.lastEndIndex=i;
				}
				if(Cindex<this.pageSize)
				{
					ReferGameObjects tile=repositionTileList[Cindex];	
					this.preRenderList.Add(tile);
					repositionIntList.Add(i);	
					scrollDirection=0;
					preRender(tile,i);//Debug.Log(String.Format("preRefresh:{0}",i));
				}
			}
		}
		
	}
	
	void preLeftUp(int i)
	{//Debug.Log(String.Format("preLeftUp i={0},recordCount={1},pageSize={2} -- i<this.recordCount i>=this.pageSize",i,recordCount,pageSize));
		if(i>=this.pageSize && !repositionIntList.Contains(i) && i<this.recordCount)
		{
			ReferGameObjects tile=repositionTileList[0];				
			repositionTileList.RemoveAt(0);//remove first
			repositionTileList.Add(tile);//to end
			
			this.preRenderList.Add(tile);//add to preRenderList
			repositionIntList.Add(i);//add data index	
			
			currFirstIndex++;
			if(currFirstIndex+pageSize>recordCount)currFirstIndex=recordCount-this.pageSize;
			
			this.lastEndIndex=i;//recorde last render data index
			scrollDirection=1;
			preRender(tile,i);//call preRender,set Postion
		}
		
	}
	
	void preRightDown(int i)
	{//Debug.Log(String.Format("preRightDown i={0},recordCount={1},pageSize={2} -- i>=0  i+pageSize<=recordCount",i,recordCount,pageSize));
		if(i>=0 && !repositionIntList.Contains(i) && i+pageSize<=recordCount) //i>pageSize)//
		{
			int end1=repositionTileList.Count-1;
			ReferGameObjects tile=repositionTileList[end1];
			repositionTileList.RemoveAt(end1);//remove end
			repositionTileList.Insert(0,tile);//to first
			
			this.preRenderList.Add(tile);//add to preRenderList
			repositionIntList.Add(i);//add data index	
			currFirstIndex--;
			if(currFirstIndex<0)currFirstIndex=0;
			CalcLastEndIndex();
			scrollDirection=-1;
			preRender(tile,i);
		}
	}
	
	void scroll(int newHead ,bool force=false)
	{
		if(newHead<0)newHead=0;
			
		int step=newHead-currFirstIndex;
		if((step!=0 && this.headIndex!=lastHeadIndex) || force)
		{//Debug.Log(String.Format("step={0} lastEndIndex={1} currFirstIndex={2}",step,lastEndIndex,currFirstIndex));
			if(step>0)
			{
				int begin=lastEndIndex+1;
				int end=begin+step;
				for(int i=begin;i<end;i++)	
				{
					preLeftUp(i);
				}
				
			}else if(step<0)
			{
				int begin=currFirstIndex-1;
				int end=begin+step;
				if(begin<0)begin=0;
				for(int i=begin;i>end;i--)
				{
					preRightDown(i);
				}
			}else
			{
				scrollDirection=0;
				int begin=newHead;//lastHeadIndex;
				int end=begin+this.pageSize;
				if(end>this.recordCount)end=recordCount;
				for(int i=begin;i<end;i++)preRefresh(i);
				
				if(begin==0)
				{
					CalcLastEndIndex();
				}
			}
		}
	}
	
	void refresh(int begin,int end)
	{
		for(int i=begin;i<=end;i++)
		{
			if(i>=this.currFirstIndex)preRefresh(i);
		}

	}

	#endregion
	
	
	/// <summary>
	/// Position the grid's contents when the script starts.
	/// </summary>
	/// 
	void Start ()
	{
		mStarted = true;
		boxCollider=this.GetComponent<BoxCollider>();

		if (moveContainer!=null)
		{
			Vector3 bg=moveContainer.transform.localPosition;
			beginPosition=new Vector3(bg.x,bg.y,bg.z);
            //mDrag = moveContainer.GetComponent<UIScrollView>();// NGUITools.FindInParents<UIDraggablePanel>(gameObject);
			mPanel=moveContainer.GetComponent<UIPanel>();
			mDragCamera=moveContainer.GetComponent<UICamera>();
			foward=mDragCamera==null?false:true;
		}
			
		CalcBounds();
		
		scroll(0,true);
	}
	
	void Update()
	{
		if(moveContainer!=null && data!=null)
		{
			currPosition=moveContainer.transform.localPosition;
			if(foward)
				dtmove=currPosition-beginPosition;
			else
				dtmove=beginPosition-currPosition;
			
			if(columns==0 && dtmove.x>=0)
			{
				headIndex=(int)(dtmove.x/rect.width);
				scroll(headIndex);
			}else if(columns>0)
			{
				int cloumnIndex=(int)(dtmove.y/rect.height);
				headIndex= (int) Mathf.Ceil((float)(Mathf.Abs(cloumnIndex)*this.columns)/(float)this.columns)*columns;//
				scroll(headIndex);
			}
			lastHeadIndex=headIndex;
		}
	}
	
	/// <summary>
	/// Is it time to reposition? Do so now.
	/// </summary>
	void LateUpdate ()
	{
		if(this.repositionIntList.Count>0)renderItem();
	}
	***/
}

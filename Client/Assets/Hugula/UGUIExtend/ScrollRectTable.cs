// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SLua;

[ExecuteInEditMode]
[AddComponentMenu("UGUI/ScrollRectTable")]
[SLua.CustomLuaClass]
public class ScrollRectTable : MonoBehaviour
{
    #region public static
    /// <summary>
    /// insert data
    /// </summary>
    public static string DataInsertStr = @"return function(data,index,script)
      if script.data==nil then script.data={} end
      local lenold=#script.data
      table.insert(script.data,index,data)
  end";

    /// <summary>
    /// remove data from table
    /// </summary>
    public static string DataRemoveStr = @"return function(data,index,script)
      table.remove(data,index)
  end";

    /// <summary>
    /// prerender
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
	[SLua.DoNotToLua]
	public RectTransform moveContainer;
	public ScrollRectItem  tileItem;//the template item
	public LuaFunction onItemRender;//function(tileItemClone,index,dataItem)
	public LuaFunction onPreRender;//function(tileItemClone,index,dataItem)
	public LuaFunction onDataRemove;//function(data,index,UIPanelCamackTable)
	public LuaFunction onDataInsert;//function(data,index,UIPanelCamackTable)
	
	public int pageSize=5;
	public float renderPerFrames=1;

	public int	recordCount  //
	{ 	get;
		private set;
	}
	
	public int columns = 0;
	public Vector2 padding = Vector2.zero;
	
	public Vector3 tileSize=new Vector3(0,0,1);

	public LuaTable data{
		get{return _data;}
		set{
			if(_data!=null)
			{
				foreach(var obj in this.repositionTileList)
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

	List<int> repositionIntList=new List<int>();
	List<ScrollRectItem> repositionTileList=new List<ScrollRectItem>();
	List<ScrollRectItem> preRenderList=new List<ScrollRectItem>();//

	
	Vector3 dtmove;	
	Vector3 beginPosition;
	Vector3 currPosition;

	bool mStarted = false;
	bool foward=false;//panel true camera
	
	Rect rect;
	private Vector2 sizeDelta;
	#endregion
	
	
	#region public method
	public int getIndex(ScrollRectItem item)
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

	public int removeChild(ScrollRectItem item)
	{
		int i=getIndex(item);
		if(i>=0)
		{
            if (onDataRemove == null) onDataRemove = LuaState.main.loadString(DataRemoveStr, "onDataRemove");
            onDataRemove.call(this.data,i+1,this);
			this.CalcPage();
        }
		return i;
	}
	
	public int insertData(object item,int index)
	{
		if(index<0)index=0;
		if(index>=this.recordCount)index=this.recordCount;
		if (onDataInsert == null) onDataInsert = LuaState.main.loadString(DataInsertStr, "onDataInsert");
		onDataInsert.call(item,index+1,this);
		this.CalcPage();
		return index;
	}
	
	public int removeDataAt(int index)
	{
		if(index>=0 && index<this.recordCount)
		{
			if (onDataRemove == null) onDataRemove = LuaState.main.loadString(DataRemoveStr, "onDataRemove");
            onDataRemove.call(data,index+1,this);
			this.CalcPage();
            return index;
		}
		
		return -1;
	}
	
	public void clear()
	{
		foreach(var item in repositionTileList)
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
			currPos.x=beginPosition.x-x;
			currPos.y=beginPosition.y;
			currPos.z=beginPosition.z;

		}else if(columns>0)
		{
			float y=((int)((float)index/(float)columns))*rect.height;

			currPos.x=beginPosition.x;
			currPos.z=beginPosition.z;
			if(this.direction==Direction.Down)
				currPos.y=Math.Abs(beginPosition.y+y+this.padding.y);//pos.y=-(rect.height*y+ this.padding.y);
			else
				currPos.y=beginPosition.y-y-this.padding.y;//pos.y=(rect.height*y+ this.padding.y);
		}

		moveContainer.localPosition = currPos;
//		SpringPanel.Begin(moveContainer,currPos,13f);
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
			bg=0;//Debug.Log("Refresh 0 ");calc
			ed=this.pageSize;
			if(moveContainer!=null)
				moveContainer.transform.localPosition=this.beginPosition;

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
	public void Refresh(ScrollRectItem item)
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
		if(this._data!=null)
		{
			recordCount=this._data.length();
		}
		else
		{
			recordCount=0;
		}
		SetRangeSymbol();
	}
	
	void SetRangeSymbol()
	{
		if(moveContainer!=null)
		{
			var delt=moveContainer.sizeDelta;
			if(columns<=0)
				delt.x = recordCount*rect.width+this.padding.x*2;
			else
			{
				int y=(recordCount)/columns+1;
				int x=(recordCount) % columns;
				if(this.direction==Direction.Down)
					delt.y=(rect.height*y+ this.padding.y*2);
				else
					delt.y=-(rect.height*y+ this.padding.y*2);
				
//				delt.x=rect.width*x+this.padding.x*2;
			}
			moveContainer.sizeDelta=delt;
			sizeDelta = delt;
		}

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
			float wi,he;
			RectTransform rectTrans =tileItem.rectTransform;
			var size=rectTrans.sizeDelta;

			wi=tileSize.x<=0?size.x:tileSize.x;
			he=tileSize.y<=0?size.y:tileSize.y;

			rect = new Rect(0,0,wi+padding.x,he+padding.y);
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
			ScrollRectItem item=preRenderList[0];
			currRenderIndex=this.repositionIntList[0];
			preRenderList.RemoveAt(0);
			repositionIntList.RemoveAt(0);
			if(currRenderIndex+1<=recordCount)
			{
				if(onItemRender!=null)onItemRender.call(item,currRenderIndex,data[currRenderIndex+1]);
			}
		}
	}
	
	void setPosition(Transform trans,int index)
	{
		if(trans.parent!=this.transform)trans.SetParent(this.transform);
		Vector3 pos=Vector3.zero;
		if(this.columns==0)
		{
			pos.x=rect.width*index+rect.width*.5f+this.padding.x;
			if(this.direction==Direction.Down)
				pos.y=-(this.padding.y+rect.height*.5f);
			else
				pos.y=this.padding.y+rect.height*.5f;
		}
		else
		{
			int y=index/columns;
			int x=index % columns;
			if(this.direction==Direction.Down)
				pos.y=-(rect.height*y+ this.padding.y+rect.height*.5f);
			else
				pos.y=rect.height*y+ this.padding.y+rect.height*.5f;

			pos.x=rect.width*x+rect.width*.5f+this.padding.x;

		}
		trans.localPosition=pos;
	}
	
	void preRender(ScrollRectItem item,int index)
	{
		setPosition(item.transform,index);
        if (this.onPreRender == null) onPreRender = LuaState.main.loadString(PreRenderStr, "onPreRenderStr");
		object dataI=index+1<=recordCount?data[index+1]:null;
		onPreRender.call(item,index,dataI);
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
					ScrollRectItem cloneRefer=clone.GetComponent<ScrollRectItem>();
					repositionTileList.Add(cloneRefer);
					Vector3 scale=clone.transform.localScale;
					clone.transform.SetParent(this.transform);
					clone.transform.localScale=scale;
					this.lastEndIndex=i;
				}
				if(Cindex<this.pageSize)
				{
					ScrollRectItem tile=repositionTileList[Cindex];	
					this.preRenderList.Add(tile);
					repositionIntList.Add(i);	
					scrollDirection=0;
					preRender(tile,i);//Debug.Log(String.Format("preRefresh:{0}",i));
				}
			}
		}
		
	}
	
	void preLeftUp(int i)
	{
		if(i>=this.pageSize && !repositionIntList.Contains(i) && i<this.recordCount)
		{
			ScrollRectItem tile=repositionTileList[0];				
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
	{
		if(i>=0 && !repositionIntList.Contains(i) && i+pageSize<=recordCount) //i>pageSize)//
		{
			int end1=repositionTileList.Count-1;
			ScrollRectItem tile=repositionTileList[end1];
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
	
	void scroll(int newHead ,bool force)
	{
		if(newHead<0)newHead=0;
			
		int step=newHead-currFirstIndex;
		if((step!=0 && this.headIndex!=lastHeadIndex) || force)
		{
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
		if (moveContainer == null)
			moveContainer = this.GetComponent<RectTransform> ();

		if (moveContainer!=null)
		{
			Vector3 bg=moveContainer.transform.localPosition;
			if(direction== Direction.Down)
				beginPosition=new Vector3(bg.x,bg.y,bg.z);
			else
				beginPosition=new Vector3(bg.x,-bg.y,bg.z);

			foward = false;
		}
			
		CalcBounds();
		
		scroll(0,true);
	}
	
	void Update()
	{
		if(moveContainer!=null && data!=null)
		{
			currPosition=moveContainer.localPosition;
			dtmove=beginPosition-currPosition;

			if(columns==0 )
			{
				headIndex=(int)(dtmove.x/rect.width);
				scroll(headIndex,false);
			}else if(columns>0)
			{
				int cloumnIndex=(int)(dtmove.y/rect.height);
				headIndex= (int) Mathf.Ceil((float)(Mathf.Abs(cloumnIndex)*this.columns)/(float)this.columns)*columns;//
				if(headIndex!=lastHeadIndex)
				{
					scroll(headIndex,false);
				}
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

}

// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections.Generic;
[SLua.CustomLuaClass]
public class CQueueRequest  {
	
	public CQueueRequest()
	{
		queue= new List<CRequest>(); 
	}
	
	public void Add(CRequest req)
	{
		queue.Add(req);
		queue.Sort(CompareFunc);
	}
	
	static int CompareFunc (CRequest a, CRequest b)
	{
		if (a.priority <b.priority) return 1;
		if (a.priority > b.priority) return -1;
		return 0;
	} 
	
	public CRequest First()
	{
		if(queue.Count>0) 
		{
			CRequest f=queue[0];
			queue.RemoveAt(0);
			return f;
		}else
			return null;
	}
	
	public int Size()
	{
		return queue.Count;	
	}
	
	private List<CRequest> queue;

}

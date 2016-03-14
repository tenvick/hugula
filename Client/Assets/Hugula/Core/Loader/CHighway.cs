// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Threading;

/// <summary>
/// C highway.
/// </summary>
[SLua.CustomLuaClass]
public class CHighway
{

    public CHighway()
    {
        requestCallBackList = new Dictionary<string, List<CRequest>>();
        queue = new CQueueRequest();
        loader = new Dictionary<string, CRequest>();
        maxLoading = 2;
        loadingEvent = new HighwayEventArg();
        loaderPool = new List<CTransport>();
    }

    /// <summary>
    /// 将多个资源加载到本地并缓存。
    /// </summary>
    /// <param name="req"></param>
    public void LoadReq(IList<CRequest> req)//onAllCompleteHandle onAllCompletehandle=null,onProgressHandle onProgresshandle=null
    {
        for (int i = 0; i < req.Count; i++)
            AddReqToQueue(req[i]);
        BeginQueue();
    }

    /// <summary>
    /// 将多个资源加载到本地并缓存。
    /// </summary>
    /// <param name="req"></param>
    public void LoadReq(CRequest req)
    {
        AddReqToQueue(req);
        BeginQueue();
    }

    protected void AddReqToQueue(CRequest req)
    {
        string key = req.udKey;
        if (CacheManager.SetRequestDataFromCache(req)) //如果有缓存
        {
            req.DispatchComplete();
        }
        else if (requestCallBackList.ContainsKey(key))
        {
            requestCallBackList[key].Add(req);
        }
        else
        {
            requestCallBackList[key] = new List<CRequest>();
            requestCallBackList[key].Add(req);

            queue.Add(req);
            if (queue.Size() == 0 && currentLoading == 0)
            {
                totalLoading = 1;
                currentLoaded = 0;
            }
            else if (!req.isShared)
            {
                totalLoading++;
            }

            if (pushGroup && !req.isShared) //如果是一组
            {
                if (currGroupRequest == null)
                    currGroupRequest = _currGroupRequestRef;

                currGroupRequest.Add(req);
            }
        }
    }

    public void InitProgressState()
    {
        if (this.currentLoading <= 0)
        {
            totalLoading = 0;
            this.currentLoaded = 0;
            this.loadingEvent.current = currentLoaded;
            OnProcess(null, 0);
        }
    }

    protected void BeginQueue()
    {
        CRequest req1;
        while (this.currentLoading <= this.maxLoading && queue.Size() > 0)
        {
            req1 = queue.First();
            if (req1 != null && !LoadRequest(req1))
            {
                queue.Add(req1);//need reload
                break;
            }
        }
    }

    protected bool LoadRequest(CRequest req)
    {
        string key = req.udKey;
        CTransport load = this.GetFreeLoader(); // new GameObject();
        if (load != null)
        {
            req.times++;
            load.key = key;
            loader[key] = req;
            //#if UNITY_EDITOR
            //            			Debug.Log ("-----------beginLoad <<:" + req.key + ">>  shared=" + req.isShared + ",currentLoading=" + this.currentLoading + "  max=" + this.maxLoading);
            //#endif
            load.BeginLoad(req);
            return true;
        }
        else
        {
#if UNITY_EDITOR
            //Debug.Log ("-----------no free transport <<:" + req.key + ">>  shared=" + req.isShared + ",currentLoading=" + this.currentLoading + "  max=" + this.maxLoading);
#endif
        }
        return false;
    }

    protected void RemoveRequest(CRequest req)
    {
        string key = req.udKey;
        CRequest load = loader[key];
        loader.Remove(key);

    }

    protected void RemoveCallbacklist(CRequest creq)
    {
        if (requestCallBackList.ContainsKey(creq.udKey))
        {
            requestCallBackList.Remove(creq.udKey);
        }
    }

    protected void Callbacklist(CRequest creq)
    {
        //group
        if (currGroupRequest != null)
        {
            if (!currGroupRequest.Remove(creq))//加载完成
            {
                //Debug.LogWarning(string.Format("{0} group remove fail",creq.key));
            }
        }

        IList<CRequest> callbacklist = requestCallBackList[creq.udKey];
        if (callbacklist != null)
        {
            requestCallBackList.Remove(creq.udKey);
            int count = callbacklist.Count;
            object data = creq.data;
            CRequest reqitem;
            for (int i = 0; i < count; i++)
            {// reqitem in  callbacklist)
                reqitem = callbacklist[i];
                //CacheManager.SetRequestDataFromCache(reqitem);
                reqitem.data = data;
                reqitem.DispatchComplete();
            }
            callbacklist.Clear();
        }
    }

    #region interface

    protected void OnProcess(CTransport loader, float progress)
    {
        loadingEvent.target = this;
        if (totalLoading <= 0)
            loadingEvent.total = 1;
        else
            loadingEvent.total = totalLoading;
        if (loadingEvent.current < currentLoaded) loadingEvent.current = currentLoaded;
        loadingEvent.progress = progress;
        if (OnProgress != null && totalLoading > 0)
        {
            OnProgress(this, loadingEvent);
        }
    }


    /// <summary>
    /// Gets the free loader.
    /// </summary>
    /// <returns>The free loader.</returns>
    protected CTransport GetFreeLoader()
    {
        CTransport loader = null;

        if (loaderPool.Count < _maxLoading)
        {
            for (int i = loaderPool.Count; i < _maxLoading; i++)
            {
                var gobj = new GameObject("loader" + i);
                CTransport cts = gobj.AddComponent<CTransport>();
                loaderPool.Add(cts);
                cts.OnComplete = LoadComplete;
                cts.OnError = LoadError;
                cts.OnProcess = OnProcess;
            }
        }

        for (int i = 0; i < _maxLoading; i++)
        {
            loader = this.loaderPool[i];
            if (loader.isFree)
                return loader;
        }

        return null;
    }

    protected void OnDependencyComp(CRequest req)
    {
        CRequest childReq = req.childrenReq;
        CountMananger.Add(req.keyHashCode); //引用数量加1
        if (childReq != null)
        {
            childReq.dependenciesCount--;
            if (childReq.dependenciesCount <= 0)
            {
                CacheManager.SetRequestDataFromCache(childReq);
                if (childReq.isShared && OnSharedComplete != null) OnSharedComplete(childReq);
                Callbacklist(childReq);
            }
        }

        BeginQueue();

        CheckAllComplete();
    }

    protected virtual void LoadComplete(CTransport loader, CRequest creq, IList<CRequest> depens)
    {

        RemoveRequest(creq);

        if (depens != null) //if have depens
        {
            currentLoaded++;

            CRequest req1 = null;
            creq.dependenciesCount = depens.Count;

            for (int i = 0; i < depens.Count; i++) //被依赖项目
            {
                req1 = depens[i];
                req1.OnComplete += OnDependencyComp;
                req1.OnEnd += OnDependencyComp;
                req1.childrenReq = creq;
                if (CacheManager.SetRequestDataFromCache(req1))
                {
                    req1.DispatchComplete();
                    continue;
                }
                LoadReq(req1);
            }
        }
        else
        {

            CacheManager.SetRequestDataFromCache(creq);

            if (creq.isShared)
            {
                if (OnSharedComplete != null)
                    OnSharedComplete(creq);
            }
            else
            {
                currentLoaded++;
            }

            Callbacklist(creq);
            BeginQueue();
            CheckAllComplete();
        }

    }

    protected void CheckAllComplete()
    {
        //判断组是否完成
        if (currGroupRequest != null && currGroupRequest.Count == 0)
        {
            if (this.OnAllComplete != null)
                this.OnAllComplete(this);

            currGroupRequest = null;
        }else if (currentLoading <= 0 && queue.Size() == 0)
        {
            loadingEvent.target = this;
            loadingEvent.total = totalLoading;
            loadingEvent.progress = 1;
            loadingEvent.current = currentLoaded;
            loadingEvent.number = totalLoading;
            if (OnProgress != null && totalLoading > 0)
            {
                OnProgress(this, loadingEvent);
            }

            if (this.OnAllComplete != null)
                this.OnAllComplete(this);

            //Debug.Log("AllComplete"+currentLoading);
        }

    }

    protected void LoadError(CTransport cloader, CRequest creq)
    {
        CRequest req = cloader.req;

#if	UNITY_EDITOR
        Debug.LogWarning("load Error : times=" + req.times + " url=" + req.url + " key= " + req.key);
#endif
        RemoveRequest(req);
        RemoveCallbacklist(creq);

        if (req.times < 2)
        {
            req.priority = req.priority - 10;
            this.AddReqToQueue(req);
            this.BeginQueue();
        }
        else
        {
            req.DispatchEnd();
            BeginQueue();
            CheckAllComplete();
        }

    }

    #endregion

    protected CQueueRequest queue;

    protected IDictionary<string, List<CRequest>> requestCallBackList;
    /// <summary>
    /// loading
    /// </summary>
    protected IDictionary<string, CRequest> loader;

    /// <summary>
    /// 当前组加载请求
    /// </summary>
    protected HashSet<CRequest> currGroupRequest;// = new HashSet<CRequest>();

    private HashSet<CRequest> _currGroupRequestRef = new HashSet<CRequest>();

    #region memeber

    public int currentLoading
    {
        get
        {
            return loader.Keys.Count;
        }
    }

    private int _maxLoading;

    public int maxLoading
    {
        get
        {
            return _maxLoading;
        }
        protected set
        {
            _maxLoading = value;
        }
    }

    private int _totalLoading = 0;

    public int totalLoading
    {
        get
        {
            return _totalLoading;
        }
        set
        {
            _totalLoading = value;
        }
    }
    private int _currentLoaded = 0;

    public int currentLoaded
    {
        get
        {
            return _currentLoaded;
        }
        protected set
        {
            _currentLoaded = value;
        }
    }

    protected HighwayEventArg loadingEvent;
    protected IList<CTransport> loaderPool;
    
    /// <summary>
    /// 将接下来的所有添加的加载请求放入一组，完成后触发on_allComplete事件
    /// </summary>
    public bool pushGroup = false;
    #endregion

    #region  delegate and event

    public event System.Action<CHighway> OnAllComplete;
    public event System.Action<CHighway, HighwayEventArg> OnProgress;
    public event System.Action<CRequest> OnSharedComplete;
    #endregion

    private static CHighway _instance;

    public static CHighway GetInstance()
    {
        if (_instance == null)
            _instance = new CHighway();
        return _instance;
    }

}
[SLua.CustomLuaClass]
public class HighwayEventArg
{
    //public object loader;
    public int number;//current loading number
    public object target;
    public int total;
    public int current;
    public float progress;
}

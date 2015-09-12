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

    /**
   * 将多个资源加载到本地，不放入资源缓存。</br>
   * 参数:</br>
   * <b>req:Array</b> 请求的队列Request类型  </br>
   * <b>onComplete:Function(e:Multiple):void</b> 全部加载完成时回调函数  </br>
   * <b>onProgress：Function(e:ProgressEvent):void</b> 加载进度调用函数  </br>
   */
    public void LoadReq(IList<CRequest> req)//onAllCompleteHandle onAllCompletehandle=null,onProgressHandle onProgresshandle=null
    {
        for (int i = 0; i < req.Count; i++)
            AddReqToQueue(req[i]);
        BeginQueue();
    }

    public void LoadReq(CRequest req)
    {
        AddReqToQueue(req);
        BeginQueue();
    }

    protected void AddReqToQueue(CRequest req)
    {
        string key = req.udKey;
        if (requestCallBackList.ContainsKey(key))
        {
            requestCallBackList[key].Add(req);
        }
        else
        {
            requestCallBackList[key] = new List<CRequest>();
            requestCallBackList[key].Add(req);
            //			Debug.Log("requestCallBackList:"+req.key+"  "+req.udKey);
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
        }
    }

    public void InitProgressState()
    {
        if (this.currentLoading <= 0)
        {
            totalLoading = 0;
            this.currentLoaded = 0;
            this.loadingEvent.current = currentLoaded;
        }
        else
        {
            //this.totalLoading=totalLoading+this.queue.size();
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
                //				Debug.Log("  no free transport:  "+req1.key);
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
#if UNITY_EDITOR
            //			Debug.Log ("-----------beginLoad <<:" + req.key + ">>  shared=" + req.isShared + ",currentLoading=" + this.currentLoading + "  max=" + this.maxLoading);
#endif
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
        //		load.Dispose();
        //		load=null;
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
                //				Debug.Log(gobj.name+" created ");
            }
        }

        for (int i = 0; i < _maxLoading; i++)
        {
            loader = this.loaderPool[i];
            //			Debug.Log(loader.name+" isFree =  "+ loader.isFree +" ");
            if (loader.isFree)
                return loader;
        }

        return null;
    }

    protected void OnDependencyComp(CRequest req)
    {
        CRequest childReq = req.childrenReq;
        if (childReq != null)
        {
            childReq.dependenciesCount--;
            if (childReq.dependenciesCount <= 0)
            {
                object data = SetReqDataFromWWW(childReq, childReq.www);
                if (childReq.cache || childReq.isShared) SetCache(childReq.key, data);
                //Debug.Log("_______loadComplete <<" + childReq.key + ">>  depens:" + req.key + "  count:" + childReq.dependenciesCount.ToString());
                Callbacklist(childReq);
            }
        }
        //Debug.Log("OnDependencyComp  <<" + req.key + ">>  is complete ");

        BeginQueue();

        CheckAllComplete();
    }

    protected virtual void LoadComplete(CTransport loader, CRequest creq, IList<CRequest> depens)
    {
//#if UNITY_EDITOR
//        if (creq.isShared)
//            Debug.Log("_______loadComplete  <<" + creq.key + ">> is Shared  ");
//        else
//            Debug.Log("______loadComplete  <<" + creq.key + ">>  depens:" + (depens != null).ToString());
//#endif
        RemoveRequest(creq);

        if (depens != null) //if have depens
        {
            currentLoaded++;

            CRequest req1 = null;
            creq.dependenciesCount = depens.Count;
            string key = string.Empty;

            //Debug.Log("______begin load<" + creq.key + "> and  depens :" + depens.Count.ToString());
            for (int i = 0; i < depens.Count; i++)
            {
                req1 = depens[i];
                req1.OnComplete += OnDependencyComp;
                req1.OnEnd += OnDependencyComp;
                req1.childrenReq = creq;
                key = req1.key;
                object cacheData = GetCache(key);
                if (cacheData != null)
                {
                    SetReqDataFromData(req1, cacheData);
                    req1.DispatchComplete();
                    continue;
                }
                LoadReq(req1);
            }
        }
        else
        {
            //Debug.Log("______loadComplete:<" + creq.key + "> ");

            object data = SetReqDataFromWWW(creq, creq.www);

            if (creq.isShared)
            {
                if (OnSharedComplete != null)
                    OnSharedComplete(creq);
            }
            else if (creq.cache)
            {
                SetCache(creq.key, data);
                currentLoaded++;
            }

            Callbacklist(creq);
            BeginQueue();
            CheckAllComplete();
        }

    }

    protected void CheckAllComplete()
    {
        if (currentLoading <= 0 && queue.Size() == 0)
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
            //initProgressState();
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


    protected virtual object GetCache(string Key)
    {
        if (this._cache != null && this._cache.ContainsKey(Key))
            return _cache[Key];
        else
            return null;
    }

    protected virtual void SetCache(string key, object Value)
    {
        if (this._cache != null)
            this._cache[key] = Value;
    }

    protected virtual object SetReqDataFromWWW(CRequest req, WWW www)
    {
        object re = null;
        AssetBundle abundle = www.assetBundle;
        System.Type assetType = LuaHelper.GetType(req.assetType);
        if (assetType == null) assetType = typeof(UnityEngine.Object);

        if (assetType.Equals(typeof(System.String)))
        {
            req.data = new string[] { www.text };
            req.assetBundle = null;
            re = req.data;
        }
        else if (assetType.Equals(typeof(System.Byte[])))
        {
            req.data = www.bytes;
            req.assetBundle = null;
            re = req.data;
        }
        else if (!assetType.IsArray)
        {
            re = req.assetBundle;
            req.data = req.assetBundle.LoadAsset(req.assetName, assetType);
        }
        else if (assetType.IsArray)
        {
            req.data = req.assetBundle.LoadAllAssets(assetType);
            re = req.assetBundle;
        }
        www.Dispose();
        return re;
    }

    public static void SetReqDataFromData(CRequest req, object data)
    {
        System.Type assetType = LuaHelper.GetType(req.assetType);
        if (assetType == null) assetType = typeof(GameObject);
        if (data is AssetBundle)
        {
            req.assetBundle = data as AssetBundle;
            if (!assetType.IsArray)
            {
                //Debug.Log(req.key + "assetName: " + req.assetName);
                req.data = req.assetBundle.LoadAsset(req.assetName, assetType);
            }
            else
            {
                req.data = req.assetBundle.LoadAllAssets(assetType);
            }
        }
        else if (data.GetType() == typeof(System.String))
        {
            req.data = data;
        }
        else if (data.GetType() == typeof(System.Byte[]))
        {
            req.data = data;
        }
    }

    #endregion

    protected CQueueRequest queue;

    protected IDictionary<string, List<CRequest>> requestCallBackList;
    /// <summary>
    /// loading
    /// </summary>
    protected IDictionary<string, CRequest> loader;

    #region memeber

    //	public int currentWillLoading{private set;get;}	

    //	protected int _currentLoading;

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

    private IDictionary<string, object> _cache;

    public virtual object cache
    {
        get { return _cache; }
        set
        {
            if (value is IDictionary)
                _cache = (IDictionary<string, object>)value;
        }
    }

    #endregion

    #region  delegate and event

    public event System.Action<CHighway> OnAllComplete;
    public event System.Action<CHighway, HighwayEventArg> OnProgress;
    public event System.Action<CRequest> OnSharedComplete;
    //	public event System.Action<CRequest> OnCache;
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

//public delegate void OnAllCompleteHandle(MultipleLoader loader);
//public delegate void OnProgressHandle(MultipleLoader loader,LoaderEventArg arg);
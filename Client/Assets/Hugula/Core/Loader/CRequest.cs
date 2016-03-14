// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Request.
/// </summary>
[SLua.CustomLuaClass]
public class CRequest // IDispose
{
    /// <summary>
    /// 加载请求 
    /// </summary>
    /// <param name="url"></param>
    public CRequest(string url)
    {
        this.url = url;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">地址 完整地址</param>
    /// <param name="assetName">加载的资源名</param>
    /// <param name="assetType">资源类型，默认为GameObject</param>
    public CRequest(string url, string assetName, string assetType)
    {
        this.url = url;
        this.assetName = assetName;
        this.assetType = assetType;
    }

    public void Dispose()
    {
        this.url = null;
        this.priority = 0;
        this.key = null;
        this.suffix = null;
        _data = null;
        _head = null;
    }

    private object _data;

    private object _head;

    private string _key, _udKey;

    private int _keyHashCode;

    private string _url;

    private string _suffix = string.Empty;

    private string _assetBundleName = string.Empty;

    private string _assetName = string.Empty;

    private bool _cache = false;

    /// <summary>
    /// 文件后缀类型</br>
    /// * 默认根据url后缀来生成</br>
    /// * 如果设定以设定值为准.解码的时候是根据type来解码的.例如 解码后成相应的对象类型
    /// </summary>
    public string suffix
    {
        get
        {
            if (string.IsNullOrEmpty(_suffix))
                _suffix = CUtils.GetURLFileSuffix(_url);
            return _suffix;
        }
        set
        {
            _suffix = value;
        }
    }

    /// <summary>
    /// assetbundleName 根据url计算出来
    /// </summary>
    public string assetBundleName
    {
        get
        {
            if (string.IsNullOrEmpty(_assetBundleName))
                _assetBundleName = CUtils.GetURLFullFileName(_url);
            return _assetBundleName;
        }
        set
        {
            _assetBundleName = value;
        }
    }

    /// <summary>
    /// 要加载的asset 名称
    /// </summary>
    public string assetName
    {
        get
        {
            if (string.IsNullOrEmpty(_assetName)) _assetName = key;
            return _assetName;
        }
        set { _assetName = value; }
    }

    /// <summary>
    /// asset Type name
    /// </summary>
    public string assetType = string.Empty;

    /// <summary>
    /// 加载的头信息
    /// </summary>
    public object head
    {
        get { return this._head; }
        set { this._head = value; }
    }


    /// <summary>
    /// 加载的数据
    /// </summary>
    public object data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
        }
    }

    ///// <summary>
    ///// assetBundle
    ///// </summary>
    //public AssetBundle assetBundle;
    ///// <summary>
    ///// www对象
    ///// </summary>
    //public WWW www;

    /// <summary>
    /// The user data.
    /// </summary>
    public object userData;

    public event CompleteHandle OnEnd;

    public event CompleteHandle OnComplete;

    public void DispatchComplete()
    {
        if (OnComplete != null)
            OnComplete(this);
    }

    public void DispatchEnd()
    {
        if (OnEnd != null)
            OnEnd(this);
    }

    public bool isShared { get;internal set; }

    /// <summary>
    /// 请求地址 网址，相对路径，绝对路径
    /// </summary>
    public string url
    {
        get
        {
            return _url;
        }
        set
        {
            _url = value;
        }
    }

    /// <summary>
    /// 缓存用的关键字
    /// 如果没有设定 则为默认key生成规则
    /// </summary>
    public string key
    {
        get
        {
            if (string.IsNullOrEmpty(_key))
                _key = CUtils.GetKeyURLFileName(url);
            return _key;
        }
        set
        {
            _key = value;
        }
    }

    /// <summary>
    /// assetBundle key的hashcode 用于计算缓存的资源key
    /// </summary>
    public int keyHashCode
    {
        get
        {
            if(_keyHashCode == 0 )
                _keyHashCode = LuaHelper.StringToHash(key);
            return _keyHashCode;
        }
        set
        {
            _keyHashCode = value;
        }
    }

    /// <summary>
    /// Sets the U dkey.
    /// </summary>
    /// <value>
    /// The U dkey.
    /// </value>
    public string udKey
    {
        get
        {
            if (string.IsNullOrEmpty(_udKey))
                _udKey = CryptographHelper.CrypfString(this.url);//_key=CUtils.getURLFullFileName(url);		    	
            return _udKey;
        }
        set
        {
            _udKey = value;
        }
    }


    /// <summary>
    /// 缓存 不需要了 暂时保留
    /// </summary>
    public bool cache
    {
        get
        {
            return _cache;
        }
        set
        {
            _cache = value;
        }
    }

    /// <summary>
    ///  优先等级
    ///  降序</br>
    /// <b>priority:int</b> 优先等级  值越大优先级越高
    /// </summary>
    public int priority = 0;//优先级

    /// <summary>
    /// 加载次数
    /// 加载失败的时候记录
    /// </summary>
    public int times = 0;

    /// <summary>
    /// 被依赖的对象
    /// </summary>
    public CRequest childrenReq;

    /// <summary>
    /// dependencies count;
    /// </summary>
    public int dependenciesCount;
}

public delegate void CompleteHandle(CRequest req);

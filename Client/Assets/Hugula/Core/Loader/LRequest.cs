// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using SLua;
[SLua.CustomLuaClass]
public class LRequest : CRequest {

    public  LRequest(string url) :base(url)
    {
        this.OnComplete += OnCompHandler;
        this.OnEnd += OnEndHandler;
    }

    public LRequest(string url, string assetName, string assetType)
        : base(url, assetName, assetType)
    {
        this.OnComplete += OnCompHandler;
        this.OnEnd += OnEndHandler;
    }

    private void OnCompHandler(CRequest req)
    {
        if (onCompleteFn != null)
            onCompleteFn.call(req);
    }

    private void OnEndHandler(CRequest req)
    {
        if (onEndFn != null)
            onEndFn.call(req);
    }

    public LuaFunction onCompleteFn;

    public LuaFunction onEndFn;

}

// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using SLua;

namespace Hugula.Loader
{
    /// <summary>
    /// 给lua使用的request
    /// </summary>
    [SLua.CustomLuaClass]
    public class LRequest : CRequest
    {

        public LRequest(string url)
            : base(url)
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
}
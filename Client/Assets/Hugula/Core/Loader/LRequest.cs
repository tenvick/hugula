// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using Hugula.Utils;
using System;
using SLua;

namespace Hugula.Loader
{
    /// <summary>
    /// 给lua使用的request
    /// </summary>
    [SLua.CustomLuaClass]
    public sealed class LRequest : CRequest
    {
        public LRequest():base()
        {
            BindAction();
        }

        public LRequest(string url)
            : base(url)
        {
            BindAction();
        }

		public LRequest(string url, string assetName, Type assetType)
            : base(url, assetName, assetType)
        {
            BindAction();
        }

        internal void BindAction()
        {
            this.OnComplete = OnCompHandler;
            this.OnEnd = OnEndHandler;
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

        public override void Dispose()
        {
            base.Dispose();
            onCompleteFn = null;
            onEndFn = null;
        }

        #region ObjectPool 
        static ObjectPool<LRequest> objectPool = new ObjectPool<LRequest>(m_ActionOnGet, m_ActionOnRelease);
        private static void m_ActionOnGet(LRequest req)
        {
            req.pool = true;
            req.BindAction();
        }

        private static void m_ActionOnRelease(LRequest req)
        {
            req.Dispose();
        }

        public static LRequest Get()
        {
            return objectPool.Get();
        }

        public static void Release(CRequest toRelease)
        {
            if(toRelease is LRequest)
                objectPool.Release((LRequest)toRelease);
        }
        #endregion
    }

}
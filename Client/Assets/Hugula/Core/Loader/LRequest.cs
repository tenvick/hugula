// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using Hugula.Utils;
using SLua;

namespace Hugula.Loader
{
    /// <summary>
    /// 给lua使用的request
    /// </summary>
    [SLua.CustomLuaClass]
    public class LRequest : CRequest
    {
        public LRequest():base()
        {
            this.OnComplete += OnCompHandler;
            this.OnEnd += OnEndHandler;
        }

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

        public override void Dispose()
        {
            base.Dispose();
            onCompleteFn = null;
            onEndFn = null;
        }
    }


    [SLua.CustomLuaClass]
    public class LRequestPool
    {
        static ObjectPool<LRequest> pool = new ObjectPool<LRequest>(m_ActionOnGet, m_ActionOnRelease);

        static public int countAll { get{return pool.countAll;} }
        static public int countActive { get { return countAll - countInactive; } }
        static public int countInactive { get { return pool.countInactive; } }

        private static void m_ActionOnGet(LRequest req)
        {
            req.pool = true;
        }

        private static void m_ActionOnRelease(LRequest req)
        {
            req.Dispose();
        }

        public static LRequest Get()
        {
            return pool.Get();
        }

        public static void Release(CRequest toRelease)
        {
            if(toRelease is LRequest)
                pool.Release((LRequest)toRelease);
        }
    }

}
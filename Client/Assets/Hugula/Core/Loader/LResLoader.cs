// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using SLua;

namespace Hugula.Loader
{
    /// <summary>
    /// 给lua使用的CResLoader
    /// </summary>
    [SLua.CustomLuaClass]
    public sealed class LResLoader : CResLoader
    {
        // Use this for initialization
        void Start()
        {
            base.OnAllComplete += L_onAllComplete;
            base.OnProgress += L_onProgress;
            base.OnSharedComplete += L_onSharedComplete;
			base.OnSharedErr += L_onSharedErr;
			base.OnAssetBundleComplete += L_OnAssetBundleComplete;
			base.OnAssetBundleErr += L_OnAssetBundleErrFn;
        }

        /// <summary>
        /// 加载luatable里面的request
        /// </summary>
        /// <param name="reqs"></param>
		public void LoadLuaTable(LuaTable reqs, System.Action<object> groupCompleteFn,System.Action<LoadingEventArg> groupProgressFn)
        {
            GroupRequestRecord re = null;

            if (groupCompleteFn != null)
            {
                re = GroupRequestRecordPool.Get();
                re.onGroupComplate = groupCompleteFn;
				re.onGroupProgress = groupProgressFn;
            }

            foreach (var pair in reqs)
            {
                AddReqToQueue((CRequest)pair.value, re);
            }

            BeginQueue();
        }

        #region protected method

        void L_onSharedComplete(CRequest req)
        {
            if (onSharedCompleteFn != null)
                onSharedCompleteFn.call(req);
        }

        void L_onProgress(LoadingEventArg arg)
        {
            if (onProgressFn != null)
                onProgressFn.call(arg);
        }

        void L_onAllComplete(CResLoader loader)
        {
            if (onAllCompleteFn != null)
                onAllCompleteFn.call(loader);
        }

		void L_onSharedErr(CRequest req)
		{
			if (onSharedErrFn != null)
				onSharedErrFn.call(req);
		}

		void L_OnAssetBundleComplete(CRequest req)
		{
			if (onAssetBundleCompleteFn != null)
				onAssetBundleCompleteFn.call (req);
		}

		void L_OnAssetBundleErrFn(CRequest req)
		{
			if (onAssetBundleErrFn != null)
				onAssetBundleErrFn.call (req);
		}

        #endregion

		public void RemoveAllEvents()
		{
			onAllCompleteFn = null;
			onProgressFn = null;
			onSharedCompleteFn = null;
			onCacheFn = null;
			onSharedErrFn = null;
			onAssetBundleCompleteFn = null;
			onAssetBundleErrFn = null;
		}

        #region  delegate and event
        public LuaFunction onAllCompleteFn;
        public LuaFunction onProgressFn;
        public LuaFunction onSharedCompleteFn;
        public LuaFunction onCacheFn;
		public LuaFunction onSharedErrFn;
		public LuaFunction onAssetBundleCompleteFn;
		public LuaFunction onAssetBundleErrFn;
		#endregion

        #region instance
        //protected static LResLoader _instance;
        /// <summary>
        /// the GetInstance
        /// </summary>
        /// <returns></returns>
        public static CResLoader instance
        {
            get
            {
                if (_instance == null)
                {
                    var chighway = new GameObject("CResManager");
                    _instance = chighway.AddComponent<LResLoader>();
                }
                return _instance;
            }
        }
        #endregion
    }
}
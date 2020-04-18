// Copyright (c) 2019 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using Hugula.Pool;
using Hugula.Utils;

namespace Hugula.Loader
{

    public static class OperationPools<T> where T : IReset, new()
    {
        private static readonly ObjectPool<T> s_Pools = new ObjectPool<T>(null, m_ActionOnRelease);

        private static void m_ActionOnGet(T op)
        {

        }

        private static void m_ActionOnRelease(T op)
        {
            op.Reset();
        }

        public static T Get()
        {
            return s_Pools.Get();
        }

        public static void Release(T toRelease)
        {
            s_Pools.Release(toRelease);
        }
    }

    /// <summary>
    /// 加载assetbundle实现类
    /// </summary>
    public sealed class BundleOperation : IReset
    {
        public string assetBundleName;
        AssetBundleCreateRequest m_abRequest;
        bool m_start = false;
        string url;

        public void Start()
        {
            m_start = true;
            var cacheData = CacheManager.TryGetCache(assetBundleName);
            if (!cacheData.canUse) //判断ab是否已经加载防止同步加载
            {
                url = ResourcesLoader.GetAssetBundleDownloadingURL(assetBundleName); // set full url
                url = CUtils.GetAndroidABLoadPath(url);
                m_abRequest = AssetBundle.LoadFromFileAsync(url);
                CacheManager.SetCacheDataLoding(assetBundleName);//修改为loading状态
            }
        }

        public bool Update()
        {
            if (!m_start) Start();

            if (m_abRequest == null) return false;

            bool isdone = m_abRequest.isDone;

            if (isdone) m_Done(m_abRequest.assetBundle);

            if (!CacheManager.CheckDependenciesComplete(assetBundleName)) // && Time.frameCount - frameBegin >= timeOutFrame)
                return true; //wait

            return !isdone;
        }

        public void Reset()
        {
            m_start = false;
            m_abRequest = null;
            assetBundleName = null;
            url = null;
        }

        //同步加载
        internal void StartSync()
        {
            m_start = true;
            url = ResourcesLoader.GetAssetBundleDownloadingURL(assetBundleName); // set full url
            url = CUtils.GetAndroidABLoadPath(url);
            var assetBundle = AssetBundle.LoadFromFile(url);
            m_Done(assetBundle);
        }

        private void m_Done(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                string error = string.Format("the asset bundle({0}) is error. CRequest({1})", assetBundleName, url);
                CacheManager.AddErrorCacheData(assetBundleName);
                Debug.LogError(error);
            }
            else
            {
                CacheManager.AddCacheData(assetBundle, assetBundleName);
            }

            m_abRequest = null;

        }

        public void ReleaseToPool()
        {
            OperationPools<BundleOperation>.Release(this);
        }
    }

    /// <summary>
    /// 加载assetbundle实现类
    /// </summary>
    public class AssetOperation : IReset
    {
        public CRequest request;

        public int id { get; private set; }//标识id用于取消完成回调

        internal LoadSceneMode loadSceneMode = LoadSceneMode.Additive;

        internal bool allowSceneActivation = true;//

        internal bool subAssets = false;//加载subassets

        protected CacheData m_Bundle;
        protected AsyncOperation m_Request = null;
#if UNITY_EDITOR
        protected bool m_isDone = false; //标记完成用于编辑器 Simulate 模式
#endif
        internal void StartSync()
        {
            if (m_Bundle == null) m_Bundle = CacheManager.TryGetCache(request.assetBundleName);
            if (!m_Bundle.canUse) //ab失效
            {
                CRequest.SetError(request, string.Format("load asset({0}) from bundle({1})  error", request.assetName, request.assetBundleName));
                Debug.LogError(request.error);
            }
            else
            {
                string assetName = request.assetName;
                var typ = request.assetType;

                if (LoaderType.Typeof_ABScene.Equals(typ))
                {
                    CRequest.SetError(request, string.Format("cant't load scene asset({0}) from bundle({1}) in sync mode", request.assetName, request.assetBundleName));
                    Debug.LogError(request.error);
                }
                else
                {
                    if (subAssets)
                    {
                        object data = m_Bundle.assetBundle.LoadAssetWithSubAssets(assetName, typ);
                        CRequest.SetData(request, data);
                    }
                    else
                    {
                        object data = m_Bundle.assetBundle.LoadAsset(assetName, typ);
                        CRequest.SetData(request, data);
                    }
                }
            }
        }

        public bool Update()
        {
            if (m_Request != null)
            {
                CRequest.SetProgress(request, m_Request.progress);
                if (!allowSceneActivation && LoaderType.Typeof_ABScene.Equals(request.assetType) && m_Request.progress >= 0.9f)//加载场景的时候如果allowSceneActivation = false 只能通过progress判断完成
                {
                    return false;
                }
                else
                    return !m_Request.isDone;
            }

#if UNITY_EDITOR
            if (m_isDone) return false; //only for editor 模拟模式使用
#endif
            if (m_Bundle == null) m_Bundle = CacheManager.TryGetCache(request.assetBundleName);

            if (m_Bundle == null || !m_Bundle.isDone /* || !CacheManager.CheckDependenciesComplete(request.assetBundleName)*/) return true; //wait bundle done

            if (!m_Bundle.canUse) //ab失效
            {
                CRequest.SetError(request, string.Format("load asset({0}) from bundle({1}).canUse = false  error", request.assetName, request.assetBundleName));
                Debug.LogError(request.error);
                return false;
            }
            else
            {
                string assetName = request.assetName;
                var typ = request.assetType;

                if (LoaderType.Typeof_ABScene.Equals(typ))
                {
                    m_Request = SceneManager.LoadSceneAsync(assetName, loadSceneMode);
                    m_Request.allowSceneActivation = allowSceneActivation;
                    CRequest.SetData(request, m_Request);//加载场景比较特殊 提前返回AsyncOperation对象方便操作
                    CacheManager.AddScene(request.assetName, request.assetBundleName);//缓存场景
                    if (!allowSceneActivation) CacheManager.AddLoadingScene(request.assetName, m_Request);
                }
                else if (subAssets)
                    m_Request = m_Bundle.assetBundle.LoadAssetWithSubAssetsAsync(assetName, typ);
                else
                    m_Request = m_Bundle.assetBundle.LoadAssetAsync(assetName, typ);
                // #if HUGULA_LOADER_DEBUG
                //                 // HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1.2 Asset  Request(assetName={0}) is done={1} key={2},frame={3} </color>", cRequest.assetName, m_Request.isDone, cRequest.key, Time.frameCount);
                // #endif
                // GS_GameLog.LogFormat("LoadAssetAsync({0},{1}) m_Request.isDone={2}", assetName, typ, m_Request.isDone);
                CRequest.SetProgress(request, m_Request.progress);

                return !m_Request.isDone;
            }
        }

        public void Reset()
        {
            m_Request = null;
            request = null;
            m_Bundle = null;
            subAssets = false;
            allowSceneActivation = true;
            id = 0;
#if UNITY_EDITOR
            m_isDone = false; //标记完成用于编辑器 Simulate 模式
#endif
        }

        public void Done()
        {
            if (request.error != null) return; //报错或者加载的是场景

            if (LoaderType.Typeof_ABScene.Equals(request.assetType))
            {
                CRequest.SetDone(request);
                return;
            }


            object m_Data = null;
            if (subAssets)
                m_Data = ((AssetBundleRequest)m_Request).allAssets;
            else
                m_Data = ((AssetBundleRequest)m_Request).asset;

            if (m_Data == null)
            {
                CRequest.SetError(request, string.Format("load asset({0}) from {1}  error, subAssets = {2}", request.assetName, request.assetBundleName, subAssets));
                Debug.LogError(request.error);
            }
            else
            {
                CRequest.SetData(request, m_Data);
                CRequest.SetDone(request);
            }
        }

        public void ReleaseToPool()
        {
            OperationPools<AssetOperation>.Release(this);
        }

        //设置ID
        public static void SetId(AssetOperation assetOp)
        {
            assetOp.id = M_ID;
            M_ID++;
            if (M_ID == int.MaxValue) M_ID = 1;
        }

        static int M_ID = 1;
    }


#if UNITY_EDITOR

    //   模拟加载
    public class AssetOperationSimulation : AssetOperation
    {
        //同步加载asset
        internal new void StartSync()
        {
            string assetName = request.assetName;
            var assetType = request.assetType;

            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(request.assetBundleName, request.assetName);
            if (assetPaths.Length == 0)
            {
                CRequest.SetError(request, "There is no asset path exist  !  \"" + request.assetName + "\" in " + request.assetBundleName);
                Debug.LogError(request.error);
            }

            object data = null;
            foreach (var p in assetPaths)
            {
                if (subAssets)
                    data = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(p);
                else
                    data = UnityEditor.AssetDatabase.LoadAssetAtPath(p, assetType);
                // GS_GameLog.LogFormat("LoadAssetAtPath({1},{2}).data={0} ", data, p, assetType);
                if (data != null && data.GetType().Equals(assetType))
                    break;
            }

            if (data == null)
            {
                CRequest.SetError(request, "There is no asset data  . \"" + request.assetName + "\" in " + request.assetBundleName);
                Debug.LogError(request.error);
            }

            CRequest.SetProgress(request, 1f);
            CRequest.SetData(request, data);
            CRequest.SetDone(request);
        }

        public new bool Update()
        {
            if (m_Request != null)
            {
                CRequest.SetProgress(request, m_Request.progress);
                if (!allowSceneActivation && LoaderType.Typeof_ABScene.Equals(request.assetType) && m_Request.progress >= 0.9f)//加载场景的时候如果allowSceneActivation = false 只能通过progress判断完成
                    return false;
                else
                    return !m_Request.isDone;
            }

            string assetName = request.assetName;
            string assetBundleName = request.assetBundleName;

            if (LoaderType.Typeof_ABScene.Equals(request.assetType)) //加载场景
            {
                var levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);

                if (levelPaths.Length == 0)
                {
                    CRequest.SetError(request, string.Format("There is no scene with name \"" + assetName + "\" in " + assetBundleName));
                    Debug.LogError(request.error);
                }
                else
                {
                    LoadSceneParameters loadSceneParameters = new LoadSceneParameters();
                    loadSceneParameters.loadSceneMode = loadSceneMode;
                    m_Request = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(levelPaths[0], loadSceneParameters);
                    m_Request.allowSceneActivation = allowSceneActivation;
                    CRequest.SetData(request, m_Request);
                    CRequest.SetProgress(request, m_Request.progress);
                    CacheManager.AddScene(request.assetName, request.assetBundleName);//缓存场景
                    if (!allowSceneActivation) CacheManager.AddLoadingScene(request.assetName, m_Request);
                }
            }
            else //加载资源
            {
                StartSync();
                m_isDone = true;//标记完成
                return false;
            }

            return false;
        }

        public new void Done()
        {

        }

        public new void ReleaseToPool()
        {
            OperationPools<AssetOperation>.Release(this);
        }
    }


#endif

    public class LoadingEventArg : System.ComponentModel.ProgressChangedEventArgs
    {
        //public int number;//current loading number
        public object target
        {
            get;
            internal set;
        }
        public long total
        {
            get;
            internal set;
        }
        public long current
        {
            get;
            internal set;
        }

        public float progress;

        public LoadingEventArg() : base(0, null)
        {

        }

        public LoadingEventArg(long bytesReceived, long totalBytesToReceive, object userState) : base((totalBytesToReceive == -1L) ? 0 : ((int)(bytesReceived * 100L / totalBytesToReceive)), userState)
        {
            this.current = bytesReceived;
            this.total = totalBytesToReceive;
            this.target = userState;
        }
    }

    public class LoaderType
    {
        #region check type
        public static readonly Type Typeof_String = typeof(System.String);
        public static readonly Type Typeof_Bytes = typeof(System.Byte[]);
        public static readonly Type Typeof_ABScene = typeof(AssetBundleScene);
        // public static readonly Type Typeof_ABAllAssets = typeof(UnityEngine.Object[]);
        public static readonly Type Typeof_AudioClip = typeof(AudioClip);
        public static readonly Type Typeof_Texture2D = typeof(Texture2D);
        public static readonly Type Typeof_Object = typeof(UnityEngine.Object);

        // LoadAssetWithSubAssetsAsync
        #endregion
    }

    public class AssetBundleScene
    { }
}

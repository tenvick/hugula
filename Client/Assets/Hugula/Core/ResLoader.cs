// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Hugula
{

    [XLua.LuaCallCSharp]
    public sealed class ResLoader
    {
        #region  初始化

        static bool s_Initialized = false;

        public static bool Ready
        {
            get { return s_Initialized; }
        }

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            m_LoadedScenes = new Dictionary<string, SceneInstance>();
            m_LoadingEvent = new LoadingEventArg();
            Addressables.InitializeAsync().Completed += InitDone;
        }

        static void InitDone(AsyncOperationHandle<IResourceLocator> obj)
        {
            s_Initialized = true;
        }

        #endregion

        #region  group  
        //标注为一个组加载
        static bool m_MarkGroup = false;
        static int m_TotalGroupCount = 0;
        static int m_CurrGroupLoaded = 0;

        static List<string> m_Groupes = new List<string>();
        static LoadingEventArg m_LoadingEvent; //事件进度

        public static System.Action OnGroupComplete; //group加载完成回调
        public static System.Action<LoadingEventArg> OnGroupProgress; //当前group进度

        static public void BeginMarkGroup()
        {
            m_MarkGroup = true;
        }

        static public void EndMarkGroup()
        {
            m_MarkGroup = false;
            if (OnGroupComplete != null && m_TotalGroupCount <= m_CurrGroupLoaded && m_Groupes.Count == 0)
            {
                m_TotalGroupCount = 0;
                m_CurrGroupLoaded = 0;
                OnGroupComplete();
            }
        }

        static void OnItemLoaded(string key)
        {
            if (OnGroupProgress != null && m_TotalGroupCount > 0)
            {
                int g_Idx = m_Groupes.IndexOf(key);
                if (g_Idx >= 0)
                {
                    m_Groupes.RemoveAt(g_Idx);
                    m_CurrGroupLoaded++;
                }
                m_LoadingEvent.total = m_TotalGroupCount;
                m_LoadingEvent.current = m_CurrGroupLoaded;
                m_LoadingEvent.progress = (float)m_LoadingEvent.current / (float)m_LoadingEvent.total;
                OnGroupProgress(m_LoadingEvent);
            }

            if (OnGroupComplete != null && m_TotalGroupCount <= m_CurrGroupLoaded && !m_MarkGroup && m_Groupes.Count == 0)
            {
                m_TotalGroupCount = 0;
                m_CurrGroupLoaded = 0;
                OnGroupComplete();
            }
        }

        #endregion


        #region  asset 加载
        /// <summary>
        /// 以同步方式加载ab资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public T LoadAsset<T>(string key)
        {
            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd yet!");

            var op = Addressables.LoadAssetAsync<T>(key);


            if (!op.IsDone)
                throw new Exception("Sync LoadAsset failed to load in a sync way! " + key);

            if (op.Result == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                if (op.OperationException != null)
                    message += " Exception: " + op.OperationException;

                throw new Exception(message);
            }

            return op.Result;
        }

        /// <summary>
        /// 以同步方式加载ab资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        static public UnityEngine.Object LoadAsset(string key, Type assetType)
        {
            if (assetType == null) assetType = TypeHelper.Object;
            if (assetType.Equals(TypeHelper.GameObject))
                return LoadAsset<UnityEngine.GameObject>(key);
            else if (assetType.Equals(TypeHelper.Sprite))
                return LoadAsset<UnityEngine.Sprite>(key);
            else if (assetType.Equals(TypeHelper.TextAsset))
                return LoadAsset<UnityEngine.TextAsset>(key);
            else if (assetType.Equals(TypeHelper.Texture))
                return LoadAsset<UnityEngine.Texture>(key);
            else
            {
                return LoadAsset<UnityEngine.Object>(key);
            }
        }

        static public UnityEngine.GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);

            if (!op.IsDone)
                throw new Exception("Sync Instantiate failed to load in a sync way! " + key);

            if (op.Result == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                if (op.OperationException != null)
                    message += " Exception: " + op.OperationException;

                throw new Exception(message);
            }

            return op.Result;
        }

        /// <summary>
        /// 以 回调方式加载 ab资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="priority"></param>
        /// <returns ></returns>
        static public void LoadAssetAsync(string key, System.Type type, System.Action<object, object> onComplete, System.Action<object, object> onEnd, object userData = null)
        {
            if (type == null) type = TypeHelper.Object;

            if (type.Equals(TypeHelper.GameObject))
            {
                LoadAssetAsync<UnityEngine.GameObject>(key, onComplete, onEnd, userData);
                return;
            }
            else if (type.Equals(TypeHelper.Sprite))
            {
                LoadAssetAsync<UnityEngine.Sprite>(key, onComplete, onEnd, userData);
                return;
            }
            else if (type.Equals(TypeHelper.TextAsset))
            {
                LoadAssetAsync<UnityEngine.TextAsset>(key, onComplete, onEnd, userData);
                return;
            }
            else if (type.Equals(TypeHelper.Texture))
            {
                LoadAssetAsync<UnityEngine.Texture>(key, onComplete, onEnd, userData);
                return;
            }
            else
            {
                LoadAssetAsync<UnityEngine.Object>(key, onComplete, onEnd, userData);
                return;
            }
        }

        static async public void LoadAssetAsync<T>(string key, System.Action<T, object> onComplete, System.Action<object, object> onEnd, object userData = null)
        {
            if (m_MarkGroup)
            {
                m_Groupes.Add(key);
                m_TotalGroupCount++;
            }

            var task = Addressables.LoadAssetAsync<T>(key).Task;
            await task;
            if (task.Result != null)
            {
                onComplete(task.Result, userData);
            }
            else
            {
                onEnd(null, userData);
            }

            OnItemLoaded(key);
        }

        static public Task<T> LoadAssetAsyncTask<T>(string key)
        {
            var task = Addressables.LoadAssetAsync<T>(key).Task;
            return task;
        }

        #endregion

        #region  GameObject 实例化
        static async public void InstantiateAsync(string key, System.Action<GameObject, object> onComplete, System.Action<object, object> onEnd, object userData = null, Transform parent = null)
        {
            if (m_MarkGroup)
            {
                m_Groupes.Add(key);
                m_TotalGroupCount++;
            }

            var task = Addressables.InstantiateAsync(key, parent, false).Task;
            await task;

            if (task.Result != null)
            {
                if (onComplete != null) onComplete(task.Result, userData);
            }
            else
            {
                if (onEnd != null) onEnd(key, userData);
            }
            OnItemLoaded(key);
        }

        #endregion

        #region  场景相关
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="userData"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        static async public void LoadSceneAsync(string key, System.Action<SceneInstance, object> onComplete, System.Action<object, object> onEnd, object userData = null, bool activateOnLoad = true, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {

            if (m_LoadedScenes.TryGetValue(key, out var sceneInstance) && sceneInstance.Scene.IsValid()) //如果场景已经加载
            {
                Executor.Execute(ActiveExistsSceneAsync(sceneInstance, onComplete, userData));
            }
            else
            {
                if (m_MarkGroup)
                {
                    m_Groupes.Add(key);
                    m_TotalGroupCount++;
                }

                var task = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive, activateOnLoad).Task;
                await task;

                if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    m_LoadedScenes[key] = task.Result;
                    if (onComplete != null) onComplete(task.Result, userData);
                }
                else
                {
                    if (onEnd != null) onEnd(key, userData);
#if UNITY_EDITOR
                    Debug.LogError("Failed to load scene at address: " + key);
#endif
                }

                OnItemLoaded(key);
            }

        }

        private static IEnumerator ActiveExistsSceneAsync(SceneInstance sceneInstance, System.Action<SceneInstance, object> onComplete, object userData = null)
        {
            yield return sceneInstance.ActivateAsync();
            if (onComplete != null) onComplete(sceneInstance, userData);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="loadedScene"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        static async public void UnloadSceneAsync(SceneInstance loadedScene, System.Action<SceneInstance, object> onComplete, System.Action<SceneInstance, object> onEnd, object userData = null)
        {
            var task = Addressables.UnloadSceneAsync(loadedScene).Task;
            await task;
            if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
            {
                if (onComplete != null) onComplete(task.Result, userData);
                string key = null;
                foreach (var kv in m_LoadedScenes)
                {
                    if (kv.Value.Equals(loadedScene))
                    {
                        key = kv.Key;
                        break;
                    }
                }

                if (key != null) m_LoadedScenes.Remove(key);
            }
            else
            {
                if (onEnd != null) onEnd(loadedScene, userData);
#if UNITY_EDITOR
                Debug.LogError("Failed to unload scene : " + loadedScene);
#endif
            }

        }

        static public void UnloadSceneAsync(string key, System.Action<SceneInstance, object> onComplete, System.Action<SceneInstance, object> onEnd, object userData = null)
        {
            if (m_LoadedScenes.TryGetValue(key, out var loadedScene))
            {
                UnloadSceneAsync(loadedScene, onComplete, onEnd, userData = null);
            }
        }

        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public AsyncOperation ActiveScene(string key)
        {
            if (m_LoadedScenes.TryGetValue(key, out var loadedScene))
            {
                return loadedScene.ActivateAsync();
            }
            return null;
        }

        static Dictionary<String, SceneInstance> m_LoadedScenes;

        #endregion


        static public void ReleaseInstance(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }

        static public void Release<T>(T obj)
        {
            Addressables.Release<T>(obj);
        }



    }

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

    static class TypeHelper
    {
        internal static readonly Type GameObject = typeof(GameObject);
        internal static readonly Type Sprite = typeof(Sprite);
        internal static readonly Type Texture = typeof(Texture);
        internal static readonly Type TextAsset = typeof(TextAsset);
        internal static readonly Type Object = typeof(UnityEngine.Object);

    }
}

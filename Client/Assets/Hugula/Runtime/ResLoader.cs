// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
#define ADDRESSABLES_INSTANTIATEASYNC
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hugula.Utils;
using Hugula.Profiler;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

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

        // [RuntimeInitializeOnLoadMethod]
        internal static void Init()
        {
            if (!s_Initialized)
            {
                m_LoadedScenes = new Dictionary<string, SceneInstance>();
                m_LoadingEvent = new LoadingEventArg();

                Addressables.InitializeAsync().Completed += InitDone; //AASHotUpdate 中已经初始化了，勿要重复初始化

#if UNITY_EDITOR
                Debug.Log("Addressables.InitializeAsync()");
#endif
            }
        }

        static void InitDone(AsyncOperationHandle<IResourceLocator> obj)
        {
            s_Initialized = true;


#if (UNITY_EDITOR || !HUGULA_NO_LOG) && !HUGULA_RELEASE && ENABLE_RESOURCELOCATORS_INFO
            Debug.Log("Addressables.InitializeAsync() InitDone");
            // #endif
            // #if !HUGULA_NO_LOG
            var sb = new System.Text.StringBuilder();
            var keys = new List<object>();
            foreach (var item in Addressables.ResourceLocators)
            {
                sb.AppendLine($"\r\n new Addressables.ResourceLocators:({item.LocatorId})  GetHashCode:{item.GetHashCode()}  {item}:");
                keys.Clear();
                keys.AddRange(item.Keys);
                sb.AppendLine($"\r\n  -------------------------------{item.LocatorId}-Count:{keys.Count}------------------");

                foreach (var key in keys)
                {
                    if (item.Locate(key, typeof(object), out var locations))
                    {
                        sb.AppendLine($"\r\n\r\n            -----{item.LocatorId} key({key.ToString()})      locations.Count:{locations.Count}");
                        foreach (var loc in locations)
                        {
                            sb.AppendLine($"                    {key.ToString()}   ResourceType:({loc.ResourceType}) PrimaryKey({loc.PrimaryKey}) InternalId({loc.InternalId}) Data:{loc.Data} ");

                            if (loc.HasDependencies)
                            {
                                sb.Append($"                        HasDependencies:{loc.HasDependencies}  DependencyHashCode:{loc.DependencyHashCode} , \r\n                        Dependencies:");
                                foreach (var dep in loc.Dependencies)
                                {
                                    sb.Append($"\r\n                          Dep.PrimaryKey({dep.PrimaryKey}) Dep.InternalId({dep.InternalId}) Dep.ResourceType:({dep.ResourceType})  Dep.ProviderId:{dep.ProviderId}  Data:{loc.Data} ");
                                }

                            }
                        }
                    }
                    else
                        sb.AppendLine($"\r\n\r\n            -----{item.LocatorId} key({key.ToString()})--Count:0");
                }

                // Debug.Log(sb.ToString());
                var logName = $"ResourceLocators_{item.LocatorId}.txt";
                string logPath = "";

#if UNITY_EDITOR
                var path = "Assets/Tmp";//Path.Combine(Application.dataPath, "../Logs");
                logPath = System.IO.Path.Combine(path, logName);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
#elif UNITY_STANDALONE
            var path = System.IO.Path.Combine(Application.dataPath, "../Logs");
            logPath = System.IO.Path.Combine(path, System.DateTime.Now.ToString("MM_dd HH_mm_ss ")+logName);
             if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
#else
            logPath = System.IO.Path.Combine(CUtils.realPersistentDataPath,logName);
#endif

                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(logPath, false))
                {
                    sr.Write(sb.ToString());
                }
                sb.Clear();
            }


#endif
        }

        /// <summary>
        /// 当前正在加载的资源数量
        /// </summary>
        internal static int CurrentLoadingAssetCount
        {
            get;
            private set;
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
            m_TotalGroupCount = m_Groupes.Count;//前一组的没有加载完成
            m_CurrGroupLoaded = 0;
        }

        static public void EndMarkGroup()
        {
            m_MarkGroup = false;
            if (OnGroupComplete != null && m_TotalGroupCount <= m_CurrGroupLoaded && m_Groupes.Count == 0)
            {
                OnGroupComplete();
            }
        }

        static void OnItemLoaded(string key)
        {

            int g_Idx = m_Groupes.IndexOf(key);
            if (g_Idx >= 0)
            {
                m_Groupes.RemoveAt(g_Idx);
                m_CurrGroupLoaded++;
            }

            if (OnGroupProgress != null && m_TotalGroupCount > 0)
            {
                m_LoadingEvent.total = m_TotalGroupCount;
                m_LoadingEvent.current = m_CurrGroupLoaded;
                m_LoadingEvent.progress = (float)m_LoadingEvent.current / (float)m_LoadingEvent.total;
                OnGroupProgress(m_LoadingEvent);
            }

            if (OnGroupComplete != null && g_Idx >= 0 && m_TotalGroupCount <= m_CurrGroupLoaded && !m_MarkGroup && m_Groupes.Count == 0)
            {
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
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"LoadAsset({typeof(T)}) key is null");

            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! ");

            key = GetKey(key);

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
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException("Instantiate key is null");

            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            AsyncOperationHandle<GameObject> op;
            key = GetKey(key);

#if PROFILER_DUMP
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("Instantiate:", key))
            {
#endif
            op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);
#if PROFILER_DUMP
            }
#endif
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
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"LoadAssetAsync({type}) key is null");

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

        static async public void LoadAssetAsync<T, R>(string key, System.Action<T, R> onComplete, System.Action<object, R> onEnd, R userData = default(R))
        {
            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            key = GetKey(key);

            if (m_MarkGroup)
            {
                m_Groupes.Add(key);
                m_TotalGroupCount++;
            }

            bool callEnd = true;

            try
            {

                Task<T> task;
#if PROFILER_DUMP
                var pkey = $"LoadAssetAsync<{typeof(T)},{typeof(R)}>:" + key;

                using (ProfilerFactory.GetAndStartProfiler(pkey))
                {
                    ProfilerFactory.BeginSample(pkey);
#endif
                task = Addressables.LoadAssetAsync<T>(key).Task;
#if PROFILER_DUMP
                    ProfilerFactory.EndSample();
#endif

                await task;
#if PROFILER_DUMP
                }

                var ckey = $"LoadAssetAsync<{typeof(T)},{typeof(R)}>.onComp:" + key;

                using (ProfilerFactory.GetAndStartProfiler(ckey, null, null, true))
                {
#endif
                callEnd = false;
                if (task.Result != null)
                {
                    onComplete?.Invoke(task.Result, userData);
                }
                else
                {
                    onEnd?.Invoke(key, userData);
                }

#if PROFILER_DUMP
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (callEnd) onEnd?.Invoke(key, userData);
                // throw;
            }
            finally
            {
                OnItemLoaded(key);
            }

        }

        static async public void LoadAssetAsync<T>(string key, System.Action<T, object> onComplete, System.Action<object, object> onEnd, object userData = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"LoadAssetAsync({typeof(T)}) key is null");

            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            key = GetKey(key);
            CurrentLoadingAssetCount++;
            if (m_MarkGroup)
            {
                m_Groupes.Add(key);
                m_TotalGroupCount++;
            }

            bool callEnd = true;
            try
            {

                Task<T> task;
#if PROFILER_DUMP
                var pkey = $"LoadAssetAsync<{typeof(T)}>:" + key;
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey))
                {
                    ProfilerFactory.BeginSample(pkey);
#endif
                task = Addressables.LoadAssetAsync<T>(key).Task;
#if PROFILER_DUMP
                    ProfilerFactory.EndSample();
#endif

                await task;
#if PROFILER_DUMP
                }

                var ckey = $"LoadAssetAsync<{typeof(T)}>.onComp:" + key;
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(ckey, null, null, true))
                {
#endif
                callEnd = false;
                if (task.Result != null)
                {
                    onComplete?.Invoke(task.Result, userData);
                }
                else
                {
                    Debug.LogError($"LoadAssetAsync<{typeof(T)}> can't find asset ({key})");
                    onEnd?.Invoke(null, userData);
                }
#if PROFILER_DUMP
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (callEnd) onEnd?.Invoke(null, userData);
                // throw;
            }
            finally
            {
                CurrentLoadingAssetCount--;
                OnItemLoaded(key);
            }

        }

        /// <summary>
        /// 返回Task 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="inGroup"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="Exception"></exception>
        static public Task<T> LoadAssetAsyncTask<T>(string key, bool inGroup = false)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"LoadAssetAsyncTask({typeof(T)}) key is null");

            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            key = GetKey(key);

            if (inGroup && m_MarkGroup)
            {
                m_Groupes.Add(key);
                m_TotalGroupCount++;
            }
#if PROFILER_DUMP
            var pkey = $"LoadAssetAsync<{typeof(T)}>:" + key;
            ProfilerFactory.BeginSample(pkey);
#endif
            var task = Addressables.LoadAssetAsync<T>(key).Task;

#if PROFILER_DUMP
            ProfilerFactory.EndSample();
#endif

            return task;
        }

        /// <summary>
        /// 增加当前加载的资源数量，必须与SubCrruentLoadingAssetCount成对出现
        /// </summary>
        internal static void AddCrruentLoadingAssetCount()
        {
            CurrentLoadingAssetCount++;
        }

        /// <summary>
        /// 减少当前加载的资源数量，必须与AddCrruentLoadingAssetCount成对出现
        /// </summary>
        internal static void SubCrruentLoadingAssetCount()
        {
            CurrentLoadingAssetCount--;
        }

        /// <summary>
        /// 用于进度提示
        /// </summary>
        /// <param name="key"></param>
        static public void LoadAssetAsyncTaskDone(string key)
        {
#if UNITY_EDITOR && !HUGULA_NO_LOG
            // Debug.Log($"ResLoader.LoadAssetAsyncTaskDone({key})");
#endif
            Hugula.Executor.Execute(() =>
            {
                OnItemLoaded(key);
            }
            );
        }

        #endregion

        #region  GameObject 实例化
#if !ADDRESSABLES_INSTANTIATEASYNC
        static Dictionary<GameObject, UnityEngine.Object> gObjInstanceRef = new Dictionary<GameObject, UnityEngine.Object>();
#else
        static Dictionary<GameObject, string> InstantiateRef = new Dictionary<GameObject, string>();

#endif
        public static AsyncOperationHandle<GameObject> InstantiateAsyncOperation(string key, Transform parent = null)
        {
            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            var task = Addressables.InstantiateAsync(key, parent, false);
            return task;
        }

        static async public void InstantiateAsync(string key, System.Action<GameObject, object> onComplete, System.Action<object, object> onEnd, object userData = null, Transform parent = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"LoadAssetAsyncTask(GameObject) key is null lua:{Hugula.EnterLua.LuaTraceback()}");

            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            key = GetKey(key);
            CurrentLoadingAssetCount++;
            if (m_MarkGroup)
            {
                m_Groupes.Add(key);
                m_TotalGroupCount++;
            }
            bool callEnd = true;
            try
            {

                Task<GameObject> task;

                GameObject inst = null;
#if ADDRESSABLES_INSTANTIATEASYNC
#if PROFILER_DUMP
                var pkey = "InstantiateAsync:" + key;
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey))
                {
                    ProfilerFactory.BeginSample(pkey);
#endif
                task = Addressables.InstantiateAsync(key, parent).Task;
#if PROFILER_DUMP
                    ProfilerFactory.EndSample();
#endif

                await task;
                inst = task.Result;
                await Task.Yield(); // 实例化完成后等待一帧执行
#if PROFILER_DUMP
                }
                var ckey = "InstantiateAsync.onComp:" + key;
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(ckey, null, null, true))
                {
#endif
                callEnd = false;
                if (inst != null)
                {
                    InstantiateRef[inst] = key;
                    onComplete?.Invoke(inst, userData);
                }
                else
                {
                    Debug.LogError($"InstantiateAsync<GameObject> can't find asset ({key})");
                    onEnd?.Invoke(key, userData);
                }
#if PROFILER_DUMP
                }
#endif

#else

#if PROFILER_DUMP
            var pkey = "InstantiateAsync:" + key;
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey))
            {
#endif
                //从缓存中读取
                task = Addressables.LoadAssetAsync<GameObject>(key).Task;
                await task;
                await Task.Yield(); // 实例化完成后等待一帧执行
                if (task.Result == null)
                {
                    Debug.LogError($"InstantiateAsync<GameObject> can't find asset ({key})");
                    if (onEnd != null) onEnd(key, userData);
                    // OnItemLoaded(key);
                    return;
                }
#if PROFILER_DUMP
            }
            var ckey = "InstantiateAsync.Instantiate:" + key;
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(ckey, null, null, true))
            {
#endif
                //实例化
                var asset = task.Result;
                inst = GameObject.Instantiate<GameObject>(asset, parent, false);
                gObjInstanceRef[inst] = asset;

#if PROFILER_DUMP
            }
            ckey = "InstantiateAsync.onComp:" + key;
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(ckey, null, null, true))
            {
#endif
                callEnd = false;
                if (inst != null)
                {
                    if (onComplete != null) onComplete(inst, userData);
                }
                else
                {
                    if (onEnd != null) onEnd(key, userData);
                }
#if PROFILER_DUMP
            }
#endif
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (callEnd) onEnd?.Invoke(key, userData);
                // throw;
            }
            finally
            {
                CurrentLoadingAssetCount--;
                OnItemLoaded(key); //确保本执行
            }
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
        static async public void LoadSceneAsync(string key, System.Action<SceneInstance, object> onComplete, System.Action<object, object> onEnd, object userData = null, bool activateOnLoad = true, int loadSceneMode = 1)
        {
            if (string.IsNullOrEmpty(key))
                throw new NullReferenceException($"LoadAssetAsyncTask(GameObject) key is null");

            if (!s_Initialized)
                throw new Exception("Whoa there friend!  We haven't init'd ResLoader yet! " + key);

            key = GetKey(key);
            CurrentLoadingAssetCount++;
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

                bool callEnd = true;
                try
                {

                    Task<SceneInstance> task;
#if PROFILER_DUMP
                var pkey = "LoadSceneAsync:" + key;

                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey))
                {
#endif
                    task = Addressables.LoadSceneAsync(key, (LoadSceneMode)loadSceneMode, activateOnLoad).Task;
                    await task;
                await Task.Yield(); // 实例化完成后等待一帧执行
#if PROFILER_DUMP
                }
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("LoadSceneAsync.onComp:", key, pkey, true))
                {
#endif
                    callEnd = false;
                    if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                    {
                        m_LoadedScenes[key] = task.Result;
                        onComplete?.Invoke(task.Result, userData);
                    }
                    else
                    {
                        Debug.LogError($"LoadSceneAsync can't find asset ({key})");
                        onEnd?.Invoke(key, userData);
                    }

#if PROFILER_DUMP
                }
#endif

                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    if (callEnd) onEnd?.Invoke(key, userData);
                    // throw;
                }
                finally
                {
                    CurrentLoadingAssetCount--;
                    OnItemLoaded(key);
                }
            }

        }

        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="sceneInstance"></param>
        /// <returns></returns>
        public static bool SetActiveScene(SceneInstance sceneInstance)
        {
            return SceneManager.SetActiveScene(sceneInstance.Scene);
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
            string key = null;
            foreach (var kv in m_LoadedScenes)
            {
                if (kv.Value.Equals(loadedScene))
                {
                    key = kv.Key;
                    break;
                }
            }
            Task<SceneInstance> task;
#if PROFILER_DUMP
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("UnloadSceneAsync:", key))
            {

#endif
            if (key != null) m_LoadedScenes.Remove(key);

            task = Addressables.UnloadSceneAsync(loadedScene).Task;
            await task;
            await Task.Yield(); // 实例化完成后等待一帧执行

#if PROFILER_DUMP
            }

            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("UnloadSceneAsync.onComp:", key))
            {

#endif
            if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
            {
                if (onComplete != null) onComplete(task.Result, userData);
            }
            else
            {
                Debug.LogError($"UnloadSceneAsync can't find scene ({key})");
                if (onEnd != null) onEnd(loadedScene, userData);
#if UNITY_EDITOR
                    Debug.LogError("Failed to unload scene : " + loadedScene);
#endif
            }
#if PROFILER_DUMP
            }

#endif
        }

        static public void UnloadSceneAsync(string key, System.Action<SceneInstance, object> onComplete, System.Action<SceneInstance, object> onEnd, object userData = null)
        {
            if (m_LoadedScenes.TryGetValue(GetKey(key), out var loadedScene))
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
            if (m_LoadedScenes.TryGetValue(GetKey(key), out var loadedScene))
            {
                return loadedScene.ActivateAsync();
            }
            return null;
        }

        static Dictionary<String, SceneInstance> m_LoadedScenes;

        #endregion

        #region  重定向address
        /// <summary>
        /// 获取真实的address 地址用于缺失资源默认显示
        /// </summary>
        internal static string GetKey(string key)
        {
            if (AddressTransformFunc != null)
            {
                return AddressTransformFunc(key);
            }
            else
            {
                return key;
            }
        }

        /// <summary>
        /// Functor to transform address key before load.
        /// </summary>
        static public Func<string, string> AddressTransformFunc
        {
            get;
            set;
        }

        #endregion
        static public void ReleaseInstance(GameObject obj)
        {
            if (obj)
            {
#if ADDRESSABLES_INSTANTIATEASYNC
#if PROFILER_DUMP
                var pkey1 = $"ResLoader.ReleaseInstance<GameObject>:" + obj;
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey1, null, null, true))
                {
#endif

                InstantiateRef.Remove(obj);
                Addressables.ReleaseInstance(obj);

#if PROFILER_DUMP
                }
#endif

#else
                GameObject.Destroy(obj);
                UnityEngine.Object asset = null;
                if (gObjInstanceRef.TryGetValue(obj, out asset))
                {
                    Release<UnityEngine.Object>(asset);
                }
#endif
            }
        }

        //释放所有缓存的GameObjecct 用于重启
        static public void ReleaseCachedInstances()
        {
            GameObject gameObject;
#if ADDRESSABLES_INSTANTIATEASYNC
            foreach (var kv in InstantiateRef)
            {
                gameObject = kv.Key;
                Addressables.ReleaseInstance(gameObject);
            }
            InstantiateRef.Clear();
#else
            foreach(var kv in gObjInstanceRef)
            {
                gameObject = kv.Key;
                GameObject.Destroy(gameObject);
                Release<UnityEngine.Object>(kv.Value);
            }
            gObjInstanceRef.Clear();
#endif

        }

        static public void Release<T>(T obj)
        {
#if UNITY_EDITOR
            if (obj == null)
                Debug.LogWarningFormat("try to release null obj \r\n trace:{1}", Hugula.EnterLua.LuaTraceback());
#endif

            if (obj == null) return;

            s_NextFrameObjects.Add(obj);
            // s_LastReleaseFrame = Time.frameCount + 1;
        }
        static public void Release(object obj)
        {
            Release<object>(obj);
        }

        #region 延时释放 asset for "Addressables was not previously aware of"
        internal static List<object> s_NextFrameObjects = new List<object>(128);
        // static int s_LastReleaseFrame;
        /// <summary>
        /// 释放列表中待释放的资源
        /// </summary>
        /// <param name="count"></param>
        static internal void DoReleaseNext(int frameCount, int count = 0)
        {

            int len = s_NextFrameObjects.Count;
            if (count <= 0)
                count = len;
            else
                count = count >= len ? len : count;

            int i = 0;
            object res = null;

            if (count == 0) return;

#if PROFILER_DUMP
            var pkey1 = $"ResLoader.DoReleaseNext: Addressables.Release.Count=" + count;
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey1, null, null, true))
            {
#endif
            while (i < count)
            {
                res = s_NextFrameObjects[i];
                Addressables.Release(res);
                i++;

                if (FrameWatcher.IsTimeOver())
                {
                    break;
                }
            }
#if PROFILER_DUMP
            }
#endif
            if (i > 0)
                s_NextFrameObjects.RemoveRange(0, i);

        }

        #endregion
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
            total = 1;
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
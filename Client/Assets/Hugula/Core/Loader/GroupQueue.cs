using UnityEngine;
using System.Collections.Generic;
using Hugula.Utils;

namespace Hugula.Loader
{

    public abstract class GroupQueue<T> : IReleaseToPool
    {

        #region info
        public System.Action<LoadingEventArg> onProgress;
        // bool isError
        public System.Action<bool> onComplete;
        public int priority;

        //private
        protected LoadingEventArg loadingEventArg = new LoadingEventArg();
        protected int loadedCount = 0;
        protected int totalCount;
        protected Queue<T> groupRes = new Queue<T>();
        protected List<T> loadingGroupRes = new List<T>();
        protected List<string> errRes = new List<string>();

        protected System.Action<T, bool> m_OnComplete;

        //public 
        public int Count
        {
            get
            {
                return this.groupRes.Count;
            }
        }

        public int LoadingCount
        {
            get
            {
                return this.loadingGroupRes.Count;
            }
        }

        public bool IsDown
        {
            get
            {
                return (groupRes.Count == 0 && loadedCount >= totalCount && loadingGroupRes.Count == 0);
            }
        }
        #endregion

        public virtual void Enqueue(T req)
        {
            groupRes.Enqueue(req);
            totalCount++;
        }

        public T Dequeue()
        {
            if (this.groupRes.Count > 0)
            {
                var req = groupRes.Dequeue();
                loadingGroupRes.Add(req);
                return req;
            }
            return default(T);
        }

        public bool Complete(T req, bool isError)
        {
            int i = loadingGroupRes.IndexOf(req);

            if (i >= 0)
            {
                loadingGroupRes.RemoveAt(i);
                loadedCount++;
                if (m_OnComplete != null)
                    m_OnComplete(req, isError);
                else
                    OnComplete(req, isError);
                DispatchOnProgress();
            }
            // Debug.LogFormat("i={0},totalCount={1},loadedCount={2},IsDown={3},req={4},isErr={5}",i,totalCount,loadedCount,IsDown,req,isError);
            if (IsDown)
                DispatchOnComplete();

            return i >= 0;
        }

        public GroupQueue()
        {

        }

        public GroupQueue(int priority)
        {
            this.priority = priority;
        }

        protected void OnComplete(T req, bool isError)
        {
            loadingEventArg.total = totalCount;
            loadingEventArg.current = loadedCount;
            if (isError)
                errRes.Add(string.Empty);
        }

        internal void Reset()
        {
            this.totalCount = 0;
            this.loadedCount = 0;
            groupRes.Clear();
            loadingGroupRes.Clear();
            errRes.Clear();
        }

        protected void DispatchOnProgress()
        {
            if (onProgress != null)
            {
                loadingEventArg.progress = (float)loadedCount / (float)totalCount;
                onProgress(loadingEventArg);
            }
        }

        protected void DispatchOnComplete()
        {
            bool isError = errRes.Count > 0;
            Reset();
            if (onComplete != null)
                onComplete(isError);

            onComplete = null;
            onProgress = null;

            ReleaseToPool();
        }

        public virtual void ReleaseToPool()
        {

        }

    }

    public sealed class BundleGroundQueue : GroupQueue<CRequest>
    {
        public override void Enqueue(CRequest req)
        {
            if (ResourcesLoader.LoadAssetFromCache(req))
            {
                ABDelayUnloadManager.CheckRemove(req.keyHashCode);
                ResourcesLoader.DispatchReqAssetOperation(req, false);
            }
            else
            {
                req.group = this;
                base.Enqueue(req);
            }
        }

        public void Enqueue(IList<CRequest> reqs)
        {
            foreach (var abInfo in reqs)
            {
                Enqueue(abInfo);
            }
        }

        public override void ReleaseToPool()
        {
            Release(this);
        }

        #region objectpool
        static ObjectPool<BundleGroundQueue> objectPool = new ObjectPool<BundleGroundQueue>(null, m_ActionOnRelease);

        private static void m_ActionOnRelease(BundleGroundQueue re)
        {
            re.onComplete = null;
            re.onProgress = null;
        }

        public static BundleGroundQueue Get()
        {
            return objectPool.Get();
        }

        public static void Release(BundleGroundQueue toRelease)
        {
            objectPool.Release(toRelease);
        }
        #endregion
    }

}
using System.Collections.Generic;
using Hugula.Collections;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Loader {

    public abstract class GroupQueue<T> : IReleaseToPool {

        #region info
        public System.Action<LoadingEventArg> onProgress;
        // bool isError
        public System.Action<bool> onComplete;
        public int priority;

        //private
        protected LoadingEventArg loadingEventArg = new LoadingEventArg ();
        protected int loadedCount = 0;
        protected int totalCount;
        protected Queue<T> groupRes = new Queue<T> ();
        protected SafeDictionary<T, int> loadingGroupRes = new SafeDictionary<T, int> ();
        protected List<T> errRes = new List<T> ();
        protected readonly object syncRoot = new object();

        internal bool pool = false;

        // protected int loadingPer;
        //public 
        public int Count {
            get {
                return this.groupRes.Count;
            }
        }

        public int LoadingCount {
            get {
                return this.loadingGroupRes.Count;
            }
        }

        public virtual bool IsDown {
            get {
                return (groupRes.Count == 0 && LoadingCount == 0);
            }
        }

        public bool IsError{
            get
            {
                return errRes.Count > 0;
            }
        }
        #endregion

        public virtual void Enqueue (T req) {
            groupRes.Enqueue (req);
            totalCount++;
        }

        public T Dequeue () {
            if (this.groupRes.Count > 0) {
                var req = groupRes.Dequeue ();
                loadingGroupRes.Add (req, 0);
                return req;
            }
            return default (T);
        }

        public void ReLoadError()
        {
            foreach (var req in errRes) {
                groupRes.Enqueue (req);
                loadedCount--;
            }

            errRes.Clear();
        }

        public bool Complete (T req, bool isError) {

            if (loadingGroupRes.ContainsKey (req)) {
                loadingGroupRes.Remove (req);
                 if (isError)
                    errRes.Add (req);
                loadedCount++;
                UpdateProgress(req,isError);
                DispatchOnProgress ();
            }
            // Debug.LogFormat("i={0},totalCount={1},loadedCount={2},IsDown={3},req={4},isErr={5}",i,totalCount,loadedCount,IsDown,req,isError);
            if (IsDown)
                DispatchOnComplete ();

            return true;
        }

        public GroupQueue () {

        }

        public GroupQueue (int priority) {
            this.priority = priority;
        }

        protected virtual void UpdateProgress (T req, bool isError) {
            loadingEventArg.total = totalCount;
            loadingEventArg.current = loadedCount;
        }

        internal void Reset () {
            this.totalCount = 0;
            this.loadedCount = 0;
            groupRes.Clear ();
            loadingGroupRes.Clear ();
            errRes.Clear ();
            onComplete = null;
            onProgress = null;
            pool = false;
        }

        public void DispatchOnProgress () {
            if (onProgress != null) {
                // loadingEventArg.progress = (loadedCount * 100 + loadingPer) / totalCount;
                onProgress (loadingEventArg);
            }
        }

        protected virtual void DispatchOnComplete () {
            if (onComplete != null)
                onComplete (IsError);
        }

        public virtual void ReleaseToPool () {

        }

    }

    public sealed class BundleGroundQueue : GroupQueue<CRequest> {
        public override void Enqueue (CRequest req) {
                req.group = this;
                base.Enqueue (req);
        }

        public void Enqueue (IList<CRequest> reqs) {
            foreach (var abInfo in reqs) {
                Enqueue (abInfo);
            }
        }

        protected override void DispatchOnComplete()
        {
            base.DispatchOnComplete();
            ReleaseToPool();
        }

        public override void ReleaseToPool () {
            if(pool)
                Release (this);
        }

        #region objectpool
        static ObjectPool<BundleGroundQueue> objectPool = new ObjectPool<BundleGroundQueue> (null, m_ActionOnRelease);

        private static void m_ActionOnRelease (BundleGroundQueue re) {
            re.Reset();
        }

        public static BundleGroundQueue Get () {
            var bgroup = objectPool.Get ();
            bgroup.pool = true;
            return bgroup;
        }

        public static void Release (BundleGroundQueue toRelease) {
            objectPool.Release (toRelease);
        }
        #endregion
    }

}
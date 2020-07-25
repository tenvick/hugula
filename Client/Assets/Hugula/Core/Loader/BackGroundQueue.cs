using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;
using Hugula.Collections;

namespace Hugula.Loader
{
    public sealed class BackGroundQueue : IReleaseToPool
    {
        private uint totalSize;
        private uint loadedSize;
        #region info
        public System.Action<LoadingEventArg> onProgress;
        // bool isError
        public System.Action<bool> onComplete;
        public int priority;

        //private
        LoadingEventArg loadingEventArg = new LoadingEventArg();
        int loadedCount = 0;
        int totalCount;
        Queue<ABInfo> groupRes = new Queue<ABInfo>();
        SafeDictionary<ABInfo, int> loadingGroupRes = new SafeDictionary<ABInfo, int>();
        List<ABInfo> errRes = new List<ABInfo>();
        readonly object syncRoot = new object();

        internal bool pool = false;

        // protected int loadingPer;
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


        public bool IsError
        {
            get
            {
                return errRes.Count > 0;
            }
        }
        #endregion

        public BackGroundQueue(int priority)
        {
            this.priority = priority;
        }

        public bool IsDown
        {
            get
            {
                return (groupRes.Count == 0 && LoadingCount == 0) || IsError;
            }
        }
        void UpdateProgress(ABInfo abInfo, bool isError)
        {

            if (!isError)
                loadedSize += abInfo.size;

            loadingEventArg.total = (int)totalSize;
            loadingEventArg.current = (int)loadedSize;
        }

        public void Enqueue(ABInfo abInfo)
        {
            abInfo.state = ABInfoState.None;
            groupRes.Enqueue(abInfo);
            totalCount++;
            totalSize += abInfo.size;
        }

        public void Enqueue(IList<ABInfo> reqs)
        {
            foreach (var abInfo in reqs)
            {
                Enqueue(abInfo);
            }
        }


        public ABInfo Dequeue()
        {
            if (this.groupRes.Count > 0)
            {
                var req = groupRes.Dequeue();
                loadingGroupRes.Add(req, 0);
                return req;
            }
            return null;
        }

        public void ReLoadError()
        {
            foreach (var req in errRes)
            {
                groupRes.Enqueue(req);
                loadedCount--;
            }

            errRes.Clear();
        }

        public bool Complete(ABInfo req, bool isError)
        {

            if (loadingGroupRes.ContainsKey(req))
            {
                loadingGroupRes.Remove(req);
                if (isError)
                    errRes.Add(req);
                loadedCount++;
                UpdateProgress(req, isError);
                DispatchOnProgress();
            }
            // Debug.LogFormat("i={0},totalCount={1},loadedCount={2},IsDown={3},req={4},isErr={5}",i,totalCount,loadedCount,IsDown,req,isError);
            if (IsDown)
                DispatchOnComplete();

            return true;
        }

        internal void Reset()
        {
            this.totalCount = 0;
            this.loadedCount = 0;
            groupRes.Clear();
            loadingGroupRes.Clear();
            errRes.Clear();
            onComplete = null;
            onProgress = null;
            pool = false;
        }

        public void DispatchOnProgress()
        {
            if (onProgress != null)
            {
                // loadingEventArg.progress = (loadedCount * 100 + loadingPer) / totalCount;
                onProgress(loadingEventArg);
            }
        }

        void DispatchOnComplete()
        {
            if (onComplete != null)
                onComplete(IsError);
        }


        public void ReleaseToPool()
        {

        }

    }
}
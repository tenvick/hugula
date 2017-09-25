using UnityEngine;
using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;

namespace Hugula.Update
{
    public sealed class BackGroundQueue : GroupQueue<ABInfo>
    {
        private uint totalSize;
        private uint loadedSize;

        public BackGroundQueue(int priority)
        {
            this.priority = priority;
            this.m_OnComplete = _OnComplete;
        }

        private void _OnComplete(ABInfo abInfo, bool isError)
        {
            loadedSize += abInfo.size;
            loadingEventArg.total = (int)totalSize;
            // loadingEventArg.progress = loadedSize;
            loadingEventArg.current = (int)loadedSize;
            if (isError)
                errRes.Add(abInfo.abName);
        }

        public override void Enqueue(ABInfo abInfo)
        {
            abInfo.state =  ABInfoState.None;
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

        public override void ReleaseToPool()
        {

        }

    }
}
using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Update {
    public sealed class BackGroundQueue : GroupQueue<ABInfo> {
        private uint totalSize;
        private uint loadedSize;

        public BackGroundQueue (int priority) {
            this.priority = priority;
            this.m_OnComplete = _OnComplete;
        }

        private void _OnComplete (ABInfo abInfo, bool isError) {
            loadingEventArg.total = (int) totalSize;
            loadingEventArg.current = (int) loadedSize;
            if (isError) {
                errRes.Add (abInfo.abName);
            } else
                loadedSize += abInfo.size;
        }

         protected override void AddProgress (ABInfo abInfo, int percent) {
            int oldVal = 0;
            if (loadingGroupRes.TryGetValue(abInfo,out oldVal)) {
                loadingGroupRes[abInfo] = percent;
                uint size = abInfo.size;
                loadingPer += (int)((percent-oldVal)*size);
            }
        }

        protected override void RemoveProgress(ABInfo req)
        {
            loadingPer -= (int)(req.size*100);
        }

        public override void Enqueue (ABInfo abInfo) {
            abInfo.state = ABInfoState.None;
            groupRes.Enqueue (abInfo);
            totalCount++;
            totalSize += abInfo.size;
        }

        public void Enqueue (IList<ABInfo> reqs) {
            foreach (var abInfo in reqs) {
                Enqueue (abInfo);
            }
        }

        public override void ReleaseToPool () {

        }

    }
}
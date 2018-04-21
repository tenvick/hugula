using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Update {
    public sealed class BackGroundQueue : GroupQueue<ABInfo> {
        private uint totalSize;
        private uint loadedSize;

        public BackGroundQueue(int priority)
        {
            this.priority = priority;
        }

        public override bool IsDown {
            get {
                return (groupRes.Count == 0 && LoadingCount == 0 ) || IsError;
            }
        }
        protected override void UpdateProgress (ABInfo abInfo, bool isError) {

            if (!isError)
                loadedSize += abInfo.size;

            loadingEventArg.total = (int) totalSize;
            loadingEventArg.current = (int) loadedSize;
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
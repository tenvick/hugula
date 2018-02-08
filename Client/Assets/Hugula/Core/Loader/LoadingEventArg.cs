using System.Collections;
using UnityEngine;

namespace Hugula.Loader {

    [SLua.CustomLuaClass]
    public class LoadingEventArg : System.ComponentModel.ProgressChangedEventArgs {
        //public int number;//current loading number
        public object target {
            get;
            internal set;
        }
        public long total {
            get;
            internal set;
        }
        public long current {
            get;
            internal set;
        }

        public float progress;

        public LoadingEventArg () : base (0, null) {

        }

        public LoadingEventArg (long bytesReceived, long totalBytesToReceive, object userState) : base ((totalBytesToReceive == -1L) ? 0 : ((int) (bytesReceived * 100L / totalBytesToReceive)), userState) {
            this.current = bytesReceived;
            this.total = totalBytesToReceive;
            this.target = userState;
        }
    }
}
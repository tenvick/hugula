 namespace Hugula.ResUpdate
 {

        [System.Serializable]
        public class VerionConfig {
            
            //强更新判断码
            public int code;

            //发布时间
            public int time;

            //当前版本号
            public string version;

            //资源下载地址多个以“,”号分割
            public string[] cdn_host;

            //强制更新跳转连接
            public string update_url;

            //对比文件名
            public string manifest_name;

            //是否需要等待fasst包完成，true下载完后才进入下一步，false不需要等待逻辑
            public FastMode fast = FastMode.sync;

        }

        public enum FastMode
        {
            ///<summary>
            /// 同步fast包加载完成后才能进入游戏
            ///</summary>
            sync,
            ///<summary>
            /// 异步fast包开始加载，无需等待可以进入游戏。
            ///</summary>
            async,
            ///<summary>
            /// 手动控制fast包
            ///</summary>
            manual

        }
 }
using UnityEngine;
using System;

namespace Hugula {
    /// <summary>
    /// 代码版本号Res verion.
    /// </summary>
    [SLua.CustomLuaClass]
    public  static class CodeVersion {

		static CodeVersion()
		{
			m_CODE_VERSION = 0;
			m_APP_NUMBER = 0;
			m_RES_VERSION = 0;
		}
        private const int BIT_SIZE = 1000;
        private static int m_CODE_VERSION;

        /// <summary>
        /// 校验版本号
		///	不一致会导致强更新
        /// </summary> 
        public static int CODE_VERSION {
            get {
                if (m_CODE_VERSION==0) {
                    var sp = SplitVersion (APP_VERSION);
                    if (sp.Length == 3) {
                        int v1 = Convert.ToInt32(sp[0]);
						int v2 = Convert.ToInt32(sp[1]);
                        m_CODE_VERSION = v1*BIT_SIZE*BIT_SIZE+v2*BIT_SIZE;
                    } else if (sp.Length == 2) {
                        int v1 = Convert.ToInt32(sp[0]);
						int v2 = Convert.ToInt32(sp[1]);
                        m_CODE_VERSION = v1*BIT_SIZE*BIT_SIZE+v2*BIT_SIZE;
						#if UNITY_EDITOR
						Debug.LogError(" Application.version set wrong ,it must be like 1.0.1");
						#endif
                    } else {
                        throw new System.Exception ("Application.version set wrong ,it must be like 1.0.1");
                    }
                }
                return m_CODE_VERSION;
            }
            set{
                m_CODE_VERSION = 0;
            }
        }

        /// <summary>
        /// app版本号
        /// </summary>	
        public static string APP_VERSION {
            get {
                return Application.version;
            }
        }

        private static int m_APP_NUMBER = 0;

		 /// <summary>
        /// app版本号转换成的number号，用于服务端版本号码对比
		/// 转换规则 v1.v2.v3
		/// v1*1000*1000 + v2*1000 +v3
        /// </summary>
		public static int APP_NUMBER
		{
			get{
				if(m_APP_NUMBER==0)
				{
					 var sp = SplitVersion (APP_VERSION);
                    if (sp.Length == 3) {
						int v1 = Convert.ToInt32(sp[0]);
						int v2 = Convert.ToInt32(sp[1]);
						int v3 = Convert.ToInt32(sp[2]);
                        m_APP_NUMBER = v1*BIT_SIZE*BIT_SIZE+v2*BIT_SIZE+v3;
                    } else if (sp.Length == 2) {
                       int v1 = Convert.ToInt32(sp[0]);
						int v2 = Convert.ToInt32(sp[1]);
                        m_APP_NUMBER = v1*BIT_SIZE*BIT_SIZE+v2*BIT_SIZE;
						#if UNITY_EDITOR
						Debug.LogError(" Application.version set wrong ,it must be like 1.0.1");
						#endif
                    } else {
                        throw new System.Exception ("Application.version set wrong ,it must be like 1.0.1");
                    }
				}
				return m_APP_NUMBER;
			}
            set
            {
                m_APP_NUMBER = 0;
            }
		}


        private static int m_RES_VERSION = 0;

        /// <summary>
        /// 资源版本号,小版本号
		/// 资源变更的时候手动修改
        /// </summary>
        public static int RES_VERSION {
            get {
                if (m_RES_VERSION == 0) {
                    var sp = SplitVersion (APP_VERSION);
                    if (sp.Length == 3) {
                        m_RES_VERSION = Convert.ToInt32 (sp[2]);
					 } else if (sp.Length == 2) {
                        m_RES_VERSION = 0;
						#if UNITY_EDITOR
						Debug.LogError(" Application.version set wrong ,it must be like 1.0.1");
						#endif
                    } 
                    else {
                        throw new System.Exception ("Application.version set wrong ,it must be like 1.0.1");
                    }
                }
                return m_RES_VERSION;
            }
            set{
                m_RES_VERSION = 0;
            }
        }

        //分离版本号信息
        private static string[] SplitVersion (string verion) {
            var sp = verion.Split ('.');
            return sp;
        }

    }

}
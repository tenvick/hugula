using UnityEngine;
using System;

namespace Hugula
{
    /// <summary>
    /// 代码版本号Res verion.
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CodeVersion
    {

        static CodeVersion()
        {
            m_CODE_VERSION = 0;
            m_APP_NUMBER = 0;
        }
        private const int BIT_SIZE = 1000;
        private static int m_CODE_VERSION;

        /// <summary>
        /// 校验版本号
		///	不一致会导致强更新
        /// </summary> 
        public static int CODE_VERSION
        {
            get
            {
                if (m_CODE_VERSION == 0)
                {
                    var sp = SplitVersion(APP_VERSION);
                    m_CODE_VERSION = sp[0] * BIT_SIZE * BIT_SIZE + sp[1] * BIT_SIZE;
                }
                return m_CODE_VERSION;
            }
            set
            {
                m_CODE_VERSION = 0;
            }
        }

        /// <summary>
        /// app版本号
        /// </summary>	
        public static string APP_VERSION
        {
            get
            {
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
            get
            {
                if (m_APP_NUMBER == 0)
                {
                    m_APP_NUMBER = CovertVerToInt(APP_VERSION);
                }
                return m_APP_NUMBER;
            }
            set
            {
                m_APP_NUMBER = 0;
            }
        }

        //分离版本号信息
        private static int[] SplitVersion(string verion)
        {
            if (string.IsNullOrEmpty(verion)) verion = string.Empty;

            var sp = verion.Split('.');
#if UNITY_EDITOR
            if (sp.Length < 3)
            {
                throw new System.Exception("Application.version set wrong ,it must be like 0.1.1");
            }
#endif
            int[] ints = new int[3];
            int outi = 0;
            for (int i = 0; i < sp.Length; i++)
            {
                if (int.TryParse(sp[i], out outi))
                    ints[i] = outi;
                else
                    ints[i] = 0;
            }
            return ints;
        }

        private static int CovertVerToInt(string version)
        {
            var ints = SplitVersion(version);
            var i = ints[0] * BIT_SIZE * BIT_SIZE + ints[1] * BIT_SIZE + ints[2];
            return i;
        }

        public static int Subtract(string ver1, string ver2)
        {
            var verInt1 = CovertVerToInt(ver1);
            var verInt2 = CovertVerToInt(ver2);
            return verInt1 - verInt2;
        }

    }

}
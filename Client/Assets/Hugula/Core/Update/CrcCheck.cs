using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Hugula.Loader;
using Hugula.Utils;

namespace Hugula.Update
{
    /// <summary>
    /// Crc check.
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CrcCheck
    {
        /// <summary>
        /// 根据组策略校验request.url的crc值。
        /// 文件不存在或者校验失败返回false
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool CheckUriCrc(CRequest req)
        {
            bool check = ManifestManager.CheckReqCrc(req); //CheckLocalFileCrc(req.url, out crc);
            if (!check)
            {
                var re = UriGroup.CheckAndSetNextUriGroup(req); //CUtils.SetRequestUri(req, 1);
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(req.key,"<color=#ffff00>CrcCheck.CheckUriCrc Req(assetname={0},url={1}) CheckFileCrc=false,SetNextUri={2}</color>", req.assetName, req.url, re);
#endif
                return re;
            }

            return true;
        }

        /// <summary>
        /// 获取文件crc
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static uint GetLocalFileCrc(string path, out uint l)
        {
            uint crc = 0;
            l = 0;
            try
            {
                FileInfo finfo = new FileInfo(path);
                if (finfo.Exists)
                {
                    using (FileStream fileStream = finfo.OpenRead())
                    {
                        byte[] array;
                        int num = 0;
                        long length = fileStream.Length;
                        int i = (int)length;
                        if (length > 2147483647L)
                        {
                            length = 2147483647L;
                        }
                        l = (uint)i;
                        array = new byte[i];
                        while (i > 0)
                        {
                            int num2 = fileStream.Read(array, num, i);
                            if (num2 == 0)
                            {
                                break;
                            }
                            num += num2;
                            i -= num2;
                        }
                        crc = Crc32.Compute(array);
                    }
                }
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning(e);
#endif
            }

            return crc;
        }
    }
}
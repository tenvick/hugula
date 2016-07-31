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
        /// crc校验对比值
        /// </summary>
        private static Dictionary<string, uint> crc32Dic = new Dictionary<string, uint>();
        /// <summary>
        /// crc验证列表
        /// </summary>
        private static Dictionary<string, uint> crcFileChecked = new Dictionary<string, uint>();

        public static bool ContainsKey(string key)
        {
            return crc32Dic.ContainsKey(key);
        }

        /// <summary>
        /// crc校验
        /// 1 crc==0 通过。
        /// 2 如果校验列表没值通过。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="crc"></param>
        /// <returns></returns>
        public static bool CheckCrc(string key, uint crc)
        {
            if (crc == 0) return true;
            uint out_crc;
            if (crc32Dic.TryGetValue(key, out out_crc))
            {
                return out_crc == crc;
            }
            return true;
        }

        /// <summary>
        /// 检测request第一个uri为persistentDataPath的crc值。
        /// 存在而且crc校验通过返回ture.
        /// 不是第一个返回true.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool CheckCrcUri0Exists(CRequest req)
        {
            string url = req.url;
            if (req.index == 0 && url.StartsWith(Application.persistentDataPath)) // && 
            {
                uint crc = 0;
                bool check = CheckFileCrc(req.url,out crc);
                if (!check)
                    return CUtils.SetRequestUri(req, 1);
            }

            return true;
        }

        /// <summary>
        /// 文件crc校验，
        /// 1 如果文件不存在不通过。
        /// 2 如果校验列表没值不通过。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckFileCrc(string path, out uint outCrc)
        {
            string key = CUtils.GetUDKey("", path);
            if (crcFileChecked.ContainsKey(key))
            {
                outCrc = crcFileChecked[key];
                return outCrc != 0;
            }

            string crcKey = CUtils.GetKeyURLFileName(path);
            bool ck = false;
            uint crc = GetFileCrc(path);
            outCrc = crc;
            if (crc == 0)//不存在
            {
                ck = false;
            }
            else if (ContainsKey(crcKey)) //存在校验值
            {
                ck = CheckCrc(crcKey, crc);
            }
            crcFileChecked[key] = crc;
            return ck;
        }

        /// <summary>
        /// 获取文件crc
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static uint GetFileCrc(string path)
        {
            uint crc = 0;

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

            return crc;
        }

        public static void Add(string key, uint crc)
        {
            crc32Dic[key] = crc;
        }

        public static bool Remove(string key)
        {
            return crc32Dic.Remove(key);
        }

        public static void Clear()
        {
            crcFileChecked.Clear();
            crc32Dic.Clear();
        }
    }
}
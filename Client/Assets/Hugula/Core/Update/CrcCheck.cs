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
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static uint GetCrc(string key)
        {
            uint out_crc = 0;
            crc32Dic.TryGetValue(key, out out_crc);
            return out_crc;
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
                return out_crc == crc || out_crc == 0;
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
                bool check = CheckFileCrc(req.url, out crc);
                if (!check)
                {
					var re = CUtils.SetRequestUri (req, 1);
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat("<color=yellow>CrcCheck.CheckCrcUri0Exists Req(assetname={0},url={1}) crc{2} setURI1=false</color>",req.assetName,req.url,crc,re);
					#endif
					return re;
                }
            }

            return true;
        }

        /// <summary>
        /// 文件crc强制校验，
        /// 
        /// 1 如果校验列表没值不通过。
        /// 1 文件不存在不通过。
        /// 1 校验列表值为0通过。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckFileCrc(string path, out uint fileCrc)
        {
            string crcKey = CUtils.GetKeyURLFileName(path);
            bool ck = false;
            fileCrc = 0;
            uint sourceCrc = 0;

            if (crc32Dic.TryGetValue(crcKey, out sourceCrc)) //存在校验值
            {
                if (sourceCrc == 0)//原始为0表示不校验
                {
                    ck = File.Exists(path);
                }
                else
                {
                    string key = CUtils.GetUDKey("", path);
                    uint checkedCrc;
                    if (crcFileChecked.TryGetValue(key, out checkedCrc) && checkedCrc != 0) //如果存在
                    {
                        fileCrc = checkedCrc;
                    }
                    else
                    {
                        fileCrc = GetFileCrc(path);//读取文件crc
                        crcFileChecked[key] = fileCrc;//保存文件crc
                    }
                    ck = sourceCrc == fileCrc;//校验
                }
//                Debug.LogWarning(string.Format("sourceCrc{0},filecrc{1},ck{2},path{3},crcKey{4}", sourceCrc, fileCrc, ck, path, crcKey));
            }
            else
            {
                ck = false;
            }
            return ck;
        }

        /// <summary>
        /// 文件crc
        /// 1 如果校验列表没值并且文件存在返回false
        /// 2 如果校验列表没值并且文件不存在返回true
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileCrc"></param>
        /// <returns></returns>
        public static bool CheckFileWeakCrc(string path, out uint fileCrc)
        {

            string crcKey = CUtils.GetKeyURLFileName(path);
            bool ck = false;
            uint sourceCrc = 0;
            fileCrc = GetFileCrc(path);//读取文件crc

            if (crc32Dic.TryGetValue(crcKey, out sourceCrc)) //存在校验值
            {
                ck = sourceCrc == fileCrc;//校验
            }
            else
            {
                ck = fileCrc == 0;
            }
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
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
        public static bool beginCheck = false;
        /// <summary>
        /// crc校验对比值
        /// </summary>
        private static Dictionary<string, uint> crc32Dic = new Dictionary<string, uint>();
        /// <summary>
        /// crc验证列表
        /// </summary>
        private static Dictionary<string, uint> crcFileChecked = new Dictionary<string, uint>();

        /// <summary>
        /// crc key与文件校验值的映射。
        /// </summary>
        // private static Dictionary<string, HashSet<string>> crcKeyCheckedFileMap = new Dictionary<string, HashSet<string>>();

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
        /// 根据组策略校验request.url的crc值。
        /// 文件不存在或者校验失败返回false
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool CheckUriCrc(CRequest req)
        {
            uint crc = 0;
            bool check = CheckLocalFileCrc(req.url, out crc);
            if (!check)
            {
                var re = UriGroup.CheckAndSetNextUriGroup(req); //CUtils.SetRequestUri(req, 1);
#if HUGULA_LOADER_DEBUG
                Debug.LogFormat("<color=#ff0000>CrcCheck.CheckUriCrc Req(assetname={0},url={1}) crc={2},CheckFileCrc=false,SetNextUri={3}</color>", req.assetName, req.url, crc, re);
#endif
                return re;
            }

            return true;
        }

        /// <summary>
        /// 本地文件crc强制校验，
        /// 
        /// 1 如果校验列表没值不通过。
        /// 1 文件不存在不通过。
        /// 1 校验列表值为0通过。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckLocalFileCrc(string path, out uint fileCrc)
        {
            fileCrc = 0;
            if (path.StartsWith(Common.HTTP_STRING)) return true;

            string crcKey = CUtils.GetAssetBundleName(path);
            bool ck = false;
            uint sourceCrc = 0;
            bool fromcache = false;
            // Debug.LogFormat("CheckLocalFileCrc:trygetvalue{0},crcKey={1},sourcecrc={2},path={3}",crc32Dic.TryGetValue(crcKey, out sourceCrc),crcKey,sourceCrc,path);
            if (crc32Dic.TryGetValue(crcKey, out sourceCrc)) //存在校验值
            {
                if (sourceCrc == 0)//原始为0表示不校验
                {
                    if (path.StartsWith(Application.persistentDataPath) && !File.Exists(path))
                        ck = false;
                    else
                        ck = true;
                }
                else
                {
                    string key = CUtils.GetUDKey(path);
                    uint checkedCrc;
                    if (crcFileChecked.TryGetValue(key, out checkedCrc) && checkedCrc != 0) //如果存在
                    {
                        fileCrc = checkedCrc;
                        fromcache = true;
                    }
                    else
                    {
                        uint fileSize = 0;
                        fileCrc = GetLocalFileCrc(path, out fileSize);//读取文件crc
                        crcFileChecked[key] = fileCrc;//保存文件crc
                    }
                    ck = sourceCrc == fileCrc;//校验
                }
#if HUGULA_LOADER_DEBUG
                if (ck)
                    Debug.LogFormat(" 0.0. crc <color=#00ff00>sourceCrc({0}==filecrc{1}),return {2},path{3},crcKey{4},from cache={5}</color>", sourceCrc, fileCrc, ck, path, crcKey,fromcache);
                else
                    Debug.LogFormat(" 0.0. crc <color=#ffff00>sourceCrc({0}!=filecrc{1}),return {2},path{3},crcKey{4}from cache={5}</color>", sourceCrc, fileCrc, ck, path, crcKey,fromcache);
#endif
            }
            else// if (!beginCheck)
            {
                if (path.StartsWith(Application.persistentDataPath) && !File.Exists(path))
                    ck = false;
                else
                    ck = true;
            }
            // else
            // {
            //     ck = false;
            // }
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
        public static bool CheckLocalFileWeakCrc(string path, out uint fileCrc)
        {

            string crcKey = CUtils.GetAssetBundleName(path);
            bool ck = false;
            uint sourceCrc = 0;
            uint l = 0;
            fileCrc = GetLocalFileCrc(path, out l);//读取文件crc

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

        public static void Add(string key, uint crc)
        {
            // ClearCrcCheckedMap(key);
            crc32Dic[key] = crc;
        }

        public static bool Remove(string key)
        {
            // ClearCrcCheckedMap(key);
            return crc32Dic.Remove(key);
        }

        public static void Clear()
        {
            // crcKeyCheckedFileMap.Clear();
            crcFileChecked.Clear();
            crc32Dic.Clear();
        }

        // private static void ClearCrcCheckedMap(string crcKey)
        // {
        //     HashSet<string> checkMap = null;
        //     if (crcKeyCheckedFileMap.TryGetValue(crcKey, out checkMap))
        //     {
        //         foreach (var k in checkMap)
        //             crcFileChecked.Remove(k);
        //         crcKeyCheckedFileMap.Remove(crcKey);
        //     }
        // }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Hugula.Utils;

namespace Hugula.ResUpdate
{
    /// <summary>
    /// Crc check.
    /// </summary>
    
    public static class CrcCheck
    {

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
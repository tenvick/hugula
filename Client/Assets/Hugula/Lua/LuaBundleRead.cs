using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using Hugula.Utils;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Bindings;
using System.Runtime.InteropServices;
using System;

namespace Hugula
{
    [XLua.DoNotGen]
    public interface ILuaBytesRead
    {
        byte[] LoadBytes(string name, ref int length);
        void Unload();
    }
    /// <summary>
    /// 读取bundle中的lua bytes
    /// </summary>
    internal class LuaBundleRead : ILuaBytesRead
    {

        private AssetBundle m_LuaBundle;


        public LuaBundleRead(string path)
        {

            m_LuaBundle = AssetBundle.LoadFromFile(path, 0, Common.BUNDLE_OFF_SET);
        }

        public void Unload()
        {
            m_LuaBundle?.Unload(true);

        }

        public byte[] LoadBytes(string name, ref int length)
        {
            byte[] ret = null;
            length = 0;
            var txt = m_LuaBundle?.LoadAsset<TextAsset>(name);
            if (txt != null)
            {
                ret = txt.bytes;
                length = ret.Length;
                Resources.UnloadAsset(txt); //释放ab资源
            }
            return ret;
        }
    }

    /// <summary>
    /// 读取streaming目录lua bytes
    /// </summary>
    internal class StreamingLuaBytesRead : ILuaBytesRead
    {
        private string m_Path;
        public StreamingLuaBytesRead(string path)
        {
            m_Path = path;

        }

        public void Unload()
        {

        }

        public byte[] LoadBytes(string name, ref int length)
        {
            length = 0;
            var buffer = LuaBytesBuffer.buffer;
            var path = ValueStrUtils.ConcatNoAlloc(m_Path, "/", name, ".bytes");     // Path.Combine(m_Path, name+ ".bytes");
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    int len = (int)fileStream.Length;
                    if (len > buffer.Length) //grow
                    {
                        buffer = new byte[len];
                        LuaBytesBuffer.buffer = buffer;
                    }
                    int nread = 0;
                    while (nread < len)
                    {
                        nread += fileStream.Read(buffer, nread, len - nread);
                    }
                    length = nread;
                }
            }
            return buffer;
        }
    }

    /// <summary>
    /// 读取全路径lua text
    /// </summary>
    internal class FileLuaTextRead : ILuaBytesRead
    {
        public FileLuaTextRead()
        {
        }

        public void Unload()
        {

        }

        public byte[] LoadBytes(string path, ref int length)
        {
            length = 0;
            var buffer = LuaBytesBuffer.buffer;
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    int len = (int)fileStream.Length;
                    if (len > buffer.Length) //grow
                    {
                        buffer = new byte[len];
                        LuaBytesBuffer.buffer = buffer;
                    }
                    int nread = 0;
                    while (nread < len)
                    {
                        nread += fileStream.Read(buffer, nread, len - nread);
                    }
                    length = nread;
                }
            }
            return buffer;
        }
    }


#if UNITY_ANDROID  && !UNITY_EDITOR

    /// <summary>
    /// android apk时候读取streamingasset目录下资源 
    /// </summary>
    internal class AndroidLuaRead: ILuaBytesRead
    {
    #region static

        const string libName = "unitystreaming";


        [DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ReadAssetsBytes(string name, ref IntPtr ptr);


        [DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ReadAssetsBytesWithOffset(string name, ref IntPtr ptr, int offset, int length);

        [DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadRawBytes(string name, ref IntPtr ptr);

        [DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseBytes(IntPtr ptr);
    #endregion


        string m_Path;

        public AndroidLuaRead(string rootpath)
        {
           this.m_Path = rootpath;
        }

        public void Unload()
        {

        }

        public byte[] LoadBytes(string name,ref int length)
        {
            var path = ValueStrUtils.ConcatNoAlloc(m_Path, "/", name, ".bytes");
            var ptr = IntPtr.Zero;
            var len = ReadAssetsBytes(path,ref ptr); //ReadAssetsBytesWithOffset(path,ref ptr, sizeof(int), sizeof(int));
            var buffer = LuaBytesBuffer.buffer;
            if (len > 0)
            {
                length = len;
                if (len > buffer.Length) //grow
                {
                    buffer = new byte[len];
                    LuaBytesBuffer.buffer = buffer;
                }
                if (ptr == IntPtr.Zero)
                {
                    Debug.LogError("Read Failed!!!");
                }
                Marshal.Copy(ptr, buffer, 0, len);
                ReleaseBytes(ptr);
            }
            else
            {
                length = 0; 
            }
            return buffer;
        }

    }
#endif

    internal static class LuaBytesBuffer
    {
        internal static byte[] buffer = new byte[1024 * 1024 * 2]; //2M空间 1024*1024

    }
}



// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using SLua;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

/// <summary>
/// 
/// </summary>
[SLua.CustomLuaClass]
public class FileHelper
{
    #region zip
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="outPath"></param>
    public static void UnZipFile(string path, string outPath)
    {
        if (File.Exists(path))
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(path)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Debug.Log("file name" + theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    //Debug.Log(string.Format("directoryName:{0},fileName:{1}",directoryName,fileName));
                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("path :" + path + " is not exists " + outPath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="outPath"></param>
    public static void UnZipFile(Stream stream, string outPath)
    {
        using (ZipInputStream s = new ZipInputStream(stream))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);
                //Debug.Log(string.Format("directoryName:{0},fileName:{1}",directoryName,fileName));
                // create directory
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(outPath + "/" + directoryName);
                }

                if (fileName != String.Empty)
                {
                    //Debug.Log(outPath + "/" + theEntry.Name);
                    using (FileStream streamWriter = File.Create(outPath + "/" + theEntry.Name))
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="outPath"></param>
    public static void UnZipFile(byte[] bytes, string outPath)
    {
        using (MemoryStream m = new MemoryStream(bytes))
        {
            UnZipFile(m, outPath);
        }
    }

    #endregion

    /// <summary>
    /// Unpacks the text zip.
    /// </summary>
    /// <param name='strem'>
    /// Strem.
    /// </param>
    /// <param name='ontz'>
    /// Ontz.
    /// </param>
    public static void UnpackConfigZip(byte[] bytes, System.Action<string,string> ontz)
    {
        using (MemoryStream m = new MemoryStream(bytes))
        {
            ZipFile z = new ZipFile(m);
            //z.Password = Common.CONFIG_ZIP_PWD;
            foreach (ZipEntry ze in z)
            {
                if (ze.IsFile)
                {
                    Stream str = z.GetInputStream(ze);
                    using (StreamReader sr = new StreamReader(str))
                    {
                        string conext = sr.ReadToEnd();
                        if (ontz != null)
                            ontz(ze.Name, conext);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Save context in persistentDataPath
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fileName"></param>
    public static void PersistentUTF8File(string context, string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        //UnityEngine.Debug.Log("PersistentUTF8File " + path);
        if (!File.Exists(path))
        {
            using (StreamWriter sr = File.CreateText(path))
            {
                sr.Write(context);
            }
        }
        else
        {
            using (StreamWriter sr = new StreamWriter(path, false, Encoding.UTF8))
            {
                sr.Write(context);
            }
        }
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="fileAbsPath"></param>
    /// <returns></returns>
    public static string ReadUTF8File(string fileAbsPath)
    {
        string re = null;
        string path = Application.persistentDataPath + "/" + fileAbsPath;
        if (File.Exists(path))
        {
            using (StreamReader sr = new StreamReader(path))
            {
                re = sr.ReadToEnd();
            }
        }

        return re;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="fileAbsPath"></param>
    public static void DeleteFile(string fileAbsPath)
    {
        string path = Application.persistentDataPath + "/" + fileAbsPath;

        if (File.Exists(path)) File.Delete(path);
    }

    private static LuaFunction callBackFn;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ab"></param>
    /// <param name="luaFn"></param>
    public static void UnpackConfigAssetBundleFn(AssetBundle ab, LuaFunction luaFn)
    {
        callBackFn = luaFn;
#if UNITY_5
        UnityEngine.Object[] all = ab.LoadAllAssets();
#else
        UnityEngine.Object[] all = ab.LoadAll();
#endif
        foreach (UnityEngine.Object i in all)
        {
            if (i is TextAsset)
            {
                TextAsset a = (TextAsset)i;
                if (callBackFn != null)
                    callBackFn.call(a.name, a.text);
            }
        }
    }     
}

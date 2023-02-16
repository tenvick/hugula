using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using System.IO;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace PSDUINewImporter
{

    public class PSDDirectoryUtility
    {

        protected static Dictionary<string, Dictionary<string, string>> m_DirectoryCache = new Dictionary<string, Dictionary<string, string>>();

        public static void ClearCache()
        {
            m_DirectoryCache.Clear();
            Debug.Log("PSDDirectoryUtility.Clear");
        }

        /// <summary>
        /// searche prefab
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string SearAttachedPrefab(string folder, string fileName)
        {
            if(!m_DirectoryCache.TryGetValue(folder,out var firectoryFound))
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var files = System.IO.Directory.GetFiles(folder, "*.*", System.IO.SearchOption.AllDirectories);
                firectoryFound = new Dictionary<string, string>();
                foreach (var f in files)
                {
                    if (!(Path.GetExtension(f) == ".meta"))
                    {
                        firectoryFound[Path.GetFileName(f)] = f;
                        firectoryFound[Path.GetFileNameWithoutExtension(f)] = f;
                    }
                }

                m_DirectoryCache[folder] = firectoryFound;
            }

            var fileFullPath = string.Empty;
            if(firectoryFound.TryGetValue(fileName, out fileFullPath))
                fileFullPath = fileFullPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            else
                Debug.LogWarning($"prefab:{fileName} miss in directory {folder}");

            return fileFullPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FindFileInDirectory(string baseDirectory, string fileName)
        {
            if (!m_DirectoryCache.TryGetValue(baseDirectory, out var firectoryFound))
            {
                if (!Directory.Exists(baseDirectory)) Directory.CreateDirectory(baseDirectory);
                var files = System.IO.Directory.GetFiles(baseDirectory, "*.*", System.IO.SearchOption.AllDirectories);
                firectoryFound = new Dictionary<string, string>();
                foreach (var f in files)
                {
                    if(!(Path.GetExtension(f) == ".meta"))
                    {
                        firectoryFound[Path.GetFileName(f)] = f;
                        firectoryFound[Path.GetFileNameWithoutExtension(f)] = f;
                    }
                }

                m_DirectoryCache[baseDirectory] = firectoryFound;
            }

            var fileFullPath = string.Empty;
            if (firectoryFound.TryGetValue(fileName, out fileFullPath))
                fileFullPath = fileFullPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            else
                Debug.LogError($"file:{fileName} miss in directory {baseDirectory}");
            return fileFullPath;
        }
    }
}
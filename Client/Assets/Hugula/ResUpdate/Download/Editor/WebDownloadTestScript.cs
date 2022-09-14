using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.IO;
using System.Net;
using System.Threading;
using Hugula.Collections;
using Hugula.Utils;
using Hugula.Loader;
namespace Tests
{
    public class UsingTryCatchTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void UsingTryCatchTestScriptSimplePasses()
        {
            string urlStr = "http://10.23.0.56/L2_LocalTestCDN_Path/Trunk/win/packages/ui_model_heros_1242960889.zip";
            // Use the Assert class to test conditions
            string tmp = Path.Combine(CUtils.GetRealPersistentDataPath(), "ui_model_heros_1242960889.zip");
            Debug.Log(tmp);
            Debug.Log(File.Exists(tmp));

            var url = new Uri(urlStr);
            var download = new  WebDownload();
            string path = CUtils.PathCombine(CUtils.GetRealPersistentDataPath() + "/", "ui_model_heros_1242960889.zip");
            download.DownloadFileCompleted=(object obj, System.ComponentModel.AsyncCompletedEventArgs args)=>
            {
                Debug.Log(obj);
                if(args.Error!=null)
                    Debug.LogException(args.Error);
            };
            download.DownloadFileMultiAsync(url, path);
            // download.DownloadFileMultiAsync(url, path);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator UsingTryCatchTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}

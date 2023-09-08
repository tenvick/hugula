using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hugula.UI;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace Hugula.ResUpdate
{

    ///<summary>
    ///  ------------------------------------------------
    /// --  Copyright © 2015-2022   Hugula: Arpg game Engine
    /// --  热更新
    /// --  author pu
    /// --	1 读取本地streaming foldermanifest文件。
    /// -- 	2 读取可持续目录foldermanifest文件并合并到 streaming foldermanifest用于后面对比热更新资源
    /// --	3 加载远端version.json 地址在Hugula/setting中http ver host配置, version.json内容配置位于Assets/Hugula/Config/Version目录
    /// --	4 判断是否加载fast_{appversion}.zip 包。  此处加载url配置位于Assets/Hugula/Config/Resources/version.txt中的cdn_host字段，会在发布阶段自动生成。
    /// --	5 fast_{appversion}.zip 包解压。 所有已经下载完成的zip包中的assetbundle重定向地址配置。
    /// --	6 对比远端版本号 判断下载远端foldermanifest文件。
    /// --	7 对比本地与远端foldermanifest文件下载变更的文件
    /// --	8 下载完成，保存远端foldermaifest到可持续化目录，更新变更文件的assetbundle重定向地址配置。
    /// --	9 完成下载进入游戏。
    /// --
    /// --	注意事项
    /// --  .1 hugulasetting中的http ver host可以支持多个地址配置以“,”号分割
    /// --	.2 BackGroundDownload 是网络加载模块，用于从网络加载URL并保存到persistent目录，大于4m的资源默认开启2个线程同时下载，如果要支持断点续传服务器需要开启etag。
    /// --	.3 version.json 版本信息，可以控制fast包的加载时机。
    /// --	.4 cdn_hosts 可以支持多个域名加载
    /// ------------------------------------------------
    ///</summary>
    public class HotUpdate : MonoBehaviour
    {
#if UNITY_EDITOR
        const string KeyDebugString = "_HotUpdate_Debug_string";
        //编辑器模式下是否开启热更新加载测试
        [XLua.DoNotGen]
        public static bool isDebug
        {
            get
            {
                bool _debug = UnityEditor.EditorPrefs.GetBool(KeyDebugString, false);
                return _debug;
            }
            set
            {
                UnityEditor.EditorPrefs.SetBool(KeyDebugString, value);
            }
        }
#endif
        public HotUpdateView view;

        bool romoteVersionTxtLoaded = false;
        string UPDATED_TEMP_LIST_NAME
        {
            get
            {
                return Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME + ".tmp";
            }
        }
        const string SCENE_NAME = "s_begin";

        #region  mono

        // Start is called before the first frame update
        IEnumerator Start()
        {
            // BackGroundDownload.Init();
            TLogger.Init();
            FileManifestManager.OverrideInternalIdTransformFunc(); //重定向热更新的文件
            FileManifestManager.LoadStreamingFolderManifests(null);//读取本地文件列表
            yield return null;
            FileManifestManager.LoadPersistentFolderManifest(null);//读取持久化目录的列表
            yield return null;
            SetVersionInfo();
            Hugula.Localization.language = PlayerPrefs.GetString(Hugula.Localization.KEY_LAN, Application.systemLanguage.ToString());

#if  UNITY_ANDROID && !UNITY_EDITOR
            if (FileManifestManager.streamingFolderManifest == null) {
                //todo 资源丢失 
            }
#endif
#if UNITY_EDITOR
            yield return null;
            //如果非ab模式
            List<ScriptableObject> allDataBuilders = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.DataBuilders;
            var activeIndex = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex;
            var curIndex = allDataBuilders.IndexOf(allDataBuilders.Find(s => s.GetType() == typeof(UnityEditor.AddressableAssets.Build.DataBuilders.BuildScriptPackedPlayMode)));

            bool check = activeIndex == curIndex;
            if (isDebug && check)
                yield return LoadRemoteVersion();
            else
            {

                yield return InternalIdTransformFunc();
            }
#else
            yield return LoadRemoteVersion ();
#endif
        }


        #endregion

        #region  hot update logic
        //加载远程版本文件
        public IEnumerator LoadRemoteVersion(int count = 0)
        {
            var hosts = GetVersionUrl();
            string response = null;
            var url = string.Empty;
            //读取本地缓存
            string versionName = Path.GetFileNameWithoutExtension(Common.CRC32_VER_FILENAME);
#if !(HUGULA_RELEASE || SDK_RELEASE)
            versionName = "dev_" + versionName;
#endif
            VersionConfig localVersion = null;
            var versionTxt = Resources.Load<TextAsset>(versionName); //优先读取本地
            if (versionTxt)
            {
                localVersion = JsonUtility.FromJson<VersionConfig>(versionTxt.text);
                Resources.UnloadAsset(versionTxt);
            }
            foreach (var host in hosts)
            {
                url = host;
                UnityWebRequest req = UnityWebRequest.Get(host);
                var async = req.SendWebRequest();
                yield return async;
                if (req.responseCode == 200)
                {
                    response = req.downloadHandler.text;
                    break;
                }
                else
                {
                    Debug.LogErrorFormat("UnityWebRequest error code = {0},error = {1}.", req.responseCode, req.error);
                    yield return null;
                }
            }

            if (!string.IsNullOrEmpty(response))
            {
                yield return ParseVerionConfigAndCheckUpdate(response, localVersion);
            }
            else if (!romoteVersionTxtLoaded)
            {
                yield return new WaitForSeconds(1);

                if (count == 0)
                {
                    MessageBox.Show(url + Localization.Get("main_download_fail"), "", Localization.Get("main_check_sure"), () =>
                        {
                            MessageBox.Close();
                            StartCoroutine(LoadRemoteVersion(1));
                        });
                    Debug.LogError($" {url}远程json 解析失败!");

                }
                else
                {
                    if (localVersion != null)
                    {
                        Debug.LogWarning($" {url} json parse error! Resources({versionName}) load success!");
                        StartCoroutine(ParseVerionConfigAndCheckUpdate(string.Empty, localVersion));
                    }
                    else
                    {
                        MessageBox.Show(url + Localization.Get("main_download_fail"), "", Localization.Get("main_check_sure"), () =>
                        {
                            MessageBox.Close();
                            StartCoroutine(LoadRemoteVersion());
                        });

                        Debug.LogError($"Resources({versionName}) load fail, {url} json parse error!");
                    }

                }

            }

            yield return null;
        }

        /// <summary>
        /// 解析VerionConfig 判断提示热跟新
        /// </summary>
        IEnumerator ParseVerionConfigAndCheckUpdate(string response, VersionConfig localVersion)
        {
            TLogger.LogSys(response);
            romoteVersionTxtLoaded = true;
            VersionConfig remoteVer = null;
            if (!string.IsNullOrEmpty(response))
                remoteVer = JsonUtility.FromJson<VersionConfig>(response);

            if (remoteVer == null)
                remoteVer = localVersion;
            else
            {
                remoteVer.EmptyFieldOverride(localVersion);
            }

            var cdn_hosts = remoteVer.cdn_host;
            BackGroundDownload.rootHosts = cdn_hosts;
            FileManifestManager.otherZipMode = remoteVer.other;//使用远端下载模式
            //check force download full app
            if (CodeVersion.Subtract(remoteVer.force_ver, CodeVersion.APP_VERSION) > 0) //强制更新版本号大于本地版本号
            { //强制更新提示
                MessageBox.Show(Localization.Get("main_download_new_app"), "", Localization.Get("main_check_sure"), () =>
                {
                    MessageBox.Destroy();
                    Application.OpenURL(remoteVer.update_url);
                });
            }
            else
            {
                //提示更新 远端 notice_ver提示更新版本号 》大于本地版本号 提示更新
                if (CodeVersion.Subtract(remoteVer.notice_ver, CodeVersion.APP_VERSION) > 0)
                {
                    MessageBox.Show(Localization.Get("main_check_update"), "", Localization.Get("main_check_sure"), () =>
                    {
                        MessageBox.Destroy();
                        Application.OpenURL(remoteVer.update_url);
                    },
                    Localization.Get("main_check_cancel"), () =>
                     {
                         MessageBox.Destroy();
                         StartCoroutine(CheckHotUpdate(remoteVer));
                     }
                    );

                }
                else
                    yield return CheckHotUpdate(remoteVer);
            }
        }

        IEnumerator CheckHotUpdate(VersionConfig remoteVer)
        {
            //判断是否等待fast包
            yield return CheckLoadFast(remoteVer);
            var appVerNum = CodeVersion.CovertVerToInt(CodeVersion.APP_VERSION);
            var maxVerNum = CodeVersion.CovertVerToInt(remoteVer.version);
            var minVerNum = CodeVersion.CovertVerToInt(remoteVer.min_ver);
            if (minVerNum <= 0) minVerNum = maxVerNum;
            var verCheck = appVerNum >= minVerNum && appVerNum <= maxVerNum;
            var subResNum = remoteVer.res_number > FileManifestManager.localResNum;
#if !HUGULA_NO_LOG
            Debug.Log($"CheckHotUpdate verCheck:{verCheck},subResNum:{subResNum},appVerNum:{appVerNum},minVerNum:{minVerNum},maxVerNum:{maxVerNum},manifest_name:{!string.IsNullOrEmpty(remoteVer.manifest_name)} ");
#endif
            if (verCheck && subResNum && !string.IsNullOrEmpty(remoteVer.manifest_name))//如果app.version相同还需要判断ResNum
                yield return LoadRemoteFoldmanifest(remoteVer);//下载热更新文件
            else
                yield return InternalIdTransformFunc();
        }

        #region  加载fast包
        ///<summary>
        /// 判断是否等待fast加载完毕
        ///</summary>
        IEnumerator CheckLoadFast(VersionConfig remoteVer)
        {
            if (remoteVer.fast == FastMode.sync) //需要等待fast包下载完成
            {
                // LoadFolderManifest
                var fastManifest = FileManifestManager.FindStreamingFolderManifest("fast") as FolderManifest;
                if (fastManifest != null && !fastManifest.isZipDone)//如果fast包没有加载
                {
                    var change = BackGroundDownload.instance.AddZipFolderManifest(fastManifest, onFastProccessChanged, onFastComplete, null);
                    if (change > 0)
                    {
                        var is_wifi = Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork;
                        UnityEngine.Events.UnityAction BeginLoad = () =>
                        {
                            BackGroundDownload.MaxLoadCount = 2;
                            BackGroundDownload.instance.Begin();
                            MessageBox.Destroy();
                        };
                        if (is_wifi)
                        {
                            BeginLoad();
                        }
                        else
                        {
                            var tips = Localization.GetFormat("main_download_from_webserver", string.Format("{0:.00}", (float)change / 1048576f));
                            MessageBox.Show(tips, "", Localization.Get("main_check_sure"), BeginLoad); //提示手动下载
                        }

                        while (!fastManifest.isZipDone) //等待zip下载完成
                            yield return null;
#if !HUGULA_NO_LOG
                        Debug.Log($"fast 解压完成:{fastManifest.zipMarkPathName} ");

#endif
                    }
                    else
                    {
                        Debug.Log($"{fastManifest.zipName}.zip :大小为0!");
                    }
                }
                else
                {
#if !HUGULA_NO_LOG
                    if (fastManifest == null)
                        Debug.Log("没有fast 包 ");
                    else
                    {
                        Debug.Log($" fast 包已经下载:{fastManifest.zipMarkPathName} ");
                    }
#endif

                }
                yield return null;
            }
        }

        void onFastProccessChanged(LoadingEventArg arg)
        {
            var m = 1024 * 1024f;
            var loaded_s = string.Format("{0:0.00}", (float)arg.current / m);
            var loaded_t = string.Format("{0:0.00}", (float)arg.total / m);
            var kbs = string.Format("{0:0.00} kb/s", (float)BackGroundDownload.BytesReceivedPerSecond / 1024f);
            var str = "fast " + Localization.GetFormat("main_downloading_tips", loaded_s, loaded_t, kbs) + System.DateTime.Now;
            SetSliderProgress(str, (float)arg.current / (float)arg.total);
        }

        void onFastComplete(FolderManifestQueue queue, bool isError)
        {
            var folderManifest = queue.currFolder;
#if !HUGULA_NO_LOG
            Debug.Log($"onFastComplete {isError} zipMarkName={folderManifest.zipMarkPathName},zipName={folderManifest.zipName} {folderManifest.ToString()}");
#endif
            if (!isError)
            {
                //default 
                // var fastManifest = FileManifestManager.FindStreamingFolderManifest("fast");
                // FileManifestManager.GenZipPackageTransform(fastManifest);
            }
            else
            {
                UnityEngine.Events.UnityAction BeginLoad = () =>
                       {
                           BackGroundDownload.MaxLoadCount = 2;
                           BackGroundDownload.instance.ReLoadErrorGroup(queue.group);
                           BackGroundDownload.instance.Begin();
                           MessageBox.Destroy();
                       };
                MessageBox.Show("fast pack download failed, please check your network!", "", "", BeginLoad);
            }
        }

        #endregion

        #region  加载远端remote foldermanifest文件与热更新资源  
        string loadRemoteVersion = string.Empty; //将要下载的远端文件    
        int loadRemoteAppNum = 0;//将要下载的远端文件appnum 
        //加载远端FolderManifest文件
        public IEnumerator LoadRemoteFoldmanifest(VersionConfig remoteVer)
        {
            yield return null;
            SetProgressTxt(Localization.Get("main_web_server_crc_list")); //--加载服务器校验列表。")
            var url = Path.Combine(remoteVer.cdn_host[0], remoteVer.manifest_name); //server_ver.cdn_host[1] .. "/" .. file_name
            Debug.Log($"begin get url ：{url}");

            UnityWebRequest req = UnityWebRequest.Get(url);
            string path = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), UPDATED_TEMP_LIST_NAME);
            req.downloadHandler = new DownloadHandlerFile(path);
            var async = req.SendWebRequest();
            yield return async;
            if (req.responseCode == 200)
            {
                SetProgressTxt(Localization.Get("main_compare_crc_list")); //校验列表对比中。"
                var ab = AssetBundle.LoadFromFile(path, 0, Common.BUNDLE_OFF_SET);
                var all = ab.LoadAllAssets<FolderManifest>();
                var totalRemoteFolder = ScriptableObject.CreateInstance<FolderManifest>();
                var totalLocalFolder = ScriptableObject.CreateInstance<FolderManifest>(); ;
                FolderManifest remoteFolder = null;
                FolderManifest localFolder = null;

                //合并所有的foldermanifest用于热更新下载对比
                for (int i = 0; i < all.Length; i++)
                {
                    remoteFolder = all[i];
                    loadRemoteVersion = remoteFolder.version;//记录要下载的远端版本号
                    loadRemoteAppNum = remoteFolder.resNumber;
                    totalRemoteFolder.version = remoteFolder.version;
                    totalRemoteFolder.resNumber = remoteFolder.resNumber;
                    totalRemoteFolder.AppendFolder(remoteFolder);
                    localFolder = FileManifestManager.FindStreamingFolderManifest(remoteFolder.fileName) as FolderManifest;
                    if (localFolder != null)
                    {
                        totalLocalFolder.AppendFolder(localFolder);
                        var curPersistent = FileManifestManager.FindPersistentFolderManifest(remoteFolder.fileName);
                        if (curPersistent != null) totalLocalFolder.AppendFolder(curPersistent);
                    }
                }

                ab.Unload(false);
                Debug.Log($"remote file({url}) is down");
#if !HUGULA_NO_LOG
                Debug.Log(totalRemoteFolder.ToString());
#endif
                // FolderManifest curStreaming = null;
                List<FolderManifest> needDownload = new List<FolderManifest>();
                totalRemoteFolder.RemoveSameFileResInfoFrom(totalLocalFolder);
                needDownload.Add(totalRemoteFolder);
                #if !HUGULA_NO_LOG
                    Debug.Log($"log update Folder \r\folder={totalLocalFolder.ToString()}\r\n remoteFolder:{remoteFolder.ToString()}");
                #endif
                //                 for (int i = 0; i < remoteFolderManifest.Count; i++)
                //                 {
                //                     remoteFolder = remoteFolderManifest[i];
                //                     loadRemoteVersion = remoteFolder.version;//记录要下载的远端版本号
                //                     loadRemoteAppNum = remoteFolder.resNumber;
                //                     curStreaming = FileManifestManager.FindStreamingFolderManifest(remoteFolder.fileName) as FolderManifest;

                //                     if (curStreaming != null) //需要对比下载
                //                     {
                //                         var downLoadFolder = remoteFolder.CloneWithOutAllFileInfos();
                //                         remoteFolder.RemoveSameFileResInfoFrom(curStreaming); //去重本地包内内容
                //                         //查找当前缓存目录
                //                         var curPersistent = FileManifestManager.FindPersistentFolderManifest(remoteFolder.fileName);
                //                         if (curPersistent != null)
                //                         {
                //                             downLoadFolder.allFileInfos = curPersistent.NotSafeCompare(remoteFolder);
                //                         }
                //                         else
                //                             downLoadFolder.allFileInfos = remoteFolder.allFileInfos;


                //                         // FileManifestManager.CheckAddOrUpdatePersistentFolderManifest(remoteFolder);

                //                         if (downLoadFolder.allFileInfos.Count > 0)
                //                         {
                //                             needDownload.Add(downLoadFolder);
                // #if !HUGULA_NO_LOG
                //                             Debug.Log($"need update Folder {downLoadFolder.ToString()}\r\folder={curStreaming.ToString()}\r\n remoteFolder:{remoteFolder.ToString()}");
                // #endif
                //                         }
                //                     }
                //                 }

                //开始加载热更新文件
                var change = BackGroundDownload.instance.AddFolderManifests(needDownload, OnHotResProccessChanged, OnBackgroundComplete, OnBackgroundAllComplete);
                Debug.Log("need load file size:" + change);
                if (change > 0)
                {
                    var is_wifi = Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork;
                    UnityEngine.Events.UnityAction BeginLoad = () =>
                    {
                        BackGroundDownload.MaxLoadCount = 4;
                        BackGroundDownload.instance.Begin();
                        MessageBox.Destroy();
                        Debug.Log($"begin load file size:{change} ");
                    };
                    if (is_wifi)
                    {
                        BeginLoad();
                    }
                    else
                    {
                        var tips = Localization.GetFormat("main_download_from_webserver", string.Format("{0:.00}", (float)change / 1048576));
                        MessageBox.Show(tips, "", "", BeginLoad); //提示手动下载
                    }
                }
                else
                    OnBackgroundAllComplete(null, false); //无需下载直接完成
            }
            else
            { //加载失败重新加载
                yield return null;
                var tips = Localization.Get("main_web_server_error");
                MessageBox.Show(tips, "", Localization.Get("main_check_sure"),
                    () =>
                    {
                        StartCoroutine(LoadRemoteFoldmanifest(remoteVer));
                    }
                );

                Debug.LogError($"load hot file fail {remoteVer.ToString()} ");

            }
        }


        void OnHotResProccessChanged(LoadingEventArg arg)
        {
            var m = 1024 * 1024f;
            var loaded_s = string.Format("{0:.00}", (float)arg.current / m);
            var loaded_t = string.Format("{0:.00}", (float)arg.total / m);
            var kbs = string.Format("{0:.00} kb/s", (float)BackGroundDownload.BytesReceivedPerSecond / 1024f);
            var str = Localization.GetFormat("main_downloading_tips", loaded_s, loaded_t, kbs);
            SetSliderProgress(str, (float)arg.current / (float)arg.total);
        }


        void OnBackgroundAllComplete(FolderQueueGroup group, bool is_error)
        {
            if (group != null && group.anyError)
            {
                var tips = Localization.Get("main_download_fail");
                SetProgressTxt(tips);
                UnityEngine.Events.UnityAction ReLoad = () =>
                {
                    BackGroundDownload.instance.ReLoadErrorGroup(group);
                    BackGroundDownload.instance.Begin();
                    MessageBox.Destroy();
                };
                MessageBox.Show(tips, "", Localization.Get("main_check_sure"), ReLoad);
                Debug.LogError($" OnBackgroundComplete Error :{tips}");
            }
            else
            {
                FileHelper.DeletePersistentFile(Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME); //删除旧文件
                FileHelper.ChangePersistentFileName(UPDATED_TEMP_LIST_NAME, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);
                FileManifestManager.localVersion = loadRemoteVersion;
                FileManifestManager.localResNum = loadRemoteAppNum;
                Debug.Log($"download sccuess :{loadRemoteVersion}");
                FileManifestManager.LoadPersistentFolderManifest(null); //刷新列表
                SetVersionInfo();
                StartCoroutine(RefreshCatalog());
            }
        }

        //热更新文件加载完成
        void OnBackgroundComplete(FolderManifestQueue queue, bool is_error)
        {
            var remoteManifest = queue.currFolder;

            if (is_error)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("thus files load fail :");
                sb.AppendLine(System.DateTime.Now.ToString());
                sb.AppendLine(remoteManifest.fileName);
                foreach (var f in queue.errorFiles)
                {
                    sb.AppendLine(f.name);
                }
                Debug.LogError(sb.ToString());
            }
            else
            {
                if (remoteManifest != null && FileManifestManager.CheckStreamingFolderManifestResNumber(remoteManifest))
                {
                    FileManifestManager.CheckAddOrUpdatePersistentFolderManifest(remoteManifest);
                }
#if !HUGULA_NO_LOG
                Debug.Log($"OnBackgroundItemComplete({remoteManifest})");
#endif
            }

        }
        #endregion

        #endregion

        #region  ui

        void SetProgressTxt(string tips)
        {
            view.tips.text = tips;
        }

        void SetSliderProgress(string tips, float per)
        {
            view.tips.text = tips;
            view.slider.value = per;
#if !HUGULA_NO_LOG
            Debug.Log($"Progress：{tips} {per * 100}% frame:{Time.frameCount} ");
#endif
        }

        void SetVersionInfo()
        {
            if (view.version)
            {
                view.version.text = $"{FileManifestManager.localVersion} ({FileManifestManager.localResNum})";
            }
        }
        #endregion

        #region  util

        ///<summary>
        /// 从配置获取加载cdn的url地址
        ///</summary>
        public void LoadBeginScene()
        {
            Debug.Log("LoadBeginScene");
            UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE_NAME, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        private IEnumerator RefreshCatalog()
        {
            // yield return FileManifestManager.RefreshCatalog();
            yield return InternalIdTransformFunc();
        }

        //重定向bundle地址
        internal IEnumerator InternalIdTransformFunc()
        {
            //zip package内容重定向
            FileManifestManager.GenLoadedZipTransform();
            yield return null;
            //热更新重定向
            FileManifestManager.GenUpdatedPackagesTransform();

            yield return null;
            LoadBeginScene();

        }


        ///<summary>
        /// 从配置获取加载cdn的url地址
        ///</summary>
        public List<string> GetVersionUrl()
        {
            List<string> urlGroup = new List<string>();
            var udid = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            var http_ver_hosts = Hugula.HugulaSetting.instance.httpVerHost;
            var ver_name = CUtils.InsertAssetBundleName(Common.CRC32_VER_FILENAME, $"_{CodeVersion.APP_VERSION}");
            var hosts = http_ver_hosts.Split(',');
            var timeline = CUtils.ConvertDateTimeInt(System.DateTime.Now).ToString();
            string verUrl;
            foreach (var host in hosts)
            {
                verUrl = host.Replace("{udid}", udid);
                verUrl = verUrl.Replace("{timeline}", timeline);
                verUrl = verUrl.Replace("{ver_file_name}", Common.CRC32_VER_FILENAME);
                verUrl = verUrl.Replace("{platform}", CUtils.platform);
                verUrl = verUrl.Replace("{ver_folder}", Common.RES_VER_FOLDER);
                verUrl = verUrl.Replace("{ver_file_version_name}", ver_name);
                verUrl = verUrl.Replace("{app_version}", CodeVersion.APP_VERSION);

                verUrl = string.Format(verUrl, CUtils.platform, Common.RES_VER_FOLDER, ver_name, CodeVersion.APP_VERSION);
                urlGroup.Add(verUrl);
#if !HUGULA_NO_LOG
                Debug.LogFormat("version host = {0} ", verUrl);
#endif
            }

            return urlGroup;
        }

        #endregion


    }

}
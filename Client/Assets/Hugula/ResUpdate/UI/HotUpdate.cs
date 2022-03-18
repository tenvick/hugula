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
    /// --	从Ver_android or Ver_ios 中的cdn_host下载资源包。
    /// -- 	1热更新流程
    /// --	.1 对比本地版本号 streaming 和 persistent 的版本文件。
    /// --	.2 加载文件列表 分别加载streaming 和 persistent的文件列表
    /// --	.3 加载版本文件 对比最新的本地版本号
    /// --	.4 如果不一致下载更新列表。
    /// --	.5 对比 本地文件列表 查找变化的文件。
    /// --	.6 下载变化文件列表。
    /// --	.7 下载完成进入游戏。
    /// --
    /// --	2注意事项
    /// --	.3 BackGroundDownload 是网络加载模块，用于从网络加载URL并保存到persistent目录。
    /// --	.4 ResVersion 版本信息，除了基本版本信息外，还可读取VerExtends_android or VerExtends_ios里面的配置字段，可以用于一些配置。
    /// --	.5 cdn_hosts 用于从网络加载资源的url列表
    /// ------------------------------------------------
    ///</summary>
    public class HotUpdate : MonoBehaviour
    {
#if UNITY_EDITOR
        const string KeyDebugString = "_HotUpdate_Debug_string";

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
        const float MAX_STEP = 6;
        const string UPDATED_LIST_NAME = Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME;
        const string UPDATED_TEMP_LIST_NAME = UPDATED_LIST_NAME + ".tmp";
        const string SCENE_NAME = "s_begin";

        #region  mono

        // Start is called before the first frame update
        IEnumerator Start()
        {
            BackGroundDownload.Init();
            FileManifestManager.OverrideInternalIdTransformFunc(); //重定向热更新的文件
            FileManifestManager.LoadStreamingFolderManifests(null);//读取本地文件列表
            FileManifestManager.LoadPersistentFolderManifest(null);//读取持久化目录的列表

            yield return null;
            Hugula.Localization.language = PlayerPrefs.GetString(Hugula.Localization.KEY_LAN, Application.systemLanguage.ToString());
            yield return new WaitForLanguageHasBeenSet();

#if  UNITY_ANDROID && !UNITY_EDITOR
            if (FileManifestManager.streamingFolderManifest == null) {
                //todo 资源丢失 
            }
#endif
#if UNITY_EDITOR
            yield return null;
            if (isDebug)
                yield return LoadRemoteVersion();
            else
            {
                LoadBeginScene();
            }
#else
            yield return LoadRemoteVersion ();
#endif
        }


        #endregion

        #region  hot update logic
        //加载远程版本文件
        public IEnumerator LoadRemoteVersion()
        {
            var hosts = GetVersionUrl();
            string response = null;
            foreach (var host in hosts)
            {
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

            //解析数据
            if (!string.IsNullOrEmpty(response))
            {
#if !HUGULA_NO_LOG
                Debug.Log(response);
#endif
                romoteVersionTxtLoaded = true;
                var remoteVer = JsonUtility.FromJson<VerionConfig>(response);
                var subVersion = CodeVersion.Subtract(remoteVer.version, FileManifestManager.localVersion);

                var cdn_hosts = remoteVer.cdn_host;
                for (int i = 0; i < cdn_hosts.Length; i++)
                {
                    remoteVer.cdn_host[i] = string.Format(cdn_hosts[i], CUtils.platform, CodeVersion.APP_VERSION);
#if !HUGULA_NO_LOG
                    Debug.Log("remote resource cdn host: " + remoteVer.cdn_host[i]);
#endif
                }

                BackGroundDownload.rootHosts = remoteVer.cdn_host;

                if (CodeVersion.CODE_VERSION < remoteVer.code)
                { //强制更新提示
                    MessageBox.Show(Localization.Get("main_download_new_app"), "", Localization.Get("main_check_sure"), () =>
                    {
                        Application.OpenURL(remoteVer.update_url);
                    });
                }
                else
                {
                    //判断是否等待fast包
                    yield return CheckLoadFast(remoteVer);
                    yield return GenLoadedZipTransform();

                    if (subVersion > 0)
                        yield return LoadRemoteFoldmanifest(remoteVer);//下载热更新文件
                    else
                        LoadBeginScene(); //直接进入
                }
            }
            else if (!romoteVersionTxtLoaded)
            {
                yield return new WaitForSeconds(2);
                StartCoroutine(LoadRemoteVersion());
                Debug.Log("ver.txt 解析失败重新加载中。。。");
            }

            yield return null;
        }

        #region  加载fast包
        ///<summary>
        /// 判断是否等待fast加载完毕
        ///</summary>
        IEnumerator CheckLoadFast(VerionConfig remoteVer)
        {
            if (remoteVer.fast == FastMode.sync) //需要等待fast包下载完成
            {
                // LoadFolderManifest
                var fastManifest = FileManifestManager.FindStreamingFolderManifest("fast");
                if (fastManifest != null && !fastManifest.isZipDone)//如果fast包没有加载
                {
                    var localVersion = Resources.Load<TextAsset>("version");
                    if (localVersion == null)
                    {
                        UnityEngine.Debug.LogError($"local version.txt does't exists! {System.DateTime.Now}");
                        yield break;
                    }
                    var localVer = JsonUtility.FromJson<VerionConfig>(localVersion.text);
                    var cdn_hosts = localVer.cdn_host;
                    for (int i = 0; i < cdn_hosts.Length; i++)
                    {
                        localVer.cdn_host[i] = string.Format(cdn_hosts[i], CUtils.platform, CodeVersion.APP_VERSION);
#if !HUGULA_NO_LOG
                        Debug.Log("local resource cdn host: " + localVer.cdn_host[i]);
#endif
                    }

                    var zipUrl = CUtils.PathCombine(localVer.cdn_host[0], fastManifest.zipName);
                    var newFastManifest = fastManifest.CloneWithOutAllFileInfos();
                    newFastManifest.Add(new FileResInfo(zipUrl, 0, fastManifest.zipSize));
#if !HUGULA_NO_LOG
                    Debug.Log(fastManifest.ToString());
                    print($"add zipUrl:{zipUrl}");
                    Debug.Log(newFastManifest.ToString());
#endif

                    var change = BackGroundDownload.instance.AddFolderManifest(newFastManifest, onFastProccessChanged, onFastComplete);
                    if (change > 0)
                    {
                        var is_wifi = false;// Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork;
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
                        Debug.Log($"{fastManifest.zipName} :大小为0!");
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
            var str = "fast " + Localization.GetFormat("main_downloading_tips", loaded_s, loaded_t, kbs) +System.DateTime.Now;
            SetSliderProgress(str, (float)arg.current / (float)arg.total);
        }

        void onFastComplete(FolderManifestQueue queue, bool isError)
        {
            var folderManifest = queue.currFolder;
            Debug.Log($"onFastComplete {isError} zipMarkName={folderManifest.zipMarkPathName},zipName={folderManifest.zipName} {folderManifest.ToString()}");
            if (!isError)
            {
                var fastManifest = FileManifestManager.FindStreamingFolderManifest("fast");
                folderManifest.MarkZipDone();
                FileManifestManager.GenZipPackageTransform(fastManifest);
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
                MessageBox.Show("fast 包下载失败请检测网络!", "", "", BeginLoad);
            }
        }

        ///<summary>
        /// 统一执行下载完毕的zip的地址重定向 
        ///</summary>
        IEnumerator GenLoadedZipTransform()
        {
            var streamingFolderManifest = FileManifestManager.streamingFolderManifest;
            FolderManifest item;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                item = streamingFolderManifest[i];
                if (item.isZipDone)
                {
                    FileManifestManager.GenZipPackageTransform(item);
                    yield return null;
                }
            }
        }
        #endregion

        #region  加载远端remote foldermanifest文件与热更新资源       
        //加载远端FolderManifest文件
        public IEnumerator LoadRemoteFoldmanifest(VerionConfig remoteVer)
        {
            yield return null;
            Debug.Log("开始加载manifest");
            SetProgressTxt(Localization.Get("main_web_server_crc_list")); //--加载服务器校验列表。")
            var url = CUtils.PathCombine(remoteVer.cdn_host[0], remoteVer.manifest_name); //server_ver.cdn_host[1] .. "/" .. file_name

#if !HUGULA_NO_LOG
            print($"get remote url({url}) ");
#endif

            UnityWebRequest req = UnityWebRequest.Get(url);
            var async = req.SendWebRequest();
            yield return async;
            if (req.responseCode == 200)
            {
                SetProgressTxt(Localization.Get("main_compare_crc_list")); //校验列表对比中。"
                var bytes = req.downloadHandler.data;
                FileHelper.SavePersistentFile(bytes, UPDATED_TEMP_LIST_NAME); //--保存server端临时文件
                var ab = LuaHelper.LoadFromMemory(bytes);
                var remoteFolderManifest = new List<FolderManifest>(ab.LoadAllAssets<FolderManifest>());
                FileManifestManager.remoteFolderManifest = remoteFolderManifest;
                ab.Unload(false);
                print($"remote file({url}) is down");
#if !HUGULA_NO_LOG
                Debug.Log("remoteFolderManifest");
                foreach (var folder in remoteFolderManifest)
                    Debug.Log(folder.ToString());

#endif
                var streamingFolderManifest = FileManifestManager.streamingFolderManifest;
                FolderManifest curStreaming = null;
                FolderManifest remoteFolder = null;
                List<FolderManifest> needDownload = new List<FolderManifest>();

                for (int i = 0; i < remoteFolderManifest.Count; i++)
                {
                    remoteFolder = remoteFolderManifest[i];
                    for (int j = 0; j < streamingFolderManifest.Count; j++)
                    {
                        curStreaming = streamingFolderManifest[j];
                        if (remoteFolder.folderName == curStreaming.folderName) //找到相同文件夹
                        {
                            var downLoadFolder = remoteFolder.CloneWithOutAllFileInfos();
                            downLoadFolder.allFileInfos = curStreaming.Compare(remoteFolder);
                            if (downLoadFolder.allFileInfos.Count > 0)
                            {
                                needDownload.Add(downLoadFolder);
#if !HUGULA_NO_LOG
                                Debug.Log($"update Folder {downLoadFolder.ToString()}\r\ncurStreaming={curStreaming.ToString()}\r\n remoteFolder:{remoteFolder.ToString()}");
#endif
                            }
                            continue;
                        }
                    }
                    //如果没找到需要下载当前所有文件
                    if(remoteFolder.Count>0)
                    {
                        needDownload.Add(remoteFolder);
#if !HUGULA_NO_LOG
                        Debug.Log($"update Folder {remoteFolder.ToString()}\r\ncurStreaming={curStreaming.ToString()}\r\n remoteFolder:{remoteFolder.ToString()}");
#endif
                    }
                }

                //开始加载热更新文件
                var change = BackGroundDownload.instance.AddFolderManifests(needDownload, OnHotResProccessChanged, OnBackgroundComplete);
                Debug.Log("need load file size:" + change);
                if (change > 0)
                {
                    var is_wifi = false;//Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork;
                    UnityEngine.Events.UnityAction BeginLoad = () =>
                    {
                        BackGroundDownload.MaxLoadCount = 4;
                        BackGroundDownload.instance.Begin();
                        MessageBox.Destroy();
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

        //热更新文件加载完成
        void OnBackgroundComplete(FolderManifestQueue queue, bool is_error)
        {
            var remoteManifest = queue.currFolder;

            if (is_error)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("thus files load fail :");
                sb.AppendLine(System.DateTime.Now.ToString());
                foreach (var f in queue.errorFiles)
                {
                    sb.AppendLine(f.name);
                }

                Debug.LogError(sb.ToString());
            }
            else
            {
                if(remoteManifest !=null && FileManifestManager.UpdateStreamingFolderManifest(remoteManifest))
                {
                    FileManifestManager.GenUpdatePackageTransform(remoteManifest);//如果版本正确重定向地址
                }
#if !HUGULA_NO_LOG
                Debug.Log($"OnBackgroundComplete({remoteManifest}) is_Error：{is_error} ");
#endif
                // SetSliderProgress (Localization.Get ("main_download_complete"), 4, 1); //"更新完毕，进入游戏！"
            }

            if (queue.group.anyError)
            {
                var tips = Localization.Get("main_download_fail");
                SetProgressTxt(tips);
                UnityEngine.Events.UnityAction ReLoad = () =>
                {
                    BackGroundDownload.instance.ReLoadErrorGroup(queue.group);
                    BackGroundDownload.instance.Begin();
                    MessageBox.Destroy();
                };
                MessageBox.Show(tips, "", Localization.Get("main_check_sure"), ReLoad);
            }
            else if (queue.group.isAllDown)
            {
                FileHelper.DeletePersistentFile(UPDATED_LIST_NAME); //删除旧文件
                FileHelper.ChangePersistentFileName(UPDATED_TEMP_LIST_NAME, UPDATED_LIST_NAME);
#if !HUGULA_NO_LOG
                Debug.Log($"OnBackgroundComplete isAllDown ({remoteManifest}) is_Error：{is_error} ");
#endif
                LoadBeginScene();
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
            Debug.Log($"Progress：{tips} {per*100}% frame:{Time.frameCount} ");
        }

        // void OnProgressEvent(LoadingEventArg arg)
        // {
        //     var m = 1024 * 1024;
        //     var loaded_s = string.Format ("{0:.00}", arg.current / m);
        //     var loaded_t = string.Format ("{0:.00}", arg.total / m);
        //     var kbs = string.Format ("{0:.00} kb/s", BackGroundDownload.BytesReceivedPerSecond / 1024);
        //     var str = Localization.GetFormat ("main_downloading_tips", loaded_s, loaded_t, kbs);
        //     SetSliderProgress (str, 4, arg.current / arg.total);
        // }
        #endregion

        #region  util

        ///<summary>
        /// 从配置获取加载cdn的url地址
        ///</summary>
        public void LoadBeginScene()
        {
            // SetSliderProgress(Localization.Get("main_enter_game"), 5, 1);
            ResLoader.Init();
            StartCoroutine(WaitAddressableInit());
        }

        //等待初始化完成进入场景
        private IEnumerator WaitAddressableInit()
        {
            while (!ResLoader.Ready)
                yield return null;

            UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE_NAME, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        ///<summary>
        /// 从配置获取加载cdn的url地址
        ///</summary>
        public List<string> GetVersionUrl()
        {
            List<string> urlGroup = new List<string>();
            var udid = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            var http_ver_hosts = Hugula.HugulaSetting.instance.httpVerHost;
            var hosts = http_ver_hosts.Split(',');
            string verUrl;
            foreach (var host in hosts)
            {
                verUrl = string.Format(host, CUtils.platform, CodeVersion.APP_VERSION, udid, CUtils.ConvertDateTimeInt(System.DateTime.Now));
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
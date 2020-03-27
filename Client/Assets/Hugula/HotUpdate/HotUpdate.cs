using System.Collections;
using System.Collections.Generic;
using Hugula.Loader;
using Hugula.UI;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Hugula.HotUpdate {

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
    public class HotUpdate : MonoBehaviour {

        public HotUpdateView view;

        //临时缓存的远程manifest
        FileManifest remoteManifest;
        bool romoteVersionTxtLoaded = false;
        const float MAX_STEP = 6;
        const string VERSION_FILE_NAME = Common.CRC32_VER_FILENAME;
        const string VERSION_TEMP_FILE_NAME = VERSION_FILE_NAME + ".tmp";
        const string UPDATED_LIST_NAME = Common.CRC32_FILELIST_NAME;
        const string UPDATED_TEMP_LIST_NAME = UPDATED_LIST_NAME + ".tmp";
        const string DOWANLOAD_TEMP_FILE = "downloaded.tmp";
        const string update_list_crc_key = UPDATED_LIST_NAME;

        const string SCENE_ASSETBUNDLE_NAME = "s_begin.u3d";
        const string SCENE_NAME = "s_begin";

        #region  mono

        // Start is called before the first frame update
        IEnumerator Start () {
            ResourcesLoader.Initialize ();
            ResourcesLoader.RegisterOverrideBaseAssetbundleURL (OverrideBaseAssetbundleURL);
            ManifestManager.LoadFileManifest (null);

#if  UNITY_ANDROID && !UNITY_EDITOR
            if (ManifestManager.fileManifest == null) {
                //todo obb丢失 
            }
#endif
#if UNITY_EDITOR
            yield return null;
            LoadBeginScene ();
#else
            return LoadRemoteVersion ();
#endif
        }

        #endregion

        #region  hot update logic
        //加载远程版本文件
        public IEnumerator LoadRemoteVersion () {

            var hosts = GetVersionUrl ();
            string response = null;
            foreach (var host in hosts) {
                UnityWebRequest req = UnityWebRequest.Get (host);
                var async = req.SendWebRequest ();
                yield return async;
                if (req.responseCode == 200) {
                    response = req.downloadHandler.text;
                    break;
                } else {
                    Debug.LogErrorFormat ("UnityWebRequest error code = {0},error = {1}.", req.responseCode, req.error);
                    yield return null;
                }
            }

            //解析数据
            if (!string.IsNullOrEmpty (response)) {
                romoteVersionTxtLoaded = true;
                var remoteVer = JsonUtility.FromJson<VerionConfig> (response);
                var subVersion = CodeVersion.Subtract (remoteVer.version, ManifestManager.localVersion);

                BackGroundDownload.instance.hosts = remoteVer.cdn_host;
                FileHelper.SavePersistentFile (response, VERSION_TEMP_FILE_NAME); //临时文件

                if (CodeVersion.CODE_VERSION < remoteVer.code) { //强制更新提示

                    MessageBox.Show (Localization.Get ("main_download_new_app"), "", "", () => {
                        Application.OpenURL (remoteVer.update_url);
                    });

                } else if (subVersion > 0) //需要热更新
                {
                    StartCoroutine (LoadRomoteFileList (remoteVer));
                } else {
                    LoadBeginScene ();
                }
            } else if (!romoteVersionTxtLoaded) {
                yield return new WaitForSeconds (1);
                StartCoroutine (LoadRemoteVersion ());
                Debug.Log ("ver.txt 解析失败重新加载中。。。");
            }
        }

        //加载热更新文件
        public IEnumerator LoadRomoteFileList (VerionConfig remoteVer) {
            yield return null;
            SetProgressTxt (Localization.Get ("main_web_server_crc_list")); //--加载服务器校验列表。")
            var crc = remoteVer.crc32.ToString ();
            var asset_name = CUtils.GetAssetName (UPDATED_LIST_NAME);
            var assetbundleName = asset_name + Common.CHECK_ASSETBUNDLE_SUFFIX;
            var fileName = CUtils.InsertAssetBundleName (assetbundleName, "_" + crc);
            var url = CUtils.PathCombine (remoteVer.cdn_host[1], fileName); //server_ver.cdn_host[1] .. "/" .. file_name

            UnityWebRequest req = UnityWebRequest.Get (url);
            var async = req.SendWebRequest ();
            yield return async;
            if (req.responseCode == 200) {
                SetProgressTxt (Localization.Get ("main_compare_crc_list")); //校验列表对比中。"
                var bytes = req.downloadHandler.data;
                FileHelper.SavePersistentFile (bytes, UPDATED_TEMP_LIST_NAME); //--保存server端临时文件
                var ab = LuaHelper.LoadFromMemory (bytes);
                var romoteManifest = ab.LoadAllAssets () [1] as FileManifest;
                ab.Unload (false);
                print ("server file list is down");
                if (!CUtils.isRelease) {
                    Debug.Log (romoteManifest);
                    Debug.Log (ManifestManager.fileManifest);
                }

                //开始加载热更新文件
                var change = BackGroundDownload.instance.AddDiffManifestTask (ManifestManager.fileManifest, romoteManifest, OnProgressEvent, OnBackgroundComplete);
                Debug.Log ("need load file size:" + change);
                if (change > 0) {
                    var is_wifi = Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork;
                    UnityEngine.Events.UnityAction BeginLoad = () => {
                        BackGroundDownload.instance.alwaysDownLoad = true;
                        BackGroundDownload.instance.loadingCount = 4;
                        BackGroundDownload.instance.Begin ();
                        MessageBox.Destroy ();
                    };
                    if (is_wifi) {
                        BeginLoad ();
                    } else {
                        var tips = Localization.GetFormat ("main_download_from_webserver", string.Format ("{0:.00}", change / 1048576));
                        MessageBox.Show (tips, "", "", BeginLoad); //提示手动下载
                    }
                }
            } else { //加载失败重新加载
                yield return null;
                var tips = Localization.Get ("main_web_server_error");
                MessageBox.Show (tips, "", "",
                    () => {
                        StartCoroutine (LoadRomoteFileList (remoteVer));
                    }
                );
            }

        }

        void OnBackgroundComplete (bool is_error) {
            if (is_error) {
                var tips = Localization.Get ("main_download_fail");
                SetProgressTxt (tips);
                UnityEngine.Events.UnityAction ReLoad = () => {
                    BackGroundDownload.instance.ReloadError ();
                    BackGroundDownload.instance.Begin ();
                    MessageBox.Destroy ();
                };
                MessageBox.Show (tips, "", "", ReLoad);
            } else {
                if (remoteManifest != null) {
                    ManifestManager.SetUpdateFileManifest (remoteManifest);
                }
                remoteManifest = null;
                if (ManifestManager.CheckFirstLoad ()) ManifestManager.FinishFirstLoad ();

                FileHelper.DeletePersistentFile (UPDATED_LIST_NAME); //删除旧文件
                FileHelper.ChangePersistentFileName (UPDATED_TEMP_LIST_NAME, UPDATED_LIST_NAME);

                FileHelper.DeletePersistentFile (VERSION_FILE_NAME); //删除旧文件
                FileHelper.ChangePersistentFileName (VERSION_TEMP_FILE_NAME, VERSION_FILE_NAME);

                SetSliderProgress (Localization.Get ("main_download_complete"), 4, 1); //"更新完毕，进入游戏！"
                LoadBeginScene ();
            }
        }

        #endregion

        #region  ui

        void SetProgressTxt (string tips) {
            view.tips.text = tips;
        }

        void SetSliderProgress (string tips, float step, float per) {
            view.tips.text = tips;
            view.slider.value = (step + per) / MAX_STEP;
        }
        void OnProgressEvent (LoadingEventArg arg) {
            var m = 1024 * 1024;
            var loaded_s = string.Format ("{0:.00}", arg.current / m);
            var loaded_t = string.Format ("{0:.00}", arg.total / m);
            var kbs = string.Format ("{0:.00} kb/s", BackGroundDownload.BytesReceivedPerSecond / 1024);
            var str = Localization.GetFormat ("main_downloading_tips", loaded_s, loaded_t, kbs);
            SetSliderProgress (str, 4, arg.current / arg.total);
        }
        #endregion

        #region  util

        public static string OverrideBaseAssetbundleURL (string abName) {
            string path = null;
            bool isupdate = ManifestManager.CheckIsUpdateFile (abName);
            if (isupdate) {
                //    if(ManifestManager.CheckReqCrc(abName))//crc check
                path = CUtils.PathCombine (CUtils.GetRealPersistentDataPath (), abName);
            }

            return path;
        }

        ///<summary>
        /// 从配置获取加载cdn的url地址
        ///</summary>
        public void LoadBeginScene () {
            ManifestManager.LoadUpdateFileManifest (null);
            SetSliderProgress (Localization.Get ("main_enter_game"), 5, 1);
            ResourcesLoader.LoadScene (SCENE_ASSETBUNDLE_NAME, SCENE_NAME, null, null, null,true, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        ///<summary>
        /// 从配置获取加载cdn的url地址
        ///</summary>
        public List<string> GetVersionUrl () {
            List<string> urlGroup = new List<string> ();
            var udid = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            var http_ver_hosts = Hugula.HugulaSetting.instance.httpVerHost;
            var hosts = http_ver_hosts.Split (',');
            string verUrl;
            foreach (var host in hosts) {
                if (host.IndexOf ('?') >= 0)
                    verUrl = string.Format (host, CodeVersion.APP_VERSION, udid, CUtils.platform, Time.time); //--http server
                else
                    verUrl = string.Format (host, CUtils.platform, "ver.txt");

                urlGroup.Add (verUrl);
#if UNITY_EDITOR
                Debug.LogFormat ("host = {0} ", verUrl);
#endif
            }

            return urlGroup;
        }
        #endregion

        #region entity
        public class VerionConfig {
            public int code;
            public int time;

            public string version;

            public string[] cdn_host;

            public string update_url;

            public uint crc32;

        }
        #endregion
    }

}
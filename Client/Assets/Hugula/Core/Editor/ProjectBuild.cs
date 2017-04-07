using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hugula.Editor;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;

public class ProjectBuild : Editor {
    /// <summary>
    /// 默认只打包第一个场景。
    /// </summary>
    static string[] GetBuildScenes () {
        List<string> names = new List<string> ();
        names.Add ("Assets/Scene/s_frist.unity");
        return names.ToArray ();
    }

    static void BuildSlua () {
        SLua.LuaCodeGen.GenerateAll ();
    }

    #region 构建分级
    static void DeleteStreamingOutPath () {
        ExportResources.DeleteStreamingOutPath ();
    }

    // [MenuItem ("Hugula/本地发布导出资源 ", false, 16)]
    static void ExportRes () {
        CUtils.DebugCastTime ("Time ExportRes Begin");
        ExportResources.exportPublish (); //资源
        CUtils.DebugCastTime ("Time exportPublish End");
    }

    #endregion

    //  [MenuItem("Hugula/Android publish ", false, 16)]
    static void BuildForAndroid () {
        string path = "../../release/android/";
        path = Path.GetFullPath (path);
        ExportResources.CheckDirectory (path);
        path = path + string.Format("hugula{0}.apk",System.DateTime.Now.ToString("MM-dd-hh-mm")); //

        BuildPipeline.BuildPlayer (GetBuildScenes (), path, BuildTarget.Android, BuildOptions.None);
    }

    //	[MenuItem("Hugula/IOS Publish  ", false, 16)]
    static void BuildForIOS () {
        CUtils.DebugCastTime ("Time BuildForIOS Begin");
        string path = "../../release/ios";
        path = Path.GetFullPath (path);
        ExportResources.DirectoryDelete (path);
        ExportResources.CheckDirectory (path);
        BuildPipeline.BuildPlayer (GetBuildScenes (), path, BuildTarget.iOS, BuildOptions.None);
        CUtils.DebugCastTime ("Time BuildForIOS End");
    }

    //[MenuItem("Hugula/project  StandaloneWindows ", false, 16)]
    static void BuildForWindows () {
        string path = "../../release/pc/";
        path = Path.GetFullPath (path);
        ExportResources.DirectoryDelete (path);
        ExportResources.CheckDirectory (path);
        path = path + "hugula.exe"; //
        BuildPipeline.BuildPlayer (GetBuildScenes (), path, BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}
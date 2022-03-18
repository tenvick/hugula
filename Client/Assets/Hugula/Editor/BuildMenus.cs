using System;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;
using UnityEditor;
using System.Text;
using System.IO;
using HugulaEditor;
using HugulaEditor.ResUpdate;
public class ProjectBuild : Editor
{
    #region console par
    ///<summary>
    /// setting
    /// release 发布版本
    /// obb Android apk extions files
    ///</summary>
    public static string setting
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("setting") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }


    public static string buildTarget
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("buildTarget") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// 编译宏
    ///</summary>
    public static string defineSymbols
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("define") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// BundleId 包名
    ///</summary>
    public static string bundleId
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("bundle") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// 签名team id
    ///</summary>
    public static string signingTeamId
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("sign") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// appName only android
    ///</summary>
    public static string productName
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("product") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// appName only android
    ///</summary>
    public static string version
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("version") && arg.Contains(":"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    #endregion

    private static StringBuilder environmentVariable = new StringBuilder();

    private static bool exportingAndroidProject = false;
    /// <summary>
    /// 在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    /// </summary>
    static string[] GetBuildScenes()
    {
        var ve = setting;//发布release或者dev版本
        List<string> names = new List<string>();
        names.Add("Assets/Scene/s_hotupdate.unity");
        names.Add("Assets/Scene/s_begin.unity");
        return names.ToArray();
    }

    #region 
    ///<summary>
    /// 设置编译命令
    ///</summary>
    static void ScriptingDefineSymbols()
    {
        Debug.Log(defineSymbols);
        var _defineSymbols = defineSymbols.Replace(",", ";");
        Debug.Log(_defineSymbols);

#if UNITY_IOS
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,BuildTarget.iOS);
        // EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _defineSymbols);
#elif UNITY_ANDROID
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        // EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineSymbols);
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _defineSymbols);
#endif

    }

    [MenuItem("Hugula/resource export and aas build ", false, 201)]
    public static void ExportRes()
    {
        Debug.LogFormat("-----------------Export resources current target {0}-------------------------", EditorUserBuildSettings.activeBuildTarget);
        CUtils.DebugCastTime("Time Clear Cache Begin");
        AddressableCacheClear();
        CUtils.DebugCastTime("Time ExportRes Begin");
        Settings();
        CUtils.DebugCastTime("Time begin Language Export");
        LanMenuItems.Export();
        CUtils.DebugCastTime("Time begin LuaProtobuf");
        LuaProtobufMenuItems.Export();
        // LuaProtobufMenuItems.GenerateApiList();
        // LuaProtobufMenuItems.GenerateProtocInit();
        CUtils.DebugCastTime("Time begin lua Export");
        LuaMenuItems.Export();
        CUtils.DebugCastTime("Time ExportRes End");
        AddressableBuild();
        CUtils.DebugCastTime("Time AddressableBuild End");

    }

    static void AddressableBuild()
    {
        List<ScriptableObject> allDataBuilders = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.DataBuilders;
        UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex =
           allDataBuilders.IndexOf(allDataBuilders.Find(s => s.GetType() == typeof(HugulaEditor.Addressable.BuildScriptHotResUpdate)));

        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
    }

    //清理构建的缓存
    static void AddressableCacheClear()
    {
        List<ScriptableObject> allDataBuilders = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.DataBuilders;
        UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex =
           allDataBuilders.IndexOf(allDataBuilders.Find(s => s.GetType() == typeof(HugulaEditor.Addressable.BuildScriptHotResUpdate)));

        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.CleanPlayerContent();
    }

    #endregion

    #region 设置相关

    static void WriteAppVerion(string folder = "apk", string appExtension = "il2cpp")
    {
        System.Environment.SetEnvironmentVariable("APP_VERSION", Hugula.CodeVersion.APP_VERSION); //app version
        environmentVariable.AppendFormat("APP_VERSION={0}\n", Hugula.CodeVersion.APP_VERSION);
        environmentVariable.AppendFormat("CODE_VERSION={0}\n", Hugula.CodeVersion.CODE_VERSION);
        environmentVariable.AppendFormat("APP_NUMBER={0}\n", Hugula.CodeVersion.APP_NUMBER);
        environmentVariable.AppendFormat("APP_BUNDLE_ID={0}\n", Hugula.Utils.CUtils.bundleIdentifier);
        environmentVariable.AppendFormat("IOS_IPA_NAME={0}\n", CUtils.GetSuffix(Hugula.Utils.CUtils.bundleIdentifier));

        var fileName = Hugula.CodeVersion.APP_VERSION;
#if HUGULA_RELEASE
        environmentVariable.AppendFormat("APP_STATE={0}\n","release");
        fileName += "_release_";
#else
        environmentVariable.AppendFormat("APP_STATE={0}\n", "dev");
        fileName += "_dev_";
#endif
        fileName += appExtension;
        // File.Create(Path.Combine(path, fileName));
        environmentVariable.AppendFormat("APP_FILENAME={0}\n", fileName);
        WriteShell();
    }

    static void WriteShell()
    {
        string path = Path.GetFullPath("shell");
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        string shellPath = Path.Combine(path, "ftp_release_shell.sh");
        StringBuilder ftpTampleta = new StringBuilder();
        ftpTampleta.Append("function foo()\n{\nlocal r\nlocal a\nr=\"$@\"\nwhile [[ \"$r\" != \"$a\" ]] ; do\na=${r%%/*}\necho \"mkdir $a\"\necho \"cd $a\"\nr=${r#*/}\ndone\n}");
        ftpTampleta.Append("\nfunction upload_ftp()\n{\necho \"current folder \"$1\necho \"upload to \"$FTP_ROOT$2\nftp -niv <<- EOF\nopen $FTP_IP\nuser $FTP_USER $FTP_PWD\nlcd $1\n$(foo \"$FTP_ROOT$2\")\ncd $FTP_ROOT$2\nbin\nhash\npwd\nprompt off\nmput *.*\nclose\nbye\nEOF\n}");
        var codeVersion = Hugula.CodeVersion.APP_NUMBER.ToString(); //文件crc 不变路径需要改变
        string firstPackagePath = BuildConfig.UpdateResOutVersionPath;// release/android/v9000 
        string ftpToPath = CUtils.platform + "/v" + codeVersion;
        StringBuilder content1 = new StringBuilder();
        content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, ftpToPath);//eg release/android/v9000 to android/v9001

        using (StreamWriter sw = new StreamWriter(shellPath, false, new UTF8Encoding(false)))
        {
            sw.Write(ftpTampleta.ToString());
            sw.Write(content1.ToString());
        }
        Debug.Log("Create shell success " + shellPath);

        string ftpVerPath = Path.Combine(path, "ftp_dev_shell.sh");
        firstPackagePath = BuildConfig.UpdateResOutVersionPath;
        content1 = new StringBuilder();
        content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, ftpToPath);//eg dev/android/v9000 to android/v9001
        // content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, string.Format("{0}/v{1}", CUtils.platform, Hugula.CodeVersion.CODE_VERSION));//eg dev/android/v9000 to android/v9000

        using (StreamWriter sw = new StreamWriter(ftpVerPath, false, new UTF8Encoding(false)))
        {
            sw.Write(ftpTampleta.ToString());
            sw.Write(content1.ToString());
        }
        Debug.Log("Create shell success " + ftpVerPath);

        //environment_shell.sh
        string environmentPath = Path.Combine(path, "environment_shell.sh");

        using (StreamWriter sw = new StreamWriter(environmentPath, false, new UTF8Encoding(false)))
        {
            sw.Write(environmentVariable.ToString());
        }

        environmentVariable = new StringBuilder();
        Debug.Log("Create shell success " + environmentPath);
    }

    static void WriteObbName()
    {
        var obbName = string.Format("main.{0}.{1}.obb", PlayerSettings.Android.bundleVersionCode.ToString(), Hugula.Utils.CUtils.bundleIdentifier);
        environmentVariable.AppendFormat("OBB_NAME={0}\n", obbName);
    }

    static void AndroidSettings(string appExtension = "il2cpp")
    {

        var ve = setting;//发布release或者dev版本
        if (ve.ToLower().Contains("aab") || ve.ToLower().Contains("apkexpansionfiles"))
        {
            PlayerSettings.Android.useAPKExpansionFiles = true;
        }
        else
        {
            PlayerSettings.Android.useAPKExpansionFiles = false;
        }

        string keystorePath = Application.dataPath + "/Config/com_hugula_demo.keystore";
        Debug.Log(keystorePath);
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = "12345678";
        PlayerSettings.Android.keyaliasName = "hugula_demo";
        PlayerSettings.Android.keyaliasPass = "12345678";
        environmentVariable.AppendFormat("Android_keystoreName={0}\n", "/Config/com_hugula_demo.keystore");
        environmentVariable.AppendFormat("Android_keystorePass={0}\n", "hugula");
        environmentVariable.AppendFormat("Android_keyaliasName={0}\n", "hugula_android_release");
        environmentVariable.AppendFormat("Android_keyaliasPass={0}\n", "hugula");
        WriteAppVerion("apk", appExtension);

    }

    static void Settings()
    {
        if (!string.IsNullOrEmpty(productName))
            PlayerSettings.productName = productName;
        if (!string.IsNullOrEmpty(version))
            PlayerSettings.bundleVersion = version;
#if UNITY_ANDROID        
        if (!string.IsNullOrEmpty(bundleId))
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleId);

        PlayerSettings.Android.bundleVersionCode = Hugula.CodeVersion.APP_NUMBER;

#elif UNITY_IOS
        if (!string.IsNullOrEmpty(bundleId))
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);


        PlayerSettings.iOS.buildNumber = Hugula.CodeVersion.APP_NUMBER.ToString();
#endif
    }

    static void IOSSettings()
    {
        if (!string.IsNullOrEmpty(signingTeamId))
            PlayerSettings.iOS.appleDeveloperTeamID = signingTeamId;

        //write app version
        WriteAppVerion("ipa");
    }

    #endregion

    #region 出包

    [MenuItem("Hugula/Build  StandaloneWindows  (export res and buildplayer)", false, 210)]
    static void BuildForWindowsOnekey()
    {
        ExportRes();
        BuildForWindows();
    }

    [MenuItem("Hugula/Build  Android  (export res and buildplayer)", false, 210)]
    static void BuildForAndroidOnekey()
    {
        ExportRes();
        BuildForAndroidIL2CPP();
    }

    static void BuildForWindows()
    {
        CUtils.DebugCastTime("begin build windows exe");
        string path = "../release/pc/";
        path = Path.GetFullPath(path);
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        WriteAppVerion();
        path = path + "hugula.exe";//
        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.StandaloneWindows, BuildOptions.None);
        CUtils.DebugCastTime("end build windows exe");

    }

    static void BuildForAndroidIL2CPP()
    {
        string fileName = $"hugula_{System.DateTime.Now.ToString("yyyyMMdd_HH_mm")}";
        string path = $"../release/android/{fileName}.apk";
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android && EditorUserBuildSettings.buildAppBundle) //如果是aab
        {
            path = $"../release/android/{fileName}.aab";
        }
        path = Path.GetFullPath(path);

        EditorUtils.CheckDirectory(Path.GetDirectoryName(path));

        Debug.Log($"build file：{path}  time:{System.DateTime.Now}");
        exportingAndroidProject = false;
        //copy android plugins
        AndroidSettings();
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        // PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.Android);
        if (setting.ToLower().Contains("development"))
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development);
        else
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        CUtils.DebugCastTime("Time BuildForAndroid iL2CPP End");

    }

     static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        AssetDatabase.Refresh();
        // EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        if (EditorUserBuildSettings.development)
            build_options |= BuildOptions.Development;
        if (EditorUserBuildSettings.connectProfiler)
            build_options |= BuildOptions.ConnectWithProfiler;
        if (EditorUserBuildSettings.allowDebugging)
            build_options |= BuildOptions.AllowDebugging;

        var result = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
            
        if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
                Debug.Log($"build {target_dir} success {System.DateTime.Now}!");
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("BuildPlayer failure:");
            sb.AppendLine(result.summary.ToString());
            sb.AppendLine(result.summary.result.ToString());
            sb.AppendLine(System.DateTime.Now.ToString());
            throw new Exception(sb.ToString());
        }
    }
    #endregion

}

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
public class AndroidIL2CPPDebugSymbolCopy
{

    [PostProcessBuildAttribute()]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.Android)
            PostProcessAndroidBuild(pathToBuiltProject);
    }

    public static void PostProcessAndroidBuild(string pathToBuiltProject)
    {
        ScriptingImplementation backend = (ScriptingImplementation)PlayerSettings.GetPropertyInt("ScriptingBackend", BuildTargetGroup.Android);

        if (backend == UnityEditor.ScriptingImplementation.IL2CPP)
        {
            CopyAndroidIL2CPPSymbols(pathToBuiltProject, PlayerSettings.Android.targetDevice);
        }
    }

    public static void CopyAndroidIL2CPPSymbols(string pathToBuiltProject, AndroidTargetDevice targetDevice)
    {
        string buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        FileInfo fileInfo = new FileInfo(pathToBuiltProject);
        string symbolsDir = "IL2CPPSymbols";

        CreateDir(symbolsDir);

        switch (PlayerSettings.Android.targetDevice)
        {
            case AndroidTargetDevice.FAT:
                {
                    CopyARMSymbols(symbolsDir);
                    CopyX86Symbols(symbolsDir);
                    break;
                }
            case AndroidTargetDevice.ARMv7:
                {
                    CopyARMSymbols(symbolsDir);
                    break;
                }
            case AndroidTargetDevice.x86:
                {
                    CopyX86Symbols(symbolsDir);
                    break;
                }
            default:
                break;
        }
    }


    public static void DeleteLibFolder()
    {
        if (Directory.Exists(libpath))
        {
            Directory.Delete(libpath, true);
        }
    }

    const string libpath = "/../Temp/StagingArea/libs/";
    const string libFilename = "libil2cpp.so.debug";
    private static void CopyARMSymbols(string symbolsDir)
    {
        string sourcefileARM = Application.dataPath + libpath + "armeabi-v7a/" + libFilename;
        CreateDir(symbolsDir + "/armeabi-v7a/");
        File.Copy(sourcefileARM, symbolsDir + "/armeabi-v7a/libil2cpp.so.debug", true);
    }

    private static void CopyX86Symbols(string symbolsDir)
    {
        string sourcefileX86 = Application.dataPath + libpath + "x86/libil2cpp.so.debug";
        CreateDir(symbolsDir + "/x86/");
        File.Copy(sourcefileX86, symbolsDir + "/x86/libil2cpp.so.debug", true);
    }

    public static void CreateDir(string path)
    {
        if (Directory.Exists(path))
            return;

        Directory.CreateDirectory(path);
    }
}

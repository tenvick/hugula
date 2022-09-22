using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Hugula.Utils;
using Hugula;

public class CUtilsEditorTest
{

    [Test]
    public void CUtilsGetRightFileNameTest()
    {
        // UnityEngine.UI.Slider s;
        //s.onValueChanged.AddListener();

        string url, name;//BindingTest
        url = "extends/ex_ui_bottom.u3d?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        name = CUtils.GetRightFileName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "extends/ex_ui_bottom?as=1d2%dfd3";
        name = CUtils.GetRightFileName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "ex_ui_bottom.u3d?as=1d2%dfd3";
        name = CUtils.GetRightFileName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "ex_ui_bottom";
        name = CUtils.GetRightFileName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "ex_ui_bottom.u3d";
        name = CUtils.GetRightFileName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);
    }

    [Test]
    public void CUtilsGetAssetBundleName()
    {
        string url, name;
        url = "extends/ex_ui_bottom.u3d?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "extends/ex_ui_bottom.u3d");

        url = CUtils.GetRealStreamingAssetsPath() + "/extends/ex_ui_bottom?as=1d2%dfd3";
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "extends/ex_ui_bottom");

        url = "ex_ui_bottom.u3d?as=1d2%dfd3";
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "ex_ui_bottom";
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "ex_ui_bottom");

        url = CUtils.GetRealStreamingAssetsPath() + "/ex_ui_bottom.u3d";
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "ex_ui_bottom.u3d");

        url = CUtils.platform;
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, url);

        url = CUtils.GetRealStreamingAssetsPath() + "/"+CUtils.platform;
        name = CUtils.GetAssetBundleName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, CUtils.platform);
        
    }

    [Test]
    public void CUtilsGetAssetName()
    {
        string url, name;
        url = "extends/ex_ui_bottom.u3d?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "ex_ui_bottom");

        url = CUtils.GetRealStreamingAssetsPath() + "/extends/ex_ui_bottom?as=1d2%dfd3";
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "ex_ui_bottom");

        url = "ex_ui_bottom.u3d?as=1d2%dfd3";
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "ex_ui_bottom";
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "ex_ui_bottom");

        url = CUtils.GetRealStreamingAssetsPath() + "/ex_ui_bottom.u3d";
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "ex_ui_bottom");

        url = CUtils.platform;
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, url);

        url = CUtils.GetRealStreamingAssetsPath() + "/" + CUtils.platform;
        name = CUtils.GetAssetName(url);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, CUtils.platform);
    }

    [Test]
    public void CUtilsInsertAssetBundleName()
    {
        string url, name,insert;
        url = "extends/ex_ui_bottom.u3d?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        insert = "_5235353589";
        name = CUtils.InsertAssetBundleName(url, insert);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);

        url = "extends/ex_ui_bottom?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        insert = "_5235353589";
        name = CUtils.InsertAssetBundleName(url, insert);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, "extends/ex_ui_bottom" + insert);

        url = "extends/ex_ui_bottom"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        insert = "_5235353589";
        name = CUtils.InsertAssetBundleName(url, insert);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, url + insert);

        url = CUtils.platformFloder; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        insert = "_5235353589";
        name = CUtils.InsertAssetBundleName(url, insert);
        Debug.Log(".............." + name);
        Assert.AreEqual(name, url + insert);
    }

    [Test]
    public void RegexTest()
    {
        string input = "android.manifest";
        string pattern = @"(\.manifest$)";

        pattern = @"\.meta$|\.manifest$";

        bool re = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        Debug.Log(input);
        Assert.AreEqual(re, true);

        input = "ex_ui_bottom.u3d.manifest.meta";
        re = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        Debug.Log(input);
        Assert.AreEqual(re, true);

        input = "android";
        re = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        Debug.Log(input);
        Assert.AreEqual(re, false);


        input = "ext/android.u3d";
        re = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
        Debug.Log(input);
        Assert.AreEqual(re, false);
    }

    [Test]
    public void GetSuffixTest()
    {
                string url, name;
        url = "extends/ex_ui_bottom.u3d?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreEqual(name, ".u3d");

        url = CUtils.GetRealStreamingAssetsPath() + "/extends/ex_ui_bottom?as=1d2%dfd3";
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreEqual(name, "");

        url = "ex_ui_bottom.u3d?as=1d2%dfd3";
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreNotEqual(name, "u3d");

        url = "ex_ui_bottom";
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreEqual(name, "");

        url = CUtils.GetRealStreamingAssetsPath() + "/ex_ui_bottom.u3d";
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreEqual(name, ".u3d");

        url = CUtils.platform;
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreEqual(name, "");

        url = CUtils.GetRealStreamingAssetsPath() + "/"+CUtils.platform;
        name = CUtils.GetSuffix(url);
        Debug.Log("..............=" + name);
        Assert.AreEqual(name, "");
    }

    [Test]
    public void TestAppVersionHotUpdate()
    {
        //eg 1 old version
        string app_version = "0.5.2";
        string min_ver= "";
        string max_ver = "0.5.2";
        int subMin;
        int appVerNum;
        int minVerNum;
        int maxVerNum;
        
        subMin = CodeVersion.Subtract(max_ver, app_version); //发布版本相同版本才能热更新
        var LoadRemoteFoldmanifest = subMin == 0 ;
        Assert.AreEqual(LoadRemoteFoldmanifest , true);

        // eg 2 new version
        app_version = "0.5.2";
        min_ver= "";
        max_ver = "0.5.2";

        appVerNum = CodeVersion.CovertVerToInt(app_version);
        minVerNum = CodeVersion.CovertVerToInt(min_ver);
        maxVerNum = CodeVersion.CovertVerToInt(max_ver);

        if (minVerNum <= 0) minVerNum = maxVerNum;
        LoadRemoteFoldmanifest = appVerNum>=minVerNum && appVerNum<=maxVerNum;
        Debug.Log($"app_version:{app_version};min_ver:{min_ver};max_ver:{max_ver};LoadRemoteFoldmanifest:{LoadRemoteFoldmanifest},app_version_num:{appVerNum},min_ver_num:{minVerNum};max_ver_num:{maxVerNum}");
        Assert.AreEqual(LoadRemoteFoldmanifest , true);

        //eg3 in range
        app_version = "0.5.3";
        min_ver= "";
        max_ver = "0.5.2";

        appVerNum = CodeVersion.CovertVerToInt(app_version);
        minVerNum = CodeVersion.CovertVerToInt(min_ver);
        maxVerNum = CodeVersion.CovertVerToInt(max_ver);

        if (minVerNum <= 0) minVerNum = maxVerNum;
        LoadRemoteFoldmanifest = appVerNum>=minVerNum && appVerNum<=maxVerNum;
        Debug.Log($"app_version:{app_version};min_ver:{min_ver};max_ver:{max_ver};LoadRemoteFoldmanifest:{LoadRemoteFoldmanifest},app_version_num:{appVerNum},min_ver_num:{minVerNum};max_ver_num:{maxVerNum}");
    
        Assert.AreEqual(LoadRemoteFoldmanifest , false);

        //eg 4in range
        app_version = "0.5.3";
        min_ver= "0.5.2";
        max_ver = "0.5.1";

        appVerNum = CodeVersion.CovertVerToInt(app_version);
        minVerNum = CodeVersion.CovertVerToInt(min_ver);
        maxVerNum = CodeVersion.CovertVerToInt(max_ver);

        if (minVerNum <= 0) minVerNum = maxVerNum;
        LoadRemoteFoldmanifest = appVerNum>=minVerNum && appVerNum<=maxVerNum;
        Debug.Log($"app_version:{app_version};min_ver:{min_ver};max_ver:{max_ver};LoadRemoteFoldmanifest:{LoadRemoteFoldmanifest},app_version_num:{appVerNum},min_ver_num:{minVerNum};max_ver_num:{maxVerNum}");
    
        Assert.AreEqual(LoadRemoteFoldmanifest , false);

        //eg 5in range
        app_version = "0.5.3";
        min_ver= "0.5.0";
        max_ver = "0.5.1";

        appVerNum = CodeVersion.CovertVerToInt(app_version);
        minVerNum = CodeVersion.CovertVerToInt(min_ver);
        maxVerNum = CodeVersion.CovertVerToInt(max_ver);

        if (minVerNum <= 0) minVerNum = maxVerNum;
        LoadRemoteFoldmanifest = appVerNum>=minVerNum && appVerNum<=maxVerNum;
        Debug.Log($"app_version:{app_version};min_ver:{min_ver};max_ver:{max_ver};LoadRemoteFoldmanifest:{LoadRemoteFoldmanifest},app_version_num:{appVerNum},min_ver_num:{minVerNum};max_ver_num:{maxVerNum}");
    
        Assert.AreEqual(LoadRemoteFoldmanifest , false);

        //eg 6in range
        app_version = "0.5.3";
        min_ver= "0.5.0";
        max_ver = "0.5.3";

        appVerNum = CodeVersion.CovertVerToInt(app_version);
        minVerNum = CodeVersion.CovertVerToInt(min_ver);
        maxVerNum = CodeVersion.CovertVerToInt(max_ver);

        if (minVerNum <= 0) minVerNum = maxVerNum;
        LoadRemoteFoldmanifest = appVerNum>=minVerNum && appVerNum<=maxVerNum;
        Debug.Log($"app_version:{app_version};min_ver:{min_ver};max_ver:{max_ver};LoadRemoteFoldmanifest:{LoadRemoteFoldmanifest},app_version_num:{appVerNum},min_ver_num:{minVerNum};max_ver_num:{maxVerNum}");
    
        Assert.AreEqual(LoadRemoteFoldmanifest , true);

    }
}

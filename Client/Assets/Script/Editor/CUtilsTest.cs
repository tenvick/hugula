using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Hugula.Utils;

public class CUtilsEditorTest
{

    [Test]
    public void CUtilsGetRightFileNameTest()
    {
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
}

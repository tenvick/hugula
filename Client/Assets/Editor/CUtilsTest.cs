using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Hugula.Utils;

public class NewEditorTest {

	[Test]
    public void GetAssetBundleNameTest()
	{
        string url = "var/asdfjalnflasdnflasdf324324432.u3d?";
       string name = CUtils.GetAssetBundleName(url);
       Debug.Log(name);
		//Assert
		//The object has a new name
       Assert.AreNotEqual(name, url);
	}

    [Test]
    public void CUtilsGetFileNameTest()
    {
        string url, name;
        url = "extends/ex_ui_bottom.u3d?adsdf=sdfdfa&dafsd"; //lastFileIndex23,lastDotIndex20,lastQueIndex0
        name = CUtils.GetFileName(url);
        Debug.Log(".............."+name);
        //Assert
        //The object has a new name
        url = "extends/ex_ui_bottom?as=1d2%dfd3";
        name = CUtils.GetFileName(url);
        Debug.Log(".............." + name);

        url = "ex_ui_bottom.u3d?as=1d2%dfd3";
        name = CUtils.GetFileName(url);
        Debug.Log(".............." + name);

        url = "ex_ui_bottom";
        name = CUtils.GetFileName(url);
        Debug.Log(".............." + name);
        Assert.AreNotEqual(name, url);
    }
}

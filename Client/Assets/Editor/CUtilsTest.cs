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
}

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class HotUpdateTest {
    [Test]
    public void HotUpdateStringToJson () {
        // string reponse = "{\"code\":4000,\"crc32\":547495871,\"time\":1531924979,\"version\":\"0.4.3\",\"cdn_host\":[\"http://192.168.99.48/release/android/v4003/res\"],\"update_url\":\"http://192.168.99.48/release/android\"}";
        // var serverVer = JsonUtility.FromJson<HotUpdate.VerionConfig> (reponse);
        // Debug.Log(serverVer.code);
        // Debug.Log(serverVer.cdn_host[0]);
    }

    [Test]
    public void StringFormat()
    {
        Debug.Log(string.Format ("{0:.00}", 5f / 2f));
    }
}
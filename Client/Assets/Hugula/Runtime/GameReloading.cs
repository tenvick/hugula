using System.Collections;
using System.Collections.Generic;
using Hugula;
using UnityEngine;
using Hugula.Framework;

public class GameReloading : MonoBehaviour
{
    public static string LoadScene = "s_hotupdate";
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Timer.Clear();
        yield return null;
        ResLoader.ReleaseCachedInstances();
        Debug.LogWarning($"GameReloading ResLoader.ReleaseCachedInstances time:{System.DateTime.Now.ToString()}");
        yield return null;
        EnterLua.BeforeLuaDispose();//销毁 manager单例
        yield return null;
        var ins = EnterLua.instance;
        GameObject.Destroy(ins?.gameObject);//销毁EnterLua
        EnterLua.DisposeLua();
        yield return null;
        Debug.LogWarning($"begin load scene({LoadScene}) time:{System.DateTime.Now.ToString()}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene);
    }
}

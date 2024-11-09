using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;
using Hugula;
using Hugula.Framework;

public class LuaBindingTest : MonoBehaviour
{
    public string vmConfigName;
    public BindableObject container;

    public LuaModule luaModule;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (Hugula.EnterLua.luaenv == null)
        {
            yield return null;
        }
        Debug.Log($" lua :{Hugula.EnterLua.luaenv}  ");
        yield return null;
        yield return null;
        yield return null;


        luaModule.container = container;
        luaModule.vmConfigName = vmConfigName;
        yield return null;
        luaModule.gameObject.SetActive(true);

    }


}

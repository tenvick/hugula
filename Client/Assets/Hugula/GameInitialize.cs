using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;
using Hugula.Utils;
using Hugula.UI;
using Hugula;

///<summary>
///游戏初始化
///</summary>
public class GameInitialize : MonoBehaviour
{
    [SerializeField] string enterLua = "begin"; //main

    void Awake()
    {
        BehaviourSingletonManager.CanCreateInstance();
        SingletonManager.CanCreateInstance();
        Hugula.Databinding.ValueConverterRegister.instance.AddConverter(typeof(ClickTipsConvert).Name, new ClickTipsConvert());
    }

    IEnumerator Start()
    {
        ResLoader.Init();

        while (!ResLoader.Ready)
            yield return null;

        var LuaBeha = this.gameObject.CheckAddComponent<EnterLua>();
        LuaBeha.enterLua = enterLua;

    }
}
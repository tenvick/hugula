using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;
using Hugula.Mvvm;
using Hugula.UI;

///<summary>
///游戏初始化
///</summary>
public class GameInitialize : MonoBehaviour {
    void Awake () {
        
        Manager.Initialize ();

        Hugula.Databinding.ValueConverterRegister.instance.AddConverter(typeof(ClickTipsConvert).Name,new ClickTipsConvert());
    }
}
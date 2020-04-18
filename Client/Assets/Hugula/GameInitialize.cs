using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;
using Hugula.Mvvm;

///<summary>
///游戏初始化
///</summary>
public class GameInitialize : MonoBehaviour {
    void Awake () {
        Manager.Register<GlobalDispatcher> ();

        Manager.Initialize ();
    }
}
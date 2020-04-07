using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Manager;

///<summary>
///游戏初始化
///</summary>
public class GameInitialize : MonoBehaviour
{
    void Awake()
    {
        Manager.Register<EnterLua>();

        Manager.Initialize();
    }
    
    void Start()
    {
        //reginster manager
        // Manager.Register<

    }

    // Update is called once per frame
    void Update()
    {

    }
}

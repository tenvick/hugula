// Copyright (c) 2015 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// UGUI event system
/// </summary>
[SLua.CustomLuaClass]
public class UGUIEventSystem : MonoBehaviour
{
    public EventSystem eventSystem;

    public StandaloneInputModule standaloneInputModule;
    public TouchInputModule touchInputModule;

    #region mono

    //void Update()
    //{
    //    //touchInputModule.
    //    //Button btn;
    //    //eventSystem.currentInputModule

    //    //if (eventSystem.currentSelectedGameObject != null)
    //    //{
    //    //    Debug.Log(eventSystem.currentSelectedGameObject.name);
    //    //}

    //    //if (eventSystem.currentInputModule != null)
    //    //{
    //    //    Debug.Log("currentInputModule" + eventSystem.currentInputModule);
    //    //}

    //    //if (eventSystem.IsPointerOverGameObject())
    //    //{
    //    //    Debug.Log("IsPointerOverGameObject" + eventSystem.IsPointerOverGameObject());
    //    //}
    //    //standaloneInputModule.
    //            //standaloneInputModule.on

    //}

    #endregion

}

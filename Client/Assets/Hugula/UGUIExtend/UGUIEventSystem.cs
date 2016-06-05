// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
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

    void Awake()
    {
#if UNITY_EDITOR  || UNITY_STANDALONE
        if (touchInputModule != null) Object.Destroy(touchInputModule);// touchInputModule.forceModuleActive = false;
        if(standaloneInputModule != null) standaloneInputModule.forceModuleActive = true;
#else
        if(touchInputModule != null)  touchInputModule.forceModuleActive = true;
        if (standaloneInputModule != null) Object.Destroy(standaloneInputModule);
#endif

    }
    #endregion

}

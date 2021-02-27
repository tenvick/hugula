using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Framework;
using System;
using Hugula;
public class AASManager : BehaviourSingleton<AASManager>
{
    #region properties
    private bool m_Inited = false;
    #endregion

    #region members
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator AsyncInit()
    {
        yield return AASHotUpdate.Start();
        m_Inited = true;
    }

    public bool InitFinished()
    {
        return m_Inited;
    }

    #endregion

}

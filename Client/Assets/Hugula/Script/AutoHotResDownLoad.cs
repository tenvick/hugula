using UnityEngine;
using System.Collections;
using Hugula.Loader;
using Hugula.Update;

public class AutoHotResDownLoad : MonoBehaviour
{
    private bool m_isStart = false;
    // Use this for initialization
    IEnumerator Start()
    {

        yield return new WaitForSeconds(1f);
        if (ManifestManager.CheckNeedBackgroundLoad())
        {
            BackGroundDownload.instance.AddBackgroundManifestTask(ManifestManager.fileManifest, null, OnComplete);// AddBackManifestTask
            BackGroundDownload.instance.Begin();
        }
        m_isStart = true;

    }

    void OnComplete(bool isErr)
    {
        if (!isErr)
            ManifestManager.FinishBackgroundLoad();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (m_isStart)
            BackGroundDownload.instance.Begin();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        BackGroundDownload.instance.Suspend();
    }


    // /// <summary>
    // /// suspend download
    // /// </summary>
    // public void Suspend()
    // {
    //     // enabled = false;
    //     BackGroundDownload.instance.Suspend();
    // }

    // /// <summary>
    // /// begin download
    // /// </summary>
    // public void Begin()
    // {
    //     BackGroundDownload.instance.Begin();
    // }

}

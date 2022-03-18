using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class DontDestroyOnLoaded : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}

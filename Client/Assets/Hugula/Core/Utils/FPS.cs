// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;

namespace Hugula.Utils
{
public class FPS : MonoBehaviour 
{
    public float updateInterval = 0.5F;
    private float lastInterval;
    private int frames = 0;
    private float fps;

    void Start() 
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }
    
#if UNITY_EDITOR
    void OnGUI()
    {
        GUILayout.Label("fps:" + fps);
    }
#endif

    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval) 
        {
            fps = frames/(timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
}

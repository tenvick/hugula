// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;

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
    void OnGUI()
    {
        GUILayout.Label("fps:" + fps);
    }
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


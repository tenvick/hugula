using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;

public class EventProfileTest : MonoBehaviour, INotifyPropertyChanged
{
    public PropertyChangedEventHandlerEvent PropertyChanged
    {
        get;
        set;
    }

    public PropertyChangedEventHandlerEvent PropertyChangedDelegate = new PropertyChangedEventHandlerEvent();
    // Start is called before the first frame update
    void Start()
    {
        PropertyChanged = new PropertyChangedEventHandlerEvent();
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("event PropertyChanged");
        PropertyChanged += OnChange;
        PropertyChanged += OnChange1;
        PropertyChanged += OnChange2;
        PropertyChanged -= OnChange;
        // PropertyChanged -= OnChange1;
        // PropertyChanged -= OnChange2;

        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("PropertyChangedEventDelegate");
        PropertyChangedDelegate += OnChange;
        PropertyChangedDelegate += OnChange1;
        PropertyChangedDelegate += OnChange2;
        PropertyChangedDelegate -= OnChange;
        // PropertyChangedDelegate -= OnChange1;
        // PropertyChangedDelegate -= OnChange2;
        UnityEngine.Profiling.Profiler.EndSample();

    }

    public void OnChange(object sender, string propertyName)
    {

    }

    public void OnChange1(object sender, string propertyName)
    {

    }

    public void OnChange2(object sender, string propertyName)
    {

    }
}


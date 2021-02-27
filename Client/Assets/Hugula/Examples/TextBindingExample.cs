using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;
using System;

public class TextBindingExample : MonoBehaviour
{
    public BindableObject container;
    public float floatValue = 3.14159f;
    public int intValue = 9;

    public string strValue = "hello";
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        var ins = ValueConverterRegister.instance;
        //    ins.AddConverter("FloatToString",this);
        container.context = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {

    }

    public object Convert(object value, Type targetType)
    {
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType)
    {
        float val = 0;
        float.TryParse(value.ToString(), out val);
        return val;
    }
}

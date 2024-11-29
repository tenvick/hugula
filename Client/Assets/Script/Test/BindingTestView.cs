using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;
using UnityEngine.UI;
using Hugula.Databinding.Binder;
using Hugula;

public class BindingTestView : MonoBehaviour
{
    [Tooltip("BindableObject")]
    [PopUpComponentsAttribute]
    public MonoBehaviour bindMonoBehaviour;
    [Tooltip("需要测试的属性")]
    public string bindingPreperty = "text";
    BindableObject bindableObject;
    [PopUpComponentsAttribute]
    public Component target;
    Binding testBinding;
    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        bindableObject = bindMonoBehaviour as BindableObject;
        testBinding = bindableObject.GetBinding(bindingPreperty);
    }


    public void OnClick()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClick({max}) testBinding={testBinding} bindingContext={testBinding.bindingContext}");
        int i = 0;
        var source = testBinding.bindingContext;
        while (true)
        {
            i++;
            // var source = new SimpleSource();
            testBinding.Apply(source);
            if (i >= max) break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }


    public void OnClick2()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClick2({max}) new Binding()");
        int i = 0;
        var source = testBinding.bindingContext;
        Binding[] bindings = new Binding[max];
        for (int j = 0; j < max; j++)
        {
            bindings[j] = new Binding();
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void OnClick1()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClick1({max}) testBinding={testBinding} bindingContext={testBinding.bindingContext}");
        int i = 0;
        var source = testBinding.bindingContext;
        while (true)
        {
            i++;
            // var source = new SimpleSource();
            //testBinding.Apply(source);
            //source, path, target, property, is_indexer, is_self, format, convert
            ExpressionUtility.instance?.m_SetTargetPropertyNoConvertInvoke(source, "text3", target, bindingPreperty, false, false, null);
            if (i >= max) break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

}

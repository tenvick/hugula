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
    public MonoBehaviour[] bindMonoBehaviour;


    [Tooltip("需要测试的属性")]
    public string bindingPreperty = "text";
    BindableObject bindableObject;

    [PopUpComponentsAttribute]
    public Component target;
    Binding[] testBinding;
    public InputField inputField;

    // [SerializeField]
    // Binding binding;

    // [SerializeField]
    // BindingPathPartConfig[] partConfigs;

    // Start is called before the first frame update
    void Start()
    {
        testBinding = new Binding[bindMonoBehaviour.Length];
        for (int i = 0; i < bindMonoBehaviour.Length; i++)
        {
            bindableObject = bindMonoBehaviour[i] as BindableObject;
            testBinding[i] = bindableObject.GetBinding(bindingPreperty);
        }
    }


    public void OnClick()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        var testBinding = this.testBinding[0];
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClick({max}) {testBinding}.apply({testBinding.bindingContext})");
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

    public void OnClickGetExp()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        var testBinding2 = this.testBinding[1];
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClickGetExp({max}) {testBinding2}.apply({testBinding2.bindingContext})");
        int i = 0;
        var source = testBinding2.bindingContext;
        while (true)
        {
            i++;
            // var source = new SimpleSource();
            testBinding2.Apply(source);
            if (i >= max) break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }


    public void OnClickSetExp()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        var testBinding3 = this.testBinding[2];
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClickSetExp({max}) {testBinding3}.apply({testBinding3.bindingContext})");
        int i = 0;
        var source = testBinding3.bindingContext;
        while (true)
        {
            i++;
            // var source = new SimpleSource();
            testBinding3.Apply(source);
            if (i >= max) break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void OnClickMethod()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        var testBinding4 = this.testBinding[3];
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClickMethod({max}) {testBinding4}.apply({testBinding4.bindingContext})");
        int i = 0;
        var source = testBinding4.bindingContext;
        while (true)
        {
            i++;
            // var source = new SimpleSource();
            testBinding4.Apply(source);
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
        var testBinding = this.testBinding[0];
        var source = testBinding.bindingContext;
        Binding[] bindings = new Binding[max];
        for (int j = 0; j < max; j++)
        {
            bindings[j] = testBinding.Clone();
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void OnClick1()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        var testBinding = this.testBinding[0];
        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClick1({max})  m_SetTargetPropertyNoConvertInvoke(bindingContext={testBinding.bindingContext},text3,{target},{bindingPreperty} )");
        int i = 0;
        var source = testBinding.bindingContext;
        while (true)
        {
            i++;
            // var source = new SimpleSource();
            //testBinding.Apply(source);
            //source, path, target, property, is_indexer, is_self, format, convert
            ExpressionUtility.instance?.m_SetTargetPropertyNoConvertInvoke(source, target, "text3", bindingPreperty, false, false, null);
            if (i >= max) break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void OnClick3()
    {
        var text = inputField.text;
        var max = int.Parse(text);
        var testBinding = this.testBinding[0];

        UnityEngine.Profiling.Profiler.BeginSample($"BindingTestView.OnClick3({max})  {testBinding}.ParsePath()");
        int i = 0;
        var source = testBinding.bindingContext;
        while (true)
        {
            i++;
            // testBinding.ParsePath();
            if (i >= max) break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

}

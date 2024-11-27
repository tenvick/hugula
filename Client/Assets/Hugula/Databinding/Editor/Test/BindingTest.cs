using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using XLua;
using Hugula.Utils;
using Hugula;
using System.IO;
using Hugula.Databinding;
using Hugula.Mvvm;
using System.Reflection;
using XLua;
using HugulaEditor;

namespace Tests
{
    public class BindingTest
    {
        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        // A Test behaves as an ordinary method
        [Test]
        public void BindingTestSimplePasses()
        {
            int i = 0;
            int max = 1000;

            LuaEnvHelper.BeforeLuaDispose();
            // Use the Assert class to test conditions
            watch.Start();
            var bindableObject = BindableHelper.CreateBindableObject();
            BindableHelper.AddBindings(bindableObject);
            bindableObject.EnableLog = false;
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds CreateBindableObject: {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            LuaEnv luaEnv = LuaEnvHelper.GetLuaEnv();
            luaEnv.DoString("require('binding_test')");
            VMStateHelper.instance.SetVMState(luaEnv);
            var luafun = luaEnv.Global.Get<LuaFunction>("set_context");
            var binding_demo = VMStateHelper.instance.GetMemeber("binding_demo", "."); //(bindableObject, luaEnv.Global);
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds GetLuaEnv: {watch.ElapsedMilliseconds} ms");
            // //开始计算开始时间
            watch.Restart();
            luafun.Call(bindableObject);
            BindingUtility.SetContext(bindableObject, binding_demo);
            watch.Stop();
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds bindableObject:set_context {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            bindableObject.value = 0.5f;
            bindableObject.text = "hello";
            bindableObject.OnValueChanged(0.51f);
            bindableObject.OnClick();
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds On Click: {watch.ElapsedMilliseconds} ms");
            watch.Restart();

            while (true)
            {
                i++;
                bindableObject.SubmitText3($"submit text{i}  TwoWay ");
                // bindableObject.SubmitText3("submit text 3 3 3 TwoWay ");
                if (i >= max) break;
            }
            bindableObject.BindText4("bind text4 method() TwoWay");
            bindableObject.SubmitText3("submit text3  TwoWay ");
            bindableObject.OnSubmitEvent("submit text2 on_input_changed OneWay ");
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds OnSubmitEvent({i}): {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            var binding_demo_tab = binding_demo as LuaTable;
            i = 0;
            var str = $"submit text-{max}  TwoWay ";
            while (true)
            {
                i++;
                var color = binding_demo_tab.GetInPath<LuaTable>("oneway_path_demo.goods.color");
                color.Set(2, str);
                if (i >= max) break;
            }
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds LuaTab.GetInPath({i}) Set2: {watch.ElapsedMilliseconds} ms");
            var path = "oneway_path_demo.goods.color[2]";//23ms 21ms
            // path = "oneway_path_demo.text1";//16
            // path = "test_tmp";//11ms
            var binding = new Binding(path, bindableObject, "text3", BindingMode.TwoWay);
            i = 0;
            binding.target = bindableObject;
            watch.Restart();
            while (true)
            {
                i++;
                binding.Apply(binding_demo);
                if (i >= max) break;
            }
            bindableObject.SubmitText3($"submit text{max}  TwoWay ");
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds Binding.{binding.mode}({i}) Set2: {watch.ElapsedMilliseconds} ms");


            var goods = binding_demo_tab.GetInPath<LuaTable>("oneway_path_demo.goods");
            i = 0;
            var OnPropertyChanged = goods.GetInPath<LuaFunction>("OnPropertyChanged");
            watch.Restart();
            while (true)
            {
                i++;
                OnPropertyChanged.Call(goods, "color");
                if (i >= max) break;
            }
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds LuaTab.OnPropertyChanged(color) {i}: {watch.ElapsedMilliseconds} ms");//31ms
            binding_demo_tab.GetInPath<LuaTable>("oneway_path_demo.goods.color").Get(2, out object objStr);
            Debug.Log($"  oneway_path_demo.goods.color[2] is {objStr}: {watch.ElapsedMilliseconds} ms");
            // var lua_BindingExpression = luaEnv.Global.Get<LuaTable>("BindingExpression");
            // var m_SetTargetMethodInvoke = ExpressionUtility.instance.m_SetTargetMethodInvoke; // fast 1 ms
            // watch.Restart();
            // i = 0;
            // while (true)
            // {
            //     i++;
            //     m_SetTargetMethodInvoke.Invoke(goods, "color", lua_BindingExpression,"OnPropertyChanged",false,false,"2",null); //fast 1 ms
            //     if (i >= max) break;
            // }
            // Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds m_SetTargetMethodInvoke(color) {i}: {watch.ElapsedMilliseconds} ms");
            // var lua_SetTargetMethodInvoke = lua_BindingExpression.Get<LuaFunction>("m_SetTargetMethodInvoke");
            // watch.Restart();
            // i = 0;
            // object a = null;
            // while (true)
            // {
            //     i++;
            //     lua_SetTargetMethodInvoke.Action(goods, "color", lua_BindingExpression,"OnPropertyChanged",false,false,"2",a); //slow 3 ms
            //     if (i >= max) break;
            // }
            // Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds luacall_SetTargetMethodInvoke(color) {i}: {watch.ElapsedMilliseconds} ms");


        }


        [Test]
        public void BindingTestSetTarget()
        {
            int max = 1000;
            int i = 0;
            string path ;
            Binding binding;
            LuaEnvHelper.BeforeLuaDispose();
            // Use the Assert class to test conditions
            watch.Start();
            var bindableObject = BindableHelper.CreateBindableObject();
            BindableHelper.AddBindings(bindableObject);
            bindableObject.EnableLog = false;
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds CreateBindableObject: {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            LuaEnv luaEnv = LuaEnvHelper.GetLuaEnv();
            luaEnv.DoString("require('binding_test')");
            VMStateHelper.instance.SetVMState(luaEnv);
            var luafun = luaEnv.Global.Get<LuaFunction>("set_context");
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds GetLuaEnv: {watch.ElapsedMilliseconds} ms");
            var binding_demo = VMStateHelper.instance.GetMemeber("binding_demo", "."); //(bindableObject, luaEnv.Global);

            /*** CS Binding
            // //创建binding对象
            path = "oneway_path_demo.goods.color[2]";  //-- 监听0 source vm.oneway_path_demo变更   --监听1 source oneway_path_demo.goods变更  --监听2 source oneway_path_demo.goods.color变更  --监听3 source oneway_path_demo.goods.color[2]变更
            // path = "oneway_path_demo.text1";
            binding = new Binding(path, bindableObject, "text3", BindingMode.TwoWay);
            // binding.target = bindableObject;

            var cs_source = new SimpleSource();
            watch.Restart();
            while (true)
            {
                i++;
                binding.Apply(cs_source);
                if (i >= max) break;
            }
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds Binding.Apply(cs_source) -{i}: {watch.ElapsedMilliseconds} ms");
            ***/


            path = "oneway_path_demo.goods.color[2]";//23ms 21ms //context.lsv_selected_idx
            // path = "oneway_path_demo.text1";//16
            // path = "test_tmp";//11ms
            binding = bindableObject.GetBinding("text3");  // new Binding(path, bindableObject, "text3", BindingMode.TwoWay);
            i = 0;
            // binding.target = bindableObject;
            watch.Restart();
            while (true)
            {
                i++;
                binding.Apply(binding_demo);
                if (i >= max) break;
            }
            bindableObject.SubmitText3($"submit text{max}  TwoWay ");
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds Binding.{binding.mode}({i}) Set2: {watch.ElapsedMilliseconds} ms");

            LuaTable inc=  binding_demo as LuaTable;
             if (inc.ContainsKey("PropertyChanged")) //lua table 监听
                {
                    var propChanged = inc.Cast<INotifyPropertyChanged>();
                    var propChanged2 = inc.Cast<INotifyPropertyChanged>();

                    Assert.AreEqual(propChanged, propChanged2);
                    // Assert.AreEqual(propChanged, inc);

                }
        }


        [Test]
        public void PropertyChangedEventHandlerTest()
        {
            int max = 1000;
            int i = 0;
                  LuaEnvHelper.BeforeLuaDispose();
            // Use the Assert class to test conditions
            watch.Start();
            var bindableObject = BindableHelper.CreateBindableObject();
            BindableHelper.AddBindings(bindableObject);
            bindableObject.EnableLog = false;
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds CreateBindableObject: {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            LuaEnv luaEnv = LuaEnvHelper.GetLuaEnv();
            luaEnv.DoString("require('binding_test')");
            VMStateHelper.instance.SetVMState(luaEnv);
            var luafun = luaEnv.Global.Get<LuaFunction>("set_context");
            Debug.Log($"BindingTestSimplePasses.ElapsedMilliseconds GetLuaEnv: {watch.ElapsedMilliseconds} ms");
            var binding_demo = VMStateHelper.instance.GetMemeber("binding_demo", "."); //(bindableObject, luaEnv.Global);


            PropertyChangedEventHandlerEvent eventHandler = bindableObject.PropertyChanged;
            watch.Restart();
            PropertyChangedEventHandler hander = (sender, property) => { 
                // Debug.Log($"{property} == empty");
                 };
            while (true)
            {
                i++;
                int property_cate =  i / 10;
                var property = $"property-{property_cate}";
                var idx = i;
                eventHandler.Add((sender, prop)=> { 
                    // Debug.Log($"{prop}   idx:{idx} cat:{property_cate} "); 
                }, property);
                if (i >= max) break;
            }
                eventHandler.Add(hander);
                eventHandler.Add(hander);
                eventHandler.Add(hander);
            Debug.Log($"EmptyKey={UnityEngine.Animator.StringToHash("")}");
            Debug.Log($"BindingTestSimplePasses.PropertyChangedEventHandlerTest .Add({i+3}): {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            i = 0;
            // while (true)
            // {
            //     i++;
            //     eventHandler.Invoke(this, "property-1");
            //     if (i >= max) break;
            // }
            eventHandler.Invoke(this, "property-1");
            eventHandler.Invoke(this, "property-2");

            Debug.Log($"BindingTestSimplePasses.PropertyChangedEventHandlerTest  Invoke(2): {watch.ElapsedMilliseconds} ms");



            var bindingPathPart = new BindingPathPart();
            // var binding = new Binding("path", null, "property", BindingMode.TwoWay);
            bindingPathPart.Initialize(null, "path", false, false);
            i = 0;
            watch.Restart();
            while (true)
            {
                i++;
                var source = new SimpleSource();
                source.m_PropertyChanged = eventHandler;
                bindingPathPart.Subscribe(source.m_PropertyChanged);
                if (i >= max) break;
            }
            Debug.Log($"BindingTestSimplePasses.PropertyChangedEventHandlerTest  Subscribe invoke{i}: {watch.ElapsedMilliseconds} ms");




        }

    

    }

    class BindableHelper
    {
        private static BindableObjectTest m_Instance;
        public static BindableObjectTest CreateBindableObject()
        {
            // if(m_Instance == null)
            // {
            var gameObject = new GameObject("BindableObjectTest");
            m_Instance = gameObject.AddComponent<BindableObjectTest>();
            // }

            return m_Instance;
        }

        static public void AddBindings(BindableObject bindableObject)
        {
                
            bindableObject.AddBinding(new Binding("enable_slider", null, "enabled", BindingMode.TwoWay));
            // bindableObject.SetBinding("enable_slider", null, "enabled", BindingMode.TwoWay, null); //双向绑定
                bindableObject.AddBinding(new Binding("btntext", null, "text", BindingMode.OneWay));
            // bindableObject.SetBinding("btntext", null, "text", BindingMode.OneWay,  null); //
            bindableObject.AddBinding(new Binding("slider1_value", null, "value", BindingMode.TwoWay));
            // bindableObject.SetBinding("slider1_value", null, "value", BindingMode.TwoWay,  null); //双向绑定
            bindableObject.AddBinding(new Binding("btn_interactable", null, "interactable", BindingMode.TwoWay));
            // bindableObject.SetBinding("btn_interactable", null, "interactable", BindingMode.TwoWay,  null); //
            bindableObject.AddBinding(new Binding("on_slider_value", null, "onValueChangedExecute", BindingMode.OneWay));
            // bindableObject.SetBinding("on_slider_value", null, "onValueChangedExecute", BindingMode.OneWay,  null); //
            bindableObject.AddBinding(new Binding("on_btn_click", null, "onClickCommand", BindingMode.OneWay));
            // bindableObject.SetBinding("on_btn_click", null, "onClickCommand", BindingMode.OneWay,  null); //

            bindableObject.AddBinding(new Binding("selectedIndex", null, "selectedIndex", BindingMode.OneWayToSource));
            // bindableObject.SetBinding("selectedIndex", null, "selectedIndex", BindingMode.OneWayToSource,  null); //

            // //path
            bindableObject.AddBinding(new Binding("oneway_path_demo.text1", null, "text2", BindingMode.OneWay));
            // bindableObject.SetBinding("oneway_path_demo.text1", null, "text2", BindingMode.OneWay,  null); //
            bindableObject.AddBinding(new Binding("oneway_path_demo.goods.color[2]", null, "text3", BindingMode.TwoWay));
            // bindableObject.SetBinding("oneway_path_demo.goods.color[2]", null, "text3", BindingMode.TwoWay,  null); //
            bindableObject.AddBinding(new Binding("oneway_path_demo.bind_text4()", null, "text4", BindingMode.TwoWay));
            // bindableObject.SetBinding("oneway_path_demo.bind_text4()", null, "text4", BindingMode.TwoWay,  null); //
            bindableObject.AddBinding(new Binding("oneway_path_demo.on_input_changed", null, "submitEventCommand", BindingMode.OneWay));
            // bindableObject.SetBinding("oneway_path_demo.on_input_changed", null, "submitEventCommand", BindingMode.OneWay,  null); //
        }

    }


    class LuaEnvHelper
    {

        public static LuaEnv GetLuaEnv()
        {
            LuaEnv luaEnv = new LuaEnv();
            luaEnv.AddLoader(Loader);
            return luaEnv;
        }

        public static void BeforeLuaDispose()
        {
            Hugula.Framework.GlobalDispatcher.instance?.Dispose();
            ExpressionUtility.instance?.Dispose();
            ValueConverterRegister.instance?.Dispose();
            var singletonManagerType = LuaHelper.GetClassType("Hugula.Framework.SingletonManager");
            // 获取 CanCreateInstance 方法信息
            MethodInfo canCreateInstanceMethod = singletonManagerType.GetMethod("CanCreateInstance", BindingFlags.NonPublic | BindingFlags.Static);
            // 调用 CanCreateInstance 方法
            canCreateInstanceMethod.Invoke(null, null);

        }

        static private ILuaBytesRead m_LocalLuaTextRead;
        static ILuaBytesRead GetLocalLuaTextRead()
        {
            if (m_LocalLuaTextRead == null)
            {
                m_LocalLuaTextRead = new FileLuaTextRead();
            }

            return m_LocalLuaTextRead;
        }
        static byte[] Loader(ref string name, ref int length)
        {
            byte[] str = null;
            string path = ValueStrUtils.ConcatNoAlloc(CUtils.dataPath, "/Lua/", name.Replace('.', '/'), ".lua");
            str = GetLocalLuaTextRead().LoadBytes(path, ref length);
            return str;
        }


    }


    /// <summary>
    /// 读取全路径lua text
    /// </summary>
    internal class FileLuaTextRead : ILuaBytesRead
    {
        public FileLuaTextRead()
        {
        }

        public void Unload()
        {

        }
        internal static byte[] GLOBAL_BUFFER = new byte[1024 * 1024 * 2]; //2M空间 1024*1024

        public byte[] LoadBytes(string path, ref int length)
        {
            length = 0;
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    int len = (int)fileStream.Length;
                    var buffer = GLOBAL_BUFFER;
                    if (len > buffer.Length) //grow
                    {
                        buffer = new byte[len];
                        GLOBAL_BUFFER = buffer;
                    }
                    int nread = 0;
                    while (nread < len)
                    {
                        nread += fileStream.Read(buffer, nread, len - nread);
                    }
                    length = nread;
                    return buffer;
                }
            }
            return null;
        }
    }


    public class BindableObjectTest : BindableObject
    {
        public bool EnableLog = true;
        private string m_Text;
        public string text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                if (EnableLog) Debug.Log("text:" + m_Text);
                OnPropertyChanged();
            }
        }

        private float m_Value;
        public float value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                if (EnableLog) Debug.Log("value:" + m_Value);
                OnPropertyChanged();
            }
        }
        private IExecute m_OnValueChangedExecute;
        public IExecute onValueChangedExecute
        {
            get
            {
                return m_OnValueChangedExecute;
            }
            set
            {
                m_OnValueChangedExecute = value;
                OnPropertyChanged();
            }
        }

        public void OnValueChanged(float value)
        {
            OnPropertyChangedBindingApply("value");
            this.value = value;
            if (m_OnValueChangedExecute != null) //&& m_OnClickCommand.can_execute (m_commandParameter)
                m_OnValueChangedExecute.Execute(value);
        }

        private string m_spriteName;

        public string spriteName
        {
            get
            {
                return m_spriteName;
            }
            set
            {
                if (!string.Equals(value, m_spriteName))
                {
                    if (EnableLog) Debug.Log("spriteName:" + m_spriteName);
                    m_spriteName = value;
                }
            }
        }



        private ICommand m_OnClickCommand;
        public ICommand onClickCommand
        {
            get
            {
                return m_OnClickCommand;
            }
            set
            {
                m_OnClickCommand = value;
                if (EnableLog) Debug.Log("onClickCommand:" + m_OnClickCommand);
                OnPropertyChanged();
            }
        }

        public object m_commandParameter;

        public object commandParameter
        {
            get { return m_commandParameter; }
            set
            {
                m_commandParameter = value;
                if (EnableLog) Debug.Log("commandParameter:" + m_commandParameter);
            }
        }

        private bool m_Interactable;
        public bool interactable
        {
            get
            {
                return m_Interactable;
            }
            set
            {
                m_Interactable = value;
                if (EnableLog) Debug.Log("interactable:" + m_Interactable);
                OnPropertyChanged();
            }
        }

        private int m_SelectedIndex;
        public int selectedIndex
        {
            get
            {
                return m_SelectedIndex;
            }
            set
            {
                m_SelectedIndex = value;
                if (EnableLog) Debug.Log("selectedIndex:" + m_SelectedIndex);
                OnPropertyChanged();
            }
        }


        public void OnClick()
        {
            if (m_OnClickCommand != null && m_OnClickCommand.CanExecute(m_commandParameter))
                m_OnClickCommand.Execute(m_commandParameter);
        }


        private ICommand m_SubmitEventCommand;
        public ICommand submitEventCommand
        {
            get
            {
                return m_SubmitEventCommand;
            }
            set
            {
                m_SubmitEventCommand = value;
                OnPropertyChanged();
            }
        }

        public void OnSubmitEvent(string value)
        {
            OnPropertyChangedBindingApply("text2");
            m_Text2 = value;
            if (m_SubmitEventCommand != null && m_SubmitEventCommand.CanExecute(m_commandParameter))
                m_SubmitEventCommand.Execute(value);
        }


        private string m_Text2;
        public string text2
        {
            get { return m_Text2; }
            set
            {
                m_Text2 = value;
                if (EnableLog) Debug.Log("text2:" + m_Text2);
                OnPropertyChanged();
            }
        }

        private string m_Text3;
        public string text3
        {
            get { return m_Text3; }
            set
            {
                m_Text3 = value;
                if (EnableLog) Debug.Log("text3:" + m_Text3);
                OnPropertyChanged();
            }
        }

        public void SubmitText3(string value)
        {
            m_Text3 = value;
            OnPropertyChangedBindingApply("text3");
        }

        private string m_Text4;
        public string text4
        {
            get { return m_Text4; }
            set
            {
                m_Text4 = value;
                if (EnableLog) Debug.Log("text4:" + m_Text4);
                OnPropertyChanged();
            }
        }

        public void BindText4(string value)
        {
            m_Text4 = value;
            OnPropertyChangedBindingApply("text4");
        }

    }

    public class SimpleSource : INotifyPropertyChanged
    {
        internal PropertyChangedEventHandlerEvent m_PropertyChanged;// = new PropertyChangedEventHandlerEvent();

        public PropertyChangedEventHandlerEvent PropertyChanged
        {
            get
            {
                if (m_PropertyChanged == null)
                    m_PropertyChanged = new PropertyChangedEventHandlerEvent();
                return m_PropertyChanged;
            }
        }
        public PathDemo oneway_path_demo = new PathDemo();


        public class PathDemo
        {
            public class Goods
            {
                public string name = "goods";
                public string[] color = new string[] { "red", "green", "blue" };
            }

            public Goods goods = new Goods();
            public string text1 = "text1";
            public string bind_text4()
            {
                return "bind text4 method()";
            }

            public void on_input_changed(string value)
            {
                Debug.Log($"on_input_changed:{value}");
            }
        }
    }
}

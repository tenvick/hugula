using System.Collections;
using Hugula.Databinding;
using UnityEngine;
public class LoopScrollExample : MonoBehaviour
{
    public BindableObject LoopScrollContainer;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        onClick = new Command(ClickHandler);
        LoopScrollContainer.context = this;
        myList.InsertRange(0, new string[] { "hello", "welcome", "to", "hugula", "demo" });
        yield return new WaitForSeconds(0.5f);
        myList.Add("auto 1");
        yield return new WaitForSeconds(0.5f);
        myList.Add("auto 2");
        yield return new WaitForSeconds(0.5f);
        myList.Add("auto 3");
        yield return new WaitForSeconds(0.5f);
        myList.Add("auto 4");
        yield return new WaitForSeconds(0.5f);
        myList.Remove("auto 4");
        yield return new WaitForSeconds(0.5f);
        myList.Remove("auto 3");
        yield return new WaitForSeconds(0.5f);
        myList.Remove("auto 2");
        yield return new WaitForSeconds(0.5f);
        myList.RemoveAt(5);
        yield return new WaitForSeconds(0.5f);
        myList.RemoveRange(new string[] { "demo" });
        yield return new WaitForSeconds(0.5f);
        myList.ReplaceRange(0, new string[] { "你好" });
        yield return new WaitForSeconds(0.5f);
        myList.ReplaceRange(1, new string[] { "欢迎" });
        yield return new WaitForSeconds(0.5f);
        myList.ReplaceRange(2, new string[] { "来到" });
        yield return new WaitForSeconds(0.5f);
        myList.ReplaceRange(3, new string[] { "呼咕啦" });
        yield return new WaitForSeconds((1f));
        myList.ReplaceRange(0, new string[] { "呼咕啦", "你好", "欢迎", "来到" });
        yield return new WaitForSeconds((1f));
        myList.ReplaceRange(0, new string[] { "来到", "呼咕啦", "你好", "欢迎" });
        yield return new WaitForSeconds((1f));
        myList.ReplaceRange(0, new string[] { "欢迎", "来到", "呼咕啦", "你好" });
        yield return new WaitForSeconds(1);
        myList.ReplaceRange(0, new string[] { "你好", "欢迎", "来到", "呼咕啦" });
        yield return new WaitForSeconds((0.45f));
        myList.ReplaceRange(0, new string[] { "", "", "", "" });
        yield return new WaitForSeconds((0.45f));
        myList.ReplaceRange(0, new string[] { "你好", "欢迎", "来到", "呼咕啦" });
        yield return new WaitForSeconds((0.45f));
        myList.ReplaceRange(0, new string[] { "", "", "", "" });
        yield return new WaitForSeconds((0.45f));
        myList.ReplaceRange(0, new string[] { "你好", "", "呼咕啦", "" });
        yield return new WaitForSeconds(0.5f);
        myList.Clear();
        yield return new WaitForSeconds(0.5f);
        myList.InsertRange(0, new string[] { "你好", "欢迎", "来到", "呼咕啦" });

    }
    public ObservableCollection<string> myList = new ObservableCollection<string>();
    public ICommand onClick;

    void ClickHandler(object parameter)
    {
        Debug.LogFormat("you are click {0} ", parameter);
    }

}
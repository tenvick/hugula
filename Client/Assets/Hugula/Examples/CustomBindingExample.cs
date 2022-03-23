using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;


public class CustomBindingExample : MonoBehaviour
{
    [SerializeField] BindableContainer container;
    [SerializeField] string m_Txt = "custom binder Text";
    [SerializeField] string m_Sprite = "0";
    [SerializeField] bool m_BtnInteractable = true;

    [Range(0, 1)]
    [SerializeField] float m_SliderValue = 0.2f;

    public string txt { get { return m_Txt; } set { m_Txt = value; } }

    public float value
    {
        get
        {
            return m_SliderValue;
        }
        set
        {
            m_SliderValue = value;
        }
    }

    public string sprite { get { return m_Sprite; } set { m_Sprite = value; } }

    public bool btnInteractable { get { return m_BtnInteractable; } set { m_BtnInteractable = value; } }



    [SerializeField] private UnityEngine.UI.Button.ButtonClickedEvent m_Click = new UnityEngine.UI.Button.ButtonClickedEvent();

    public UnityEngine.UI.Button.ButtonClickedEvent click
    {
        get
        {
            return m_Click;
        }
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return Hugula.ResLoader.Ready;
        yield return null;
        yield return null;

        m_Click.AddListener(OnBtnClick);
        container.context = this;
    }

    void OnBtnClick()
    {
        Debug.Log(" you are click Button");
    }


}

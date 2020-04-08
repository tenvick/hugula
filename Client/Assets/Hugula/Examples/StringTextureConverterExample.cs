using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;
using Hugula.Databinding.Binder;

public class StringTextureConverterExample : BindableObject
{
    [SerializeField]
    private string m_RawImageName;
    public string rawImageName
    {
        get
        {
            return m_RawImageName;
        }
        set
        {
            SetProperty<string>(ref m_RawImageName, value);
        }
    }
    public BindableContainer container;
    // Start is called before the first frame update
    void Start()
    {
        container.context = this;
    }

}

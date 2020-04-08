using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hugula.Databinding
{
    ///<summary>
    ///直接绑定UnityEngine.Object对象
    ///</summary>
    public class CustomBinder : BindableObject
    {

        protected override void InitBindingsDic()
        {
            m_IsbindingsDictionary = true;
            foreach (var item in bindings)
            {
                item.target = this.target;
                m_BindingsDic[item.propertyName] = item;
            }
        }

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding.Binder
{
    ///<summary>
    ///直接绑定UnityEngine.Object对象
    ///</summary>
    [RequireComponent(typeof(UnityEngine.Component))]
    public sealed class CustomBinder : UIBehaviourBinder<Component>
    {

        protected override void InitBindingsDic()
        {
            m_IsbindingsDictionary = true;
            foreach (var item in bindings)
            {
                item.target = target;
                m_BindingsDic[item.propertyName] = item;
                // Debug.LogWarningFormat("InitBindingsDic({0},{1},{2}) frameCount={3} ", item.propertyName, item, binderTarget,Time.frameCount);
            }
        }

    }

}
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding
{
    /// <summary>
    /// 多目标对象绑定
    /// 每个binder都单独序列化自己的Binding.target对象。
    /// </summary>
    public sealed class MulitTargetsBinder : BindableObject,ISharedContext
    {

    }
}
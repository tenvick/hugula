using System;
using UnityEngine;

namespace Hugula
{

    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class BindingsAttribute : PropertyAttribute
    {

        // Attribute used to make a float or int variable in a script be restricted to a specific range.
        public BindingsAttribute()
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class PopUpComponentsAttribute : PropertyAttribute
    {

        // Attribute used to make a float or int variable in a script be restricted to a specific range.
        public PopUpComponentsAttribute()
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class BindableObjectAttribute : PropertyAttribute
    {

        // Attribute used to make a float or int variable in a script be restricted to a specific range.
        public BindableObjectAttribute()
        {

        }
    }
    

    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class PopUpObjectAttribute : PropertyAttribute
    {

        // Attribute used to make a float or int variable in a script be restricted to a specific range.
        public PopUpObjectAttribute()
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class NormalAttribute : PropertyAttribute
    {

        // Attribute used to make a float or int variable in a script be restricted to a specific range.
        public NormalAttribute()
        {

        }
    }

}
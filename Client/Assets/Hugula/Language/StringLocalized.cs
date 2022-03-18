using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Databinding;
using System;

namespace Hugula
{

    public class StringLocalized : IValueConverter
    {
        public object Convert(object value, Type targetType)
        {
            return Localization.Get(value.ToString());
        }

        public object ConvertBack(object value, Type targetType)
        {
            return value;
        }

        public static void Register()
        {
            ValueConverterRegister.instance?.AddConverter(typeof(StringLocalized).Name, new StringLocalized());
        }

        public static void UnRegister()
        {
            ValueConverterRegister.instance?.RemoveConverter(typeof(StringLocalized).Name);
        }
    }
}
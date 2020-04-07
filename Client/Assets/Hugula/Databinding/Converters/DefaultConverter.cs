using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding {

    public class DefaultConverter : IValueConverter {
        readonly Type StringType = typeof (string);
        object SystemConvert (object value, Type targetType) {
            if (targetType == null) {
                throw new ArgumentNullException ("targetType");
            }

            if (value == null) {
                return null;
            }

            var sourceType = value.GetType ();
            if (targetType.IsAssignableFrom (sourceType)) {
                return value;
            }

            if (targetType.IsEnum) {
                if (value is string) {
                    var enumValue = Enum.Parse (targetType, (string) value);
                    return enumValue;
                } else {
                    var number = System.Convert.ChangeType (value, Enum.GetUnderlyingType (targetType));
                    var enumValue = Enum.ToObject (targetType, number);
                    return enumValue;
                }
            }

            if (targetType == StringType) {
                var stringValue = value.ToString ();
                return stringValue;
            }

            var objectValue = System.Convert.ChangeType (value, targetType);
            return objectValue;
        }
        public object Convert (object value, Type targetType) {
            return SystemConvert (value, targetType);
        }

        public object ConvertBack (object value, Type targetType) {
            return SystemConvert (value, targetType);
        }
    }
}
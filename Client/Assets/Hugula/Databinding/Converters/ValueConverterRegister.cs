using System;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;

namespace Hugula.Databinding {

    public class ValueConverterRegister : Singleton<ValueConverterRegister>, IDisposable {
        private readonly Dictionary<string, object> m_Converter;

        private DefaultConverter m_DefaultConverter;

        public IValueConverter DefaultConverter {
            get { return m_DefaultConverter; }
        }

        public ValueConverterRegister () {
            m_Converter = new Dictionary<string, object> ();
            m_DefaultConverter = new DefaultConverter ();
            AddConverter ("Default", m_DefaultConverter);
        }

        public void AddConverter (string name, object converter) {
            object outConvts;
            if (m_Converter.TryGetValue (name, out outConvts)) {
                Debug.LogWarningFormat ("The key({0}) converter({1}) already exists", name, outConvts);
            }
            m_Converter.Add (name, converter);
        }

        public void RemoveConverter (string name) {
            m_Converter.Remove (name);
        }

        public object Get (string name) {
            object outConvts = null;
            m_Converter.TryGetValue (name, out outConvts);
            return outConvts;
        }

        public IValueConverter<S, T> Get<S, T> (string name) {
            object outConvts = null;
            m_Converter.TryGetValue (name, out outConvts);
            return (IValueConverter<S, T>) outConvts;
        }

        public override void Dispose () {
            m_Converter.Clear ();
            m_DefaultConverter = null;
            base.Dispose();
        }

    }

}
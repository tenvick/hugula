// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula

using System.Collections;
using UnityEngine;

namespace Hugula.Cryptograph
{

    public static class DESHelper
    {
        public const string DES_FILE_NAME = "K18";
        static KeyVData m_KeyVData;
        public static KeyVData KeyVData
        {
            get
            {
                if (m_KeyVData == null)
                {
                    m_KeyVData = Resources.Load<KeyVData>(DES_FILE_NAME);
                }
                return m_KeyVData;
            }
        }

    }
}
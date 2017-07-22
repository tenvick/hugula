// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System.Collections;

namespace Hugula.Cryptograph
{
    [SLua.CustomLuaClass]
    public class DESHelper : MonoBehaviour
    {

        public KeyVData KEYData;

        public KeyVData IVData;


        // Use this for initialization
        void Awake()
        {
            _desHlper = this;
        }

        public byte[] Key
        {
            get
            {
                return KEYData.KEY;
            }
        }

        public byte[] IV
        {
            get
            {
                return IVData.IV;
            }
        }

        private static DESHelper _desHlper;

        public static DESHelper instance
        {
            get
            {
                return _desHlper;
            }
        }

    }
}
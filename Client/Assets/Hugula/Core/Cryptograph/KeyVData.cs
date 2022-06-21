﻿// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System.Collections;

namespace Hugula.Cryptograph
{
    public class KeyVData : ScriptableObject
    {
        public byte[] KEY;
        public byte[] IV;
        public int version;
    }
}

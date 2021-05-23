using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace PSDUIImporter
{
    public class Size
    {
        public float width;
        public float height;

        public override string ToString()
        {
            return string.Format("width={0},height={1}",width,height);
        }
    }
}
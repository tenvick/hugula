using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace PSDUIImporter
{
    public class Position
    {
        public float x;
        public float y;

        public override string ToString()
        {
            return string.Format("x={0},y={1} ",x,y);
        }

         public static Vector2 operator +(Position man, Vector2 femal) 
         {
            femal.x += man.x;
            femal.y += man.y;
            return femal;
         }

        public static Vector2 operator +(Vector2 femal, Position man) 
         {
            femal.x += man.x;
            femal.y += man.y;
            return femal;
         }

         public static Vector2 operator -(Position man, Vector2 femal) 
         {
            femal.x = man.x - femal.x;
            femal.y = man.y - femal.y;
            return femal;
         }

        public static Vector2 operator -(Vector2 femal , Position man ) 
         {
            femal.x -= man.x;
            femal.y -= man.y;
            return femal;
         }
    }
}
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace PSDUINewImporter
{
    public class PSDUtils
    {

        public static IComponentImport CreateComponentImport(string typeName, PSDComponentImportCtrl ctrl)
        {
            var type = FindComponentImportType(typeName);
            var argType = new Type[1];
            argType[0] = typeof(PSDComponentImportCtrl);

            ConstructorInfo ctr = type.GetConstructor(argType);
            var ret = ctr.Invoke(new object[] { ctrl });
            return (IComponentImport)ret;
        }

        private static Type FindComponentImportType(string typeName)
        {
            if(typeName.StartsWith("cus#"))
            {
                typeName = "Customer";
            }
            string binderType = string.Format("PSDUINewImporter.{0}ComponentImport", typeName); 
            var reType = Hugula.Utils.LuaHelper.GetClassType(binderType);
            if(reType == null)
            {
                Debug.LogErrorFormat("ImportType({0})ComponentImport was't found. use DefaultComponentImport repleaced. ", binderType);
                reType = typeof(DefaultComponentImport);
            }

            return reType;
        }
    }
}
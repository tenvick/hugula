using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

#if UNITY_5_3
using UnityEditor.SceneManagement;
#endif

namespace PSDUIImporter
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
            string binderType = string.Format("PSDUIImporter.{0}ComponentImport", typeName); 
            var reType = Hugula.Utils.LuaHelper.GetClassType(binderType);
            if(reType == null)
                Debug.LogErrorFormat("ImportType {0} isn't find. ",binderType);

            return reType;
        }
    }
}
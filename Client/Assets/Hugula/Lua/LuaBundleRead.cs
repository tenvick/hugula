using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using Hugula.Utils;
using System.IO;

namespace Hugula
{
    public class LuaBundleRead
    {
        private AssetBundle m_LuaBundle;
        public LuaBundleRead(string zipPath, bool android)
        {
            m_LuaBundle = AssetBundle.LoadFromFile(zipPath, 0, Common.BUNDLE_OFF_SET);
        }

        public LuaBundleRead(string path)
        {
            m_LuaBundle = AssetBundle.LoadFromFile(path, 0, Common.BUNDLE_OFF_SET);
        }

        public byte[] LoadBytes(string name)
        {
            byte[] ret = null;
            var txt = m_LuaBundle.LoadAsset<TextAsset>(name);
            if (txt != null)
            {
                ret = txt.bytes;
                Resources.UnloadAsset(txt); //释放ab资源
                // m_LuaBundle.Unload(false);
            }
            return ret;
        }

    }

}
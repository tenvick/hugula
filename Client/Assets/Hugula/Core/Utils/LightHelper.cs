using UnityEngine;
using System.Collections.Generic;

namespace Hugula.Utils
{
    /// <summary>
    /// 光影 操作
    /// </summary>
    [SLua.CustomLuaClass]
    public static class LightHelper
    {

        /// <summary>
        /// 设置场景lightmap
        /// </summary>
        /// <param name="index"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public static void SetLightMapSetting(ushort index, Texture2D near, Texture2D far)
        {
            LightmapData[] old = LightmapSettings.lightmaps;
            List<LightmapData> newLightDatas = new List<LightmapData>(old);

            while (newLightDatas.Count <= index)
                newLightDatas.Add(new LightmapData());

            LightmapData newLightData = newLightDatas[index];

            if (far != null) newLightData.lightmapFar = far;
            if (near != null) newLightData.lightmapNear = near;

            LightmapSettings.lightmaps = newLightDatas.ToArray();
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        }
    }
}
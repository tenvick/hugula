using UnityEngine;

namespace Hugula
{

    [SLua.CustomLuaClass]
    public class BytesAsset : ScriptableObject
    {
        public byte[] bytes;
    }
}
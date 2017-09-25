using System.Collections;
using Hugula.Loader;
using Hugula.Pool;
using Hugula.Utils;
using LuaInterface;
using SLua;
using UnityEngine;

namespace Hugula
{

    [SLua.CustomLuaClass]
    public static class LogicHelper
    {

        //asset_loader.lua on_assets_load逻辑
        public static void OnAssetsLoad(CRequest req, LuaTable asset)
        {
            var main = (GameObject)req.data;
            var root = LuaHelper.Instantiate(main);
            root.name = main.name;

            //set root refer
            asset["root"] = root;
            // var refer = root.GetComponentInChildren<ReferGameObjects>(true);
            // asset["refer"] = refer;

            //set child
            Transform pr = root.transform;
            int count = pr.childCount;
            Transform child = null;
            LuaTable items = (LuaTable)asset["items"];
            for (int i = 0; i < count; i++)
            {
                child = pr.GetChild(i);
                items[child.name] = child.gameObject;
            }

        }
    }
}
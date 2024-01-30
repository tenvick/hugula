using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;
using UnityEngine.UI;
using Hugula.Databinding.Binder;
using TMPro;
using Hugula.ResUpdate;
using Hugula.Utils;

public class AddresableDenpTool : MonoBehaviour
{
    public TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckDenp()
    {
        Debug.Log("CheckDenp");
        var sb1 = new System.Text.StringBuilder();
        var keys = new List<object>();
        var resourceLocators = Addressables.ResourceLocators;
        int i = 0;

        foreach (var item in resourceLocators)
        {
            sb1.AppendLine("new Addressables.ResourceLocators:(");
            sb1.Append(item.LocatorId);
            sb1.AppendLine("):");
            keys.Clear();
            keys.AddRange(item.Keys);
            sb1.AppendLine($"  ------------------------------{item.LocatorId}-Count:{keys.Count}------------------");
            var c = keys.Count;
            foreach (var key in keys)
            {
                i++;
                if (item.Locate(key, typeof(UnityEngine.Object), out var locations))
                {
                    foreach (var loc in locations)
                    {
                        var dependencies = loc.Dependencies;
                        sb1.AppendLine($"            -----key:{key} ,LocatorId:{loc.InternalId},dependencies:{dependencies.Count}");
                        for (int x = 0; x < dependencies.Count; x++)
                        {
                            sb1.AppendLine($"                    {dependencies[x].InternalId}");
                        }
                        break;
                    }
                }

            }
            sb1.AppendLine($"  end-------------------------------{item.LocatorId}-Count:{keys.Count}-------------------");

            Debug.Log(sb1.ToString());
        }
    }


    public void CheckZipAddress()
    {
        var sb = new System.Text.StringBuilder();
        var text = inputField.text;
        if(string.IsNullOrEmpty(text)) text = "game_scene,UnityEngine.ResourceManagement.ResourceProviders.SceneInstance";
        sb.AppendLine("CheckZipAddress:" + text);

        var inpus = text.Split(',');
        var address = inpus[0].Trim();
        var type = LuaHelper.GetClassType(inpus[1].Trim());
        var folders = FileManifestManager.FindFolderManifestByAddress(address, type);
        sb.AppendLine($"folders:{folders.Count}");
        foreach (var item in folders)
        {
            sb.AppendLine($"folder:{item.ToString()}");
        }

        Debug.Log(sb.ToString());

        sb.Clear();

        var abNames = FileManifestManager.FindBundleNameByAddress(address, type);
        sb.AppendLine($"abNames:{abNames.Count} ,{address} ,{type}");
        foreach (var item in abNames)
        {
            sb.AppendLine($"abName:{item}");
        }
        var ret = FileManifestManager.FindFolderManifestByBundleName(abNames);
        sb.AppendLine($"ret:{ret.Count}");
        foreach (var item in ret)
        {
            sb.AppendLine($"folder:{item.ToString()}");
        }
        Debug.Log(sb.ToString());

        ListPool<string>.Release(abNames);

    }

}

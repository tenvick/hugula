using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula;
using Hugula.Utils;
using UnityEngine.AddressableAssets;
using System.Text;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor;
#endif
public class CheckResUpdatePackingDenpendences : MonoBehaviour
{
#if UNITY_EDITOR
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Hugula.ResLoader.Init();

        Debug.Log($" wait Hugula.ResLoader.Ready & {Hugula.ResLoader.Ready}");
        while (!ResLoader.Ready)
        {
            yield return null;
        }
        Hugula.ResUpdate.FileManifestManager.LoadStreamingFolderManifests();

        int i = 0;
        float c;

        var sb1 = new System.Text.StringBuilder();
        var keys = new List<object>();
        var resourceLocators = Addressables.ResourceLocators;

        foreach (var item in resourceLocators)
        {
            sb1.AppendLine("new Addressables.ResourceLocators:(");
            sb1.Append(item.LocatorId);
            sb1.AppendLine("):");
            keys.Clear();
            keys.AddRange(item.Keys);
            sb1.AppendLine($"  ------------------------------{item.LocatorId}-Count:{keys.Count}------------------");
            c = keys.Count;
            foreach (var key in keys)
            {
                i++;
                if (item.Locate(key, typeof(UnityEngine.Object), out var locations))
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Addressables.ResourceLocators", key.ToString(), i / c))
                    {
                        break;
                    }
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

        EditorUtility.ClearProgressBar();
        WriteToTmpFile("CheckResUpdatePackingDenpendences_ResourceLocators.txt", sb1.ToString());

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 300, 80), "开始检测"))
        {

            var sb = new StringBuilder();
            var sbItem = new StringBuilder();
            sb.AppendLine("CheckResUpdatePackingDenpendences:");
            //遍历所有资源判断依赖分包情况。
            var allGroups = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups;

            var title = "Check Res UpdatePacking Denpendences";

            var i = 0;
            var c = allGroups.Count;
            var streaming = "streaming";
            try
            {
                foreach (AddressableAssetGroup group in allGroups)
                {
                    i++;
                    if (group == null) continue;
                    if (EditorUtility.DisplayCancelableProgressBar(title, group.Name, i / c))
                    {
                        break;
                    }

                    foreach (AddressableAssetEntry entry in group.entries)
                    {
                        if (entry == null) continue;
                        System.Type targetType = null;
                        if (entry.TargetAsset)
                        {
                            targetType = entry.TargetAsset.GetType();
                        }

                        //var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                        //if (resupPacking == null || resupPacking.packingType != HugulaResUpdatePacking.PackingType.custom)
                        //{

                        //    if (CheckBundleDen(entry.address,entry.AssetPath,targetType, sbItem))
                        //    {
                        //        sb.AppendLine(sbItem.ToString());
                        //    }

                        //}

                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log(sb.ToString());
                WriteToTmpFile("CheckResUpdatePackingDenpendences.txt", sb.ToString());
            }
        }

        str = GUI.TextArea(new Rect(100, 300, 300, 200), str);

        if(GUI.Button(new Rect(100, 500, 200, 100), "检测指定"))
        {
            var sb = new StringBuilder();
            var sbItem = new StringBuilder();
            if (CheckBundleDen(str,"",typeof(Object), sbItem))
            {
                sb.AppendLine(sbItem.ToString());
            }

            EditorUtility.ClearProgressBar();
            Debug.Log(sb.ToString());
            WriteToTmpFile($"CheckResUpdatePackingDenpendences_{str}.txt", sb.ToString());
        }

        if(GUI.Button(new Rect(300, 500, 200, 100), "下载资源"))
        {
            Debug.Log($"ResLoader.LoadAssetAsync ({str})");
            Hugula.Utils.CUtils.DebugCastTime($"ResLoader.LoadAssetAsync({str})");
           ResLoader.LoadAssetAsync<GameObject>(str, (o,a) => { 
            Debug.Log(o); 
            Hugula.Utils.CUtils.DebugCastTime($"ResLoader.LoadAssetAsync({str}) finished");
            ResLoader.Release(0);
            },null);
        }
    }

    string str = "yanhuiting_gangqin";

    bool CheckBundleDen(string address,string AssetPath,System.Type targetType,StringBuilder sbItem)
    {
        List<string> dependenciesInfo = ListPool<string>.Get();
        Hugula.ResUpdate.FileManifestManager.AnalyzeAddressDependencies(address, targetType, dependenciesInfo);
        bool hasZipDependencies = false;
        sbItem.Clear();
        sbItem.AppendLine($"{address} path:{AssetPath}     Dependencies:{dependenciesInfo.Count/3}");
        bool ckCount = dependenciesInfo.Count >= 15;
        for (int j = 0; j < dependenciesInfo.Count;)
        {
            var folderName = dependenciesInfo[j];
            var bundle = dependenciesInfo[j + 2];
            if (folderName != "streaming") //在依赖包
            {
                sbItem.AppendLine($"--     {folderName}        {bundle}");
                hasZipDependencies = true;
            }else if(ckCount)
            {
                sbItem.AppendLine($"     {folderName}        {bundle}");
                hasZipDependencies = true;
            }
            j = j + 3;
        }

        ListPool<string>.Release(dependenciesInfo);

        return hasZipDependencies;
    }

    void WriteToTmpFile(string fileName, string context)
    {
        string tmpPath = Path.Combine(Application.dataPath, "Tmp/");
        if (!Directory.Exists(tmpPath))
        {
            Directory.CreateDirectory(tmpPath);
        }
        string outPath = Path.Combine(tmpPath, fileName);
        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(context);
        }
        Debug.Log("write to path=" + outPath);
    }
#endif
}

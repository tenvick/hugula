using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace HugulaEditor.ResUpdate
{
    public class ResUpdateMenu 
    {
        private const  string ResUpdatePackingPath = "Assets/Hugula/ResUpdate/Addressable/Editor/Config/ResUpdatePacking.txt";
        [MenuItem("Hugula/AAS Tool/Update Addressables Group HugulaResUpdatePacking By Config(ResUpdatePacking.txt)", false, 211)]
        static void UpdateAddressablesGroupHugulaResUpdatePacking()
        {

            var sb = new StringBuilder();
            sb.AppendLine("ResUpdatePacking:");
            Dictionary<string, object[]> addressPackingName = new Dictionary<string, object[]>();
            string line;
            string[] kv;
            using (var sr = new StreamReader(ResUpdatePackingPath,true))
            {
                while(!sr.EndOfStream)
                {
                    line= sr.ReadLine();
                    if(!line.Contains("//"))
                    {
                        kv = line.Split(',');
                        if(kv.Length==2)
                        {
                            addressPackingName[kv[0].Trim()] = new object[] { kv[1].Trim(), typeof(UnityEngine.Object) };
                            sb.AppendLine($"address[{kv[0].Trim()}],folderName({kv[1].Trim()}),type({typeof(UnityEngine.Object)})");

                        }
                        else if(kv.Length>2)
                        {
                            var tp = Hugula.Utils.LuaHelper.GetClassType(kv[2].Trim());
                            addressPackingName[kv[0].Trim()] = new object[] { kv[1].Trim(), tp };
                            sb.AppendLine($"address[{kv[0].Trim()}],folderName({kv[1].Trim()}),type({tp})");
                        }
                    }
                }
            }

            Debug.Log(sb.ToString());

           var allGroups = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups;

            sb.AppendLine("AddressableAssetGroup:");

            var title = "Check AddressableAssetGroup entries";
            EditorUtility.DisplayProgressBar(title, title, 0);
            int i;
            float c;
            i = 0;
            c = allGroups.Count;
            try
            {

            foreach (AddressableAssetGroup group in allGroups)
            {
                i++;
                if (group == null) continue;
                if (EditorUtility.DisplayCancelableProgressBar(title,group.Name,i/c))
                {
                    break;
                }
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    if (entry == null) continue;
                    if (EditorUtility.DisplayCancelableProgressBar(title, entry.address, i / c))
                    {
                        break;
                    }

                    System.Type targetType = null;
                    if(entry.TargetAsset)
                    {
                        targetType = entry.TargetAsset.GetType(); 
                    }
                    else
                    {
                        var str = $"LogWarning[{ group.Name}] [{ entry.address}], ({ entry.TargetAsset}), { entry.AssetPath} entry.TargetAsset is null \r\n";
                        Debug.LogWarning(str);
                        sb.AppendLine(str);
                        continue;
                    }

                    Debug.Log($"{entry.address}");
                    if ( addressPackingName.TryGetValue(entry.address,out var objs))
                    {
                        var type = objs[1];
                        if ( targetType.IsSubclassOf((System.Type)type) || targetType.Equals((System.Type)type))
                        {

                            var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                            if (resupPacking == null)
                            {
                                resupPacking = group.AddSchema<HugulaResUpdatePacking>();
                            }
                            else
                            {
                                if(resupPacking.packingType != HugulaResUpdatePacking.PackingType.custom)
                                {
                                    var str = $"LogWarning[{ group.Name}].HugulaResUpdatePacking.packingType is { resupPacking.packingType } now force change to PackingType.custom .[{ entry.address}], ({ entry.TargetAsset.GetType()}), { entry.AssetPath} \r\n";
                                    Debug.LogWarning(str);
                                    sb.AppendLine(str);
                                }
                            }

                            resupPacking.packingType = HugulaResUpdatePacking.PackingType.custom;
                            resupPacking.customName = objs[0].ToString();
                            EditorUtility.SetDirty(resupPacking);
                            AssetDatabase.SaveAssets();

                            sb.AppendLine($"[{group.Name}]:  \r\n               zipFolderName:{objs[0].ToString()}     found:[{entry.address}], ({entry.TargetAsset.GetType()}), {entry.AssetPath} \r\n");
                            //debug info
                            foreach(var g in group.entries)
                            {
                                if (g == null) continue;
                                sb.AppendLine($"                [{g.address}], ({g.TargetAsset.GetType()}), {g.AssetPath}");
                            }

                            break;
                        }

                    }

                }

            }

            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
            EditorUtility.ClearProgressBar();
            Debug.Log(sb.ToString());
            EditorUtils.WriteToTmpFile("Update_AddressablesGroup_HugulaResUpdatePacking.txt",sb.ToString());
            AssetDatabase.Refresh();
            }

        }
       }
}
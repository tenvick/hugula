using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using Hugula.Utils;
using UnityEngine.AddressableAssets;
using System.Text.RegularExpressions;
using Hugula.Utils;

namespace HugulaEditor.ResUpdate
{
    public class ResUpdateMenu
    {

        [MenuItem("Hugula/Gen AAS Packing Schema/Clear All Group HugulaResUpdatePacking", false, 211)]
        static void ClearAddressablesGroupHugulaResUpdatePacking()
        {
            int i;
            float c;
            var sb = new StringBuilder();
            var allGroups = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups;

            var title = "Clear AddressableAssetGroup entries";
            EditorUtility.DisplayProgressBar(title, title, 0);
            i = 0;
            c = allGroups.Count;
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

                    var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                    bool remove = false;
                    if (resupPacking != null)
                    {
                        remove = group.RemoveSchema<HugulaResUpdatePacking>();
                        var str = $"LogWarning[{group.Name}].RemoveSchema<HugulaResUpdatePacking>() = {remove} \r\n";
                        Debug.LogWarning(str);
                        sb.AppendLine(str);
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
                AssetDatabase.Refresh();
            }
        }

        private const string ResUpdatePackingPath = "Assets/Hugula/ResUpdate/Addressable/Editor/Config/ResUpdatePacking.txt";
        [MenuItem("Hugula/Gen AAS Packing Schema/Update Addressables Group HugulaResUpdatePacking By Config(ResUpdatePacking.txt)", false, 211)]
        static void UpdateAddressablesGroupHugulaResUpdatePacking()
        {

            var sb = new StringBuilder();
            sb.AppendLine("ResUpdatePacking:");
            Dictionary<string, object[]> addressPackingName = new Dictionary<string, object[]>();
            string line;
            string[] kv;
            int lineNumber = 0;
            using (var sr = new StreamReader(ResUpdatePackingPath, true))
            {
                while (!sr.EndOfStream)
                {
                    lineNumber++;
                    line = sr.ReadLine();
                    if (!line.Contains("//"))
                    {
                        kv = line.Split(',');
                        if (kv.Length == 2)
                        {
                            addressPackingName[kv[0].Trim()] = new object[] { kv[1].Trim(), typeof(UnityEngine.Object) };
                            sb.AppendLine($"address[{kv[0].Trim()}],folderName({kv[1].Trim()}),type({typeof(UnityEngine.Object)})");

                        }
                        else if (kv.Length == 3)
                        {
                            var tp = Hugula.Utils.LuaHelper.GetClassType(kv[2].Trim());
                            if (tp == null)
                            {
                                var erro = $"line:{lineNumber}  {line}   error type({kv[1].Trim()}) is null ";
                                Debug.LogError(erro);
                                sb.AppendLine(erro);
                            }
                            else
                            {
                                addressPackingName[kv[0].Trim()] = new object[] { kv[1].Trim(), tp };
                                sb.AppendLine($"address[{kv[0].Trim()}],folderName({kv[1].Trim()}),type({tp})");
                            }
                        }
                        else if (kv.Length > 3)
                        {
                            var tp = Hugula.Utils.LuaHelper.GetClassType(kv[2].Trim());
                            if (tp == null)
                            {
                                var erro = $"line:{lineNumber} {line} error type({kv[1].Trim()}) is null ";
                                Debug.LogError(erro);
                                sb.AppendLine(erro);
                            }
                            else
                            {
                                addressPackingName[kv[0].Trim()] = new object[] { kv[1].Trim(), tp, kv[3] };
                                sb.AppendLine($"address[{kv[0].Trim()}],folderName({kv[1].Trim()}),type({tp})");
                            }
                        }
                    }
                }
            }

            Debug.Log(sb.ToString());

            var allGroups = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups;

            sb.AppendLine("AddressableAssetGroup:");

            int i;
            float c = allGroups.Count;

            var PackingTypeMapping = GetPackingTypeMapping();


            i = 0;
            try
            {
                var title = "Check AddressableAssetGroup entries";

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
                        else
                        {
                            var str = $"LogWarning[{group.Name}] [{entry.address}], ({entry.TargetAsset}), {entry.AssetPath} entry.TargetAsset is null \r\n";
                            Debug.LogWarning(str);
                            sb.AppendLine(str);
                            continue;
                        }

                        Debug.Log($"{entry.address}");
                        if (addressPackingName.TryGetValue(entry.address, out var objs))
                        {
                            var type = objs[1];
                            if (targetType.IsSubclassOf((System.Type)type) || targetType.Equals((System.Type)type))
                            {

                                var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                                if (resupPacking == null)
                                {
                                    resupPacking = group.AddSchema<HugulaResUpdatePacking>();
                                }
                                else
                                {
                                    if (resupPacking.packingType != HugulaResUpdatePacking.PackingType.custom)
                                    {
                                        var str = $"LogWarning[{group.Name}].HugulaResUpdatePacking.packingType is {resupPacking.packingType} now force change to PackingType.custom .[{entry.address}], ({entry.TargetAsset.GetType()}), {entry.AssetPath} \r\n";
                                        Debug.LogWarning(str);
                                        sb.AppendLine(str);
                                    }
                                }

                                var customName = objs[0].ToString().ToLower();

                                if (PackingTypeMapping.TryGetValue(customName, out var packingType))
                                {
                                    resupPacking.packingType = packingType;
                                }
                                else
                                {
                                    resupPacking.packingType = HugulaResUpdatePacking.PackingType.custom;
                                }

                                resupPacking.customName = customName;
                                int priority = 0;
                                if (objs.Length >= 3 && int.TryParse(objs[2].ToString(), out priority))
                                {
                                    resupPacking.priority = priority;
                                }
                                EditorUtility.SetDirty(resupPacking);
                                // AssetDatabase.SaveAssets();

                                sb.AppendLine($"[{group.Name}]:  \r\n               zipFolderName:{customName}     found:[{entry.address}], ({entry.TargetAsset.GetType()}), {entry.AssetPath} \r\n");
                                //debug info
                                foreach (var g in group.entries)
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
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log(sb.ToString());
                EditorUtils.WriteToTmpFile("Update_AddressablesGroup_HugulaResUpdatePacking.txt", sb.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }

        private const string ResUpdatePackingCustomFolderPath = "Assets/Hugula/ResUpdate/Addressable/Editor/Config/ResUpdatePackingCustomFolder.txt";
        [MenuItem("Hugula/Gen AAS Packing Schema/Update Addressables Group HugulaResUpdatePacking By Folder(ResUpdatePackingCustomFolder.txt)", false, 211)]
        static void UpdateAddressablesGroupHugulaResUpdatePackingByFolder()
        {
            // P{d},demand_p_{d},{d}
            var sb = new StringBuilder();
            sb.AppendLine("ResUpdatePackingCustomFolder:");
            List<Regex> regices = new List<Regex>();
            List<string[]> regexArgs = new List<string[]>();
            string line;
            string[] kv;
            int lineNumber = 0;
            using (var sr = new StreamReader(ResUpdatePackingCustomFolderPath, true))
            {
                while (!sr.EndOfStream)
                {
                    lineNumber++;
                    line = sr.ReadLine();
                    if (!line.Contains("//"))
                    {
                        kv = line.Split(',');
                        if (kv.Length >= 3)
                        {
                            regices.Add(new Regex(kv[0].Trim()));
                            regexArgs.Add(new string[] { kv[1].Trim(), kv[2].Trim() });
                            sb.AppendLine($"folder[{kv[0].Trim()}],Custom Packing Name({kv[1].Trim()}),Priority({kv[2].Trim()})");
                        }
                    }
                }
            }

            Debug.Log(sb.ToString());

            var allGroups = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups;

            sb.AppendLine("AddressableAssetGroup:");

            var PackingTypeMapping = GetPackingTypeMapping();

            int i;
            float c = allGroups.Count;
            i = 0;
            try
            {
                var title = "Check AddressableAssetGroup entries";
                var dirtyList = new List<HugulaResUpdatePacking>();

                foreach (AddressableAssetGroup group in allGroups)
                {
                    i++;
                    if (group == null || group.Name.StartsWith("dup_g") || group.Name.StartsWith("dup_s")) continue;
                    if (EditorUtility.DisplayCancelableProgressBar(title, group.Name, i / c))
                    {
                        break;
                    }

                    foreach (AddressableAssetEntry entry in group.entries)
                    {
                        if (entry == null) continue;

                        if (ReplaceMatch(regices, regexArgs, entry.AssetPath, out var customName, out var priority))
                        {

                            bool isDirty = false;
                            var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                            if (resupPacking == null)
                            {
                                resupPacking = group.AddSchema<HugulaResUpdatePacking>();
                                isDirty = true;

                            }
                            else
                            {
                                if (resupPacking.packingType != HugulaResUpdatePacking.PackingType.custom)
                                {
                                    var str = $"LogWarning[{group.Name}].HugulaResUpdatePacking.packingType is {resupPacking.packingType} now force change to PackingType.custom .[{entry.address}], ({entry.TargetAsset.GetType()}), {entry.AssetPath} \r\n";
                                    Debug.LogWarning(str);
                                    sb.AppendLine(str);
                                    isDirty = true;
                                }
                            }

                            if (PackingTypeMapping.TryGetValue(customName, out var packingType))
                            {
                                isDirty = resupPacking.packingType != packingType;
                                resupPacking.packingType = packingType;
                            }
                            else
                            {
                                isDirty = resupPacking.packingType != HugulaResUpdatePacking.PackingType.custom;
                                resupPacking.packingType = HugulaResUpdatePacking.PackingType.custom;
                            }

                            sb.AppendLine($"group:{group.Name}      packName:{customName}     asset:{entry.AssetPath}");

                            isDirty = resupPacking.customName != customName;
                            resupPacking.customName = customName;
                            isDirty = resupPacking.priority != priority;
                            resupPacking.priority = priority;
                            if (isDirty)
                                dirtyList.Add(resupPacking);
                            Debug.Log($"ReplaceMatch(  path:{entry.AssetPath},customName:{customName},priority:{priority} )  ");
                            break;
                        }
                    }
                }

                foreach (var resPacking in dirtyList)
                    EditorUtility.SetDirty(resPacking);

            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log(sb.ToString());
                EditorUtils.WriteToTmpFile("Update_AddressablesGroup_ResUpdatePackingCustomFolder.txt", sb.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }

        static bool ReplaceMatch(List<Regex> regices, List<string[]> regexArgs, string path, out string customName, out int priority)
        {
            priority = 1;
            customName = string.Empty;
            string strPriority = string.Empty;

            for (int j = 0; j < regices.Count; j++)
            {
                var regex = regices[j];
                var match = regex.Match(path);
                var args = regexArgs[j];
                if (match.Success)
                {
                    customName = args[0];
                    strPriority = args[1];
                    var groups = regex.GetGroupNumbers();
                    foreach (int gi in groups)
                    {
                        var gName = regex.GroupNameFromNumber(gi);
                        if (!string.IsNullOrEmpty(gName))
                        {
                            var pName = $"${{{gName}}}";
                            customName = customName.Replace(pName, match.Groups[gName].ToString());
                            strPriority = strPriority.Replace(pName, match.Groups[gName].ToString());
                            // Debug.Log($"groupName: {gName}, category:{match.Groups[gName]} ;  Name:{match.Name},Value:{match.Value}     customName:{customName}, strPriority:{strPriority}           path:{path} ");
                        }
                    }
                    int.TryParse(strPriority, out priority);
                    return true;
                }
            }

            return false;
        }
        // [MenuItem("Hugula/Res Packing/CheckResUpdatePackingDenpendences(Need Build)", false, 212)]

        static Dictionary<string, HugulaResUpdatePacking.PackingType> GetPackingTypeMapping()
        {
            var PackingTypeMapping = new Dictionary<string, HugulaResUpdatePacking.PackingType>();
            PackingTypeMapping.Add("fast", HugulaResUpdatePacking.PackingType.fast);
            PackingTypeMapping.Add("demand", HugulaResUpdatePacking.PackingType.demand);

            return PackingTypeMapping;
        }
        static void CheckResUpdatePackingDenpendences()
        {

            EditorUtils.InvokeStatic(typeof(Hugula.ResLoader), "Init");
            Hugula.ResUpdate.FileManifestManager.LoadStreamingFolderManifests();

            Debug.Log($" wait Hugula.ResLoader.Ready & {Hugula.ResLoader.Ready}");
            int i = 0;
            float c;
            // #if !HUGULA_NO_LOG
            var sb1 = new System.Text.StringBuilder();
            var keys = new List<object>();
            foreach (var item in Addressables.ResourceLocators)
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
                        sb1.AppendLine($"            -----key:{key} ,LocatorId:{item.LocatorId} Count:{locations.Count}");
                        foreach (var loc in locations)
                        {
                            sb1.AppendLine($"                    locations:{key.ToString()} (type:{loc.ResourceType},PrimaryKey:{loc.PrimaryKey},InternalId:{loc.InternalId})");
                        }
                        // sb1.AppendLine($"            end-----{item.LocatorId}-{key.ToString()}--Count:{locations.Count}");
                    }
                    // else
                    //     sb1.AppendLine($"            -----{item.LocatorId}-{key.ToString()}--Count:0");

                }
                sb1.AppendLine($"  end-------------------------------{item.LocatorId}-Count:{keys.Count}-------------------");

                Debug.Log(sb1.ToString());
            }

            EditorUtils.WriteToTmpFile("CheckResUpdatePackingDenpendences_ResourceLocators.txt", sb1.ToString());

            // #endif

            var sb = new StringBuilder();
            var sbItem = new StringBuilder();
            sb.AppendLine("CheckResUpdatePackingDenpendences:");
            //遍历所有资源判断依赖分包情况。
            var allGroups = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups;

            var title = "Check Res UpdatePacking Denpendences";
            EditorUtility.DisplayProgressBar(title, title, 0);

            i = 0;
            c = allGroups.Count;
            var streaming = HugulaResUpdatePacking.PackingType.streaming.ToString();
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
                        if (EditorUtility.DisplayCancelableProgressBar(title, entry.address, i / c))
                        {
                            break;
                        }

                        System.Type targetType = null;
                        if (entry.TargetAsset)
                        {
                            targetType = entry.TargetAsset.GetType();
                        }

                        var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                        if (resupPacking == null || resupPacking.packingType != HugulaResUpdatePacking.PackingType.custom)
                        {
                            List<string> dependenciesInfo = ListPool<string>.Get();
                            Hugula.ResUpdate.FileManifestManager.AnalyzeAddressDependencies(entry.address, targetType, dependenciesInfo);
                            bool hasZipDependencies = false;
                            sbItem.Clear();
                            sbItem.AppendLine($"{entry.address} path:{entry.AssetPath}     Dependencies:");
                            for (int j = 0; j < dependenciesInfo.Count;)
                            {
                                var folderName = dependenciesInfo[j];
                                var bundle = dependenciesInfo[j + 2];
                                if (folderName != streaming) //在依赖包
                                {
                                    sbItem.AppendLine($"     {folderName}        {bundle}");
                                    hasZipDependencies = true;
                                }
                                j = j + 3;
                            }

                            ListPool<string>.Release(dependenciesInfo);
                            if (hasZipDependencies)
                            {
                                sb.AppendLine(sbItem.ToString());
                            }

                        }

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
                EditorUtils.WriteToTmpFile("CheckResUpdatePackingDenpendences.txt", sb.ToString());
                AssetDatabase.Refresh();
            }
        }
    }
}
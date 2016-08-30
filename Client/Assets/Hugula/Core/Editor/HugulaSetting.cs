using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 本地设置
/// </summary>
public class HugulaSetting : ScriptableObject
{
    /// <summary>
    /// 列表
    /// </summary>
    public List<string> AssetLabels = new List<string>();

    public const string SettingPath = "Assets/Hugula/Core/Hugula.asset";

    private static HugulaSetting _instance = null;

    public static HugulaSetting instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<HugulaSetting>(SettingPath);
                if (_instance == null)
                {
                    _instance = HugulaSetting.CreateInstance<HugulaSetting>();
                    AssetDatabase.CreateAsset(_instance, SettingPath);
                }
            }
            return _instance;
        }
    }
}

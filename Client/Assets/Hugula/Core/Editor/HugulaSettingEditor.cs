using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Hugula;

/// <summary>
/// 
/// </summary>
public class HugulaSettingEditor  {

	public const string SettingPath = "Assets/Config/Hugula.asset";

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
                    _instance = ScriptableObject.CreateInstance<HugulaSetting>();
                    _instance.AssetLabels = new List<string>();
                    AssetDatabase.CreateAsset(_instance, SettingPath);
                }
            }
            return _instance;
        }
    }
}

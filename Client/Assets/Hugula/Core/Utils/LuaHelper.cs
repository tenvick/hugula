// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

using SLua;
using LuaState = SLua.LuaState;

/// <summary>
/// lua helper类
/// </summary>
[SLua.CustomLuaClass]
public class  LuaHelper {

    /// <summary>
    /// Destroy object
    /// </summary>
    /// <param name="original"></param>
    public static void Destroy(Object original)
    {
        GameObject.Destroy(original);
    }

    public static void Destroy(Object original,float t)
    {
        GameObject.Destroy(original,t);
    }

    public static void DestroyImmediate(Object original)
    {
        GameObject.DestroyImmediate(original);
    }

    public static void DestroyImmediate(Object original, bool allowDestroyingAssets)
    {
        GameObject.DestroyImmediate(original,allowDestroyingAssets);
    }

    /// <summary>
    /// Instantiate Object
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static Object Instantiate(Object original)
    {
        return GameObject.Instantiate(original);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
	public static GameObject InstantiateLocal(GameObject original,GameObject parent)
    {
		return InstantiateLocal(original,parent,Vector3.zero);
    }

	public static GameObject InstantiateLocal(GameObject original)
	{
		return InstantiateLocal(original,null,Vector3.zero);
	}

	public static GameObject InstantiateLocal(GameObject original,Vector3 pos)
	{
		return InstantiateLocal(original,null,pos);
	}

	public static GameObject InstantiateLocal(GameObject original,GameObject parent,Vector3 pos)
	{
		var tranformTa = original.transform;
		if(pos==Vector3.zero) pos = tranformTa.localPosition;
		Quaternion rota = tranformTa.localRotation;
		Vector3 scale = tranformTa.localScale;
		GameObject clone = (GameObject)GameObject.Instantiate(original);
		var transform=clone.transform;
		if(parent!=null)clone.transform.parent = parent.transform;
		transform.localPosition = pos;
		transform.localScale = scale;
		transform.localRotation = rota;
		return clone;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
	public static GameObject InstantiateGlobal(GameObject original, GameObject parent=null)
    {
        var tranformTa = original.transform;
		var pos = tranformTa.position;
		Quaternion rota = tranformTa.rotation;
        Vector3 scale = tranformTa.localScale;
        GameObject clone = (GameObject)GameObject.Instantiate(original);
        var transform = clone.transform;
        if (parent != null) clone.transform.parent = parent.transform;
        transform.position = pos;
        transform.localScale = scale;
        transform.rotation = rota;
        return clone;
    }

    /// <summary>
    /// 设置父对象
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    public static void SetParent(GameObject child, GameObject parent)
    {
        var tranformTa = child.transform;
        var  pos = tranformTa.localPosition;
        var rota = tranformTa.localRotation;
        var scale = tranformTa.localScale;

        child.transform.parent = parent.transform;
        tranformTa.localPosition = pos;
        tranformTa.localScale = scale;
        tranformTa.localRotation = rota;
    }

    /// <summary>
    /// getType
    /// </summary>
    /// <param name="classname"></param>
    /// <returns></returns>
    public static System.Type GetType(string classname)
    {
        if (string.IsNullOrEmpty(classname)) return null;

        System.Type t = null;
        Assembly[] assbs = System.AppDomain.CurrentDomain.GetAssemblies();
        Assembly assb = null;
        for (int i = 0; i < assbs.Length; i++)
        {
            assb = assbs[i];
            t = assb.GetType(classname);
            if (t != null) return t;
        }

        return null;
    }

    /// <summary>
    /// 寻找GAMEOBJECT
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject Find(string name)
    {
        return GameObject.Find(name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindWithTag(string tag)
    {
        return GameObject.FindWithTag(tag);
    }

    /// <summary>
    /// GetComponentInChildren
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="classname"></param>
    /// <returns></returns>
    public static Component GetComponentInChildren(GameObject obj, string classname)
    {
        System.Type t = GetType(classname);
        Component comp = null;
        if (t != null && obj != null)comp = obj.GetComponentInChildren(t);
        return comp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="classname"></param>
    /// <returns></returns>
    public static Component GetComponent(GameObject obj, string classname)
    {
        //System.Type t = getType(classname);
        Component comp = null;
        if (obj != null) comp = obj.GetComponent(classname);
        return comp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="classname"></param>
    /// <returns></returns>
    public static Component[] GetComponents(GameObject obj, string classname)
    {
        System.Type t = GetType(classname);
        Component[] comp = null;
        if (obj != null) comp = obj.GetComponents(t);
        return comp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="classname"></param>
    /// <returns></returns>
    public static Component[] GetComponentsInChildren(GameObject obj, string classname)
    {
        System.Type t = GetType(classname);
        if (t != null && obj != null) return obj.transform.GetComponentsInChildren(t);
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Transform[] GetAllChild(GameObject obj)
    {
        Transform[] child=null;
        int count=obj.transform.childCount;
        child =new Transform[count];
        for (int i = 0; i < count; i++)
        {
            child[i] = obj.transform.GetChild(i);
        }
        return child;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="eachFn"></param>
    public static void ForeachChild(GameObject parent, LuaFunction eachFn)
    {
        Transform pr=parent.transform;
        int count = pr.childCount;
        Transform child = null;
        for (int i = 0; i < count; i++)
        {
            child = pr.GetChild(i);
            eachFn.call(i, child.gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="eachFn"></param>
    public static void ForeachChild(ReferGameObjects parent, LuaFunction eachFn)
    {
        List<GameObject> lists = parent.refers;
        int count = lists.Count;
        GameObject child = null;
        for (int i = 0; i < count; i++)
        {
            child = lists[i];
            eachFn.call(i, child);
        }
    }

	/// <summary>
	/// Raycast the specified ray.
	/// </summary>
	/// <param name="ray">Ray.</param>
	public static object Raycast(Ray ray)
	{
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit))
		{
			return hit;
		}
		return null;
	}

    /// <summary>
    /// 得到类型
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static System.Type GetType(object obj)
    {
        return obj.GetType();
    }

    /// <summary>
    /// 刷新shader
    /// </summary>
    /// <param name="assetBundle"></param>
    public static void RefreshShader(AssetBundle assetBundle)
    {

#if UNITY_5
        UnityEngine.Material[] materials = assetBundle.LoadAllAssets<Material>();
#else
        UnityEngine.Object[] materials = assetBundle.LoadAll(typeof(Material));  //LoadAll<Material>();
#endif
        foreach (UnityEngine.Object m in materials)
        {
            Material mat = m as Material;
            string shaderName = mat.shader.name;
            Shader newShader = Shader.Find(shaderName);
            if (newShader != null)
            {
                mat.shader = newShader;
            }
            else
            {
                Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + m.name);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="www"></param>
    public static void RefreshShader(WWW www)
    {
        if (www.assetBundle != null) RefreshShader(www.assetBundle);
    }

    /// <summary>
    /// 得到两个点之间的夹角
    /// </summary>
    /// <param name="p1x"></param>
    /// <param name="p1y"></param>
    /// <param name="p2x"></param>
    /// <param name="p2y"></param>
    /// <returns></returns>
    public static float GetAngle(float p1x,float p1y,float p2x,float p2y)
    {
        var tmpx = p2x - p1x;
        var tmpy= p2y -p1y;
        var angle = Mathf.Atan2(tmpy, tmpx) * (180.0f / Mathf.PI);
        return angle;
    }

    /// <summary>
    /// 获取utf8字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string GetUTF8String(System.Byte[] bytes)
    {
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 获取bytes
    /// </summary>
    /// <param name="utf8Str"></param>
    /// <returns></returns>
    public static byte[] GetBytes(string utf8Str)
    {
        return System.Text.Encoding.UTF8.GetBytes(utf8Str);
    }

    /// <summary>
    /// 调用GC.Collect
    /// </summary>
    public void GCCollect()
    {
        System.GC.Collect();
    }
}


public class Vector3Helper
{
    public static Vector3 Subtracts(Vector3 v1, Vector3 v2)
    {
        return v1 - v2;
    }

    public static Vector3 Add(Vector3 v1, Vector3 v2)
    {
        return v1 + v2;
    }
}
    //Subtracts

/// <summary>
/// 渲染
/// </summary>
public class RenderSettingsHelper
{
    public static void Fog(bool fog)
    {
        RenderSettings.fog = fog;
    }
}
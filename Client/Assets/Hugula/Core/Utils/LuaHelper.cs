// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
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
public static class  LuaHelper {

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
        Object clone = GameObject.Instantiate(original);
#if UNITY_EDITOR
        if (clone is GameObject)
        {
            LuaHelper.RefreshShader(clone as GameObject);
        }
#endif
        return clone;
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
#if UNITY_EDITOR
        if (clone is GameObject)
        {
            LuaHelper.RefreshShader(clone as GameObject);
        }
#endif
		var transform=clone.transform;
		if(parent!=null)clone.transform.SetParent(parent.transform);
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
#if UNITY_EDITOR
        if (clone is GameObject)
        {
            LuaHelper.RefreshShader(clone as GameObject);
        }
#endif
        var transform = clone.transform;
        if (parent != null) clone.transform.SetParent(parent.transform);
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

        child.transform.SetParent(parent.transform);
        tranformTa.localPosition = pos;
        tranformTa.localScale = scale;
        tranformTa.localRotation = rota;
    }

	/// <summary>
	/// Sets the layer.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="layer">Layer.</param>
	public static void SetLayer(Transform transform,int layer)
	{
		transform.gameObject.layer = layer;
		int c = transform.childCount;
		for (int i=0; i<c; i++) {
			var child =transform.GetChild(i);
			SetLayer(child,layer);
		}
	}

	/// <summary>
	///  the Layersmask
	/// </summary>
	/// <returns>The mask get mask.</returns>
	/// <param name="args">Arguments.</param>
	public static int GetLayerMask(string args)
	{
		string[] a = args.Split (',');
		return LayerMask.GetMask (a);
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
		return GetComponentInChildren (obj,t);
    }

	public static Component GetComponentInChildren(GameObject obj, System.Type t)
	{
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
		System.Type t = GetType(classname);
		return GetComponent (obj, t);
    }

	public static Component GetComponent(GameObject obj, System.Type t)
	{
		Component comp = null;
		if (obj != null && t!=null) comp = obj.GetComponent(t);
		return comp;
	}

	public static Component AddComponent(GameObject obj, string className)
	{
		System.Type t = GetType(className);
		return AddComponent (obj, t);
	}

	public static Component AddComponent(GameObject obj, System.Type t)
	{
		Component comp = null;
		comp = GetComponent(obj, t);
		if(comp == null) comp = obj.AddComponent(t);

		return comp;
	}

	public static void RemoveComponent(GameObject obj, string className)
	{
		Component comp = GetComponent(obj, className);
		if(comp != null) RemoveComponent(comp);
	}

	public static void RemoveComponent(Component comp)
	{
		Destroy(comp);
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
		return GetComponents (obj, t);
    }

	public static Component[] GetComponents(GameObject obj, System.Type t)
	{
		Component[] comp = null;
		if (obj != null && t!=null) comp = obj.GetComponents(t);
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
		return GetComponentsInChildren (obj, t);
    }

	public static Component[] GetComponentsInChildren(GameObject obj, System.Type t)
	{
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

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="parent"></param>
    ///// <param name="eachFn"></param>
    //public static void ForeachChild(ReferGameObjects parent, LuaFunction eachFn)
    //{
    //    GameObject[] lists = parent.refers;
    //    int count = lists.Length;
    //    GameObject child = null;
    //    for (int i = 0; i < count; i++)
    //    {
    //        child = lists[i];
    //        eachFn.call(i, child);
    //    }
    //}

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

        //foreach (UnityEngine.Object m in materials)
        for(int i = 0;i<materials.Length;i++)
        {
            Material mat = materials[i];
            string shaderName = mat.shader.name;
			Shader newShader = Shader.Find(shaderName);
            if (newShader != null)
            {
                mat.shader = newShader;
            }
            else
            {
                Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + mat.name);
            }
        }
    }

    public static void RefreshShader(GameObject obj)
    {
        List<Renderer> meshrs = new List<Renderer>(obj.GetComponentsInChildren<Renderer>(false));
        List<Material> mats = new List<Material>();
        //meshrs.Add(obj.GetComponent<Renderer>());
        for (int i = 0; i < meshrs.Count; i++)
        {
            Material[] mat = meshrs[i].sharedMaterials;
            if (mat == null) mat = meshrs[i].materials;
            if (mat != null)
            {
                mats.AddRange(mat);
            }
        }

        for (int i = 0; i < mats.Count; i++)
        {
            Material mat = mats[i];
            if (mat != null)
            {
                string shaderName = mat.shader.name;
                Shader newShader = Shader.Find(shaderName);
                if (newShader != null)
                {
                    mat.shader = newShader;
                }
            }
        }

    }

    /// <summary>
    /// 得到两个点之间的夹角
    /// </summary>
    /// <param name="p1x"></param>
    /// <param name="p1y"></param>
    /// <param name="p2x"></param>
    /// <param name="p2y"></param>
    /// <returns></returns>
    public static float GetAngle(float p1x, float p1y, float p2x, float p2y)
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
    public static void GCCollect()
    {
        System.GC.Collect();
    }

    /// <summary>
    /// str 转换为hash code
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int StringToHash(string str)
    {
        int hash = Animator.StringToHash(str);
        return hash;
    }

    /// <summary>
    /// 播放动画片段
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="name"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static AnimationState PlayAnimation(Animation anim, string name, AnimationOrTween.Direction dir)
    {
        var state = anim[name];
        if (state)
        {
            float speed = Mathf.Abs(state.speed);
            state.speed = speed * (int)dir;
            if (dir == AnimationOrTween.Direction.Reverse && state.time == 0f) state.time = state.length;
            else if (dir == AnimationOrTween.Direction.Forward && state.time == state.length) state.time = 0f;
            //Debug.Log(string.Format(" speed {0},dir ={1},time = {2},length={3}",state.speed,dir,state.time,state.length));
            anim.Play(name);
            anim.Sample();
        }
        return state;
    }
}

namespace AnimationOrTween
{
    [SLua.CustomLuaClass]
    public enum Direction
    {
        Reverse = -1,
        Toggle = 0,
        Forward = 1,
    }
}
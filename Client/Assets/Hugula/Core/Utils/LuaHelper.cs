// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using SLua;
using System.IO;
using System.Text;
using Hugula.Cryptograph;

namespace Hugula.Utils
{
    /// <summary>
    /// lua helper类
    /// </summary>
    [SLua.CustomLuaClass]
    public static class LuaHelper
    {
        private static byte[] key = new byte[] { 0x32, 0x0f, 0x8d, 0xe9, 0x3b, 0x24, 0xa5, 0xd3, 0xf2, 0xd3, 0x64, 0x58, 0xb7, 0xae, 0x3f, 0x28 };
        private static byte[] iv = new byte[] { 0x08, 0xf3, 0xd3, 0x23, 0x8b, 0xc2, 0x90, 0x7d, 0xe9, 0x73, 0x4f, 0x80, 0x84, 0xcc, 0x25, 0x6e };

        /// <summary>
        /// Destroy object
        /// </summary>
        /// <param name="original"></param>
        public static void Destroy(Object original)
        {
            GameObject.Destroy(original);
        }

        public static void Destroy(Object original, float t)
        {
            GameObject.Destroy(original, t);
        }

        public static void DestroyImmediate(Object original)
        {
            GameObject.DestroyImmediate(original);
        }

        public static void DestroyImmediate(Object original, bool allowDestroyingAssets)
        {
            GameObject.DestroyImmediate(original, allowDestroyingAssets);
        }

        /// <summary>
        /// Instantiate Object
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static GameObject Instantiate(GameObject original)
        {
            GameObject clone = GameObject.Instantiate(original);
#if UNITY_EDITOR
            LuaHelper.RefreshShader(clone as GameObject);
#endif
            return clone;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject InstantiateLocal(GameObject original, GameObject parent)
        {
            return InstantiateLocal(original, parent, Vector3.zero);
        }

        public static GameObject InstantiateLocal(GameObject original)
        {
            return InstantiateLocal(original, null, Vector3.zero);
        }

        public static GameObject InstantiateLocal(GameObject original, Vector3 pos)
        {
            return InstantiateLocal(original, null, pos);
        }

        public static GameObject InstantiateLocal(GameObject original, GameObject parent, Vector3 pos)
        {
            var tranformTa = original.transform;
            if (pos == Vector3.zero) pos = tranformTa.localPosition;
            Quaternion rota = tranformTa.localRotation;
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
        public static GameObject InstantiateGlobal(GameObject original, GameObject parent = null)
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
        /// 创建一个新的GameObject
        /// </summary>
        public static GameObject CreateNewGameObject(string rName)
        {
            return new GameObject(rName);
        }

        /// <summary>
        /// 设置父对象
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        public static void SetParent(GameObject child, GameObject parent)
        {
            var tranformTa = child.transform;
            var pos = tranformTa.localPosition;
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
        public static void SetLayer(Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            int c = transform.childCount;
            for (int i = 0; i < c; i++)
            {
                var child = transform.GetChild(i);
                SetLayer(child, layer);
            }
        }

        /// <summary>
        ///  the Layersmask
        /// </summary>
        /// <returns>The mask get mask.</returns>
        /// <param name="args">Arguments.</param>
        public static int GetLayerMask(string args)
        {
            string[] a = args.Split(',');
            return LayerMask.GetMask(a);
        }


        /// <summary>
        /// getType
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static System.Type GetClassType(string classname)
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
            System.Type t = GetClassType(classname);
            return GetComponentInChildren(obj, t);
        }

        public static Component GetComponentInChildren(GameObject obj, System.Type t)
        {
            Component comp = null;
            if (t != null && obj != null) comp = obj.GetComponentInChildren(t);
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
            System.Type t = GetClassType(classname);
            return GetComponent(obj, t);
        }

        public static Component GetComponent(GameObject obj, System.Type t)
        {
            Component comp = null;
            if (obj != null && t != null) comp = obj.GetComponent(t);
            return comp;
        }

        public static Component AddComponent(GameObject obj, string className)
        {
            System.Type t = GetClassType(className);
            return AddComponent(obj, t);
        }

        public static Component AddComponent(GameObject obj, System.Type t)
        {
            Component comp = null;
            comp = GetComponent(obj, t);
            if (comp == null) comp = obj.AddComponent(t);

            return comp;
        }

        public static void RemoveComponent(GameObject obj, string className)
        {
            Component comp = GetComponent(obj, className);
            if (comp != null) RemoveComponent(comp);
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
            System.Type t = GetClassType(classname);
            return GetComponents(obj, t);
        }

        public static Component[] GetComponents(GameObject obj, System.Type t)
        {
            Component[] comp = null;
            if (obj != null && t != null) comp = obj.GetComponents(t);
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
            System.Type t = GetClassType(classname);
            return GetComponentsInChildren(obj, t);
        }

        public static Component[] GetComponentsInChildren(GameObject obj, System.Type t)
        {
            if (t != null && obj != null) return obj.transform.GetComponentsInChildren(t);
            return null;
        }

        public static void SetBehaviourEnabled(Behaviour behaviour, bool enabled)
        {
            if (behaviour == null) return;
            behaviour.enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Transform[] GetAllChild(GameObject obj)
        {
            Transform[] child = null;
            int count = obj.transform.childCount;
            child = new Transform[count];
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
            Transform pr = parent.transform;
            int count = pr.childCount;
            Transform child = null;
            for (int i = 0; i < count; i++)
            {
                child = pr.GetChild(i);
                eachFn.call(i, child.gameObject);
            }
        }

        /// <summary>
        /// Raycast the specified ray.
        /// </summary>
        /// <param name="ray">Ray.</param>
        public static object Raycast(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit;
            }
            return null;
        }

        //        /// <summary>
        //        /// 得到类型
        //        /// </summary>
        //        /// <param name="obj"></param>
        //        /// <returns></returns>
        //        public static System.Type GetType(object obj)
        //        {
        //            return obj.GetType();
        //        }

        /// <summary>
        /// 刷新shader
        /// </summary>
        /// <param name="assetBundle"></param>
        public static void RefreshShader(AssetBundle assetBundle)
        {
            UnityEngine.Material[] materials = assetBundle.LoadAllAssets<Material>();
            //foreach (UnityEngine.Object m in materials)
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                string shaderName = mat.shader.name;
                Shader newShader = Shader.Find(shaderName);
                if (newShader != null)
                {
                    mat.shader = newShader;  //    Debug.Log("refresh shader: " + shaderName + " in material " + mat.name);
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
            var tmpy = p2y - p1y;
            var angle = Mathf.Atan2(tmpy, tmpx) * (180.0f / Mathf.PI);
            return angle;
        }

        /// <summary>
        /// 获取utf8字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetUTF8String(System.Array bytes)
        {
            return System.Text.Encoding.UTF8.GetString((byte[])bytes);
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
        /// Creates from byte array.
        /// </summary>
        /// <returns>The from byte array.</returns>
        /// <param name="bytes">Bytes.</param>
        public static AssetBundle LoadFromMemory(System.Array bytes)
        {
            byte[] bts = (byte[])bytes;
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(bts);
#else
            AssetBundle ab = AssetBundle.LoadFromMemory(bts);
#endif
            return ab;
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
        public static AnimationState PlayAnimation(Animation anim, string name, AnimationDirection dir)
        {
            if (string.IsNullOrEmpty(name)) name = anim.clip.name;
            var state = anim[name];
            if (state)
            {
                float speed = state.speed;
                if (dir == AnimationDirection.Toggle)
                {
                    if (speed > 0 && state.time == 0)
                        dir = AnimationDirection.Reverse;
                    else
                        dir = AnimationDirection.Forward;
                }

                if (dir == AnimationDirection.Reverse && state.time == 0f)
                {
                    state.time = state.length;
                }
                else if (dir == AnimationDirection.Forward && state.time == state.length)
                {
                    state.time = 0f;
                }

                state.speed = Mathf.Abs(speed) * (int)dir;

                anim.Play(name);
                anim.Sample();
            }
            return state;
        }

        public static Vector2 GetCanvasPos(Transform tr, Vector3 screenPos)
        {
            Vector2 pos = Vector2.zero;
            Canvas canvas = tr.root.GetComponentInChildren<Canvas>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out pos))
            {

            }

            return pos;
        }

        /// <summary>
        /// 播放动画片段
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="name"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static AnimatorStateInfo PlayAnimator(Animator anim, string name, AnimationDirection dir)
        {

            var state = anim.GetCurrentAnimatorStateInfo(0);
            float speed = anim.GetFloat("speed");
            float normalizedTime = 1;
            if (dir == AnimationDirection.Toggle)
            {
                if (speed > 0)
                    dir = AnimationDirection.Reverse;
                else
                    dir = AnimationDirection.Forward;
            }

            if (dir == AnimationDirection.Reverse)
            {
                anim.SetFloat("speed", -1 * Mathf.Abs(speed));
                normalizedTime = 1;
            }
            else if (dir == AnimationDirection.Forward)
            {
                anim.SetFloat("speed", Mathf.Abs(speed));
                normalizedTime = 0;
            }

            if (!string.IsNullOrEmpty(name))
                anim.Play(name, 0, normalizedTime);
            else
                anim.Play(state.fullPathHash, 0, normalizedTime);

            return state;
        }


        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scenename"></param>
        public static void UnloadScene(string sceneName)
        {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            Application.UnloadLevel(sceneName);
#else
            UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneName);
#endif
        }

        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="isAdditive">If set to <c>true</c> is additive.</param>
        public static void LoadScene(string sceneName, bool isAdditive)
        {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (isAdditive)
				Application.LoadLevelAdditive(sceneName);
			else
				Application.LoadLevel(sceneName);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, isAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
#endif
        }


        public static void ReleaseLuaFn(LuaFunction fn)
        {
            if (fn != null) fn.Dispose();
        }

        /// <summary>
        /// check Object is Null
        /// </summary>
        /// <param name="scenename"></param>
        public static bool IsNull(Object obj)
        {
            return obj == null || !obj;
        }

        /// <summary>
        /// 本地存储
        /// </summary>
        public static bool SaveLocalData(string fileName, string saveData)
        {
            string fullPath = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), fileName);
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            if (fs != null)
            {
                byte[] bytes = CryptographHelper.Encrypt(Encoding.UTF8.GetBytes(saveData), key, iv);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
                return true;
            }
            return false;
        }

        public static string LoadLocalData(string fileName)
        {
            string fullPath = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), fileName);
            if (System.IO.File.Exists(fullPath))
            {
                FileStream fs = new FileStream(fullPath, FileMode.Open);
                if (fs != null && fs.Length > 0)
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    string loadData = string.Empty;
                    try{
                        loadData = Encoding.UTF8.GetString(CryptographHelper.Decrypt(bytes, key, iv));
                    }catch(System.Exception ex){
                        Debug.LogError(ex);
                    }
                    return loadData;
                }
            }
            return "";
        }
    }

}
//namespace AnimationOrTween
//{
[SLua.CustomLuaClass]
public enum AnimationDirection
{
    Reverse = -1,
    Toggle = 0,
    Forward = 1,
}
//}

/// <summary>
/// 用来加载场景的type类型
/// </summary>
[SLua.CustomLuaClass]
public class AssetBundleScene
{

}

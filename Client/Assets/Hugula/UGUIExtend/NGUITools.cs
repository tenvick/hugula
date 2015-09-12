//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/// <summary>
/// Helper class containing generic functions used throughout the UI library.
/// </summary>
[SLua.CustomLuaClass]
static public class NGUITools
{
	static AudioListener mListener;

	static bool mLoaded = false;
	static float mGlobalVolume = 1f;

	/// <summary>
	/// Globally accessible volume affecting all sounds played via NGUITools.PlaySound().
	/// </summary>

	static public float soundVolume
	{
		get
		{
			if (!mLoaded)
			{
				mLoaded = true;
				mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
			}
			return mGlobalVolume;
		}
		set
		{
			if (mGlobalVolume != value)
			{
				mLoaded = true;
				mGlobalVolume = value;
				PlayerPrefs.SetFloat("Sound", value);
			}
		}
	}

	/// <summary>
	/// Helper function -- whether the disk access is allowed.
	/// </summary>

	static public bool fileAccess
	{
		get
		{
			return Application.platform != RuntimePlatform.WindowsWebPlayer &&
				Application.platform != RuntimePlatform.OSXWebPlayer;
		}
	}

	/// <summary>
	/// Play the specified audio clip.
	/// </summary>

	static public AudioSource PlaySound (AudioClip clip) { return PlaySound(clip, 1f, 1f); }

	/// <summary>
	/// Play the specified audio clip with the specified volume.
	/// </summary>

	static public AudioSource PlaySound (AudioClip clip, float volume) { return PlaySound(clip, volume, 1f); }

	/// <summary>
	/// Play the specified audio clip with the specified volume and pitch.
	/// </summary>

	static public AudioSource PlaySound (AudioClip clip, float volume, float pitch)
	{
		volume *= soundVolume;

		if (clip != null && volume > 0.01f)
		{
			if (mListener == null || !NGUITools.GetActive(mListener))
			{
				AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

				if (listeners != null)
				{
					for (int i = 0; i < listeners.Length; ++i)
					{
						if (NGUITools.GetActive(listeners[i]))
						{
							mListener = listeners[i];
							break;
						}
					}
				}

				if (mListener == null)
				{
					Camera cam = Camera.main;
					if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
					if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
				}
			}

			if (mListener != null && mListener.enabled && NGUITools.GetActive(mListener.gameObject))
			{
				AudioSource source = mListener.GetComponent<AudioSource>();
				if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
				source.pitch = pitch;
				source.PlayOneShot(clip, volume);
				return source;
			}
		}
		return null;
	}

	/// <summary>
	/// New WWW call can fail if the crossdomain policy doesn't check out. Exceptions suck. It's much more elegant to check for null instead.
	/// </summary>

	static public WWW OpenURL (string url)
	{
#if UNITY_FLASH
		Debug.LogError("WWW is not yet implemented in Flash");
		return null;
#else
		WWW www = null;
		try { www = new WWW(url); }
		catch (System.Exception ex) { Debug.LogError(ex.Message); }
		return www;
#endif
	}

	/// <summary>
	/// New WWW call can fail if the crossdomain policy doesn't check out. Exceptions suck. It's much more elegant to check for null instead.
	/// </summary>

	static public WWW OpenURL (string url, WWWForm form)
	{
		if (form == null) return OpenURL(url);
#if UNITY_FLASH
		Debug.LogError("WWW is not yet implemented in Flash");
		return null;
#else
		WWW www = null;
		try { www = new WWW(url, form); }
		catch (System.Exception ex) { Debug.LogError(ex != null ? ex.Message : "<null>"); }
		return www;
#endif
	}

	/// <summary>
	/// Same as Random.Range, but the returned value is between min and max, inclusive.
	/// Unity's Random.Range is less than max instead, unless min == max.
	/// This means Range(0,1) produces 0 instead of 0 or 1. That's unacceptable.
	/// </summary>

	static public int RandomRange (int min, int max)
	{
		if (min == max) return min;
		return UnityEngine.Random.Range(min, max + 1);
	}

	/// <summary>
	/// Returns the hierarchy of the object in a human-readable format.
	/// </summary>

	static public string GetHierarchy (GameObject obj)
	{
		if (obj == null) return "";
		string path = obj.name;

		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			path = obj.name + "\\" + path;
		}
		return path;
	}

	/// <summary>
	/// Find all active objects of specified type.
	/// </summary>

	static public T[] FindActive<T> () where T : Component
	{
#if UNITY_3_5 || UNITY_4_0
		return GameObject.FindSceneObjectsOfType(typeof(T)) as T[];
#else
		return GameObject.FindObjectsOfType(typeof(T)) as T[];
#endif
	}

	/// <summary>
	/// Find the camera responsible for drawing the objects on the specified layer.
	/// </summary>

	static public Camera FindCameraForLayer (int layer)
	{
		int layerMask = 1 << layer;

		Camera cam;

		cam = Camera.main;
		if (cam && (cam.cullingMask & layerMask) != 0) return cam;

		Camera[] cameras = NGUITools.FindActive<Camera>();

		for (int i = 0, imax = cameras.Length; i < imax; ++i)
		{
			cam = cameras[i];
			if (cam && (cam.cullingMask & layerMask) != 0)
				return cam;
		}
		return null;
	}

	
	/// <summary>
	/// Helper function that returns the string name of the type.
	/// </summary>

	static public string GetTypeName<T> ()
	{
		string s = typeof(T).ToString();
		if (s.StartsWith("UI")) s = s.Substring(2);
		else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
		return s;
	}

	/// <summary>
	/// Helper function that returns the string name of the type.
	/// </summary>

	static public string GetTypeName (UnityEngine.Object obj)
	{
		if (obj == null) return "Null";
		string s = obj.GetType().ToString();
		if (s.StartsWith("UI")) s = s.Substring(2);
		else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
		return s;
	}

	/// <summary>
	/// Convenience method that works without warnings in both Unity 3 and 4.
	/// </summary>

	static public void RegisterUndo (UnityEngine.Object obj, string name)
	{
#if UNITY_EDITOR
 #if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		UnityEditor.Undo.RegisterUndo(obj, name);
 #else
		UnityEditor.Undo.RecordObject(obj, name);
 #endif
		NGUITools.SetDirty(obj);
#endif
	}

	/// <summary>
	/// Convenience function that marks the specified object as dirty in the Unity Editor.
	/// </summary>

	static public void SetDirty (UnityEngine.Object obj)
	{
#if UNITY_EDITOR
		if (obj)
		{
			//if (obj is Component) Debug.Log(NGUITools.GetHierarchy((obj as Component).gameObject), obj);
			//else if (obj is GameObject) Debug.Log(NGUITools.GetHierarchy(obj as GameObject), obj);
			//else Debug.Log("Hmm... " + obj.GetType(), obj);
			UnityEditor.EditorUtility.SetDirty(obj);
		}
#endif
	}

	/// <summary>
	/// Add a new child game object.
	/// </summary>

	static public GameObject AddChild (GameObject parent) { return AddChild(parent, true); }

	/// <summary>
	/// Add a new child game object.
	/// </summary>

	static public GameObject AddChild (GameObject parent, bool undo)
	{
		GameObject go = new GameObject();
#if UNITY_EDITOR
		if (undo) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
		if (parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}

	/// <summary>
	/// Instantiate an object and add it to the specified parent.
	/// </summary>

	static public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

#if UNITY_EDITOR && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
		UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif

		if (go != null && parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}
   

	/// <summary>
	/// Helper function that recursively sets all children with widgets' game objects layers to the specified value.
	/// </summary>

	static public void SetChildLayer (Transform t, int layer)
	{
		for (int i = 0; i < t.childCount; ++i)
		{
			Transform child = t.GetChild(i);
			child.gameObject.layer = layer;
			SetChildLayer(child, layer);
		}
	}

	/// <summary>
	/// Add a child object to the specified parent and attaches the specified script to it.
	/// </summary>

	static public T AddChild<T> (GameObject parent) where T : Component
	{
		GameObject go = AddChild(parent);
		go.name = GetTypeName<T>();
		return go.AddComponent<T>();
	}

	/// <summary>
	/// Add a child object to the specified parent and attaches the specified script to it.
	/// </summary>

	static public T AddChild<T> (GameObject parent, bool undo) where T : Component
	{
		GameObject go = AddChild(parent, undo);
		go.name = GetTypeName<T>();
		return go.AddComponent<T>();
	}

	/// <summary>
	/// Get the rootmost object of the specified game object.
	/// </summary>

	static public GameObject GetRoot (GameObject go)
	{
		Transform t = go.transform;

		for (; ; )
		{
			Transform parent = t.parent;
			if (parent == null) break;
			t = parent;
		}
		return t.gameObject;
	}

	/// <summary>
	/// Finds the specified component on the game object or one of its parents.
	/// </summary>

	static public T FindInParents<T> (GameObject go) where T : Component
	{
		if (go == null) return null;
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
		T comp = go.GetComponent<T>();
#endif
		if (comp == null)
		{
			Transform t = go.transform.parent;

			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
		}
#if UNITY_FLASH
		return (T)comp;
#else
		return comp;
#endif
	}

	/// <summary>
	/// Finds the specified component on the game object or one of its parents.
	/// </summary>

	static public T FindInParents<T> (Transform trans) where T : Component
	{
		if (trans == null) return null;
#if UNITY_FLASH
		object comp = trans.GetComponent<T>();
#else
		T comp = trans.GetComponent<T>();
#endif
		if (comp == null)
		{
			Transform t = trans.transform.parent;

			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
		}
#if UNITY_FLASH
		return (T)comp;
#else
		return comp;
#endif
	}

	/// <summary>
	/// Destroy the specified object, immediately if in edit mode.
	/// </summary>

	static public void Destroy (UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isPlaying)
			{
				if (obj is GameObject)
				{
					GameObject go = obj as GameObject;
					go.transform.parent = null;
				}

				UnityEngine.Object.Destroy(obj);
			}
			else UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	/// <summary>
	/// Destroy the specified object immediately, unless not in the editor, in which case the regular Destroy is used instead.
	/// </summary>

	static public void DestroyImmediate (UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isEditor) UnityEngine.Object.DestroyImmediate(obj);
			else UnityEngine.Object.Destroy(obj);
		}
	}

	/// <summary>
	/// Call the specified function on all objects in the scene.
	/// </summary>

	static public void Broadcast (string funcName)
	{
		GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		for (int i = 0, imax = gos.Length; i < imax; ++i) gos[i].SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Call the specified function on all objects in the scene.
	/// </summary>

	static public void Broadcast (string funcName, object param)
	{
		GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		for (int i = 0, imax = gos.Length; i < imax; ++i) gos[i].SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Determines whether the 'parent' contains a 'child' in its hierarchy.
	/// </summary>

	static public bool IsChild (Transform parent, Transform child)
	{
		if (parent == null || child == null) return false;

		while (child != null)
		{
			if (child == parent) return true;
			child = child.parent;
		}
		return false;
	}

	/// <summary>
	/// Activate the specified object and all of its children.
	/// </summary>

	static void Activate (Transform t) { Activate(t, true); }

	/// <summary>
	/// Activate the specified object and all of its children.
	/// </summary>

	static void Activate (Transform t, bool compatibilityMode)
	{
		SetActiveSelf(t.gameObject, true);

		// Prior to Unity 4, active state was not nested. It was possible to have an enabled child of a disabled object.
		// Unity 4 onwards made it so that the state is nested, and a disabled parent results in a disabled child.
#if UNITY_3_5
		for (int i = 0, imax = t.GetChildCount(); i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			Activate(child);
		}
#else
		if (compatibilityMode)
		{
			// If there is even a single enabled child, then we're using a Unity 4.0-based nested active state scheme.
			for (int i = 0, imax = t.childCount; i < imax; ++i)
			{
				Transform child = t.GetChild(i);
				if (child.gameObject.activeSelf) return;
			}

			// If this point is reached, then all the children are disabled, so we must be using a Unity 3.5-based active state scheme.
			for (int i = 0, imax = t.childCount; i < imax; ++i)
			{
				Transform child = t.GetChild(i);
				Activate(child, true);
			}
		}
#endif
	}

	/// <summary>
	/// Deactivate the specified object and all of its children.
	/// </summary>

	static void Deactivate (Transform t)
	{
#if UNITY_3_5
		for (int i = 0, imax = t.GetChildCount(); i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			Deactivate(child);
		}
#endif
		SetActiveSelf(t.gameObject, false);
	}

	/// <summary>
	/// Activate or deactivate children of the specified game object without changing the active state of the object itself.
	/// </summary>

	static public void SetActiveChildren (GameObject go, bool state)
	{
		Transform t = go.transform;

		if (state)
		{
			for (int i = 0, imax = t.childCount; i < imax; ++i)
			{
				Transform child = t.GetChild(i);
				Activate(child);
			}
		}
		else
		{
			for (int i = 0, imax = t.childCount; i < imax; ++i)
			{
				Transform child = t.GetChild(i);
				Deactivate(child);
			}
		}
	}

	/// <summary>
	/// Helper function that returns whether the specified MonoBehaviour is active.
	/// </summary>

	[System.Obsolete("Use NGUITools.GetActive instead")]
	static public bool IsActive (Behaviour mb)
	{
#if UNITY_3_5
		return mb != null && mb.enabled && mb.gameObject.active;
#else
		return mb != null && mb.enabled && mb.gameObject.activeInHierarchy;
#endif
	}

	/// <summary>
	/// Helper function that returns whether the specified MonoBehaviour is active.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public bool GetActive (Behaviour mb)
	{
#if UNITY_3_5
		return mb && mb.enabled && mb.gameObject.active;
#else
		return mb && mb.enabled && mb.gameObject.activeInHierarchy;
#endif
	}

	/// <summary>
	/// Unity4 has changed GameObject.active to GameObject.activeself.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public bool GetActive (GameObject go)
	{
#if UNITY_3_5
		return go && go.active;
#else
		return go && go.activeInHierarchy;
#endif
	}

	/// <summary>
	/// Unity4 has changed GameObject.active to GameObject.SetActive.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public void SetActiveSelf (GameObject go, bool state)
	{
#if UNITY_3_5
		go.active = state;
#else
		go.SetActive(state);
#endif
	}

	/// <summary>
	/// Recursively set the game object's layer.
	/// </summary>

	static public void SetLayer (GameObject go, int layer)
	{
		go.layer = layer;

		Transform t = go.transform;
		
		for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			SetLayer(child.gameObject, layer);
		}
	}

	/// <summary>
	/// Helper function used to make the vector use integer numbers.
	/// </summary>

	static public Vector3 Round (Vector3 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		return v;
	}


	/// <summary>
	/// Save the specified binary data into the specified file.
	/// </summary>

	static public bool Save (string fileName, byte[] bytes)
	{
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8
		return false;
#else
		if (!NGUITools.fileAccess) return false;

		string path = Application.persistentDataPath + "/" + fileName;

		if (bytes == null)
		{
			if (File.Exists(path)) File.Delete(path);
			return true;
		}

		FileStream file = null;

		try
		{
			file = File.Create(path);
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.Message);
			return false;
		}

		file.Write(bytes, 0, bytes.Length);
		file.Close();
		return true;
#endif
	}

	/// <summary>
	/// Load all binary data from the specified file.
	/// </summary>

	static public byte[] Load (string fileName)
	{
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8
		return null;
#else
		if (!NGUITools.fileAccess) return null;

		string path = Application.persistentDataPath + "/" + fileName;

		if (File.Exists(path))
		{
			return File.ReadAllBytes(path);
		}
		return null;
#endif
	}

	/// <summary>
	/// Pre-multiply shaders result in a black outline if this operation is done in the shader. It's better to do it outside.
	/// </summary>

	static public Color ApplyPMA (Color c)
	{
		if (c.a != 1f)
		{
			c.r *= c.a;
			c.g *= c.a;
			c.b *= c.a;
		}
		return c;
	}

    /// <summary>
	/// Access to the clipboard via undocumented APIs.
	/// </summary>

	static public string clipboard
	{
		get
		{
			TextEditor te = new TextEditor();
			te.Paste();
			return te.content.text;
		}
		set
		{
			TextEditor te = new TextEditor();
			te.content = new GUIContent(value);
			te.OnFocus();
			te.Copy();
		}
	}

	/// <summary>
	/// Extension for the game object that checks to see if the component already exists before adding a new one.
	/// If the component is already present it will be returned instead.
	/// </summary>

	static public T AddMissingComponent<T> (this GameObject go) where T : Component
	{
		T comp = go.GetComponent<T>();
		if (comp == null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				RegisterUndo(go, "Add " + typeof(T));
#endif
			comp = go.AddComponent<T>();
		}
		return comp;
	}

	// Temporary variable to avoid GC allocation
	static Vector3[] mSides = new Vector3[4];

	/// <summary>
	/// Get sides relative to the specified camera. The order is left, top, right, bottom.
	/// </summary>

	static public Vector3[] GetSides (this Camera cam)
	{
		return cam.GetSides(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), null);
	}

	/// <summary>
	/// Get sides relative to the specified camera. The order is left, top, right, bottom.
	/// </summary>

	static public Vector3[] GetSides (this Camera cam, float depth)
	{
		return cam.GetSides(depth, null);
	}

	/// <summary>
	/// Get sides relative to the specified camera. The order is left, top, right, bottom.
	/// </summary>

	static public Vector3[] GetSides (this Camera cam, Transform relativeTo)
	{
		return cam.GetSides(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), relativeTo);
	}

	/// <summary>
	/// Get sides relative to the specified camera. The order is left, top, right, bottom.
	/// </summary>

	static public Vector3[] GetSides (this Camera cam, float depth, Transform relativeTo)
	{
		mSides[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth));
		mSides[1] = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, depth));
		mSides[2] = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, depth));
		mSides[3] = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, depth));

		if (relativeTo != null)
		{
			for (int i = 0; i < 4; ++i)
				mSides[i] = relativeTo.InverseTransformPoint(mSides[i]);
		}
		return mSides;
	}

	/// <summary>
	/// Get the camera's world-space corners. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	static public Vector3[] GetWorldCorners (this Camera cam)
	{
		return cam.GetWorldCorners(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), null);
	}

	/// <summary>
	/// Get the camera's world-space corners. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	static public Vector3[] GetWorldCorners (this Camera cam, float depth)
	{
		return cam.GetWorldCorners(depth, null);
	}

	/// <summary>
	/// Get the camera's world-space corners. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	static public Vector3[] GetWorldCorners (this Camera cam, Transform relativeTo)
	{
		return cam.GetWorldCorners(Mathf.Lerp(cam.nearClipPlane, cam.farClipPlane, 0.5f), relativeTo);
	}

	/// <summary>
	/// Get the camera's world-space corners. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	static public Vector3[] GetWorldCorners (this Camera cam, float depth, Transform relativeTo)
	{
		mSides[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
		mSides[1] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, depth));
		mSides[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
		mSides[3] = cam.ViewportToWorldPoint(new Vector3(1f, 0f, depth));

		if (relativeTo != null)
		{
			for (int i = 0; i < 4; ++i)
				mSides[i] = relativeTo.InverseTransformPoint(mSides[i]);
		}
		return mSides;
	}

	/// <summary>
	/// Convenience function that converts Class + Function combo into Class.Function representation.
	/// </summary>

	static public string GetFuncName (object obj, string method)
	{
		if (obj == null) return "<null>";
		string type = obj.GetType().ToString();
		int period = type.LastIndexOf('.');
		if (period > 0) type = type.Substring(period + 1);
		return string.IsNullOrEmpty(method) ? type : type + "." + method;
	}
}

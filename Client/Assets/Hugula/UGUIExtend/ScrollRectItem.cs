// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[SLua.CustomLuaClass]
[RequireComponent(typeof(RectTransform))]
public class ScrollRectItem : MonoBehaviour {

	public RectTransform rectTransform;

	public GameObject[] refers;

	public Behaviour[] monos;

	public object data;

	public float fdata;

	public int idata;

	public string sdata;

	// Use this for initialization
	void Awake () {
		if (rectTransform == null)
			rectTransform = this.GetComponent<RectTransform> ();
	}


}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using Hugula;
using Hugula.Utils;
using UnityEngine;

//string to raw image
public class StringTextureConverter : MonoBehaviour, IValueConverter {
    void Awake () {
        ValueConverterRegister.instance.AddConverter (typeof (StringTextureConverter).Name, this);
    }

    void OnDestroy () {
        ValueConverterRegister.instance?.RemoveConverter (typeof (StringTextureConverter).Name);
    }

    public object Convert (object target, Type targetType) {
        string rawImageUrl = target.ToString ();
        Texture tex = ResLoader.LoadAsset<Texture> (rawImageUrl);
        return tex;
    }

    public object ConvertBack (object rawImage, Type targetType) {
        if (rawImage is Texture)
            return ((Texture) rawImage).name;
        return string.Empty;
    }
}
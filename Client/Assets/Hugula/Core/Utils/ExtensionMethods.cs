using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{

    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    //visible
    public static void SetVisible(this GameObject obj, bool isVisible)
    {
        bool useActive = false;
        var trans = obj.transform;
        if (useActive)
        {
            if(Vector3.zero == trans.localScale)
                trans.localScale = Vector3.one;
            obj.SetActive(isVisible);
        }
        else
        {
            if (!obj.activeSelf)
                obj.SetActive(true);
            if (isVisible)
            {                
                trans.localScale = Vector3.one;                
            }
            else
            {
                trans.localScale = Vector3.zero;               
            }
        }
    }

    public static bool IsVisible(this GameObject obj)
    {
        return obj.transform.localScale != Vector3.zero;
    }

}

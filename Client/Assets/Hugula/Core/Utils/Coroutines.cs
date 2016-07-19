using UnityEngine;
using System.Collections;

public class Coroutines {
	private class CoreCoroutines : MonoBehaviour {
	}
    private static CoreCoroutines _this = null;

	public static Coroutine Run(IEnumerator function)
	{
	    if (_this == null)
	    {
			GameObject target = new GameObject("COROUTINES");
			target.hideFlags = HideFlags.HideAndDontSave;
	        UnityEngine.Object.DontDestroyOnLoad(target);
	        _this = target.AddComponent<CoreCoroutines>();
	    }
	    return _this.StartCoroutine(function);
	}
}

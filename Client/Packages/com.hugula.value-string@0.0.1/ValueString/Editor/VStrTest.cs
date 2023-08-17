using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.IO;
using System;

public class VStrTest : MonoBehaviour
{
    string str1;
    string str2;
    // Use this for initialization

    // Start is called before the first frame update
    void Start()
    {
        str1 = Application.dataPath;
        str2 = "Assets";
    }

    string s;
    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    // Update is called once per frame
    void Update()
    {

        Profiler.BeginSample(" string str");
        s = string.Concat(str1, str2, "abc.txt");
        Profiler.EndSample();

        Profiler.BeginSample(" + str");
        s = str1 + str2 + "abc.txt";
        Profiler.EndSample();

        Profiler.BeginSample(" value str");
        s = ValueStrUtils.Concat(str1, str2, "abc.txt");
        Profiler.EndSample();

        Profiler.BeginSample(" value no gc str");
        s = ValueStrUtils.ConcatNoAlloc(str1, str2, "abc.txt");
        Profiler.EndSample();

        Profiler.BeginSample("Directory.Exists");
        if(System.IO.Directory.Exists(str1))
        {

        }
        Profiler.EndSample();

        Profiler.BeginSample("File.exist");
        var path = ValueStrUtils.ConcatNoAlloc(str1, "/Xlua/CHANGELOG.txt");
        if (System.IO.File.Exists(path))
        {
            //
            // var bytes = File.ReadAllBytes(path);

        }
        Profiler.EndSample();

        Profiler.BeginSample("ReplaceNoAlloc char");
        s = ValueStrUtils.ReplaceNoAlloc(str2, 's', 'b');
        Profiler.EndSample();

        Debug.Log(s);

        Profiler.BeginSample("ReplaceNoAlloc str");
        s = ValueStrUtils.ReplaceNoAlloc(str1, "xlua", "XLUA");
        Profiler.EndSample();
        Debug.Log(s);

        Profiler.BeginSample("ToLower");
        s = ValueStrUtils.ToLowerNoAlloc(s);
        Profiler.EndSample();


        Debug.Log(s);

        Profiler.BeginSample("getPathOfType 120b");
        getPathOfType(typeof(List<int>));
        Profiler.EndSample();

        Profiler.BeginSample("Encoding.UTF8.GetBytes 61b");
        var bs = System.Text.Encoding.UTF8.GetBytes(s);
        Profiler.EndSample();

        //getPathOfType(typeof(List<int>));
    }

	static List<string> pathCache = new List<string>();
	static List<string> getPathOfType(Type type)
	{
		pathCache.Clear();
		var Namespace = type.Namespace;
		var typeName = type.ToString();

		int pos = -1;
		int nsLen = 0;
		bool needNext = true;
		ReadOnlySpan<char> curSplit;

		if (Namespace != null)
		{
			nsLen = Namespace.Length;
			var path = Namespace.AsSpan();
			pos = path.IndexOf('.');
			while (needNext)
			{
				if (pos >= 0)
				{
					curSplit = path.Slice(0, pos);//
					path = path.Slice(pos + 1, path.Length - (pos + 1));
				}
				else
				{
					curSplit = path;
					needNext = false;
				}
				pos = path.IndexOf('.');
				pathCache.Add(curSplit.ToString());
			}
		}

		//string class_name = type.ToString().Substring(type.Namespace == null ? 0 : type.Namespace.Length + 1);
		ReadOnlySpan<char> class_name = typeName.AsSpan().Slice(nsLen);

		if (type.IsNested)
		{
			needNext = true;
			pos = class_name.IndexOf('+');
			while (needNext)
			{
				if (pos >= 0)
				{
					curSplit = class_name.Slice(0, pos);//
					class_name = class_name.Slice(pos + 1, class_name.Length - (pos + 1));
				}
				else
				{
					curSplit = class_name;
					needNext = false;
				}
				pos = class_name.IndexOf('.');
				pathCache.Add(curSplit.ToString());
			}
			//pathCache.AddRange(class_name.Split(new char[] { '+' }));
		}
		else
		{
			pathCache.Add(class_name.ToString());
		}
		return pathCache;
	}
}

// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using ICSharpCode.SharpZipLib.Zip;
using System.IO;
/// <summary>
/// 
/// </summary>
public class Begin : MonoBehaviour {
	
	public bool editorDebug=false;
    //public bool openUpdate = true;
    //public bool openFixedUpdate = true;
    //public bool openLateUpdate = true;
    public string enterLua = "main";

    private static Begin _instance;
    public const string VERSION_FILE_NAME = "Ver.t";

    private int persistentVersion = 0;
    private int streamingVersion = 1;

    public static Begin instance
    {
        get
        {
            return _instance;
        }
    }

    private LHighway multipleLoader;

    void Awake()
    {
        multipleLoader = LHighway.instance;
        _instance = this;
    }

	// Use this for initialization
	void Start () 
	{
        LuaBegin();
	}

	#region init
	
	void LuaBegin()
    {
        //Debug.Log("LuaBegin");
		PLua luab=this.gameObject.GetComponent<PLua>();
		if(luab==null)
		{
            PLua.isDebug = editorDebug;
            PLua.enterLua = this.enterLua;
            PLua p=gameObject.AddComponent<PLua>();
        }
        else if (luab.enabled == false)
        {
            luab.enabled = true;
        }
	
	}
	#endregion

    #region protected

    private IEnumerator CompareAndroidVersion()
    {
        ReadPersistentVersion();
        string path = Application.streamingAssetsPath + "/" + VERSION_FILE_NAME;
        WWW www = new WWW(path);
        yield return www;
        this.streamingVersion = int.Parse(www.text.Trim());
        Debug.Log(string.Format(" persistentVersion= {0},streamingVersion = {1}", this.persistentVersion, this.streamingVersion));
        if (this.persistentVersion < this.streamingVersion)// copy streaming to persistent
        {
            string fileName = Application.streamingAssetsPath + "/data.zip";//  --System.IO.Path.ChangeExtension(Application.streamingAssetsPath,".zip");
            CRequest req = new CRequest(fileName);
            req.OnComplete += delegate(CRequest r)
            {
                byte[] bytes = null;
                if (r.data is WWW)
                {
                    WWW www1 = r.data as WWW;
                    bytes = www1.bytes;
                }
                FileHelper.UnZipFile(bytes, Application.persistentDataPath);
                LuaBegin();
            };
            this.multipleLoader.LoadReq(req);
        }
        else
        {
            LuaBegin();
        }
    }

    private void ReadPersistentVersion()
    {
        string path = Application.persistentDataPath + "/" + VERSION_FILE_NAME; 
        if (File.Exists(path))//if exists version file
        {
            using(StreamReader sr=File.OpenText(path))
            {
                this.persistentVersion =int.Parse(sr.ReadToEnd());
            }
        }
    }

    #endregion
}

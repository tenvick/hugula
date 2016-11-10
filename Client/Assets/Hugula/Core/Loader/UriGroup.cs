// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Hugula.Utils;
using Hugula.Update;

namespace Hugula.Loader
{

	/// <summary>
	/// uri 组策略
	/// </summary>
	[SLua.CustomLuaClass]
	public class UriGroup
	{
		#region member
		//private
		private List<string> uris;
		private List<Action<CRequest,Array>> onWWWCompletes;
		private List<Func<CRequest,bool>> onCrcChecks;

		public int count { get { return uris.Count; } }
		#endregion

		public UriGroup()
		{
			uris = new List<string>();
			onWWWCompletes = new List<Action<CRequest, Array>> ();
			onCrcChecks = new List<Func<CRequest,bool>> ();
		}

		/// <summary>
		/// 添加uri
		/// </summary>
		/// <param name="uri"></param>
		public void Add(string uri)
		{
			uris.Add(uri);
			onWWWCompletes.Add (null);
			onCrcChecks.Add (null);
		}

		public void Add(string uri,bool needCheckCrc)
		{
			uris.Add(uri);

			onWWWCompletes.Add (null);

			if(needCheckCrc)
				onCrcChecks.Add (CrcCheck.CheckUriCrc);
			else
				onCrcChecks.Add (null);
		}

		public void Add(string uri,bool needCheckCrc,bool onWWWComp)
		{
			uris.Add(uri);

			if (onWWWComp)
				onWWWCompletes.Add (SaveWWWFileToPersistent);
			else
				onWWWCompletes.Add (null);

			if(needCheckCrc)
				onCrcChecks.Add (CrcCheck.CheckUriCrc);
			else
				onCrcChecks.Add (null);
		}

		/// <summary>
		/// 添加uri
		/// </summary>
		/// <param name="uri"></param>
		public void Add(string uri,Action<CRequest, Array> onWWWComplete,Func<CRequest,bool> onCrcCheck)
		{
			uris.Add(uri);
			onWWWCompletes.Add (onWWWComplete);
			onCrcChecks.Add (onCrcCheck);
		}

		public bool CheckUriCrc(CRequest req)
		{
			if (onCrcChecks.Count > 0 && onCrcChecks.Count > req.index) {
				var act = onCrcChecks[req.index];
				if (act != null) {
					return act (req);
				}
			}

			return true;
		}

		internal void OnWWWComplete(CRequest req,WWW www)
		{
			if (onWWWCompletes.Count > 0 && onWWWCompletes.Count > req.index) {
				
				var act = onWWWCompletes[req.index];
				if (act != null) {
					act (req,www.bytes);
				}

			}
		}

		/// <summary>
		/// 获取当前索引的uri
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetUri(int index)
		{
			string uri = "";
			if (uris.Count > index && index >= 0)
			{
				uri = uris[index];
			}
			return uri;
		}

		/// <summary>
		/// 设置req index处的uri
		/// </summary>
		/// <param name="req"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool SetNextUri(CRequest req)
		{
			int index = req.index + 1;
			if (index >= count) index = 0;
			string uri = GetUri(index);
			if (!string.IsNullOrEmpty(uri))
			{
				req.index = index;
				req.uri = uri;
				return true;
			}
			return false;
		}

		public void Clear()
		{
			uris.Clear ();
			onWWWCompletes.Clear ();
			onCrcChecks.Clear ();
		}


		public static void SaveWWWFileToPersistent(CRequest req,Array www)
		{
			string saveName = req.assetBundleName;
			FileHelper.SavePersistentFile (www, saveName);
		}
	}
}
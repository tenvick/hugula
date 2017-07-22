// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Hugula.Utils;
using System.IO;
using Hugula.Collections;

namespace Hugula.Update
{

    /// <summary>
    /// 超时功能的download
    /// </summary>
    public class WebDownload : WebClient
    {
        public object userData;

        public bool isError;

        public int Timeout { get; set; }

        public WebDownload() : this(8000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
                request.ReadWriteTimeout = Timeout;
                // Debug.LogFormat("request.Timeout={0}",request.Timeout);
            }
            return request;
        }

        #region ObjectPool 
        static ObjectPool<WebDownload> objectPool = new ObjectPool<WebDownload>(m_ActionOnGet, m_ActionOnRelease);
        private static void m_ActionOnGet(WebDownload item)
        {
            item.isError = false;
        }

        private static void m_ActionOnRelease(WebDownload item)
        {

        }

        public static WebDownload Get()
        {
            return objectPool.Get();
        }

        public static void Release(WebDownload toRelease)
        {
            objectPool.Release(toRelease);
        }
        #endregion
    }
}
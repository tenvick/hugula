// Copyright (c) 2018 hugula
// direct https://github.com/tenvick/hugula

using System;
using System.Collections.Generic;
using Hugula.Collections;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Loader {
    public static class HttpDns {

        public static int DnsCacheTimeOutSecond = 400;
        public static int DnsRequestTimeOutSecond = 6;
        private static SafeDictionary<string, string> cacheDns = new SafeDictionary<string, string> ();
        private static SafeDictionary<string, DateTime> cacheTime = new SafeDictionary<string, DateTime> ();
        public static Action<string, Action<string, string>> GetHttpDsnIp;

        public static void ClearDns () {
            cacheDns.Clear ();
            cacheTime.Clear ();
        }

        public static void AddDns (string host, string ip) {
            cacheDns[host] = ip;
        }

        public static void RequestHttpDnsIP (string host) {
            if (GetHttpDsnIp != null)
                GetHttpDsnIp (host, AddDns);
        }

        public static string GetIp (string host) {
            string ip = null;
            System.DateTime time = System.DateTime.Now;
            if (cacheTime.TryGetValue (host, out time)) {
                double dt = (System.DateTime.Now - time).TotalSeconds;
                if (dt >= DnsCacheTimeOutSecond) {// dns cache time out 
                    cacheDns.Remove (host);
                } else if (cacheDns.TryGetValue (host, out ip) && string.IsNullOrEmpty(ip) && dt >= DnsRequestTimeOutSecond) {//dns request time out
                    cacheDns[host] = host;//超时设置为原始值
                }
            }

            if (!cacheDns.TryGetValue (host, out ip)) {
                cacheDns[host] = string.Empty; //标记为已经开始加载
                Debug.LogFormat("request http dns {0}",host);
                RequestHttpDnsIP (host);
                cacheTime[host] = System.DateTime.Now;
            }

            return ip;
        }

        public static string GetUrl (string url) {
            Uri uri = new Uri (url);
            string host = uri.Host;
            string ip = GetIp (host);
            if (!string.IsNullOrEmpty (ip)) {
                url = url.Replace (host, ip);
                Debug.LogFormat ("HttpDns.GetUrl host={0},ip={1},url={2};", host, ip, url);
                return url;
            }else
            {
                Debug.LogFormat ("HttpDns.GetUrl wait ips  host={0},ip={1},url={2};", host, ip, url);
                return null;
            }
        }

    }


        public static class HttpDnsHelper
        {
            public static void Initialize()
            {
                HttpDns.GetHttpDsnIp = GetHttpDnsIP;
            }

            public static void GetHttpDnsIP (string strUrl, System.Action<string, string> onComplete) 
            {
                string url = string.Format ("http://119.29.29.29/d?dn={0}", strUrl); System.Action<CRequest> onEnd = delegate (CRequest req) {
                    string text = req.data.ToString ();
                    string[] strIps = text.Split (';');
                    string strIp = strIps[0];
                    onComplete (strUrl, strIp);
                }; 
                Debug.Log ("ResourcesLoader.HttpRequest:" + url); 
                // ResourcesLoader.HttpRequest (url, null, typeof (string), onEnd, null);
                ResourcesLoader.UnityWebRequest(url, null, typeof (string), onEnd, null);
            }
        }
}
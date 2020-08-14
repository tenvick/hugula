using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Hugula.Net
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class NetworkProxy : MonoBehaviour
    {

        public int pingTimeoutMilliSeconds = 5000;
        private bool isDone = false;
        private bool isBegin = false;
        private IPEndPoint fastIP;
        private IPEndPoint lastIP;
        private int port;
        private bool isEncode;
        private System.DateTime beginTime;
        public void Connect(string host, int port, int testPort, bool isEncode)
        {
            isDone = false;
            isBegin = true;
            this.port = port;
            this.isEncode = isEncode;
            enabled = true;
            beginTime = System.DateTime.Now;
            var ipadds = GetAddresses(host);
            Ping(ipadds, testPort);
        }

        public IPAddress[] GetAddresses(string hosts)
        {
            List<IPAddress> ipadds = new List<IPAddress>();
            string[] ipsArr = hosts.Split(',');
            foreach (string s in ipsArr)
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(s);
                    foreach (var ip in ips)
                    {
                        if (!ipadds.Contains(ip))
                            ipadds.Add(ip);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
#if !HUGULA_NO_LOG
                Debug.Log(s);
#endif

            }
#if !HUGULA_NO_LOG
            Debug.LogFormat("GetAddresses().Length={0}", ipadds.Count);
#endif

            return ipadds.ToArray();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (isBegin)
            {
                if (isDone && fastIP != null)
                {
                    string ipstr = fastIP.Address.ToString();
#if !HUGULA_NO_LOG
                    Debug.LogFormat("Connected {0}:{1}", ipstr, port);
#endif
                    isBegin = false;
                    NetWork.main.Connect(ipstr, ipstr, port, isEncode);
                    this.enabled = false;
                }
                else if (IsPingTimeOut())
                {
                    string ipstr = lastIP.Address.ToString();
#if !HUGULA_NO_LOG
                    Debug.LogFormat("ping time out Connected {0}:{1}", ipstr, port);
#endif
                    isBegin = false;
                    NetWork.main.Connect(ipstr, ipstr, port, isEncode);
                    this.enabled = false;
                }
            }
        }

        private bool IsPingTimeOut()
        {
            var dt = System.DateTime.Now - beginTime;
            return dt.TotalMilliseconds >= pingTimeoutMilliSeconds;
        }

        private void Ping(IPAddress[] ipaddes, int port)
        {
            foreach (var ipadd in ipaddes)
            {
                var async_thread = new Thread(delegate ()
                {
                    try
                    {
                        this.Connect(ipadd, port);
                    }
                    catch (Exception error)
                    {
                        Debug.LogError(error);
                    }
                });
                async_thread.Start();
            }
        }

        private void OnConnected(IPEndPoint ip, Socket socket, System.DateTime begin)
        {
            if (!isDone)
            {
                fastIP = ip;
                isDone = true;
            }
#if !HUGULA_NO_LOG
            Debug.LogFormat("ip{0} OnConnected {1}ms", ip.ToString(), (System.DateTime.Now - begin).TotalMilliseconds);
#endif
            socket.Close();
        }

        private void Connect(IPAddress ipadd, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(ipadd, port);
            lastIP = endPoint;
            Socket s = null;
            if (ipadd.AddressFamily == AddressFamily.InterNetworkV6)
                s = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            else
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var begin = System.DateTime.Now;
#if !HUGULA_NO_LOG
            Debug.LogFormat("Connect ip=>{0}:{1}", endPoint, port);
#endif
            s.Connect(endPoint);
            if (!s.Connected)
            {
                //error
            }
            byte[] RecvBytes = new byte[4];
            int bytes = s.Receive(RecvBytes, RecvBytes.Length, 0);
            if (bytes == 4)
            {
                //sucess
                OnConnected(endPoint, s, begin);
            }
            else
            {
                //error
            }
        }

        void OnDestroy()
        {
            _main = null;
        }

        private static NetworkProxy _main;

        public static NetworkProxy main
        {
            get
            {
                if (_main == null)
                {
                    var WarNetObj = new GameObject("NetWorkProxy");
                    _main = WarNetObj.AddComponent<NetworkProxy>();
                }
                return _main;
            }
        }

    }
}

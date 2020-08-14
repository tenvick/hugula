//#define NETWORK_DEBUG
// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.IO;
using System.Threading;
using System.Net;
using Hugula.Utils;

namespace Hugula.Net
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    /// <summary>
    /// 网络模块
    /// </summary>
    public class NetWork : MonoBehaviour
    {
        private TcpClient client;
        private NetworkStream stream;
        private DateTime begin;
        private Thread receiveThread;
        private Thread conThread;

        private bool isbegin = false;
        private bool isConnectioned = false;
        private float lastSeconds = 0;

        private object mutex_lock = new object();

        private ArrayList queue;

        // 发送线程
        private ArrayList sendQueue;
        private Thread sendThread;
        private AutoResetEvent sendThreadEvent = new AutoResetEvent(false);

        private Encoder encoder;
        private bool isEncode = false;
        private uint seed = 0;
        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        public float pingDelay = 20;
        public int timeoutMilliSeconds = 10000;
        public int netHandlerTimeoutMilliSeconds = 10;
        public int receiveTimeout = 10000;
        public int sendTimeout = 10;

        public float pingGateMaxTime = 10.0f;
        public float pingGateWaitMaxTime = 10.0f;
        private float pingGateCurTime = 0.0f;
        private float pingGateWaitCurTime = 0.0f;
        private bool isStartPingGate = false;
        public bool isLogin = false;

        public string dnsHost = "";
        public string Host { get; private set; }
        public string Host2 { get; private set; }
        public int Port { get; private set; }
        public XLua.LuaFunction luaNetHandler;

        private static GameObject WarNetObj;
        private static NetWork _main;

        bool isSync = false;
        ArrayList syncWaitQueue;
        public static NetWork main
        {
            get
            {
                if (_main == null)
                {
                    if (WarNetObj == null) WarNetObj = new GameObject("NetWork");
                    _main = WarNetObj.AddComponent<NetWork>();
                }
                return _main;
            }
        }

        public static NetWork New()
        {
            if (WarNetObj == null) WarNetObj = new GameObject("NetWork");
            var cnet = WarNetObj.AddComponent<NetWork>();
            if (_main == null) _main = cnet;
            return cnet;
        }

        void Awake()
        {
            queue = ArrayList.Synchronized(new ArrayList());
            sendQueue = ArrayList.Synchronized(new ArrayList());
            syncWaitQueue = ArrayList.Synchronized(new ArrayList());
        }

        void Update()
        {
            watch.Reset();
            watch.Start();

            while (queue != null && queue.Count > 0)
            {
                // 处理包之后重置ping gate的计时器  剔除事件Packet
                //if (packet.Type < 0 || packet.Type > 6)
                {
                    pingGateCurTime = Time.time;
                    pingGateWaitCurTime = Time.time;
                    isStartPingGate = false;
                }

                Packet packet = (Packet)queue[0];
                queue.RemoveAt(0);

                if (luaNetHandler == null)
                    luaNetHandler = Hugula.EnterLua.luaenv.Global.GetInPath<XLua.LuaFunction>("Net.net_handler");

                if (luaNetHandler != null)
                {
                    try
                    {
#if HUGULA_PROFILER_DEBUG
                    Profiler.BeginSample("WarNet.Type=" + packet.Type);
#endif
                        if (isSync && packet.Type != NetEventPacketType.DISCONNECT && packet.Type != NetEventPacketType.CONNECT_ERROR)
                        {
                            syncWaitQueue.Add(packet);
                            if (packet.Type == 159043482)//sync
                            {
                                isSync = false;
                                while (syncWaitQueue.Count > 0)
                                {
                                    Packet tpacket = (Packet)syncWaitQueue[0];
                                    syncWaitQueue.RemoveAt(0);

                                    // LuaPacket luaPacket = new LuaPacket(tpacket);
                                    // luaNetHandler.call(luaPacket);
                                    //   luaNetHandler.Action<int, byte[]>(msgid, data);
                                }
                            }
                        }
                        else if (packet.Type != 12)// Ping gate的包不走Lua Rpc逻辑
                        {
                            // LuaPacket luaPacket = new LuaPacket(packet);
                            // luaNetHandler.call(luaPacket);
                        }
                        else
                        {
                            // this.NetLog("Ping received..");
                        }
#if HUGULA_PROFILER_DEBUG
                    Profiler.EndSample();
#endif
                        //this.NetLogFormat("<color=#409021> netHandler end type({0}) , count ={1},time{2} </color>", packet.Type,queue.Count,Hugula.Utils.CUtils.ConvertDateTimeInt(System.DateTime.Now));
                    }
                    catch (Exception e)
                    {
                        SendErro(e.Message, e.StackTrace);
                        Debug.Log(e.ToString());
                    }
                }

                if (watch.ElapsedMilliseconds >= netHandlerTimeoutMilliSeconds && queue.Count > 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat("<color=yellow> netHandler Timeout type({0}) , wait.count ={1},timeline{2} cost={3}ms </color>", packet.Type, queue.Count, System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), watch.ElapsedMilliseconds);
#endif
                    break;
                }
            }

            if (isbegin)
            {
                // this.NetLog(" Connected "+this.client.Connected);

                bool connted = IsConnected;
                if (!connted && isConnectioned == false)
                {
                    TimeSpan ts = DateTime.Now - begin;
                    //this.NetLogFormat("TotalMilliseconds {0} timeoutMilliSeconds {1}.", ts.TotalMilliseconds, this.timeoutMilliSeconds);

                    if (ts.TotalMilliseconds > this.timeoutMilliSeconds)
                    {
                        isbegin = false;
                        NetEvent(NetEventPacketType.CONNECT_ERROR);
                        return;
                    }
                }
                else if (!connted && isConnectioned)
                {
                    isbegin = false;
                    NetEvent(NetEventPacketType.DISCONNECT);
                    return;
                }

                if (connted && isConnectioned)
                {
                    float dt = Time.time - lastSeconds;
                    if (dt > pingDelay)
                    {
                        // onIntervalFn.call(this);
                        lastSeconds = Time.time;
                    }

                    float waitDt = Time.time - pingGateWaitCurTime;
                    if (isStartPingGate && waitDt > pingGateWaitMaxTime)
                    {
#if !HUGULA_NO_LOG
                        Debug.LogFormat("ping gate timeout pingGateWaitCurTime={0} waitDt={1}", pingGateWaitCurTime, waitDt);
#endif
                        //Debug.Log(": Update ping gate timeout...");
                        this.Close();
                        isStartPingGate = false;
                    }
                }
            }
        }

        void OnDestroy()
        {
            Release();
            if (_main == this) _main = null;
        }

        private IPEndPoint[] GetConnectInfo(string host, int port)
        {
            IPAddress[] ipas = Dns.GetHostEntry(host).AddressList;
            List<IPEndPoint> eps = new List<IPEndPoint>();
            for (int i = 0; i < ipas.Length; i++)
            {
                eps.Add(new IPEndPoint(ipas[i], port));
            }
            return eps.ToArray();
        }

        void Connect(string host, int port, bool isEncode)
        {
            this.isEncode = isEncode;
            this.Host = host;
            this.Port = port;
            this.begin = DateTime.Now;
            this.isConnectioned = false;
            this.isbegin = true;
            this.isLogin = false;
            this.encoder = null;
            this.pingGateCurTime = float.MaxValue;
            this.pingGateWaitCurTime = float.MaxValue;

            this.NetLogFormat("<color=green>begin connect:{0} :{1} time:{2}</color>", host, port, begin.ToString());

            try
            {
                if (client != null)
                    client.Close();
                IPAddress[] ips;
                bool flag = false;

                ips = Dns.GetHostAddresses(host);
                if (ips.Length > 0)
                    dnsHost = ips[0].ToString();
                foreach (IPAddress ip in ips)
                {
                    // Debug.Log(ip);
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    client = new TcpClient(AddressFamily.InterNetworkV6);
                    Debug.Log("[WarNet.Connect]client.BeginConnect ipv6 async...");
                }
                else
                {
                    client = new TcpClient();
                    Debug.Log("[WarNet.Connect]client.BeginConnect async...");
                }
                client.NoDelay = true;
                client.BeginConnect(ips, port, new AsyncCallback(OnConnected), client);
            }
            catch (Exception e)
            {
                if (client != null)
                    client.Close();
                client = null;

                Debug.LogWarning(e.ToString());
                NetEvent(NetEventPacketType.CONNECT_ERROR);
            }
        }

        public void Connect(string host, string host2, int port, bool isEncode)
        {
            if (string.IsNullOrEmpty(host))
            {
                Debug.LogWarningFormat("WARNET Host is Empty! and the host2 is {0}:{1}", host2, port);
            }
            this.Host2 = host2;
            Connect(host, port, isEncode);
        }

        private void OnConnected(IAsyncResult rs)
        {
            TimeSpan ts = DateTime.Now - begin;
            Debug.LogFormat("<color=green>Connection success {0} cast {1} milliseconds</color>", Host, ts.TotalMilliseconds);

            try
            {
                client.EndConnect(rs);

                client.SendTimeout = sendTimeout;
                stream = client.GetStream();

                Host2 = null;
                receiveThread = new Thread(new ThreadStart(Receive));
                receiveThread.Start();

                sendThread = new Thread(new ThreadStart(SendWorker));
                sendThread.Start();

                conThread = new Thread(new ThreadStart(DoConnect));
                conThread.Start();

                this.NetLogFormat("[WarNet.OnConnected] EndConnected...");
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                NetEvent(NetEventPacketType.CONNECT_ERROR);
            }
        }

        private void DoConnect()
        {
            this.NetLogFormat("**Connect thread start.**");
            DateTime startTime = DateTime.Now;

            // 非加密的直接连接成功
            if (!isEncode)
            {
                isConnectioned = true;
                pingGateCurTime = float.MaxValue;
                pingGateWaitCurTime = float.MaxValue;
                NetEvent(NetEventPacketType.CONNECT);
                this.NetLog("[WarNet.DoConnect] Is not encode.. Connected..");
            }
            else
            {
                while (encoder == null)
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMilliSeconds)
                    {
                        Debug.LogWarning("[WarNet.DoConnect] Handshake Timeout!");
                        if (client != null)
                        {
                            client.Close();
                            client = null;
                        }
                        NetEvent(NetEventPacketType.CONNECT_ERROR);
                        break;
                    }
                    Thread.Sleep(100);
                }
            }

            conThread = null;
            this.NetLogFormat("**Connect thread exit.**");
        }

        public void ReConnect()
        {
            Connect(Host, Port, isEncode);
            NetEvent(NetEventPacketType.RECONNECT);
        }

        public void Close()
        {
            NetEvent(NetEventPacketType.DISCONNECT);

            if (sendThreadEvent != null)
                sendThreadEvent.Set();

            sendThread = null;

            if (sendQueue != null)
                sendQueue.Clear();

            conThread = null;

            isConnectioned = false;

            if (receiveThread != null)
                receiveThread.Abort();

            isSync = false;

            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
            stream = null;

            if (client != null)
                client.Close();

            client = null;

        }

        private void SendWorker()
        {
            NetLog("**Send thread start.**");

            while (true)
            {
                if (client == null)
                    break;

                sendThreadEvent.WaitOne();
                while (sendQueue.Count > 0)
                {
                    if (!isStartPingGate)
                    {
                        try
                        {
                            Packet packet = (Packet)sendQueue[0];
                            sendQueue.RemoveAt(0);
                            if (packet != null)
                            {
                                lock (mutex_lock)
                                {
                                    //Debug.Log("send packet type is: " + packet.Type);
                                    if (isEncode && encoder != null)
                                    {
                                        encoder.Encode(packet.GetBytes(), packet.Size + sizeof(int), 0);
                                    }
                                    stream.Write(packet.GetBytes(), 0, packet.Size + sizeof(int));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.ToString());
                            this.Close();
                            break;
                        }
                    }
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return client != null && client.Connected;
            }
        }

        private void SendPing()
        {
            //Debug.Log(": Start ping gate...");

            pingGateCurTime = Time.time;
            pingGateWaitCurTime = Time.time;
            isStartPingGate = true;

            Packet pingPacket = new Packet(12);
            pingPacket.Finish();
            try
            {
                lock (mutex_lock)
                {
                    if (isEncode && encoder != null)
                    {
                        encoder.Encode(pingPacket.GetBytes(), pingPacket.Size + sizeof(int), 0);
                    }
                    stream.Write(pingPacket.GetBytes(), 0, pingPacket.Size + sizeof(int));
                }
            }
            catch (Exception e)
            {
                NetLog(e.ToString());
                this.Close();
            }
        }

        public void SendBytes(int type,byte[] bytes)
        {
            // var pack =  packPool.Get();
            // pack.ReUse(type);
            var pack = new Packet(type);
            pack.WriteBytes(bytes);
            Send(pack);
        }


        private ObjectPool<Packet> packPool = new ObjectPool<Packet>(null,null);
       
        public void Send(Packet msg)
        {
            if (!IsConnected)
            {
                Debug.Log(": SyncConnector.send: discards because not connected.");
                //this.NetEvent(NetEventPacketType.DISCONNECT);
                return;
            }

            // 如果超时 就发ping gate包  同时将当前包加入队列
            float dt = Time.time - pingGateCurTime;
            //Debug.LogFormat("Send =================: {0}, {1}, {2}, {3}  ", dt, pingGateCurTime, isConnectioned, isLogin);
            if (dt > pingGateMaxTime && isConnectioned && isLogin)
            {
#if !HUGULA_NO_LOG
                Debug.LogFormat("send ping pingGateMaxTime={0},dt={1} ", pingGateMaxTime, dt);
#endif
                this.SendPing();
            }

            if (msg.Type == 159043482)
                isSync = true;

            msg.Finish();

            sendQueue.Add(msg);
            sendThreadEvent.Set();
        }

        public void Receive()
        {
            byte[] header = new byte[4];
            byte[] buffer = new byte[102400];

            int len = 0;
            int nread = 0;
            int size_int = sizeof(int);

            while (IsConnected)
            {
                nread = 0;
                while (nread < size_int && client.Connected)
                {
                    try
                    {
                        nread += stream.Read(header, nread, size_int - nread);
                    }
                    catch (Exception e)
                    {
#if UNITY_EDITOR
                        Debug.LogErrorFormat("[NET] stream.Read, catch:{0}", e);
#endif
                        Close();
                        break;
                    }
                }

                if (nread != size_int)
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("[NET]  stream.Read, Error nread({0}) != size_int({1}) ", nread, size_int);
#endif
                    this.Close();
                    break;
                }

                // 解密
                if (isEncode && encoder != null)
                {
                    encoder.Decode(header, size_int, 0);
                }

                Array.Reverse(header);
                len = BitConverter.ToInt32(header, 0);

                if (buffer.Length < len)
                {
                    if (len >= 256 * 1024)       // 大于256K，认为他是异常的，输出一些日志出来
                    {
                        Debug.LogFormat("WarNet.Receive > 256K, Maybe it's Exception, Len = {0}", len);
                        this.Close();
                        break;
                    }

                    //Array.Resize(ref buffer, len);
                    buffer = new byte[len];
                }

                nread = 0;
                while (nread < len && client.Connected)
                {
                    try
                    {
                        nread += stream.Read(buffer, nread, len - nread);
                    }
                    catch (Exception e)
                    {
#if UNITY_EDITOR
                        Debug.LogWarningFormat("[NET] stream.Read, Error {0} ", e);
#endif
                        this.Close();
                        break;
                    }
                }

                if (nread != len)
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat("[NET] stream.Read, Error nread({0}) != len({1}) ", nread, len);
#endif
                    this.Close();
                    break;
                }

                if (isEncode && encoder != null)
                {
                    encoder.Decode(buffer, len, 0);
                }

                Packet packet = new Packet();
                packet.Set(len, buffer, 0);

                if (packet.Type == 0)       // 加密握手包
                {
                    seed = packet.ReadUInt();
                    encoder = new Encoder(seed);

                    isConnectioned = true;
                    NetEvent(NetEventPacketType.CONNECT);

                    this.NetLog("Packet type.... Encrypt shake handle.. Connected..");
                }
                else
                {
                    //Debug.Log("Receive Packet: " + packet.Type);
                    queue.Add(packet);
                }
            }
        }

        public void NetEvent(int type)
        {
            //Debug.Log("NetEvent: " + type);
            if (type == NetEventPacketType.CONNECT_ERROR && !string.IsNullOrEmpty(Host2))
            {
                Connect(Host2, Port, isEncode);
                Host2 = null;
            }
            else if (queue != null)
            {
                Packet packet = new Packet();
                packet.SetType(type);
                queue.Add(packet);
            }
            else
            {
                Debug.LogWarning("WarNet.queue is null");
            }
        }

        private void SendCallback(IAsyncResult rs)
        {
            try
            {
                client.Client.EndSend(rs);
                //this.NetLog(string.Format("<color=green> sendcallback success {1} </color> ",rs,Hugula.Utils.CUtils.ConvertDateTimeInt(System.DateTime.Now)));
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("[NET] SendCallback, Error {0}; close im.", e);
#endif
                this.Close();
                //this.client = null;
            }
        }

        public void SendErro(string type, string desc)
        {

            // if (onAppErrorFn != null)
            // {
            //     onAppErrorFn.call(type, desc);
            // }
            // else
            // {
            //     //var error = new Msg();
            //     //error.Type = 233;
            //     //error.WriteString(type);
            //     //error.WriteString(desc);
            //     //this.Send(error);
            // }
        }

        void Release()
        {
            this.Close();
            isbegin = false;
            isSync = false;

            client = null;

            conThread = null;
            sendThread = null;
            receiveThread = null;
        }

        private void NetLog(string msg)
        {
#if NETWORK_DEBUG
        Debug.Log(msg);
#endif
        }

        private void NetLogFormat(string msg, params object[] args)
        {
#if NETWORK_DEBUG
        Debug.LogFormat(msg, args);
#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) //切换到后台
            {

            }
            else //切换回来
            {
                if ((!isConnectioned || !IsConnected) && isLogin)
                {
                    Debug.Log("OnApplicationFocus:  Disconnect..");
                    this.Close();
                }
            }
        }

        void OnApplicationQuit()
        {
           this.Close();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Application.Quit();
#endif
        }

        //void OnApplicationFocus(bool focusStatus)
        //{
        //    if (focusStatus)
        //    {
        //        if ((!isConnectioned || !IsConnected) && isLogin)
        //        {
        //            Debug.Log("OnApplicationFocus:  Disconnect..");
        //            this.Close();
        //            NetEvent(NetEventPacketType.DISCONNECT);
        //        }
        //    }
        //}
    }


    public static class NetEventPacketType
    {
        public const int UNKOWN = 50;
        public const int CONNECT = 51;
        public const int DISCONNECT = 52;
        public const int RECONNECT = 53;
        public const int CONNECT_ERROR = 54;
        public const int RECEIVE_ERROR = 55;
        public const int SEND_ERROR = 56;
        public const int ECHO = 62;
    }

}
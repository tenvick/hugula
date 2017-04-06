// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.IO;
using System.Threading;
using SLua;

namespace Hugula.Net
{

    /// <summary>
    /// ����������
    /// </summary>
    [SLua.CustomLuaClass]
    public class LNet : MonoBehaviour, IDisposable
    {
        TcpClient client;
        NetworkStream stream;
        BinaryReader breader;
        DateTime begin;
        private Thread receiveThread;
        private bool isbegin = false;
        private bool callConnectioneFun = false;
        private bool callTimeOutFun = false;
        private bool isConnectioned = false;
        private float lastSeconds = 0;
        public bool isConnectCall { private set; get; }
        public float pingDelay = 120;
        public int timeoutMiliSecond = 8000;

        void Awake()
        {
            queue = ArrayList.Synchronized(new ArrayList());
            sendQueue = ArrayList.Synchronized(new ArrayList());
        }

        void Update()
        {
            if (queue.Count > 0)
            {
                object msg = queue[0];
                queue.RemoveAt(0);

                if (onMessageReceiveFn != null)
                {
                    try
                    {
                        onMessageReceiveFn.call(msg);
                    }
                    catch (Exception e)
                    {
                        SendErro(e.Message, e.StackTrace);
                        Debug.LogError(e);
                    }
                }
            }

            if (isbegin)
            {
                //			Debug.Log(" Connected "+this.client.Connected);
                if (client.Connected == false && isConnectioned == false)
                {
                    TimeSpan ts = DateTime.Now - begin;

                    if (onConnectionTimeoutFn != null && ts.TotalMilliseconds > this.timeoutMiliSecond && !callTimeOutFun)
                    {
                        isbegin = false;
                        callConnectioneFun = false;
                        callTimeOutFun = true;
                        onConnectionTimeoutFn.call(this);
                    }
                }
                else if (client.Connected == false && isConnectioned)
                {
                    isbegin = false;
                    callConnectioneFun = false;
                    callTimeOutFun = false;
                    //if(receiveThread!=null)receiveThread.Abort();
                    if (onConnectionCloseFn != null)
                        onConnectionCloseFn.call(this);

                }

                if (client.Connected && callConnectioneFun)
                {
                    callConnectioneFun = false;
                    if (onConnectionFn != null)
                        onConnectionFn.call(this);
                }


                if (client.Connected)
                {
                    float dt = Time.time - lastSeconds;
                    if (dt > pingDelay && onIntervalFn != null)
                    {
                        onIntervalFn.call(this);
                        lastSeconds = Time.time;
                    }

                    if (this.sendQueue.Count > 0)
                    {
                        object msg = sendQueue[0];
                        sendQueue.RemoveAt(0);
                        Send((Msg)msg);
                    }
                }
            }
        }

        void OnDestroy()
        {
            Dispose();
            //if (_main == this) _main = null;
        }

        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            begin = DateTime.Now;
            callConnectioneFun = false;
            callTimeOutFun = false;
            isConnectioned = false;
            isbegin = true;
            isConnectCall = true;
            Debug.LogFormat("<color=green>begin connect:{0} :{1} time:{2}</color>", host, port, begin.ToString());
            if (client != null)
                client.Close();
            client = new TcpClient();
            client.BeginConnect(host, port, new AsyncCallback(OnConnected), client);

        }

        public void ReConnect()
        {
            Connect(Host, Port);
            if (onReConnectFn != null)
                onReConnectFn.call(this);
        }

        public void Close()
        {
            if (receiveThread != null) receiveThread.Abort();
            if (client != null) client.Close();
            if (breader != null) breader.Close();
        }

        public void Send(byte[] bytes)
        {
            if (client.Connected)
                stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(SendCallback), stream);
            //		else
            //			this.reConnect();
        }

        public bool IsConnected
        {
            get
            {
                return client == null ? false : client.Connected;
            }
        }

        public void Send(Msg msg)
        {
            if (client != null && client.Connected)
                Send(msg.ToCArray());
            else
                sendQueue.Add(msg);
        }

    public void Receive()
   {
       int len = 0;
       byte[] header = new byte[2];
       byte[] buffer = new byte[102400];
       int readLen = 0;

       while (client.Connected)
       {
           if (len == 0 && client.Available >= 2) //��ȡ��ͷ
           {
               stream.Read(header, 0, 2);
               Array.Reverse(header);
               len = BitConverter.ToUInt16(header, 0);
               readLen = 0;
#if NETWORK_DEBUG
				Debug.Log(string.Format("<color=#8abacb>begin read len {0} </color>", len));
#endif
           }

           if (len > 0 && readLen < len)
           {
               int offset = readLen;//��ʼ��
               int msgLen = client.Available;//�ɶ�����
               int size = offset + msgLen;
               if (size > len)//�����ɶ����ȴ���len
               {
                   msgLen = len - offset;
#if NETWORK_DEBUG
				    Debug.Log(string.Format("<color=#8abacb>msgLen msgLen{0} len{1} offset{2} </color>", msgLen,len,offset));
#endif
               }

               msgLen = stream.Read(buffer, offset, msgLen);
               readLen = offset + msgLen;
               if (readLen >= len)//��ȡ����
               {
                   Msg msg = new Msg(buffer);
                   queue.Add(msg);
#if NETWORK_DEBUG
						Debug.Log(string.Format("<color=#8abacb>read all byte len {0} type{1} </color>", len,packet.Type));
#endif
                   len = 0;
               }
           }

           //Thread.Sleep(16);
       }

   }
        // public void Receive()
        // {
        //     ushort len = 0;
        //     byte[] buffer = null;
        //     ushort readLen = 0;
        //     while (client.Connected)
        //     {
        //         if (len == 0 && client.Available>=2)
        //         {
        //             byte[] header = new byte[2];
        //             stream.Read(header, 0, 2);
        //             Array.Reverse(header);
        //             len = BitConverter.ToUInt16(header, 0);
        //             buffer = new byte[len];
        //             readLen = 0;
        //         }

        //         if (len > 0 && readLen < len)
        //         {
        //             int offset = readLen;//��ʼ��
        //             int msgLen = client.Available;//�ɶ�����
        //             int size = offset + msgLen;
        //             if (size > len)//�����ɶ����ȴ���len
        //             {
        //                 msgLen = len - offset;
        //             }

        //             stream.Read(buffer, offset, msgLen);
        //             readLen = Convert.ToUInt16(offset + msgLen);
        //             if (readLen >= len)//��ȡ����
        //             {
        //                 Msg msg = new Msg(buffer);
        //                 queue.Add(msg);
        //                 len = 0;
        //             }
        //         }

        //         // Thread.Sleep(16);
        //     }

        // }

        #region protected

        #region  memeber

        public string Host
        {
            get;
            private set;
        }

        public int Port
        {
            get;
            private set;
        }

        private ArrayList queue;
        private ArrayList sendQueue;
        #endregion

        private void SendCallback(IAsyncResult rs)
        {
            try
            {
                client.Client.EndSend(rs);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void OnConnected(IAsyncResult rs)
        {
            TimeSpan ts = DateTime.Now - begin;
            stream = client.GetStream();
            breader = new BinaryReader(stream);
            Debug.LogFormat("<color=green>Connection success {0} cast {1} milliseconds</color>", Host, ts.TotalMilliseconds);
            callConnectioneFun = true;
            isConnectioned = true;
            if (receiveThread != null)
                receiveThread.Abort();
            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();
        }

        #endregion

        public void SendErro(string type, string desc)
        {
            if (onAppErrorFn != null)
            {
                onAppErrorFn.call(type, desc);
            }
            else
            {
                //var error = new Msg();
                //error.Type = 233;
                //error.WriteString(type);
                //error.WriteString(desc);
                //this.Send(error);
            }
        }

        public void Dispose()
        {
            this.Close();
            isbegin = false;
            client = null;
            breader = null;
            onAppErrorFn = null;
            onConnectionCloseFn = null;
            onConnectionFn = null;
            onMessageReceiveFn = null;
            onConnectionTimeoutFn = null;
            onReConnectFn = null;
            onIntervalFn = null;
        }

        #region lua Event
        public LuaFunction onAppErrorFn;

        public LuaFunction onConnectionCloseFn;

        public LuaFunction onConnectionFn;

        public LuaFunction onMessageReceiveFn;

        public LuaFunction onConnectionTimeoutFn;

        public LuaFunction onReConnectFn;

        public LuaFunction onIntervalFn;
        #endregion

        private static GameObject lNetObj;
        private static LNet _main;

        public static LNet main
        {
            get
            {
                if (_main == null)
                {
                    if (lNetObj == null) lNetObj = new GameObject("LNet");
                    _main = lNetObj.AddComponent<LNet>();
                }
                return _main;
            }
        }

        public static LNet New()
        {
            if (lNetObj == null) lNetObj = new GameObject("LNet");
            var cnet = lNetObj.AddComponent<LNet>();
            if (_main == null) _main = cnet;
            return cnet;
        }

    }
}
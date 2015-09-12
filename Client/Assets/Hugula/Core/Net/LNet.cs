// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.IO;
using System.Threading;
using SLua;
/// <summary>
/// 网络连接类
/// </summary>
[SLua.CustomLuaClass]
public class LNet :IDisposable  {

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
	private bool bChatInst = false;
    public bool isConnectCall { private set; get; }
    public float pingDelay = 120;
    public int timeoutMiliSecond = 8000;

    private LNet()
	{
        queue = ArrayList.Synchronized(new ArrayList());
        sendQueue = ArrayList.Synchronized(new ArrayList());
	}
	
	public void Connect(string host,int port)
	{
        this.Host = host;
        this.Port = port;
        begin = DateTime.Now;
        callConnectioneFun = false;
        callTimeOutFun = false;
        isConnectioned = false;
        isbegin = true;
        isConnectCall = true;
        //Debug.Log("begin connect:" + host + " :" + port + " time:" + begin.ToString());
        if (client != null)
            client.Close();
        client = new TcpClient();
        client.BeginConnect(host, port, new AsyncCallback(OnConnected), client);

	}
	
	public void ReConnect()
	{
        Connect(Host, Port);
        if (onReConnectFn != null)
            onReConnectFn.call(new object[] { this });
	}
	
	public void Close()
	{
        if(receiveThread!=null)receiveThread.Abort();
        if(client!=null)client.Close();
        if(breader!=null)breader.Close();
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
		get{
            return client==null?false:client.Connected;
		}
	}
	
	public void Send(Msg msg)
	{
        if (client != null && client.Connected)
            Send(msg.ToCArray());
        else
            sendQueue.Add(msg);
	}
	
	public void Update()
	{
        if (queue.Count > 0)
        {
            object msg = queue[0];
            queue.RemoveAt(0);

            if (onMessageReceiveFn != null)
            {
                try
                {
                    onMessageReceiveFn.call(new object[] { msg });
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
                    onConnectionTimeoutFn.call(new object[] { this });
                }
            }
            else if (client.Connected == false && isConnectioned)
            {
                isbegin = false;
                callConnectioneFun = false;
                callTimeOutFun = false;
                //if(receiveThread!=null)receiveThread.Abort();
                if (onConnectionCloseFn != null)
                    onConnectionCloseFn.call(new object[] { this });

            }

            if (client.Connected && callConnectioneFun)
            {
                callConnectioneFun = false;
                if (onConnectionFn != null)
                    onConnectionFn.call(new object[] { this });
            }


            if (client.Connected)
            {
                float dt = Time.time - lastSeconds;
                if (dt > pingDelay && onIntervalFn != null)
                {
                    onIntervalFn.call(new object[] { this });
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
	
	public void OnApplicationPause(bool pauseStatus) {
        if (onAppPauseFn != null && isConnectCall)
            onAppPauseFn.call(new object[] { pauseStatus });
	}
	
	
	public void Receive()
	{
        ushort len = 0;
        byte[] buffer = null;
        ushort readLen = 0;
        while (client.Connected)
        {
            if (len == 0)
            {
                byte[] header = new byte[2];
                stream.Read(header, 0, 2);
                Array.Reverse(header);
                len = BitConverter.ToUInt16(header, 0);
                buffer = new byte[len];
                readLen = 0;
                //if (len > client.ReceiveBufferSize)//如果长度大于了缓冲区
                //{
                //    buffer = new byte[len];
                //}
                //else
                //{
                //    buffer = null;
                //}
            }

            if (len > 0 && readLen < len)
            {
                int offset = readLen;//开始点
                int msgLen = client.Available;//可读长度
                int size = offset + msgLen;
                if (size > len)//如果可读长度大于len
                {
                    msgLen = len - offset;
                }

                stream.Read(buffer, offset, msgLen);
                readLen = Convert.ToUInt16(offset + msgLen);
                if (readLen >= len)//读取完毕
                {
                    Msg msg = new Msg(buffer);
                    queue.Add(msg);
                    len = 0;
                }
            }
            //if (len > 0 && buffer==null && len <= client.Available) //如果没有分页
            //{
            //    byte[] message = new byte[len];
            //    stream.Read(message, 0, message.Length);
            //    Msg msg = new Msg(message);
            //    queue.Add(msg);
            //    len = 0;
            //}
            //else if (len > 0 && buffer != null)
            //{

            //}

            Thread.Sleep(16);
        }
		
	}
	
	#region protected 
	
	#region  memeber
	
	public string Host
	{
		get;
		private set;
	}
	
	public int Port{
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
        Debug.Log("Connection success" + Host + " cast" + ts.TotalMilliseconds);
        callConnectioneFun = true;
        isConnectioned = true;
        if (receiveThread != null)
            receiveThread.Abort();
        receiveThread = new Thread(new ThreadStart(Receive));
        receiveThread.Start();
	}
	
	#endregion
	
	public void SendErro(string type,string desc)
	{
		if(onAppErrorFn!=null)
		{
			onAppErrorFn.call(new object[]{type,desc});
		}else
		{
			var error=new Msg();
			error.Type=233;
            error.WriteString(type);
            error.WriteString(desc);
			this.Send(error);
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
        onAppPauseFn = null;
        onIntervalFn = null;
		if ( bChatInst ) 
		{
			_instChat = null;
		}
		else _inst = null;
    }
	
	#region lua Event
	public LuaFunction onAppErrorFn;
	
	public LuaFunction onConnectionCloseFn;

	public LuaFunction onConnectionFn;
	
	public LuaFunction onMessageReceiveFn;
	
	public LuaFunction onConnectionTimeoutFn;
	
	public LuaFunction onReConnectFn;
	
	public LuaFunction onAppPauseFn;
	
	public LuaFunction onIntervalFn;
	#endregion
	
	private static LNet _inst;
	private static LNet _instChat;

	public static LNet ChatInstance
	{
		get
		{
			if (_instChat == null)
				_instChat = new LNet();

			_instChat.bChatInst = true;
			return _instChat;
		}
	}

	public static LNet instance
	{
		get
		{
			if (_inst == null)
				_inst = new LNet();

			_inst.bChatInst = false;
			return _inst;
		}
	}
    
}

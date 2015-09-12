// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using SLua;
[SLua.CustomLuaClass]
public class TcpServer : MonoBehaviour
{

    #region pulic member
    /// <summary>
    /// server端口
    /// </summary>
    public static int port = 12000;

    public const String GAME_TYPE = "Hugula";

    /// <summary>
    /// 自动广播
    /// </summary>
    public bool autoBroadcast = false;

    /// <summary>
    /// 有客户端连接
    /// </summary>
    public LuaFunction onClientConnectFn;

    /// <summary>
    /// 客户端消息
    /// </summary>
    public LuaFunction onMessageArriveFn;

    /// <summary>
    /// 客户端关闭
    /// </summary>
    public LuaFunction onClientCloseFn;
    #endregion

    #region private member
    ///// <summary>
    ///// IP 地址
    ///// </summary>
    //private IPAddress localAddr = IPAddress.Parse("127.0.0.1");

    private TcpListener server;

    private Hashtable sessions;// = new List<TcpClient>();
    private ArrayList broadMsg;//广播的消息
    private List<int> closeClients;
    private List<Session> newClients;//新添加进来的
    public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
    #endregion

    #region mono

    void OnDisable()
    {
        Stop();
    }

    //void OnEnable()
    //{


    //}

    /// <summary>
    /// 开启服务
    /// </summary>
    void Start()
    {
        newClients = new List<Session>();
        closeClients = new List<int>();
        sessions = Hashtable.Synchronized(new Hashtable());//线程安全的  //ArrayList.Synchronized(new ArrayList());
        broadMsg = ArrayList.Synchronized(new ArrayList());

        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        RegisterHost();
        server.BeginAcceptTcpClient(DoAcceptTcpClientCallback, server); //开始监听
    }


    // Update is called once per frame
    void Update()
    {

        foreach (var ses in newClients)
        {
            if (onClientConnectFn != null)
            {
                onClientConnectFn.call(ses);
            }
        }

        newClients.Clear();
        closeClients.Clear();

        var list = sessions.Values;
        foreach (var client in list)
        {
            Session ses = ((Session)client);
            if (ses.Client.Connected)
            {
                ses.Receive();
                byte[] msg = ses.GetMessage();
                if (msg != null)
                {
                    if (autoBroadcast)
                    {
                        broadMsg.Add(msg);
                    }

                    if (onMessageArriveFn != null)
                    {
                        try
                        {
                             byte[] send = new byte[msg.Length -2];
                             System.Array.Copy(msg, 2, send, 0, send.Length);
                             onMessageArriveFn.call(ses, new Msg(send));
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning(ex);
                        }
                    }
                }
            }
            else
            {
                closeClients.Add(ses.id);
            }
        }

        foreach (int id in closeClients)
        {
            Kick(id);
        }

        closeClients.Clear();

        if (autoBroadcast)
        {
            BroadCast();
        }

        broadMsg.Clear();
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    public void BroadCast()
    {
        foreach (var client in sessions.Values)
        {
            Session ses = ((Session)client);
            foreach (var msg in broadMsg)
            {
                //UnityEngine.Debug.Log("send to session ");
                ses.Send((byte[])msg);
            }
        }

        //broadMsg.Clear();
    }

    public void BroadCast(byte[] msg)
    {
        foreach (var client in sessions.Values)
        {
            Session ses = ((Session)client);
            ses.Send(msg);
        }
    }

    public void BroadCast(Msg msg)
    {
        foreach (var client in sessions.Values)
        {
            Session ses = ((Session)client);
            ses.Send(msg);
        }
    }

    public void Kick(int SensionId)
    {
        if (sessions.ContainsKey(SensionId))
        {
            Session delses = (Session)sessions[SensionId];
            if (onClientCloseFn != null)
                onClientCloseFn.call(delses);

            delses.Close();
            sessions.Remove(SensionId);
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public void Stop()
    {
        closeClients.Clear();
        foreach (Session se in sessions.Values)
            closeClients.Add(se.id);

        foreach (int i in closeClients)
            Kick(i);

        broadMsg.Clear();
        sessions.Clear();
        server.Stop();
        Network.Disconnect();
        MasterServer.UnregisterHost();
        Debug.Log("server.stop");
    }
    #endregion

    #region pulic method

    public static TcpServer currTcpServer;

    private static GameObject svrObj;
    /// <summary>
    /// 开始服务
    /// </summary>
    public static void StartTcpServer()
    {
        if (svrObj == null)
            svrObj = new GameObject("TcpServer");

        TcpServer comp = svrObj.GetComponent<TcpServer>();
        if (comp == null)
        {
            currTcpServer = svrObj.AddComponent<TcpServer>();
        }
        else
        {
            currTcpServer = comp;
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public static void StopTcpServer()
    {
        if (currTcpServer != null)
        {
            LuaHelper.Destroy(currTcpServer);
        }
        currTcpServer = null;
    }

    /// <summary>
    /// 获取本机局域网IP
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLocalIP()
    {
        // Get server related information.
        IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
        //本机IP
        IPAddress localIP = null;
        // Loop on the AddressList
        foreach (IPAddress curAdd in heserver.AddressList)
        {
            localIP = curAdd;
            //Debug.Log("local IP :" + localIP.ToString());
        }

        return localIP;
    }

    #endregion

    #region private method
    /// <summary>
    /// 注册地址
    /// </summary>
    private void RegisterHost()
    {
        //本机IP
        IPAddress localIP = GetLocalIP();
        Debug.Log(localIP.ToString() + " is Start  ManagedThreadId " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        if (localIP != null)
        {
            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(32, port + 2, useNat);
            MasterServer.RegisterHost(GAME_TYPE, SystemInfo.deviceName, localIP.ToString());
        }
    }

    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        TcpClient client = listener.EndAcceptTcpClient(ar);
        //add to server list
        int hashCode = client.GetHashCode();
        if (!sessions.ContainsKey(hashCode))
        {
            var ses = new Session(client);
            sessions[hashCode] = ses;//.Add(ses);
            newClients.Add(ses);
            Debug.Log("new client" + client.GetHashCode() + " ManagedThreadId " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
        server.BeginAcceptTcpClient(DoAcceptTcpClientCallback, server); //开始监听
    }
    #endregion
}

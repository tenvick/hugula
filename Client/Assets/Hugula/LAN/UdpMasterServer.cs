// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
[SLua.CustomLuaClass]
public class UdpMasterServer : MonoBehaviour
{

    #region pulic member

    public static int UdpPort = 22200;
    

    #endregion

    #region private member
    private UdpClient client;
    #endregion

    #region mono
    void Start()
    {
        client = new UdpClient(UdpPort);
        //client.Send(
    }


    #endregion

 
    #region pulic method

    #endregion

    #region private method

    #endregion
}

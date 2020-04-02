using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LiteNetLibManager;
using LiteNetLib.Utils;

public class TestNetworkClient : MonoBehaviour
{
    private int port = 9132;
    public void Start()
    {
        LitNetClient.Init(port);
        LitNetClient.ControllerManager.Add<LoginController>();
        LitNetClient.NetManager.discoveryPeersHandler.OnServerDiscover += OnServerDiscover;
       // LitNetClient.Start("127.0.0.1");
    }

    RemoteTagetInfo remoteInfo;
    private void OnServerDiscover(RemoteTagetInfo info)
    {
        remoteInfo = info;
    }

    private void Update()
    {
        LitNetClient.Update(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.A) )
        {
           // Debug.Log(remoteInfo.endPoint.Address.ToString());
            LitNetClient.Start(remoteInfo.GetIPAddress().ToString());
        }
        if (Input.GetKeyDown(KeyCode.S) )
        {
            LoginController controller = LitNetClient.ControllerManager.Get<LoginController>();
            controller.LoginByAccount("123456", "123456");
        }
        if (Input.GetKeyDown(KeyCode.D) )
        {
            LoginController controller = LitNetClient.ControllerManager.Get<LoginController>();
            controller.Logout();
        }
    }

    private void OnGUI()
    {
       
          GUILayout.Label("Remote:" + remoteInfo);
        GUILayout.Label("Connect:" + LitNetClient.NetManager.IsConnected);
        //if (Input.GetKeyDown( KeyCode.A)|| GUILayout.Button("连接A"))
        //{
        //    Debug.Log(remoteInfo.endPoint.Address.ToString());
        //    LitNetClient.Start(remoteInfo.endPoint.Address.ToString()); 
        //}
        //if (Input.GetKeyDown(KeyCode.S) || GUILayout.Button("登录S"))
        //{
        //    LoginController controller = LitNetClient.NetControllerManager.GetController<LoginController>();
        //    controller.LoginByAccount("123456","123456");
        //}
        //if (Input.GetKeyDown(KeyCode.D) || GUILayout.Button("登出D"))
        //{
        //    LoginController controller = LitNetClient.NetControllerManager.GetController<LoginController>();
        //    controller.Logout();
        //}
    }
    
}
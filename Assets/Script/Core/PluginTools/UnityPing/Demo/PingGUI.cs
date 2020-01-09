//using System;
//using System.Collections.Generic;
//using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
//using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
//using System.Text;
using UnityEngine;
//using Ping = System.Net.NetworkInformation.Ping;

public class PingGUI : MonoBehaviour
{
    public List<string> hostLists =new List<string>{
        "www.google.com",
    "baidu.cn",
    "127.0.0.1"
    };
    public int pingTime = 4;
    private List<string> resultString= new List<string>();
    //private Ping sender;
    void Start()
    {
        //{
        //    IPAddress ipv4 = IPAddress.Parse("127.0.0.1");
        //    Debug.Log(ipv4.ToString() + " " + ipv4.AddressFamily);
        //}
        //{
        //    IPAddress ipv4 = IPAddress.Parse("47.252.29.132");
        //    Debug.Log(ipv4.ToString() + " " + ipv4.AddressFamily);
        //}
        //{
        //    IPAddress ipv4 = IPAddress.Parse("fe80:0000:0000:0000:1543:7ae1:297a:89c0");
        //    Debug.Log(ipv4.ToString() + " " + ipv4.AddressFamily);
        //}
        //在开始就调用在手机上可能会卡一会儿
        foreach (var item in hostLists)
        {
            UnityPingManager.Ping(item, pingTimes: pingTime, resultCallBack: ResultCallBack);
        }
        //UnityPingManager.Ping("baidu.cn",resultCallBack: ResultCallBack);
        //UnityPingManager.Ping("service-us-01.sghdservice.com", resultCallBack: ResultCallBack);
        //UnityPingManager.Ping("ellrland.sghdservice.com", resultCallBack: ResultCallBack);
        //UnityPingManager.Ping("www.google.com", resultCallBack: ResultCallBack);
        //UnityPingManager.Ping("127.0.0.1", resultCallBack: ResultCallBack);
        //UnityPingManager.Ping(IPOrURL, resultCallBack: ResultCallBack);
    }

    private void ResultCallBack(string res, UnityPingManager.PingStatistics arg2)
    {
        resultString.Add(res);
    }

    Vector2 pos;


    private string customURL = "";
    
    private void OnGUI()
    {
        GUIStyle style = "box";
        style.fontSize = 35;
        GUI.skin.textField.fontSize = 32;
        GUI.skin.label.fontSize = 35;
        GUI.skin.button.fontSize = 30;
        GUILayout.Label("URL：");
        customURL =  GUILayout.TextField(customURL,GUILayout.Width(Screen.width),GUILayout.Height(60));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Ping", GUILayout.Height(60)))
        {
            UnityPingManager.Ping(customURL, resultCallBack: ResultCallBack);
        }
        if (GUILayout.Button("Ping List All", GUILayout.Height(60)))
        {
            foreach (var item in hostLists)
            {
                UnityPingManager.Ping(item,pingTimes:pingTime, resultCallBack: ResultCallBack);
            }
        }
        if (GUILayout.Button("PingGetOptimalItem", GUILayout.Height(60)))
        {
          
                UnityPingManager.PingGetOptimalItem(hostLists.ToArray(),(res)=>
                {
                    resultString.Add(res.ToString());

                }, pingTimes: pingTime);
            
        }
        if (GUILayout.Button("Clear", GUILayout.Height(60)))
        {
            resultString.Clear();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Ping Times：");
        pingTime =int.Parse( GUILayout.TextField(pingTime.ToString(), GUILayout.Width(Screen.width), GUILayout.Height(60)));

        pos = GUILayout.BeginScrollView(pos);
        foreach (var item in resultString)
        {
            GUILayout.Box(item,style);
        }
        GUILayout.EndScrollView();
    }
}


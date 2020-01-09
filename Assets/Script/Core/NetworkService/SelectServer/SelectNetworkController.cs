using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SelectNetworkController
{
    private static string url = null;

    public static string URL
    {
        get
        {
            if(string.IsNullOrEmpty(url))
            {
                GetURI();
            }

            return url;
        }

        set
        {
            url = value;
        }
    }

    static void GetURI()
    {
        URL = URLManager.GetURL(SDKInterfaceDefine.PropertiesKey_SelectServerURL);

        if(!string.IsNullOrEmpty(SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_SelectServerURL,null)))
        {
            URL = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_SelectServerURL, null);
        }
    }
    /// <summary>
    /// 获取所有配置的服务器
    /// </summary>
    /// <param name="callBack"></param>
    public static void GetAllSupportServers( CallBack<string> callBack)
    {
        if (string.IsNullOrEmpty(URL))
        {
            Debug.LogError("URI is null!");
            if (callBack != null)
            {
                callBack(null);
            }
            return;
        }
        MonoBehaviourRuntime.Instance.StartCoroutine(LoopRequest(URL + GetServerDefine.ListenContext_getAllServers, null, callBack));
       
    }
    /// <summary>
    /// 获取可用服务器
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="group"></param>
    /// <param name="callBack"></param>
    public static void GetSupportServer(RuntimePlatform platform,string group, CallBack<string> callBack)
    {
        if (string.IsNullOrEmpty(URL))
        {
            Debug.LogError("URI is null!");
            if (callBack != null)
            {
                callBack(null);
            }
            return;
        }
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add(GetServerDefine.ClientVersion, Application.version);
        pairs.Add(GetServerDefine.ClientPlatform, platform.ToString());
        pairs.Add(GetServerDefine.Group, group);
        //Debug.Log("-1 Send Http Post");
        MonoBehaviourRuntime.Instance.StartCoroutine(LoopRequest(URL + GetServerDefine.ListenContext_getServer,pairs,callBack));
      
    }
    static bool isRequesting = false;
  public static bool IsRequesting
    {
        set
        {
            if (value == false)
                isRequesting = value;
        }
        get
        {
            return isRequesting;
        }
    }


    static IEnumerator LoopRequest(string uri, Dictionary<string, string> pairs, CallBack<string> callBack)
    {
        isRequesting = true;

        bool isSendEnd = true;
        while (isRequesting)
        {
            if (isSendEnd)
            {
                isSendEnd = false;
                //Debug.Log("0 Send Http Post");
                UnityWebRequestTool.Post(uri, pairs, (error, data) =>
                {
                    Debug.Log("返回 http post");
                    isSendEnd = true;
                    string netData = null;
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError(uri+" GetSupportServers :" + error);

                    }
                    else
                    {
                        isRequesting = false;

                        if (data.ContainsKey(GetServerDefine.SelectNetworkData))
                        {
                            netData = data[GetServerDefine.SelectNetworkData];

                        }
                        else
                        {
                            netData = "";
                        }

                    }

                    if (callBack != null)
                    {
                        callBack(netData);
                    }
                });
            }

            yield return new WaitForEndOfFrame();
        }
    }
}

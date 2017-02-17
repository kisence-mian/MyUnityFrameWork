using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager 
{
    static LoginCallBack s_loginCallBack;
    static PayCallBack s_payCallBack;

    static LoginInterface s_loginService;
    static PayInterface   s_payService;
    static LoginInterface s_logService;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {

    }

    /// <summary>
    /// 登陆
    /// </summary>
    public static void Login()
    {

    }

    /// <summary>
    /// 支付
    /// </summary>
    public static void Pay()
    {

    }

    /// <summary>
    /// 数据上报
    /// </summary>
    /// <param name="data"></param>
    public static void Log(Dictionary<string,object> data)
    {

    }
}

public delegate void LoginCallBack(string ID, Dictionary<string,object> data);
public delegate void PayCallBack(string ID, Dictionary<string, object> data);
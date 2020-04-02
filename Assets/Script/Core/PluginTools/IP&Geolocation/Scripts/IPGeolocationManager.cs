using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IPGeolocationDetail
{
    /// <summary>
    /// 从什么地址获取的
    /// </summary>
    public string formURI { get; set; }
    /// <summary>
    /// 获取时消耗时间
    /// </summary>
    public float useTime { get; set; }
    public string ipv4 { get; set; }
    public string ipv6 { get; set; }

    public string country { get; set; }
    /// <summary>
    /// ISO 3166-1 alpha-2 
    /// </summary>
    public string country_code { get; set; }

    public string city { get; set; }

    public void SetIP(string ip)
    {
        if (string.IsNullOrEmpty(ip))
            return;

        if (ip.Contains("."))
            ipv4 = ip;
        else if (ip.Contains(":"))
            ipv6 = ip;

    }
}

public class IPGeolocationManager : MonoBehaviour {

    private  Action<IPGeolocationDetail> OnIPGeolocationCallBack;
    public static void GetIPGeolocation(Action<IPGeolocationDetail> OnIPGeolocationCallBack)
    {

        GameObject obj = new GameObject("[IPGeolocationManager]");
        GameObject.DontDestroyOnLoad(obj);
        IPGeolocationManager manager = obj.AddComponent<IPGeolocationManager>();

        manager.OnIPGeolocationCallBack = OnIPGeolocationCallBack;
        manager.Run();
    }

    private void Run()
    {
        isCallResult = false;
        errorCallBackCount = 0;
        maxRequest = 0;

       
        RunHttpQuest("https://ip.seeip.org/geoip", (detail, result) =>
        {
            //{"offset":"8","city":"Chengdu","region":"Sichuan","dma_code":"0","organization":"AS4134 No.31,Jin-rong Street","area_code":"0","timezone":"Asia\/Chongqing","longitude":104.0667,"country_code3":"CHN","ip":"182.138.139.116","continent_code":"AS","country":"China","region_code":"32","country_code":"CN","latitude":30.6667}
            object temp = FrameWork.Json.Deserialize(result);
            // Debug.Log("Type:" + temp.GetType());
            IDictionary<string, object> dic = temp as IDictionary<string, object>;
            detail.SetIP( DicGetString(dic, "ip"));
            detail.city = DicGetString(dic, "city");
            detail.country = DicGetString(dic, "country");
            detail.country_code = DicGetString(dic, "country_code");

        });
        RunHttpQuest("http://ip-api.com/json", (detail, result) =>
        {
            //{"status":"success","country":"China","countryCode":"CN","region":"SC","regionName":"Sichuan","city":"Chengdu","zip":"","lat":30.6667,"lon":104.0667,"timezone":"Asia/Shanghai","isp":"Chinanet","org":"Chinanet SC","as":"AS4134 No.31,Jin-rong Street","query":"182.138.139.116"}
            object temp = FrameWork.Json.Deserialize(result);
            //  Debug.Log("Type:" + temp.GetType());
            IDictionary<string, object> dic = temp as IDictionary<string, object>;
            if ("success".Equals(DicGetString(dic, "status")))
            {
                detail.SetIP(DicGetString(dic, "query"));
                detail.city = DicGetString(dic, "city");
                detail.country = DicGetString(dic, "country");
                detail.country_code = DicGetString(dic, "countryCode");
            }
            else
            {
                throw new Exception("http://ip-api.com/json GetHttpResult status not success:" + result);
            }

        });

        RunHttpQuest("https://ip.nf/me.json", (detail,result) =>
         {
             //{"ip":{"ip":"182.138.139.116","asn":"AS4134 No.31,Jin-rong Street","netmask":14,"hostname":"","city":"Chengdu","post_code":"","country":"China","country_code":"CN","latitude":30.66670036315918,"longitude":104.06670379638672}}
             object temp = FrameWork.Json.Deserialize(result);
             // Debug.Log("Type:" + temp.GetType());
             IDictionary<string, object> dic0 = temp as IDictionary<string, object>;

             IDictionary<string, object> dic = dic0["ip"] as IDictionary<string, object>;
             detail.SetIP(DicGetString(dic, "ip"));
             detail.city = DicGetString(dic, "city"); 
             detail.country = DicGetString(dic, "country");
             detail.country_code = DicGetString(dic, "country_code"); 

         });

        RunHttpQuest("https://api.ip.sb/geoip", (detail, result) =>
        {
            // { "longitude":104.0667,"city":"Chengdu","timezone":"Asia\/Shanghai","offset":28800,"region":"Sichuan","asn":4134,"organization":"No.31,Jin-rong Street","country":"China","ip":"182.138.139.116","latitude":30.6667,"continent_code":"AS","country_code":"CN","region_code":"SC"}
            object temp = FrameWork.Json.Deserialize(result);
            // Debug.Log("Type:" + temp.GetType());
            IDictionary<string, object> dic = temp as IDictionary<string, object>;
            detail.SetIP(DicGetString(dic, "ip"));
            detail.city = DicGetString(dic, "city");
            detail.country = DicGetString(dic, "country");
            detail.country_code = DicGetString(dic, "country_code");

        });
        RunHttpQuest("https://api.db-ip.com/v2/free/self", (detail, result) =>
        {
            // { "longitude":104.0667,"city":"Chengdu","timezone":"Asia\/Shanghai","offset":28800,"region":"Sichuan","asn":4134,"organization":"No.31,Jin-rong Street","country":"China","ip":"182.138.139.116","latitude":30.6667,"continent_code":"AS","country_code":"CN","region_code":"SC"}
            object temp = FrameWork.Json.Deserialize(result);
            // Debug.Log("Type:" + temp.GetType());
            IDictionary<string, object> dic = temp as IDictionary<string, object>;
            detail.SetIP(DicGetString(dic, "ipAddress"));
            detail.city = DicGetString(dic, "city");
            detail.country = DicGetString(dic, "countryName");
            detail.country_code = DicGetString(dic, "countryCode");

        });
    }

    private string DicGetString(IDictionary<string, object> dic,string key)
    {
        object value = null;
        if (dic.ContainsKey(key))
            value = dic[key];
        if (value != null)
            return value.ToString();
        return null;
    }

    private delegate void ParseJsonReultCallBack(IPGeolocationDetail detail, string result);

    private void RunHttpQuest(string uri, ParseJsonReultCallBack callBack)
    {
        maxRequest++;
        StartCoroutine(GetHttpResult(uri, (res) =>
        {
            if (!string.IsNullOrEmpty(res.error))
            {
                Debug.LogError(res.uri + " GetHttpResult error:" + res.error);
                CallResult(null);
                return;
            }
            IPGeolocationDetail detail = new IPGeolocationDetail();
            detail.formURI = uri;
            detail.useTime = Time.realtimeSinceStartup - res.startRequestTime;
            try
            {
                 callBack(detail, res.result);
                Debug.Log(uri + " 返回信息:[" + detail.useTime+"]\n res:"+res.result);
                CallResult(detail);
            }
            catch (Exception e)
            {
                CallResult(null);
                Debug.LogError(res.uri + "GetHttpResult 解析失败:"+ res.result +"\n"+ e);
            }


        }));
    }

    private bool isCallResult = false;
    private int errorCallBackCount = 0;
    private int maxRequest = 0;
	private void CallResult (IPGeolocationDetail detail)
    {
        if (isCallResult)
            return;

        if (detail == null)
        {
            errorCallBackCount++;
            if (errorCallBackCount >= maxRequest)
            {
                if (OnIPGeolocationCallBack != null)
                {
                    OnIPGeolocationCallBack(detail);
                }
                Destroy(gameObject);
            }
            return;
        }
        
        isCallResult = true;

       
        if (OnIPGeolocationCallBack != null)
        {
            OnIPGeolocationCallBack(detail);
        }
       // Destroy(gameObject);
    }
    private IEnumerator GetHttpResult(string uri, Action<HttpResult> callBack)
    {
        HttpResult res = new HttpResult();
        res.uri = uri;
        res.startRequestTime = Time.realtimeSinceStartup;

        string error = "";
        string result = "";

        if (string.IsNullOrEmpty(uri))
        {
            res.error = "URI is null";
        }
        else
        {

            WWW webRequest = new WWW(uri);
            yield return webRequest;

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                error = webRequest.error;
            }
            else
            {
                result = webRequest.text;
            }
        }
        //Debug.Log("结果==》"+result);
        if (callBack != null)
        {
            res.error = error;
            res.result = result;
            callBack(res);
        }
        yield break;
    }

    public class HttpResult
    {
        public string uri="";
       public string error = "";
       public string result = "";
        public float startRequestTime;
    }
}

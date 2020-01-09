using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UnityPing : MonoBehaviour
{
    public enum ErrorReason
    {
        None,
        /// <summary>
        /// Network is break
        /// </summary>
        NetNotReachable,
        /// <summary>
        /// 无法访问地址
        /// </summary>
        UnreachableAddress,
        TimeOut,
    }
    public struct PingResult
    {
        public bool isSuccess;
        public ErrorReason errorReason;
        public string host;
        public string ip;
        public int pingTime;
        //public float useTime;
        public PingResult(bool isSuccess, ErrorReason errorReason, string host, string ip, int time)
        {
            this.isSuccess = isSuccess;
            this.errorReason = errorReason;
            this.host = host;
            this.ip = ip;
            this.pingTime = time;
        }
    }
    public class PingRuntimeData
    {
        public string host;
        public string ip;
        public  float s_timeout = 2;
        public bool isFinish = false;

        public float currentUseTime = 0;
        public float delayStartTime = 0;
        public ErrorReason errorReason = ErrorReason.None;
        public Ping ping;
    }
    private  Action<PingResult> s_pingResultCallback = null;
    private Action<PingResult[]> s_OnComplete;

    public static UnityPing CreatePing(string host, Action<PingResult> pingResultCallback,Action<PingResult[]> OnComplete=null, float timeOut=5f, int runTimes=1)
    {
        if (string.IsNullOrEmpty(host)) return null;
        if (pingResultCallback == null) return null;

        GameObject go = new GameObject("[UnityPing]");
        DontDestroyOnLoad(go);
        UnityPing s_unityPing = go.AddComponent<UnityPing>();
        s_unityPing.s_pingResultCallback = pingResultCallback;
        s_unityPing.s_OnComplete = OnComplete;
        s_unityPing.StartRun(host,timeOut,runTimes);

        return s_unityPing;
    }
    /// <summary>
    /// host to ip
    /// </summary>
    /// <returns></returns>
    private   string ResolveHostNameToIP(string host, out AddressFamily af)
    {
        af = AddressFamily.Unknown;
        IPAddress ipAddr;
        if (IPAddress.TryParse(host,out ipAddr))
        {
            af = ipAddr.AddressFamily;

            byte[] ipb = ipAddr.GetAddressBytes();
            StringBuilder sb = new StringBuilder();
            foreach (var b in ipb)
            {
                if (sb.Length > 0)
                {
                    sb.Append(".");
                }
                sb.Append(b);
            }
            return sb.ToString();
            
        }
        IPAddress[] AddressList = null;
        try
            
        {
            IPHostEntry IPinfo = Dns.GetHostEntry(host);
            AddressList = IPinfo.AddressList;
           // AddressList = Dns.GetHostAddresses(host);
        }
        catch (SocketException e)
        {
            AddressList = new IPAddress[] { };
            Debug.LogError(host+ " Dns.GetHostAddresses:" + e);
        }
        foreach (var ip in AddressList)
        {
            af = ip.AddressFamily;
            //IPv4
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] ipb = ip.GetAddressBytes();
                StringBuilder sb = new StringBuilder();
                foreach (var b in ipb)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(".");
                    }
                    sb.Append(b);
                }
                return sb.ToString();
            }
            //IPv6
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                byte[] ipb = ip.GetAddressBytes();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ipb.Length; ++i)
                {
                    if (i % 2 == 0)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(":");
                        }
                    }
                    sb.Append(ipb[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        af = AddressFamily.Unknown;
        return string.Empty;
    }

    private NetworkReachability network;
    private List<PingRuntimeData> pingRuntimeDatas = new List<PingRuntimeData>();
    private void StartRun(string host,float timeOut,int runTimes)
    {
        if (runTimes <= 0)
        {
            Debug.LogError("Ping runTimes cant < 1, now:" + runTimes);
            Destroy(gameObject, 1f);
            return;
        }
        AddressFamily af;
      string  ip = ResolveHostNameToIP(host, out af);
        Debug.Log("host :" + host + " to ip :" + ip + " AddressFamily:" + af);
        network = Application.internetReachability;
        for (int i = 0; i < runTimes; i++)
        {
            PingRuntimeData runtimeData = new PingRuntimeData();
            runtimeData.ip = ip;
            runtimeData.host = host;
            runtimeData.s_timeout = timeOut;
            runtimeData.currentUseTime = 0;
            runtimeData.delayStartTime = 0.05f * (i+1);
            if (string.IsNullOrEmpty(ip))
            {
                runtimeData.isFinish = true;
                runtimeData.errorReason = ErrorReason.UnreachableAddress;
            }

            pingRuntimeDatas.Add(runtimeData);

        }
       
    }
    private List<PingRuntimeData> pingFinishedDatas = new List<PingRuntimeData>();
    private void Update()
    {
        foreach (var runtimeData in pingRuntimeDatas)
        {
            if (runtimeData.isFinish)
            {
                pingFinishedDatas.Add(runtimeData);
                continue;
            }
            if (runtimeData.delayStartTime > 0)
            {
                runtimeData.delayStartTime -= Time.unscaledDeltaTime;
                
            }
            else
            {
                if (runtimeData.ping == null)
                {
                    runtimeData.ping = new Ping(runtimeData.ip);
                }
                RunPingAction(runtimeData);
            }
            
        }

        foreach (var item in pingFinishedDatas)
        {
            pingRuntimeDatas.Remove(item);
            CallResult(item);
        }
        pingFinishedDatas.Clear();
    }
    private void OnDestroy()
    {
        s_pingResultCallback = null;
        pingFinishedDatas.Clear();
        pingResults.Clear();
    }

    private void RunPingAction(PingRuntimeData runtimeData)
    {
        switch (network)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork: // 3G/4G
            case NetworkReachability.ReachableViaLocalAreaNetwork: // WIFI
                {
                    
                    if (runtimeData.ping.isDone)
                    {
                        runtimeData.isFinish = true;
                        if (runtimeData.ping.time == -1)
                        {
                            runtimeData.errorReason = ErrorReason.TimeOut;
                        }
                      
                    }
                    else
                    {
                       
                        if(runtimeData.currentUseTime>= runtimeData.s_timeout)
                        {
                            runtimeData.isFinish = true;
                            network = Application.internetReachability;
                            if (network == NetworkReachability.NotReachable)
                            {
                                runtimeData.errorReason = ErrorReason.NetNotReachable;
                            }
                            else
                                runtimeData.errorReason = ErrorReason.TimeOut;
                        }
                        else
                        {
                            runtimeData.currentUseTime += Time.unscaledDeltaTime;
                        }
                    }
                }
                break;
            case NetworkReachability.NotReachable: // 网络不可用
            default:
                {
                    runtimeData.isFinish = true;
                    runtimeData.errorReason = ErrorReason.NetNotReachable;
                    //  CallResult(PingState.NetNotReachable, -1);
                    network = Application.internetReachability;
                }
                break;
        }
    }

    private List<PingResult> pingResults = new List<PingResult>(); 
    private void CallResult(PingRuntimeData runtimeData)
    {
        int time = runtimeData.ping == null ? -1 : runtimeData.ping.time;
        bool isSuccess = runtimeData.errorReason == ErrorReason.None ? true : false;
        PingResult result = new PingResult(isSuccess, runtimeData.errorReason, runtimeData.host, runtimeData.ip, time);
        pingResults.Add(result);

        if (runtimeData.ping != null)
        {
            runtimeData.ping.DestroyPing();
        }

        if (s_pingResultCallback != null)
        {
           
            s_pingResultCallback(result);
           
        }
        
        if (pingRuntimeDatas.Count<=0)
        {
            if (s_OnComplete != null)
            {
                s_OnComplete(pingResults.ToArray());
            }
           Destroy(this.gameObject,0.5f);
        }
    }
}

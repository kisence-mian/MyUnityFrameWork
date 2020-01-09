using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DownloadRegionServerListFlowItem : FlowItemBase
{
    private string[] regionServerURLs;

    public const string P_IPGeolocationDetail = "IPGeolocationDetail";
    public const string P_GameServerAreaData = "GameServerAreaData";

    public  bool IsChinaIP;
    public  IPGeolocationDetail iPGeolocationDetail;
  //  Action<string, GameServerAreaDataGenerate> OnGetServerAreaCompleted;
    protected override void OnFlowStart(params object[] paras)
    {
        index = 0;
        IPGeolocationManager.GetIPGeolocation(ReciveIPDetail);
       
    }
    internal void SetURLs(string[] pathArr)
    {

        regionServerURLs = pathArr;
        if (regionServerURLs == null)
        {
            Debug.LogError("DownloadRegionServerListFlowItem.regionServerURLs is null!");
            return;
        }
       

    }
    private static int retryTimes = 0;
    private void ReciveIPDetail(IPGeolocationDetail detail)
    {
        if (detail == null)
        {
            retryTimes++;
            if (retryTimes > 1)
            {
                retryTimes = 0;
                RunDownloadRegionServer();
            }
            else
            {
                OnFlowStart();
            }

            return;
        }
        iPGeolocationDetail = detail;
        if (detail.country_code == "CN")
            IsChinaIP = true;
        Debug.Log("IP地区：" + detail.ipv4 + " 国家:" + detail.country);

        flowManager.SetVariable(P_IPGeolocationDetail, detail);

        RunDownloadRegionServer();
    }

    int index = 0;
    private void RunDownloadRegionServer()
    {
        if (index >= regionServerURLs.Length)
        {
            Finish("DownloadRegionServerList fail!");

            return;
        }
       
        string country_code = iPGeolocationDetail == null ? null : iPGeolocationDetail.country_code;
        string url = regionServerURLs[index];
        DownloadRegionServerList(url, country_code, (error, data) =>
          {
              if (!string.IsNullOrEmpty(error))
              {
                  Debug.LogError("RunDownloadRegionServer url:"+ url + "\n error:" + error);
                  RunDownloadRegionServer();
                  return;
              }

              flowManager.SetVariable(P_GameServerAreaData, data);
              Finish(null);

          });
        index++;
    }



    /// <summary>
    /// 下载大区服务器列表
    /// </summary>
    public void DownloadRegionServerList(string url, string country_code, Action<string, GameServerAreaDataGenerate> OnCompleted)
    {
        DataTableExtend.DownLoadTableConfig<GameServerAreaDataGenerate>(url, (dataList) =>
        {
            if (dataList == null)
            {

                if (OnCompleted != null)
                {
                    OnCompleted("download fail!", null);
                }
                return;
            }

            if (!string.IsNullOrEmpty(country_code))
            {
                foreach (var item in dataList)
                {
                    if (ArrayContains(item.m_CountryCode, country_code))
                    {
                        Debug.Log("选定大区key：" + item.m_key);
                        if (OnCompleted != null)
                        {
                            OnCompleted(null, item);
                        }
                        return;
                    }
                }
            }

            Debug.Log("使用ping选择大区：" + dataList.Count);
            List<string> specialServerHostList = new List<string>();
            foreach (var item in dataList)
            {
                specialServerHostList.Add(item.m_SpecialServerHost);
            }
            UnityPingManager.PingGetOptimalItem(specialServerHostList.ToArray(), (statistics) =>
            {
                Debug.Log("选出最优Ping:" + statistics);
                foreach (var item in dataList)
                {
                    if (item.m_SpecialServerHost == statistics.host)
                    {
                        if (OnCompleted != null)
                        {
                            OnCompleted(null, item);
                        }
                        break;
                    }
                }
            });

        });
    }

    private static bool ArrayContains<T>(T[] arrays, T a)
    {
        if (arrays == null || arrays.Length == 0)
            return false;
        foreach (var item in arrays)
        {
            if (item.Equals(a))
            {
                return true;
            }
        }
        return false;
    }
}

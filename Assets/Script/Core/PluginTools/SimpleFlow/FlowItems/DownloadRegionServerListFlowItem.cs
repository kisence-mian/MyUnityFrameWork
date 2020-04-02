using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DownloadRegionServerListFlowItem : FlowItemBase
{
    private string[] regionServerURLs;

    public const string P_IPGeolocationDetail = "IPGeolocationDetail";
    public const string P_GameServerAreaData = "GameServerAreaData";
    public const string P_GameServerAreaDataConfigURL = "GameServerAreaDataConfigURL";

    public const string P_Country_Code = "country_Code";

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
        GameInfoCollecter.AddNetworkStateInfoValue("Device IP", detail.ipv4);
        GameInfoCollecter.AddNetworkStateInfoValue("Device IP Country", detail.country);
        GameInfoCollecter.AddNetworkStateInfoValue("Device IP Country Code", detail.country_code);

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

        string country_code = PlayerPrefs.GetString(P_Country_Code, "");
        if (string.IsNullOrEmpty(country_code))
            country_code = iPGeolocationDetail == null ? null : iPGeolocationDetail.country_code;

        Debug.Log("使用Country code:" + country_code);

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
              flowManager.SetVariable(P_GameServerAreaDataConfigURL, url);
              Finish(null);

          });
        index++;
    }



    /// <summary>
    /// 下载大区服务器列表
    /// </summary>
    public void DownloadRegionServerList(string url, string country_code, Action<string, GameServerAreaData> OnCompleted)
    {
        DataTableExtend.DownLoadTableConfig<GameServerAreaData>(url, (dataList, urlError) =>
        {
            if (!string.IsNullOrEmpty(urlError))
            {
                Debug.LogError("DownloadRegionServerList download fail!");
                if (OnCompleted != null)
                {
                    OnCompleted("download fail! " + urlError, null);
                }
                return;
            }
            if (dataList.Count == 0)
            {
                Debug.LogError("DownloadRegionServerList GameServerAreaData is Empty!");
                if (OnCompleted != null)
                {
                    OnCompleted("GameServerAreaData is Empty!", null);
                }
                return;
            }

            if (!string.IsNullOrEmpty(country_code))
            {
                
                //根据国家选择大区
                foreach (var item in dataList)
                {
                    if (ArrayContains(item.m_CountryCode, country_code))
                    {
                        Debug.Log("国家选定大区key：" + item.m_key);
                        GameInfoCollecter.AddNetworkStateInfoValue("选定大区", item.m_key);
                        if (OnCompleted != null)
                        {
                            OnCompleted(null, item);
                        }
                        return;
                    }
                }
            }
            //根据大洲选择大区
            string continentName = GetContinentByCountryCode(country_code);
            if (!string.IsNullOrEmpty(continentName))
            {
                foreach (var item in dataList)
                {
                    if (ArrayContains(item.m_ContinentName, continentName))
                    {
                        Debug.Log("根据大洲选定大区key：" + item.m_key);
                        GameInfoCollecter.AddNetworkStateInfoValue("选定大区", item.m_key);
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
                GameServerAreaData saData = null;
                foreach (var item in dataList)
                {
                    if (item.m_SpecialServerHost == statistics.host)
                    {
                        saData = item;
                        break;
                    }
                }

                string error = null;
                if (saData == null)
                {
                    error = "Select Ping Result Error!";
                }
                if (OnCompleted != null)
                {
                    OnCompleted(error, saData);
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

    private static Dictionary<string, ContinentCountryTableData> continentCountryTableDic = new Dictionary<string, ContinentCountryTableData>();
    /// <summary>
    /// 获得国家码所在大洲
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns>大洲缩写</returns>
    public static string GetContinentByCountryCode(string countryCode)
    {
        if (continentCountryTableDic.Count == 0)
        {
            try
            {
                TextAsset textAsset = Resources.Load<TextAsset>("ContinentCountryTable");
                ContinentCountryTableData[] data = JsonUtils.FromJson<ContinentCountryTableData[]>(textAsset.text);
                foreach (var item in data)
                {
                    continentCountryTableDic.Add(item.country_code, item);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }

        if (continentCountryTableDic.ContainsKey(countryCode))
        {
            return continentCountryTableDic[countryCode].continent_name;
        }
        else
        {
            Debug.LogError("数据缺失，没有找到对应的大洲 countryCode：" + countryCode);
            return null;
        }
    }
}

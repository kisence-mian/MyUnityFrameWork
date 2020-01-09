using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnityPingManager
{
    public struct PingStatistics
    {
        public string host;
        public string ip;
        /// <summary>
        /// ping次数
        /// </summary>
        public int pingTimes;
        /// <summary>
        /// ping通次数
        /// </summary>
        public int pingPass;
        /// <summary>
        /// 最长ping时间
        /// </summary>
        public int maxPingTime;
        /// <summary>
        /// 最短ping时间
        /// </summary>
        public int minPingTime;
        /// <summary>
        /// 平均ping时间
        /// </summary>
        public int averagePingTime;

        public PingStatistics(string host, string ip, int pingTimes,int pingPass, int maxPingTime, int minPingTime, int averagePingTime)
        {
            this.host = host;
            this.ip = ip;
            this.pingTimes = pingTimes;
            this.pingPass = pingPass;
            this.maxPingTime = maxPingTime;
            this.minPingTime = minPingTime;
            this.averagePingTime = averagePingTime;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Ping:" + host + "\n");
            builder.Append("Ping Times:" + pingTimes + " Pass Times:" + pingPass + "\n");

            if (pingPass > 0)
            {
                builder.Append("Min Ping:" + minPingTime + "ms Max Ping:" + maxPingTime + "ms AVG:" + averagePingTime + "\n");
            }
            return builder.ToString();
        }
    }

    public static void Ping(string host, float timeOut = 4f,int pingTimes=4, Action<string, PingStatistics> resultCallBack=null)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Start Ping " + host+"\n");
        UnityPing.CreatePing(host, ( res) =>
          {
              string stateInfo = res.isSuccess ? "Success" : res.errorReason.ToString();
              builder.Append("return:" + stateInfo + " time:" + res.pingTime+"\n");
              
          },OnComplete:(pingResults) =>
          {
              int passTimes = 0;
              int maxTime=0;
              int minTime=-1;
              int allTime=0;
              int caculateTimes=0;
       
              string ip = "";
              foreach (var item in pingResults)
              {
                  ip = item.ip;
                  if (item.isSuccess)
                  {
                      passTimes++;
                      if (maxTime < item.pingTime)
                      {
                          maxTime = item.pingTime;
                      }
                      if (minTime == -1)
                          minTime = item.pingTime;
                      if(minTime> item.pingTime)
                      {
                          minTime = item.pingTime;
                      }
                      allTime += item.pingTime;
                      caculateTimes++;
                  }
                     
              }
              int averagePingTime = caculateTimes==0? 0:(allTime / caculateTimes);
              PingStatistics statistics = new PingStatistics(host,ip, pingResults.Length, passTimes, maxTime, minTime, averagePingTime);
              builder.Append("Ping Times:" +  statistics.pingTimes+ " Pass Times:" + statistics. pingPass +"\n");

              if (passTimes > 0)
              {
                  builder.Append("Min Ping:" + statistics.minPingTime  + "ms Max Ping:" + statistics.maxPingTime + "ms AVG:" + statistics .averagePingTime+ "\n");
              }
              if (resultCallBack != null)
              {
                  resultCallBack(builder.ToString(),statistics);
              }
              else
              {
                  Debug.Log(builder);
              }
          },timeOut:timeOut, runTimes: pingTimes);
    }

    private static bool isRunPingGetOptimalItem =false;
    private static List<PingStatistics> pingStatistics = new List<PingStatistics>();
    /// <summary>
    /// 选取ping中最好的返回
    /// </summary>
    /// <param name="hostList"></param>
    /// <param name="resultCallBack"></param>
    /// <param name="timeOut"></param>
    /// <param name="pingTimes"></param>
    public static void PingGetOptimalItem(string[] hostList, Action<PingStatistics> resultCallBack, float timeOut = 4f, int pingTimes = 4)
    {
        if (isRunPingGetOptimalItem)
            return;
        isRunPingGetOptimalItem = true;

        pingStatistics.Clear();

        if(hostList==null|| hostList.Length==0)
        {
            Debug.LogError("hostList is Empty!");
           
            isRunPingGetOptimalItem = false;
            SelectPingResult(pingStatistics, resultCallBack);
            return;
        }
        foreach (var host in hostList)
        {
            Ping(host, timeOut, pingTimes, (str, statistics) =>
               {
                   pingStatistics.Add(statistics);

                   if(pingStatistics.Count>= hostList.Length)
                   {
                       isRunPingGetOptimalItem = false;

                       SelectPingResult(pingStatistics, resultCallBack);
                   }
               });
        }
    }

    private static void SelectPingResult(List<PingStatistics> pingStatistics,Action<PingStatistics> resultCallBack)
    {
        if (pingStatistics.Count <= 1)
        {
            if (pingStatistics.Count == 0)
            {
                if (resultCallBack != null)
                {
                    resultCallBack(default(PingStatistics));
                }
            }
            else
            {
                if (resultCallBack != null)
                {
                    resultCallBack(pingStatistics[0]);
                }
            }
        }
        else
        {
            List<PingStatistics> tempStatistics = new List<PingStatistics>();
            //选取ping通率在50%及以上的
            foreach (var ss in pingStatistics)
            {
                if (ss.pingPass * 1f / ss.pingTimes >= 0.5f)
                {
                    tempStatistics.Add(ss);
                }
            }

            if (tempStatistics.Count == 0)
            {
                tempStatistics.AddRange(pingStatistics);
            }
            //选取平均Ping最少的
            PingStatistics minPing = tempStatistics[0];
            foreach (var ss in tempStatistics)
            {
                if (ss.averagePingTime < minPing.averagePingTime && ss.pingPass > 0)
                {
                    minPing = ss;
                }
            }
            if (resultCallBack != null)
            {
                resultCallBack(minPing);
            }
        }
    }
}


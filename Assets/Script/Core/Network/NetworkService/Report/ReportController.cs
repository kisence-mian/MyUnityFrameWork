using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ReportController
{
    /// <summary>
    /// 数据上报
    /// </summary>
    /// <param 上报事件名 ="eventName"></param>
    /// <param 具体数据 ="datas"></param>
    public static void ReportEvent(string eventName,Dictionary<string,string> datas)
    {
        SDKManager.Log(eventName, datas);
    }
    /// <summary>
    /// 构建上报广告数据
    /// </summary>
    /// <param name="eventName">示例AD_xxx</param>
    /// <param name="name">只能为"Play"、"Load"其中之一，区分大小写。"Play"=广告播放、"Load"=广告加载）</param>
    /// <param name="cause">什么原因播放，来源</param>
    /// <param name="result">ADState是否成功</param>
    /// <param name="source">广告商来源</param>
    public static Dictionary<string, string> BuildADEventData(string eventName, ADState name= ADState.Play, bool result = true, String source="")   
    {
        if(string.IsNullOrEmpty(eventName)|| !eventName.ToLower().Contains("ad_"))
        {
            Debug.LogError("上报广告名不合规则：" + eventName);
            return new Dictionary<string, string>();
        }
        string typeName = eventName.Substring(3);
        Debug.Log("AD TypeName:" + typeName);
        Dictionary<string, string> datas = new Dictionary<string, string>();
        datas.Add("ad_id", typeName);
        datas.Add("name", name.ToString());
        datas.Add("cause", typeName);
        datas.Add("result", result.ToString());
        datas.Add("source", source);

        return datas;
    }

    public enum ADState
    {
        Play,
        Load,
        Click,
    }
}


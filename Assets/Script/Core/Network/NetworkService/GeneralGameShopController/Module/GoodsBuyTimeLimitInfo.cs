using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/***
 * 限制购买时间
 * @author xgs2
 *
 */
public class GoodsBuyTimeLimitInfo
{

    public GoodsBuyTimeLimitType timeLimitType = GoodsBuyTimeLimitType.Forever;
   // public TimerEnum timePer;
    /***
	 * 时间范围（格式：2019-01-12 00:00:00=2019-02-01 12:00:00）,不限制为null
	 */
    public String timeRange;
    /// <summary>
    /// 直接返回（每日，每周,每月的多语言字段）
    /// </summary>
    public String timePerString;
    public string GetTimePerString()
    {
        return timePerString;
    }
  
    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss";
    /// <summary>
    /// 获得时间范围
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    public void GetTimeRange(out DateTime startTime,out DateTime endTime)
    {
        String[] arrs = timeRange.Split('=');
        startTime = DateTime.Now;
        endTime = DateTime.Now;
        try
        {
             startTime = DateTime.ParseExact(arrs[0], TimeFormat,null);
             endTime = DateTime.ParseExact(arrs[1], TimeFormat,null);

           
        }
        catch (Exception e)
        {
            Debug.LogError("转换时间格式失败：" + timeRange + "\n" + e);
        }
    }
}
public enum GoodsBuyTimeLimitType
{
    /// <summary>
    /// 永远（限制终身，比如终身只能购买1次）
    /// </summary>
    Forever,
    /// <summary>
    /// 	 具体时间范围
    /// </summary>
    TimeRange,
    /// <summary>
    /// 定义循环时间 每周每天么，每月等等
    /// </summary>
    PerTime,
}
//public enum TimerEnum
//{
//    preSecond,
//    pre5S,
//    pre10S,
//    preMinute,
//    preHour,
//    preDay,
//    preWeek,
//    preMonth,
//}
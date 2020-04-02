using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 控制公告消息
/// </summary>
public static class AnnouncementController
{
    public static CallBack<AnnouncementContent2Client> OnAnnouncementMessage;

    private static List<AnnouncementContent2Client> messageCache = new List<AnnouncementContent2Client>();
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        GlobalEvent.AddTypeEvent<AnnouncementContent2Client>(OnAnnouncement);
    }

    private static void OnAnnouncement(AnnouncementContent2Client e, object[] args)
    {
        if (OnAnnouncementMessage != null)
            OnAnnouncementMessage(e);
    }
    /// <summary>
    /// 获得公告信息缓存
    /// </summary>
    /// <returns></returns>
    public static List<AnnouncementContent2Client> GetMessageCache()
    {
        return messageCache;
    }
    public static void AddCache(AnnouncementContent2Client e)
    {
        messageCache.Add(e);
    }
    /// <summary>
    /// 清除缓存
    /// </summary>
    public static void ClearCache()
    {
        messageCache.Clear();
    }
    /// <summary>
    /// 确认已阅读公告信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="useTag"></param>
    public static void ConfirmMessage(string id, string useTag)
    {
        AnnouncementConfirm2Server msg = new AnnouncementConfirm2Server();
        msg.id = id;
        msg.useTag = useTag;
        JsonMessageProcessingController.SendMessage(msg);
    }
}


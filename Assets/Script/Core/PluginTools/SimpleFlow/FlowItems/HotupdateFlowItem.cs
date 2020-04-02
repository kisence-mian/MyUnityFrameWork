using UnityEngine;
using System.Collections;
using System;
using System.IO;
using FrameWork.SDKManager;
using System.Collections.Generic;

public class HotupdateFlowItem : FlowItemBase
{
    public Action<HotUpdateStatusInfo> OnHotUpdateStatus;

    public const string P_GameServerAreaData = "GameServerAreaData";
    public const string P_GameServerAreaDataConfigURL = "GameServerAreaDataConfigURL";
    /// <summary>
    /// 热更新配置文件名
    /// </summary>
    public const string P_HotUpdatePathData = "HotUpdatePathData.txt";
    /// <summary>
    /// 测试热跟新地址保存名称(使用远程工具修改)
    /// </summary>
    public const string P_SelectHotUpdateTestPath = "SelectHotUpdateTestPath";
    protected override void OnFlowStart(params object[] paras)
    {
        //GameServerAreaData gameServerArea = flowManager.GetVariable<GameServerAreaData>(P_GameServerAreaData);

        string hotupdateConfigUrl = null;
        try
        {
            string url = flowManager.GetVariable<string>(P_GameServerAreaDataConfigURL);
            Debug.Log("P_GameServerAreaDataConfigURL:" + url);
            int lastIndex = url.LastIndexOf("/");

            hotupdateConfigUrl = url;// Path.GetDirectoryName(url) + "/"+ P_HotUpdatePathData;
            hotupdateConfigUrl = hotupdateConfigUrl.Replace(hotupdateConfigUrl.Substring(lastIndex), "/" + P_HotUpdatePathData);
            Debug.Log("hotupdateConfigUrl:" + hotupdateConfigUrl);

        }
        catch (Exception e)
        {
            Finish("Parse URL failed :" + e);
            return;
        }

        DataTableExtend.DownLoadTableConfig<HotUpdatePathData>(hotupdateConfigUrl, GetHotUpdatePath);

    }

    private void GetHotUpdatePath(List<HotUpdatePathData> datas, string error)
    {

        if (!string.IsNullOrEmpty(error))
        {
            Finish("Download HotUpdatePathData failed :" + error);
            return;
        }
        else
        {
            string channel = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_ChannelName, "GameCenter");

            Debug.Log("Download HotUpdatePathData count:" + datas.Count);
            HotUpdatePathData pathData = null;
            foreach (var d in datas)
            {
                if (d.m_key == channel)
                {
                    pathData = d;
                    break;
                }
            }

            if (pathData == null)
            {
                Finish("No Channel in HotUpdatePathData Channel:" + channel);
                return;
            }
            string m_HotupdatePath = pathData.m_HotupdatePath;
            string testPath = PlayerPrefs.GetString(P_SelectHotUpdateTestPath, "");
            if (!string.IsNullOrEmpty(testPath))
            {
                //使用测试地址
                m_HotupdatePath = testPath;
            }
            Debug.Log("Start hotUpdate in Channel:" + channel + "\nPath:" + m_HotupdatePath);
            if (ApplicationManager.Instance.UseAssetsBundle)
                HotUpdateManager.StartHotUpdate(m_HotupdatePath, ReceviceUpdateStatus);
            else
            {
                HotUpdateStatusInfo info = new HotUpdateStatusInfo();
                info.m_status = HotUpdateStatusEnum.NoUpdate;
                info.m_loadState = new LoadState();
                info.m_loadState.isDone = true;
                info.m_loadState.progress = 1f;
                ReceviceUpdateStatus(info);
            }
        }
    }

    private void ReceviceUpdateStatus(HotUpdateStatusInfo info)
    {
        if (OnHotUpdateStatus != null)
        {
            OnHotUpdateStatus(info);
        }

        if (info.m_status == HotUpdateStatusEnum.NoUpdate || info.m_status == HotUpdateStatusEnum.UpdateSuccess)
        {
            Finish(null);
        }
        else if (info.m_status == HotUpdateStatusEnum.Md5FileDownLoadFail || info.m_status == HotUpdateStatusEnum.UpdateFail || info.m_status == HotUpdateStatusEnum.VersionFileDownLoadFail)
        {
            Finish("error:update failed! " + info.m_status);
        }
    }

}

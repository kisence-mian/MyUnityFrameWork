using UnityEngine;
using System.Collections;
using System;

public class HotupdateFlowItem : FlowItemBase
{
    public  Action<HotUpdateStatusInfo> OnHotUpdateStatus;

    public const string P_GameServerAreaData = "GameServerAreaData";
    protected override void OnFlowStart(params object[] paras)
    {
        GameServerAreaDataGenerate gameServerArea = flowManager.GetVariable<GameServerAreaDataGenerate>(P_GameServerAreaData);

        HotUpdateManager.StartHotUpdate(gameServerArea.m_ClientHotUpdateURL, ReceviceUpdateStatus);
    }

    private  void ReceviceUpdateStatus(HotUpdateStatusInfo info)
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
            Finish( "error:update failed!");
        }
    }

}

using UnityEngine;
using System.Collections;
using System;
using FrameWork.SDKManager;

public class SelectServerFlowItem : FlowItemBase
{
    public const string P_GameServerAreaData = "GameServerAreaData";
    protected override void OnFlowStart(params object[] paras)
    {
        GameServerAreaDataGenerate gameServerArea = flowManager.GetVariable<GameServerAreaDataGenerate>(P_GameServerAreaData);
        StartSelectServer(gameServerArea);
    }

    public  Action<SelectNetworkData> OnSelectServerCompleted;
    public  Action<Action<SelectNetworkData>> OnSelectServerLocal;



    public  void StartSelectServer(GameServerAreaDataGenerate gameServerArea)
    {
        Debug.Log("开始选服");
        if (ApplicationManager.Instance.m_AppMode == AppMode.Release)
        {
            RuntimePlatform platform = Application.platform;
            if (Application.isEditor)
            {
                if (platform == RuntimePlatform.OSXEditor)
                    platform = RuntimePlatform.IPhonePlayer;
                else
                {
                    platform = RuntimePlatform.Android;
                }
            }
            string channel = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_ChannelName, "GameCenter");
            string selectNetworkPath = gameServerArea.m_SelectServerURL;
            SelectSeverController.Start(selectNetworkPath, Application.version, platform, channel, (data) =>
            {
                SelectNetworkData select = null;

                if (data == null || data.Count == 0)
                {
                    Debug.LogError("没有合适的服务器！");
                    //return;
                    string networkID = SDKManager.GetProperties("NetworkID", "3");
                    select = DataGenerateManager<SelectNetworkData>.GetData(networkID);

                }
                else
                {

                    int r = UnityEngine.Random.Range(0, data.Count);
                    select = data[r];
                }
                SelectServerCompleted(select);
            });
        }
        else
        {
            if (OnSelectServerLocal != null)
            {
                OnSelectServerLocal(SelectServerCompleted);
            }
        }
    }
    private  void SelectServerCompleted(SelectNetworkData select)
    {
        Debug.Log("选服完成:" + select.m_key);
        if (OnSelectServerCompleted != null)
        {
            OnSelectServerCompleted(select);
        }

        Finish(null);
    }
}

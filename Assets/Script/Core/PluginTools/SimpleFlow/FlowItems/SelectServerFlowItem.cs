using UnityEngine;
using System.Collections;
using System;
using FrameWork.SDKManager;

public class SelectServerFlowItem : FlowItemBase
{
    public const string P_GameServerAreaData = "GameServerAreaData";
    protected override void OnFlowStart(params object[] paras)
    {
        GameServerAreaData gameServerArea = flowManager.GetVariable<GameServerAreaData>(P_GameServerAreaData);
        StartSelectServer(gameServerArea);
    }

    public  Action<SelectNetworkData> OnSelectServerCompleted;
    public  Action<Action<SelectNetworkData>> OnSelectServerLocal;



    public  void StartSelectServer(GameServerAreaData gameServerArea)
    {
        Debug.Log("开始选服");
        if (ApplicationManager.Instance.m_AppMode == AppMode.Release)
        {
            Debug.Log(" Application.isMobilePlatform:" + Application.isMobilePlatform);
            RuntimePlatform platform = Application.platform;
#if UNITY_ANDROID
            platform = RuntimePlatform.Android;

#elif UNITY_IPHONE
             platform = RuntimePlatform.IPhonePlayer;

#endif


            string defaultChannel = "GameCenter";

            string channel = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_ChannelName, defaultChannel);

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

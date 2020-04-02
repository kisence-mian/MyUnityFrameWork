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
            RuntimePlatform platform = Application.platform;
            //if (Application.isEditor)
            //{
            //    if (platform == RuntimePlatform.OSXEditor)
            //        platform = RuntimePlatform.IPhonePlayer;
            //    else
            //    {
            //        platform = RuntimePlatform.Android;
            //    }
            //}

            string defaultChannel = "GameCenter";

            string channel = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_ChannelName, defaultChannel);
            GameInfoCollecter.AddNetworkStateInfoValue("渠道", channel);

            string selectNetworkPath = gameServerArea.m_SelectServerURL;
            SelectSeverController.Start(selectNetworkPath, Application.version, platform, channel, (data) =>
            {
                SelectNetworkData select = null;
                bool isSelectRight = false;
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
                    isSelectRight = true;
               
                }
                GameInfoCollecter.AddNetworkStateInfoValue("是否正确匹配服务器", isSelectRight);
                GameInfoCollecter.AddNetworkStateInfoValue("匹配服务器", select.m_serverIP+":"+select.m_port);
                GameInfoCollecter.AddNetworkStateInfoValue("服务器描述", select.m_description);
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

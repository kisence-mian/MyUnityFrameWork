using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayReSend  {

    static PayReSend instance;

    LocalPayInfo localPayInfo;

    public static PayReSend Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PayReSend();
                instance.ReadData();
            }
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    #region 信息读写

    /// <summary>
    /// 读取信息
    /// </summary>
    private void ReadData()
    {
        string json = RecordManager.GetStringRecord("localPayInfo", "localPayInfo", null);
        localPayInfo = JsonUtils.FromJson<LocalPayInfo>(json);
        if (localPayInfo == null)
        {
            localPayInfo = new LocalPayInfo();
            localPayInfo.payInfos = new List<OnPayInfo>();
        }
    }

    /// <summary>
    /// 写入信息
    /// </summary>
    private void WriteData()
    {
        string json = JsonUtils.ToJson(localPayInfo);
        RecordManager.SaveRecord("localPayInfo", "localPayInfo", json);
    }
    #endregion

    /// <summary>
    /// 添加预支付ID
    /// </summary>
    public void AddPrePayID(OnPayInfo onPayInfo)
    {
        onPayInfo.userID = SDKManager.UserID;
        localPayInfo.payInfos.Add(onPayInfo);
        WriteData();
        ReadData();

        Debug.LogWarning("已经写入预支付订单" + localPayInfo.payInfos.Count);
    }

    /// <summary>
    /// 清除预支付ID
    /// </summary>
    /// <param name="mch_orderID"></param>
    public void ClearPrePayID(string mch_orderID)
    {
        Debug.LogWarning("清空预支付" + mch_orderID);

        List<OnPayInfo> removeList = new List<OnPayInfo>();

        for (int i = 0; i < localPayInfo.payInfos.Count; i++)
        {
            if (localPayInfo.payInfos[i].receipt == mch_orderID)
            {
                removeList.Add(localPayInfo.payInfos[i]);
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            localPayInfo.payInfos.Remove(removeList[i]);
        }
        WriteData();
        removeList.Clear();
    }

    /// <summary>
    /// 重发购买
    /// </summary>
    public void ReSendPay()
    {
        Debug.LogWarning("重发支付" + localPayInfo.payInfos.Count);
        GlobalEvent.AddTypeEvent<StoreBuyGoods2Client>(OnStoreBuyGood);

        for (int i = 0; i < localPayInfo.payInfos.Count; i++)
        {
            OnPayInfo payInfo = localPayInfo.payInfos[i];
            if (string.IsNullOrEmpty( payInfo.userID)||  payInfo.userID == SDKManager.UserID)
            {
                Debug.LogWarning("重发购买+" + payInfo.goodsId + "==receipt==" + payInfo.receipt);
                //发送消息
                StoreBuyGoods2Server.SenBuyMsg(payInfo.goodsId, 1, localPayInfo.payInfos[i].storeName, payInfo.receipt);
            }
        }
    }

    private void OnStoreBuyGood(StoreBuyGoods2Client e, object[] args)
    {
        ClearPrePayID(e.receipt);
    }
}



public class LocalPayInfo
{
    public List<OnPayInfo> payInfos = new List<OnPayInfo>();
}
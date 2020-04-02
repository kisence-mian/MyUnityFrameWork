using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class PrePay2Service
{
    public StoreName storeName;
    public string goodsID;
    public string prepay_id;

    public PrePay2Service(StoreName storeName, string goodsID)
    {
        this.storeName = storeName;
        this.goodsID = goodsID;
    }



    static public void SendPrePayMsg(StoreName storeName, string goodsID)
    {
        DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        string timeStamp = (new DateTime(DateTime.UtcNow.Ticks - dt1970.Ticks).AddHours(8).Ticks / 10000000).ToString();
        Debug.LogWarning("支付发起时间" + timeStamp);

        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    return;
        //}

        //PrePay2Service msg = new PrePay2Service(storeName, goodsID);
        //PrePay2Service.SendPrePayMsg(storeName, goodsID);

        //ResendMessageManager.AddResendMessage(msg, typeof(StoreBuyGoods2Client).Name, (resMsg) =>
        //{

        //});

        JsonMessageProcessingController.SendMessage<PrePay2Service>(new PrePay2Service(storeName, goodsID));

    }

}

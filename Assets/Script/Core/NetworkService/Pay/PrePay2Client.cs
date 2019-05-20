using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrePay2Client : CodeMessageBase
{
    public StoreName storeName;
    public string goodsID;
    public string prepay_id;
    public string mch_orderID;

    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}

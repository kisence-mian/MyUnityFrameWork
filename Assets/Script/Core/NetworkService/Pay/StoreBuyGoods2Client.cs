using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreBuyGoods2Client : MessageClassInterface
{
    public int code;
    public string id;
    public int number;
    //是否是重复的凭据
    public bool repeatReceipt = false;
    public string receipt;

    public  void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}

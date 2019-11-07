using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogInterface : SDKInterfaceBase
{
    public virtual void Log(string eventID, Dictionary<string, string> data) { }

    public virtual void LogLogin(string accountID, Dictionary<string, string> data) { }

    //public virtual void LogEventBegin(string eventID, Dictionary<string, string> data) { }
    //public virtual void LogEventEnd(string eventID, Dictionary<string, string> data){}

    public virtual void LogLoginOut(string accountID) { }

    public virtual void LogPay(string orderID,string goodsID,int count,float price,string currency ,string payment) { }

    public virtual void LogPaySuccess(string orderID) { }

    //以下三个配合使用，用于追踪虚拟物品的产出消耗
    public virtual void LogRewardVirtualCurrency(float count,string reason) { }

    public virtual void LogPurchaseVirtualCurrency(string goodsID,int num,float price) { }

    public virtual void LogUseItem(string goodsID, int num) { }

}

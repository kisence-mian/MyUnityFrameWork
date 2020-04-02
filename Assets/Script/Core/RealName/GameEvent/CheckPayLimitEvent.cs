using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检测支付限制
/// </summary>
public class CheckPayLimitEvent
{
    public int payAmount;



    public CheckPayLimitEvent(int payAmount)
    {
        this.payAmount = payAmount;
    }

    static public void Dispatch(int l_payAmount)
    {
        GlobalEvent.DispatchTypeEvent<CheckPayLimitEvent>(new CheckPayLimitEvent(l_payAmount));
    }

}



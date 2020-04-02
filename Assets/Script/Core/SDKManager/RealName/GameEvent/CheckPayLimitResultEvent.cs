using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检测支付限制
/// </summary>
public class CheckPayLimitResultEvent
{
    public int payAmount;
    public PayLimitType payLimitType = PayLimitType.None;//限制购买（如： 本单超出未成年限制）

    public CheckPayLimitResultEvent(int payAmount, PayLimitType l_payLimitType)
    {
        this.payAmount = payAmount;
        payLimitType = l_payLimitType;
    }

    static public void Dispatch(int l_payAmount, PayLimitType l_payLimitType)
    {
        GlobalEvent.DispatchTypeEvent<CheckPayLimitResultEvent>(new CheckPayLimitResultEvent(l_payAmount, l_payLimitType));
    }

}

/// <summary>
/// 支付限制类型
/// </summary>
public enum PayLimitType
{
    /// <summary>
    /// 无限制
    /// </summary>
    None,
    /// <summary>
    /// 未完成实名认证
    /// </summary>
    NoRealName,
    /// <summary>
    /// 未成年，超出了限制
    /// </summary>
    ChildLimit,
}
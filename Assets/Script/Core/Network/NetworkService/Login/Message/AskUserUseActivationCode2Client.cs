using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 要求用户使用兑换码
/// </summary>
public class AskUserUseActivationCode2Client : MessageClassInterface
{


    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}


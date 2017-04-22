using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInterface : SDKInterfaceBase
{

    public override void Init()
    {
        base.Init();
    }

    public virtual void Log(string eventID, Dictionary<string, object> data)
    {

    }

    public virtual void LoginLog(string accountID, Dictionary<string, object> data)
    {

    }

    public virtual void LogEventBegin(string eventID, Dictionary<string, object> data)
    {

    }

    public virtual void LogEventEnd(string eventID, Dictionary<string, object> data)
    {

    }
}

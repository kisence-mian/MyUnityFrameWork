using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LogInterface : SDKInterfaceBase
{
    void Log(string eventID, Dictionary<string, object> data);
}

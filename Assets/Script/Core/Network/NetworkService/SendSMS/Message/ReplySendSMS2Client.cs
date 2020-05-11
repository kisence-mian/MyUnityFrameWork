using UnityEngine;
using System.Collections;

public class ReplySendSMS2Client : CodeMessageBase
{

    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ConfirmMergeExistAccount2Client : CodeMessageBase
{
    public LoginPlatform loginType;

    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}
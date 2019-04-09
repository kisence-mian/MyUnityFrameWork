using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UserRegister2Client:CodeMessageBase
{
    public LoginPlatform loginType;
    public String typeKey;

    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UserLogin2Client :CodeMessageBase
{
    public User user;
    public LoginPlatform loginType;
    public String typeKey;

    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UserLogin2Client :CodeMessageBase
{
    public User user;
    public LoginPlatform loginType;
    public String typeKey;
    //是否是新玩家登录
    public bool newPlayerState;
    /// <summary>
    /// 标记是否是重连
    /// </summary>
    public bool reloginState = false;
    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}


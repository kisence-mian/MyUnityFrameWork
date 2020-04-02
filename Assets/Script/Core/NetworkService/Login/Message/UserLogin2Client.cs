using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UserLogin2Client :CodeMessageBase
{
    public User user;
 
    //是否是新玩家登录
    public bool newPlayerState;
    /// <summary>
    /// 标记是否是重连
    /// </summary>
    public bool reloginState = false;
    /// <summary>
    /// 服务器是否支持消息压缩
    /// </summary>
    public bool supportCompressMsg = false;
    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}


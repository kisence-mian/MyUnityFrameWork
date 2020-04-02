using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class User
{
    public String userID = "";
    //昵称
    public String nickName;
    /// <summary>
    /// 头像
    /// </summary>
    public String portrait;
    /// <summary>
    /// 登录平台类型
    /// </summary>
    public LoginPlatform loginType;
    /// <summary>
    /// 登录账号（平台不同而不同）
    /// </summary>
    public String typeKey;

    /// <summary>
    /// 游戏时长(分钟)
    /// </summary>
    public long playTime = 0;

    /// <summary>
    /// 累计登录天数
    /// </summary>
    public int totalLoginDays = 0;
}

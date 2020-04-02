
using System.Collections;

public class RequestRealNameState2Server 
{
    /// <summary>
    /// 实名制状态
    /// </summary>
    public RealNameStatus realNameStatus = RealNameStatus.NotNeed;

    public bool isAdult = true;

    public RequestRealNameState2Server(RealNameStatus realNameStatus, bool isAdult)
    {
        this.realNameStatus = realNameStatus;
        this.isAdult = isAdult;
    }

    public static void RequestRealName(RealNameStatus l_realNameStatus,bool l_isAdult)
    {
        JsonMessageProcessingController.SendMessage(new RequestRealNameState2Server(l_realNameStatus, l_isAdult));
    }
}

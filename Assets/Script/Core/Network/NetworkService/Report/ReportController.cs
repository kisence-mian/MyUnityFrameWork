using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class ReportController
{
    /// <summary>
    /// 数据上报
    /// </summary>
    /// <param 上报事件名 ="eventName"></param>
    /// <param 具体数据 ="datas"></param>
    public static void ReportEvent(string eventName,Dictionary<string,string> datas)
    {
        ClientReport2Server msg = new ClientReport2Server();
        msg.eventName = eventName;

        msg.datas = KeyValueData.Dictionary2KeyValueDataList(datas);

        JsonMessageProcessingController.SendMessage(msg);
    }
}


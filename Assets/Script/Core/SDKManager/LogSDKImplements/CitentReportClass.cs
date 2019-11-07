using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CitentReportClass : LogInterface
{
    public override void Log(string eventID, Dictionary<string, string> data)
    {
        CitentReport2Server msg = new CitentReport2Server();
        msg.eventName = eventID;

        msg.datas = KeyValueData.Dictionary2KeyValueDataList(data);

        JsonMessageProcessingController.SendMessage(msg);
    }
}


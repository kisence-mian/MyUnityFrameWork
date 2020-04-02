using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralShopTableData2Client : MessageClassInterface
{
    public string classType;
    public List<string> content;

    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}

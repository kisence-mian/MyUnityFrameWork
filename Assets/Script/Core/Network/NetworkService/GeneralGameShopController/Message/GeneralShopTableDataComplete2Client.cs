using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 传输表格数据完成
/// </summary>
public class GeneralShopTableDataComplete2Client : MessageClassInterface
{
    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}


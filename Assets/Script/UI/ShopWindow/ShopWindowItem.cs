using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindowItem : ReusingScrollItemBase
{
    public override void SetContent(int index, Dictionary<string, object> data)
    {
        SetText("Text_Name", (string)data["Name"]);
        SetText("Text_Cost", "$ "+ (string)data["Cost"]);
    }
}

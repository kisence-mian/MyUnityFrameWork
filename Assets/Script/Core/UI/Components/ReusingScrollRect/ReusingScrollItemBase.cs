using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReusingScrollItemBase : UIBase 
{
    public int m_index = 0;

    public void SetConetnt(int index,Dictionary<string, object> data)
    {
        GetText("Text").text = index.ToString();
    }

}

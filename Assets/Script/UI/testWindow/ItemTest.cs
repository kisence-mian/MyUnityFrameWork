using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemTest :ReusingScrollItemBase
{
    public override void OnInit()
    {
        base.OnInit();

        AddOnClickListener("Image_item", OnClick);
    }

    public override void SetConetnt(int index, Dictionary<string, object> data)
    {
        GetText("Text").text = index.ToString();
    }

    public void OnClick(InputUIOnClickEvent e)
    {
        Debug.Log("item onclick " + m_index);
    }
}

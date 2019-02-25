using UnityEngine;
using System.Collections;
using FrameWork.GuideSystem;

public class GuideWindow : GuideWindowBase
{
    public override void OnInit()
    {
        AddOnClickListener("mask", OnClickMask);
    }

    void OnClickMask(InputUIOnClickEvent e)
    {
        Debug.Log("clickMask");
    }

    public override void ShowTips(string content, Vector3 pos)
    {
        if(content != null && content != "")
        {
            SetActive("Tips", true);

            SetText("Text_tip", content);
            GetRectTransform("Tips").anchoredPosition3D = pos;
        }
        else
        {
            SetActive("Tips", false);
        }
    }
}
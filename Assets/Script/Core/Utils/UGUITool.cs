using System.Collections.Generic;
/// <summary>
/// 存点儿 用户登陆数据 游戏设置 相关的数据（可读写）
/// </summary>
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UGUITool
{
    static PointerEventData eventDatas = new PointerEventData(EventSystem.current);
    static List<RaycastResult> hit = new List<RaycastResult>();

    static public bool isHitUI()
    {
        eventDatas.position = Input.mousePosition;
        eventDatas.pressPosition = Input.mousePosition;
        EventSystem.current.RaycastAll(eventDatas, hit);

        if (hit.Count > 0)
            return true;

        if (EventSystem.current.IsPointerOverGameObject())  //鼠标点在UI上
            return true;
        return false;
    }
    
    static public void set_icon(Image img,string name)
    {
        img.overrideSprite = ResourceManager.Load<Sprite>(name);
        img.SetNativeSize();
    }
}

using System.Collections.Generic;
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
    
    static public void set_icon(Image img,string name,bool is_nativesize = true)
    {
        try
        {
            Texture2D tex = ResourceManager.Load<Texture2D>(name);
            img.overrideSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector3.zero);

            if (is_nativesize)
                img.SetNativeSize();
        }
        catch (System.Exception e)
        {
            Debug.LogError("set_icon Exception:" + e.ToString());           
        }
    }
   
}

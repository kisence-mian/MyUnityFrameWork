using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UGUITool
{
    static PointerEventData eventDatas = new PointerEventData(EventSystem.current);
    static List<RaycastResult> hit = new List<RaycastResult>();

    static public bool IsHitUI()
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
    
    static public void SetImageSprite(Image img,string name,bool is_nativesize = false)
    {
        if(name == null)
        {
            Debug.LogError("set_icon Image name 不能为 null !" );
            return;
        }

        if (img == null)
        {
            Debug.LogError("set_icon Image 不能为 null !");
            return;
        }

            Sprite sp = ResourceManager.Load<Sprite>(name);// LoadSprite(name);
            if (sp != null)
            {
                img.overrideSprite = sp;// Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector3.zero);
                img.sprite = img.overrideSprite;
               
                if (is_nativesize)
                    img.SetNativeSize();
            }
            else
            {
                Debug.LogError("SetImageSprite 加载失败，查看资源是否存在，图片格式是否正确:" + name);
            }
       
    }

    static public void SetSpriteRender(GameObject go, string name)
    {
        SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
        sprite.sprite = LoadSprite(name);
    }

    public static Sprite LoadSprite(string resName)
    {
        Texture2D texture = ResourceManager.Load<Texture2D>(resName);
        if (texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("加载图片失败：" + resName);
            return null;
        }
    }
   
}

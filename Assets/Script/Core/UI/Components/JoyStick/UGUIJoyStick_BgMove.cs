using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UGUIJoyStick_BgMove : UIBase, IDragHandler, IEndDragHandler
{

    const float c_baseScreenWidth = 1920; //基础屏幕宽度分辨率
    private Vector3 originalPos = new Vector3(-725.5f, -285.5f, 0);
    private Vector2 screenSize = new Vector3(Screen.width, Screen.height);
    protected float mRadius;

    private float screenScale;

    public RectTransform content;

    public UGUIJoyStickHandle onJoyStick;

    void Start()
    {
        //计算摇杆块的半径
        mRadius = ((transform as RectTransform).sizeDelta.x - content.sizeDelta.x) * 0.5f;
        screenScale = c_baseScreenWidth / Screen.width;
    }


    public void OnDrag(PointerEventData eventData)
    {
        
        Vector3 contentPostion = content.anchoredPosition + eventData.delta * screenScale;
        if (contentPostion.magnitude > mRadius)
        {
            transform.localPosition = (Vector3)(eventData.position - screenSize * 0.5f) * screenScale - contentPostion.normalized * mRadius;
            contentPostion = contentPostion.normalized * mRadius;
        }

        content.anchoredPosition3D = contentPostion;

        //onJoyStick(GetDir());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = originalPos;
        content.anchoredPosition3D = Vector3.zero;
        onJoyStick(Vector3.zero);
    }

    public Vector3 GetDir()
    {
        Vector3 dir = new Vector3(content.anchoredPosition3D.x, 0, content.anchoredPosition3D.y);

        dir /= mRadius;

        return dir;
    }

    void Update()
    {
        if (onJoyStick != null)
        {
            try
            {
                if (GetDir() != Vector3.zero)
                {
                    onJoyStick(GetDir());
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

    }

}

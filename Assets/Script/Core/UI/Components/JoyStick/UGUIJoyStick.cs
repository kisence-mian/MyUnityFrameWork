using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UGUIJoyStick : UIBase, IDragHandler, IEndDragHandler,IBeginDragHandler
{
    protected float mRadius;

    public RectTransform content;

    public UGUIJoyStickHandle onJoyStick;

    public bool canMove = true;

    //当前分辨率，与UImanager上的标准分辨率之间的换算比
    public float conversionX;
    public float conversionY;

    void Start()
    {
        //计算摇杆块的半径
        mRadius = ((transform as RectTransform).sizeDelta.x-content.sizeDelta.x) * 0.5f;
        Vector2 referenceResolution = UIManager.UIManagerGo.GetComponent<CanvasScaler>().referenceResolution;
        conversionX =  referenceResolution.x / Screen.width ;
        conversionY =  referenceResolution.y/Screen.height ;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canMove = true;
    }

    Vector2 centerDelta = new Vector2(0, 0);

    public void OnDrag(PointerEventData eventData)
    {
        centerDelta.x = eventData.delta.x * conversionX;
        centerDelta.y = eventData.delta.y * conversionY;
        Vector3 contentPostion = content.anchoredPosition + centerDelta;
        if (contentPostion.magnitude > mRadius)
        {
            contentPostion = contentPostion.normalized * mRadius;
        }

        content.anchoredPosition3D = contentPostion;

        //onJoyStick(GetDir());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canMove = false;
        content.anchoredPosition3D = Vector3.zero;
        //onJoyStick(Vector3.zero);
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
                if (GetDir() != Vector3.zero && canMove)
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

    public void ReHomePos()
    {
        canMove = false;
        
        content.anchoredPosition3D = Vector3.zero;
        onJoyStick(Vector3.zero);
    }
}

public delegate void UGUIJoyStickHandle(Vector3 dir);
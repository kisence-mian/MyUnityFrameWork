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
    void Start()
    {
        //计算摇杆块的半径
        mRadius = ((transform as RectTransform).sizeDelta.x-content.sizeDelta.x) * 0.5f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canMove = true;
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector3 contentPostion = content.anchoredPosition + eventData.delta;
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
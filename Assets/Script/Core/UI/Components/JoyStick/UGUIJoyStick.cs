using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UGUIJoyStick : UIBase, IDragHandler, IEndDragHandler
{
    protected float mRadius;

    public RectTransform content;

    public UGUIJoyStickHandle onJoyStick;

    void Start()
    {
        //计算摇杆块的半径
        mRadius = ((transform as RectTransform).sizeDelta.x-content.sizeDelta.x) * 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 contentPostion = content.anchoredPosition + eventData.delta;
        if (contentPostion.magnitude > mRadius)
        {
            contentPostion = contentPostion.normalized * mRadius;
        }

        content.anchoredPosition3D = contentPostion;

        Vector3 dir = new Vector3(contentPostion.x, 0, contentPostion.y);

        dir /= mRadius;

        onJoyStick(dir);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        content.anchoredPosition3D = Vector3.zero;
        onJoyStick(Vector3.zero);
    }

}

public delegate void UGUIJoyStickHandle(Vector3 dir);
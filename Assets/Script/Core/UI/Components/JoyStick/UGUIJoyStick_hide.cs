using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UGUIJoyStick_hide : UIBase, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    protected float mRadius;

    public RectTransform content;

    public RectTransform bg; //控制可拖拽范围的大小
    public UGUIJoyStickHandle onJoyStick;

    public bool canMove = true;

    //当前分辨率，与UImanager上的标准分辨率之间的换算比
    public float conversionX;
    public float conversionY;


    public GameObject rocker; //需要移动和隐藏的摇杆背景加摇杆
    private RectTransform rockerRectTran;
    Vector2 referenceResolution;
    void Start()
    {
        //计算摇杆块的半径
        mRadius =( bg.sizeDelta.x - content.sizeDelta.x) * 0.5f;
        referenceResolution = UIManager.UIManagerGo.GetComponent<CanvasScaler>().referenceResolution;
        conversionX = referenceResolution.x / Screen.width;
        conversionY = referenceResolution.y / Screen.height;
        rocker.SetActive(false);
        rockerRectTran = rocker.GetComponent<RectTransform>();
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
        onJoyStick(Vector3.zero);
        rocker.SetActive(false);
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

    //从屏幕坐标，换算到UI坐标
    public  Vector3 ScreenPosToUIPos(Vector2 screenPos)
    {
        Vector2 normalized = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);

        normalized = normalized * 2 - Vector2.one;

        Vector2 UIpos = new Vector2(normalized.x * referenceResolution.x * 0.5f, normalized.y * referenceResolution.y * 0.5f);

        return UIpos;
    }

    //鼠标按下时
    public void OnPointerDown(PointerEventData eventData)
    {
        rocker.SetActive(true);
        rockerRectTran.localPosition = ScreenPosToUIPos(eventData.position); ;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{

    #region 重载方法

    //当UI第一次打开时调用OnInit方法，调用时机在OnOpen之前
    public virtual void OnInit()
    {

    }

    public virtual void OnDestroy()
    {

    }

    #endregion

    #region 继承方法
    private int m_UIID = -1;

    public int UIID
    {
        get { return m_UIID; }
        //set { m_UIID = value; }
    }

    public string UIEventKey
    {
        get { return name + m_UIID; }
        //set { m_UIID = value; }
    }
    public void Init(int id)
    {
        m_UIID = id;
        CreateObjectTable();
        OnInit();
    }

    public void Destroy()
    {
        OnDestroy();
    }

    #region 获取对象

    public List<GameObject> m_objectList = new List<GameObject>();

    //生成对象表，便于快速获取对象，并忽略层级
    void CreateObjectTable()
    {
        m_objects = new Dictionary<string, GameObject>();

        for (int i = 0; i < m_objectList.Count; i++)
        {
            if (m_objectList[i] != null)
            {
                m_objects.Add(m_objectList[i].name, m_objectList[i]);
            }
            else
            {
                Debug.LogWarning(name + " m_objectList[" + i + "] is Null !");
            }
        }
    }

    Dictionary<string, GameObject> m_objects;
    Dictionary<string, Image> m_images = new Dictionary<string, Image>();
    Dictionary<string, Text> m_texts = new Dictionary<string, Text>();
    Dictionary<string, Button> m_buttons = new Dictionary<string, Button>();
    Dictionary<string, ScrollRect> m_scrollRects = new Dictionary<string, ScrollRect>();
    Dictionary<string, ReusingScrollRect> m_reusingScrollRects = new Dictionary<string, ReusingScrollRect>();
    Dictionary<string, RawImage> m_rawImages = new Dictionary<string, RawImage>();
    Dictionary<string, RectTransform> m_rectTransforms = new Dictionary<string, RectTransform>();
    Dictionary<string, InputField> m_inputFields = new Dictionary<string, InputField>();

    Dictionary<string, UGUIJoyStick> m_joySticks = new Dictionary<string, UGUIJoyStick>();

    public GameObject GetGameObject(string name)
    {
        if (m_objects == null)
        {
            CreateObjectTable();
        }

        if (m_objects.ContainsKey(name))
        {
            return m_objects[name];
        }
        else
        {
            throw new Exception("UIWindowBase GetGameObject error: dont find ->" + name + "<-");
        }
    }

    public RectTransform GetRectTransform(string name)
    {
        if (m_rectTransforms.ContainsKey(name))
        {
            return m_rectTransforms[name];
        }

        RectTransform tmp = GetGameObject(name).GetComponent<RectTransform>();
        m_rectTransforms.Add(name, tmp);
        return tmp;
    }

    public Image GetImage(string name)
    {
        if (m_images.ContainsKey(name))
        {
            return m_images[name];
        }

        Image tmp = GetGameObject(name).GetComponent<Image>();
        m_images.Add(name, tmp);
        return tmp;
    }

    public Text GetText(string name)
    {
        if (m_texts.ContainsKey(name))
        {
            return m_texts[name];
        }

        Text tmp = GetGameObject(name).GetComponent<Text>();
        m_texts.Add(name, tmp);
        return tmp;
    }

    public Button GetButton(string name)
    {
        if (m_buttons.ContainsKey(name))
        {
            return m_buttons[name];
        }

        Button tmp = GetGameObject(name).GetComponent<Button>();
        m_buttons.Add(name, tmp);
        return tmp;
    }

    public InputField GetInputField(string name)
    {
        if (m_inputFields.ContainsKey(name))
        {
            return m_inputFields[name];
        }

        InputField tmp = GetGameObject(name).GetComponent<InputField>();
        m_inputFields.Add(name, tmp);
        return tmp;
    }

    public ScrollRect GetScrollRect(string name)
    {
        if (m_scrollRects.ContainsKey(name))
        {
            return m_scrollRects[name];
        }

        ScrollRect tmp = GetGameObject(name).GetComponent<ScrollRect>();
        m_scrollRects.Add(name, tmp);
        return tmp;
    }



    public RawImage GetRawImage(string name)
    {
        if (m_rawImages.ContainsKey(name))
        {
            return m_rawImages[name];
        }

        RawImage tmp = GetGameObject(name).GetComponent<RawImage>();
        m_rawImages.Add(name, tmp);
        return tmp;
    }


    private RectTransform m_rectTransform;
    public RectTransform m_RectTransform
    {
        get
        {
            if (m_rectTransform == null)
            {
                m_rectTransform = GetComponent<RectTransform>();
            }

            return m_rectTransform;
        }
        set { m_rectTransform = value; }
    }

    #region 自定义组件

    public ReusingScrollRect GetReusingScrollRect(string name)
    {
        if (m_reusingScrollRects.ContainsKey(name))
        {
            return m_reusingScrollRects[name];
        }

        ReusingScrollRect tmp = GetGameObject(name).GetComponent<ReusingScrollRect>();
        m_reusingScrollRects.Add(name, tmp);
        return tmp;
    }

    public UGUIJoyStick GetJoyStick(string name)
    {
        if (m_joySticks.ContainsKey(name))
        {
            return m_joySticks[name];
        }

        UGUIJoyStick tmp = GetGameObject(name).GetComponent<UGUIJoyStick>();
        m_joySticks.Add(name, tmp);
        return tmp;
    }
    #endregion

    #endregion

    #region 注册监听

    protected List<Enum> m_EventNames = new List<Enum>();
    protected List<EventHandRegisterInfo> m_EventListeners = new List<EventHandRegisterInfo>();

    protected List<InputEventRegisterInfo<InputUIOnClickEvent>> m_OnClickEvents = new List<InputEventRegisterInfo<InputUIOnClickEvent>>();

    public virtual void RemoveAllListener()
    {
        for (int i = 0; i < m_OnClickEvents.Count; i++)
        {
            m_OnClickEvents[i].RemoveListener();
        }

        m_OnClickEvents.Clear();

        for (int i = 0; i < m_EventListeners.Count; i++)
        {
            m_EventListeners[i].RemoveListener();
        }

        m_EventListeners.Clear();
    }


    protected void AddOnClickListener(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
    {

        InputEventRegisterInfo<InputUIOnClickEvent> info = InputUIEventProxy.AddOnClickListener(GetButton(buttonName), UIEventKey, buttonName, parm, callback);
        m_OnClickEvents.Add(info);
    }

    protected void AddEventListener(Enum EventEnum,EventHandle handle)
    {
        EventHandRegisterInfo info = new EventHandRegisterInfo();
        info.m_EventKey = EventEnum;
        info.m_hande = handle;

        GlobalEvent.AddEvent(EventEnum, handle);

        m_EventListeners.Add(info);
    }

    #endregion

    #endregion
}

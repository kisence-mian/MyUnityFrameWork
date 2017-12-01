using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public Canvas m_canvas;

    #region 重载方法
    //当UI第一次打开时调用OnInit方法，调用时机在OnOpen之前
    public virtual void OnInit()
    {
    }

    public void DestroyUI()
    {
        ClearGuideModel();
        RemoveAllListener();
        CleanItem();
        OnUIDestroy();
    }

    protected virtual void OnUIDestroy()
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
        get { return UIName + m_UIID; }
        //set { m_UIID = value; }
    }

    string m_UIName = null;
    public string UIName
    {
        get
        {
            if (m_UIName == null)
            {
                m_UIName = name;
            }

            return m_UIName;
        }
        set
        {
            m_UIName = value;
        }
    }

    public void Init(int id)
    {
        m_UIID = id;
        m_canvas = GetComponent<Canvas>();
        m_UIName = null;
        CreateObjectTable();
        OnInit();
    }

    //public void Destroy()
    //{
    //    OnDestroy();
    //}

    #region 获取对象

    public List<GameObject> m_objectList = new List<GameObject>();

    //生成对象表，便于快速获取对象，并忽略层级
    void CreateObjectTable()
    {
        m_objects.Clear();

        m_images.Clear();
        m_Sprites.Clear();
        m_texts.Clear();
        m_textmeshs.Clear();
        m_buttons.Clear();
        m_scrollRects.Clear();
        m_reusingScrollRects.Clear();
        m_rawImages.Clear();
        m_rectTransforms.Clear();
        m_inputFields.Clear();
        m_Sliders.Clear();
        m_joySticks.Clear();
        m_joySticks_ro.Clear();
        m_longPressList.Clear();
        m_Canvas.Clear();

        for (int i = 0; i < m_objectList.Count; i++)
        {
            if (m_objectList[i] != null)
            {
                //Debug.Log("===>"+m_objectList[i].name);
                if (m_objects.ContainsKey(m_objectList[i].name))
                {
                    Debug.LogError("CreateObjectTable ContainsKey ->" + m_objectList[i].name + "<-");
                }
                else
                {
                    m_objects.Add(m_objectList[i].name, m_objectList[i]);
                }
            }
            else
            {
                Debug.LogWarning(name + " m_objectList[" + i + "] is Null !");
            }
        }
    }

    Dictionary<string, UIBase> m_uiBases = new Dictionary<string, UIBase>();

    Dictionary<string, GameObject> m_objects = new Dictionary<string, GameObject>();
    Dictionary<string, Image> m_images = new Dictionary<string, Image>();
    Dictionary<string, Sprite> m_Sprites = new Dictionary<string, Sprite>();
    Dictionary<string, Text> m_texts = new Dictionary<string, Text>();
    Dictionary<string, TextMesh> m_textmeshs = new Dictionary<string, TextMesh>();
    Dictionary<string, Button> m_buttons = new Dictionary<string, Button>();
    Dictionary<string, ScrollRect> m_scrollRects = new Dictionary<string, ScrollRect>();
    Dictionary<string, ReusingScrollRect> m_reusingScrollRects = new Dictionary<string, ReusingScrollRect>();
    Dictionary<string, RawImage> m_rawImages = new Dictionary<string, RawImage>();
    Dictionary<string, RectTransform> m_rectTransforms = new Dictionary<string, RectTransform>();
    Dictionary<string, InputField> m_inputFields = new Dictionary<string, InputField>();
    Dictionary<string, Slider> m_Sliders = new Dictionary<string, Slider>();
    Dictionary<string, Canvas> m_Canvas = new Dictionary<string, Canvas>();
    Dictionary<string, Toggle> m_Toggle = new Dictionary<string, Toggle>();

    Dictionary<string, UGUIJoyStick> m_joySticks = new Dictionary<string, UGUIJoyStick>();
    Dictionary<string, UGUIJoyStickBase> m_joySticks_ro = new Dictionary<string, UGUIJoyStickBase>();
    Dictionary<string, LongPressAcceptor> m_longPressList = new Dictionary<string, LongPressAcceptor>();
    Dictionary<string, DragAcceptor> m_dragList = new Dictionary<string, DragAcceptor>();

    public bool HaveObject(string name)
    {
        bool has = false;
        has = m_objects.ContainsKey(name);
        return has;
    }

    public GameObject GetGameObject(string name)
    {
        if (m_objects == null)
        {
            CreateObjectTable();
        }

        if (m_objects.ContainsKey(name))
        {
            GameObject go = m_objects[name];

            if (go == null)
            {
                throw new Exception("UIWindowBase GetGameObject error: " + UIName + " m_objects[" + name + "] is null !!");
            }

            return go;
        }
        else
        {
            throw new Exception("UIWindowBase GetGameObject error: " + UIName + " dont find ->" + name + "<-");
        }
    }

    public RectTransform GetRectTransform(string name)
    {
        if (m_rectTransforms.ContainsKey(name))
        {
            return m_rectTransforms[name];
        }

        RectTransform tmp = GetGameObject(name).GetComponent<RectTransform>();


        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetRectTransform ->" + name + "<- is Null !");
        }

        m_rectTransforms.Add(name, tmp);
        return tmp;
    }

    public UIBase GetUIBase(string name)
    {
        if (m_uiBases.ContainsKey(name))
        {
            return m_uiBases[name];
        }

        UIBase tmp = GetGameObject(name).GetComponent<UIBase>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetUIBase ->" + name + "<- is Null !");
        }

        m_uiBases.Add(name, tmp);
        return tmp;
    }
    public Sprite GetSprite(string name)
    {
        if (m_Sprites.ContainsKey(name))
        {
            return m_Sprites[name];
        }

        Sprite tmp = GetGameObject(name).GetComponent<Sprite>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetImage ->" + name + "<- is Null !");
        }

        m_Sprites.Add(name, tmp);
        return tmp;
    }
    public Image GetImage(string name)
    {
        if (m_images.ContainsKey(name))
        {
            return m_images[name];
        }

        Image tmp = GetGameObject(name).GetComponent<Image>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetImage ->" + name + "<- is Null !");
        }

        m_images.Add(name, tmp);
        return tmp;
    }
    public TextMesh GetTextMesh(string name)
    {
        if (m_textmeshs.ContainsKey(name))
        {
            return m_textmeshs[name];
        }

        TextMesh tmp = GetGameObject(name).GetComponent<TextMesh>();



        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetText ->" + name + "<- is Null !");
        }

        m_textmeshs.Add(name, tmp);
        return tmp;
    }
    public Text GetText(string name)
    {
        if (m_texts.ContainsKey(name))
        {
            return m_texts[name];
        }

        Text tmp = GetGameObject(name).GetComponent<Text>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetText ->" + name + "<- is Null !");
        }

        m_texts.Add(name, tmp);
        return tmp;
    }
    public Toggle GetToggle(string name)
    {
        if (m_Toggle .ContainsKey(name))
        {
            return m_Toggle[name];
        }

        Toggle tmp = GetGameObject(name).GetComponent<Toggle>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetText ->" + name + "<- is Null !");
        }

        m_Toggle.Add(name, tmp);
        return tmp;
    }

    public Button GetButton(string name)
    {
        if (m_buttons.ContainsKey(name))
        {
            return m_buttons[name];
        }

        Button tmp = GetGameObject(name).GetComponent<Button>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetButton ->" + name + "<- is Null !");
        }

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

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetInputField ->" + name + "<- is Null !");
        }

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

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetScrollRect ->" + name + "<- is Null !");
        }

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

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetRawImage ->" + name + "<- is Null !");
        }

        m_rawImages.Add(name, tmp);
        return tmp;
    }

    public Slider GetSlider(string name)
    {
        if (m_Sliders.ContainsKey(name))
        {
            return m_Sliders[name];
        }

        Slider tmp = GetGameObject(name).GetComponent<Slider>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetSlider ->" + name + "<- is Null !");
        }

        m_Sliders.Add(name, tmp);
        return tmp;
    }

    public Canvas GetCanvas(string name)
    {
        if (m_Canvas.ContainsKey(name))
        {
            return m_Canvas[name];
        }

        Canvas tmp = GetGameObject(name).GetComponent<Canvas>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetSlider ->" + name + "<- is Null !");
        }

        m_Canvas.Add(name, tmp);
        return tmp;
    }

    public Vector3 GetPosition(string name, bool islocal)
    {
        Vector3 tmp = Vector3.zero;
        GameObject go = GetGameObject(name);
        if (go != null)
        {
            if (islocal)
                tmp = GetGameObject(name).transform.localPosition;
            else
                tmp = GetGameObject(name).transform.position;
        }
        return tmp;
    }

    private RectTransform m_rectTransform;
    public RectTransform RectTransform
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

    public void SetSizeDelta(float w,float h)
    {
        RectTransform.sizeDelta = new Vector2(w,h);
    }

    #region 自定义组件

    public ReusingScrollRect GetReusingScrollRect(string name)
    {
        if (m_reusingScrollRects.ContainsKey(name))
        {
            return m_reusingScrollRects[name];
        }

        ReusingScrollRect tmp = GetGameObject(name).GetComponent<ReusingScrollRect>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetReusingScrollRect ->" + name + "<- is Null !");
        }

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

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetJoyStick ->" + name + "<- is Null !");
        }

        m_joySticks.Add(name, tmp);
        return tmp;
    }

    public UGUIJoyStickBase GetJoyStick_ro(string name)
    {
        if (m_joySticks_ro.ContainsKey(name))
        {
            return m_joySticks_ro[name];
        }

        UGUIJoyStickBase tmp = GetGameObject(name).GetComponent<UGUIJoyStickBase>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetJoyStick_ro ->" + name + "<- is Null !");
        }

        m_joySticks_ro.Add(name, tmp);
        return tmp;
    }


    public LongPressAcceptor GetLongPressComp(string name)
    {
        if (m_longPressList.ContainsKey(name))
        {
            return m_longPressList[name];
        }

        LongPressAcceptor tmp = GetGameObject(name).GetComponent<LongPressAcceptor>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetLongPressComp ->" + name + "<- is Null !");
        }

        m_longPressList.Add(name, tmp);
        return tmp;
    }

    public DragAcceptor GetDragComp(string name)
    {
        if (m_dragList.ContainsKey(name))
        {
            return m_dragList[name];
        }

        DragAcceptor tmp = GetGameObject(name).GetComponent<DragAcceptor>();

        if (tmp == null)
        {
            throw new Exception(m_EventNames + " GetDragComp ->" + name + "<- is Null !");
        }

        m_dragList.Add(name, tmp);
        return tmp;
    }
    #endregion

    #endregion

    #region 注册监听

    protected List<Enum> m_EventNames = new List<Enum>();
    protected List<EventHandRegisterInfo> m_EventListeners = new List<EventHandRegisterInfo>();

    protected List<InputEventRegisterInfo> m_OnClickEvents = new List<InputEventRegisterInfo>();
    protected List<InputEventRegisterInfo> m_LongPressEvents = new List<InputEventRegisterInfo>();
    protected List<InputEventRegisterInfo> m_DragEvents = new List<InputEventRegisterInfo>();
    protected List<InputEventRegisterInfo> m_BeginDragEvents = new List<InputEventRegisterInfo>();
    protected List<InputEventRegisterInfo> m_EndDragEvents = new List<InputEventRegisterInfo>();

    public virtual void RemoveAllListener()
    {
        for (int i = 0; i < m_EventListeners.Count; i++)
        {
            m_EventListeners[i].RemoveListener();
        }
        m_EventListeners.Clear();

        for (int i = 0; i < m_OnClickEvents.Count; i++)
        {
            m_OnClickEvents[i].RemoveListener(true);
        }
        m_OnClickEvents.Clear();

        for (int i = 0; i < m_LongPressEvents.Count; i++)
        {
            m_LongPressEvents[i].RemoveListener(true);
        }
        m_LongPressEvents.Clear();

        #region 拖动事件
        for (int i = 0; i < m_DragEvents.Count; i++)
        {
            m_DragEvents[i].RemoveListener(true);
        }
        m_DragEvents.Clear();

        for (int i = 0; i < m_BeginDragEvents.Count; i++)
        {
            m_BeginDragEvents[i].RemoveListener(true);
        }
        m_BeginDragEvents.Clear();

        for (int i = 0; i < m_EndDragEvents.Count; i++)
        {
            m_EndDragEvents[i].RemoveListener(true);
        }
        m_EndDragEvents.Clear();
    #endregion
    }

    #region 添加监听

    bool GetRegister(List<InputEventRegisterInfo> list, string eventKey)
    {
        int registerCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].eventKey == eventKey)
            {
                registerCount++;
            }
        }

        return registerCount == 0;
    }

    public void AddOnClickListener(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
    {
        InputButtonClickRegisterInfo info = InputUIEventProxy.GetOnClickListener(GetButton(buttonName), UIEventKey, buttonName, parm, callback);
        info.AddListener(GetRegister(m_OnClickEvents, info.eventKey));
        m_OnClickEvents.Add(info);
    }

    public void AddOnClickListenerByCreate(Button button, string compName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
    {
        InputButtonClickRegisterInfo info = InputUIEventProxy.GetOnClickListener(button, UIEventKey, compName, parm, callback);
        info.AddListener(GetRegister(m_OnClickEvents, info.eventKey));
        m_OnClickEvents.Add(info);
    }

    public void AddLongPressListener(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUILongPressEvent> info = InputUIEventProxy.GetLongPressListener(GetLongPressComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener(GetRegister(m_LongPressEvents, info.eventKey));
        m_LongPressEvents.Add(info);

    }

    public void AddDragListener(string compName, InputEventHandle<InputUIOnDragEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUIOnDragEvent> info = InputUIEventProxy.GetOnDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener(GetRegister(m_DragEvents, info.eventKey));
        m_DragEvents.Add(info);
    }

    public void AddBeginDragListener(string compName, InputEventHandle<InputUIOnBeginDragEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUIOnBeginDragEvent> info = InputUIEventProxy.GetOnBeginDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener(GetRegister(m_BeginDragEvents, info.eventKey));
        m_BeginDragEvents.Add(info);
    }

    public void AddEndDragListener(string compName, InputEventHandle<InputUIOnEndDragEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUIOnEndDragEvent> info = InputUIEventProxy.GetOnEndDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener(GetRegister(m_EndDragEvents, info.eventKey));
        m_EndDragEvents.Add(info);
    }

    public void AddEventListener(Enum EventEnum, EventHandle handle)
    {
        EventHandRegisterInfo info = new EventHandRegisterInfo();
        info.m_EventKey = EventEnum;
        info.m_hande = handle;

        GlobalEvent.AddEvent(EventEnum, handle);

        m_EventListeners.Add(info);
    }

    #endregion

    #region 移除监听

    //TODO 逐步添加所有的移除监听方法

    public InputButtonClickRegisterInfo GetClickRegisterInfo(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm )
    {
        string eventKey = InputUIOnClickEvent.GetEventKey(UIEventKey, buttonName, parm);
        for (int i = 0; i < m_OnClickEvents.Count; i++)
        {
            InputButtonClickRegisterInfo info = (InputButtonClickRegisterInfo)m_OnClickEvents[i];
            if (info.eventKey == eventKey
                && info.callBack == callback)
            {
                return info;
            }
        }

        throw new Exception("GetClickRegisterInfo Exception not find RegisterInfo by " + buttonName + " parm " + parm);
    }

    public void RemoveOnClickListener(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
    {
        InputButtonClickRegisterInfo info = GetClickRegisterInfo(buttonName, callback, parm);
        m_OnClickEvents.Remove(info);
        info.RemoveListener(GetRegister(m_OnClickEvents, info.eventKey));
    }

    #endregion

    #endregion

    #region 创建对象

    List<UIBase> m_ChildList = new List<UIBase>();
    int m_childUIIndex = 0;
    public UIBase CreateItem(string itemName, string prantName,bool isActive)
    {
        GameObject item = GameObjectManager.CreateGameObjectByPool(itemName, GetGameObject(prantName), isActive);

        item.transform.localScale = Vector3.one;
        UIBase UIItem = item.GetComponent<UIBase>();

        if (UIItem == null)
        {
            throw new Exception("CreateItem Error : ->" + itemName + "<- don't have UIBase Component!");
        }

        UIItem.Init(m_childUIIndex++);
        UIItem.UIName = UIEventKey + "_" + UIItem.UIName;

        m_ChildList.Add(UIItem);

        return UIItem;
    }

    public UIBase CreateItem(string itemName, string prantName)
    {
        return CreateItem(itemName, prantName, true);
    }


    public void DestroyItem(UIBase item)
    {
        DestroyItem(item, true);
    }

    public void DestroyItem(UIBase item, bool isActive)
    {
        if (m_ChildList.Contains(item))
        {
            m_ChildList.Remove(item);
            item.OnUIDestroy();
            GameObjectManager.DestroyGameObjectByPool(item.gameObject, isActive);
        }
    }

    public void DestroyItem(UIBase item,float t)
    {
        if (m_ChildList.Contains(item))
        {
            m_ChildList.Remove(item);
            item.OnUIDestroy();
            GameObjectManager.DestroyGameObjectByPool(item.gameObject,t);
        }
    }

    public void CleanItem()
    {
        CleanItem(true);
    }

    public void CleanItem(bool isActive)
    {
        for (int i = 0; i < m_ChildList.Count; i++)
        {
            m_ChildList[i].RemoveAllListener();
            m_ChildList[i].OnUIDestroy();
            GameObjectManager.DestroyGameObjectByPool(m_ChildList[i].gameObject, isActive);
        }

        m_ChildList.Clear();
        m_childUIIndex = 0;
    }

    public UIBase GetItem(string itemName)
    {
        for (int i = 0; i < m_ChildList.Count; i++)
        {
            if(m_ChildList[i].name == itemName)
            {
                return m_ChildList[i];
            }
        }

        throw new Exception(UIName + " GetItem Exception Dont find Item: " + itemName);
    }

    public UIBase GetItemByIndex(string itemName,int index)
    {
        for (int i = 0; i < m_ChildList.Count; i++)
        {
            if (m_ChildList[i].name == itemName)
            {
                //Debug.Log("GetItemByIndex " + index, m_ChildList[i]);

                index--;
                if(index == 0)
                {
                    return m_ChildList[i];
                }
            }
        }

        throw new Exception(UIName + " GetItem Exception Dont find Item: " + itemName);
    }

    public UIBase GetItemByKey(string uiEvenyKey)
    {
        for (int i = 0; i < m_ChildList.Count; i++)
        {
            if (m_ChildList[i].UIEventKey == uiEvenyKey)
            {
                return m_ChildList[i];
            }
        }

        throw new Exception(UIName + " GetItemByKey Exception Dont find Item: " + uiEvenyKey);
    }

    public bool GetItemIsExist(string itemName)
    {
        for (int i = 0; i < m_ChildList.Count; i++)
        {
            if (m_ChildList[i].name == itemName)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #endregion

    #region 赋值方法

    public void SetText(string TextID, string content)
    {
        GetText(TextID).text = content.Replace("//n", "/n");
    }

    public void SetImageColor(string ImageID, Color color)
    {
        GetImage(ImageID).color = color;
    }

    public void SetTextColor(string TextID,Color color)
    {
        GetText(TextID).color = color;
    }

    public void SetImageAlpha(string ImageID, float alpha)
    {
        Color col = GetImage(ImageID).color;
        col.a = alpha;
        GetImage(ImageID).color = col;
    }

    public void SetInputText(string TextID, string content)
    {
        GetInputField(TextID).text = content;
    }

    /// <summary>
    /// 不再建议使用
    /// </summary>
    [Obsolete]
    public void SetTextByLangeage(string textID, string contentID, params object[] objs)
    {
        GetText(textID).text = LanguageManager.GetContent(LanguageManager.c_defaultModuleKey, contentID, objs);
    }

    public void SetTextByLangeage(string textID, string moduleName, string contentID, params object[] objs)
    {
        GetText(textID).text = LanguageManager.GetContent(moduleName, contentID, objs);
    }

    public void SetSlider(string sliderID, float value)
    {
        GetSlider(sliderID).value = value;
    }

    public void SetActive(string gameObjectID, bool isShow)
    {
        GetGameObject(gameObjectID).SetActive(isShow);
    }
    /// <summary>
    /// Only Button
    /// </summary>
    public void SetEnabeled(string ID,bool enable)
    {
        GetButton(ID).enabled = enable;
    }

    public void SetRectWidth(string TextID, float value, float height)
    {
        GetRectTransform(TextID).sizeDelta = Vector2.right * -value * 2 + Vector2.up * height;
    }

    public void SetWidth(string TextID, float width, float height)
    {
        GetRectTransform(TextID).sizeDelta = Vector2.right * width + Vector2.up * height;
    }

    public void SetPosition(string TextID, float x, float y, float z, bool islocal)
    {
        if (islocal)
            GetRectTransform(TextID).localPosition = Vector3.right * x + Vector3.up * y + Vector3.forward * z;
        else
            GetRectTransform(TextID).position = Vector3.right * x + Vector3.up * y + Vector3.forward * z;

    }

    public void SetScale(string TextID, float x, float y, float z)
    {
        GetGameObject(TextID).transform.localScale = Vector3.right * x + Vector3.up * y + Vector3.forward * z;
    }

    public void SetMeshText(string TextID, string txt)
    {
        GetTextMesh(TextID).text = txt;
    }

    #endregion

    #region 新手引导使用

    protected List<GameObject> m_GuideList = new List<GameObject>();
    protected Dictionary<GameObject, GuideChangeData> m_CreateCanvasDict = new Dictionary<GameObject, GuideChangeData>(); //保存Canvas的创建状态

    public void SetGuideMode(string objName, int order = 1)
    {
        SetGuideMode(GetGameObject(objName), order);
    }

    public void SetItemGuideMode(string itemName, int order = 1)
    {
        SetGuideMode(GetItem(itemName).gameObject, order);
    }

    public void SetItemGuideModeByIndex(string itemName, int index ,int order = 1)
    {
        SetGuideMode(GetItemByIndex(itemName, index).gameObject, order);
    }

    public void SetSelfGuideMode(int order = 1)
    {
        SetGuideMode(gameObject,order);
    }

    public void SetGuideMode(GameObject go,int order = 1)
    {
        Canvas canvas = go.GetComponent<Canvas>();
        GraphicRaycaster graphic = go.GetComponent<GraphicRaycaster>();

        GuideChangeData status = new GuideChangeData();

        if(canvas == null)
        {
            canvas = go.AddComponent<Canvas>();

            status.isCreateCanvas = true;
        }

        if(graphic == null)
        {
            graphic = go.AddComponent<GraphicRaycaster>();

            status.isCreateGraphic = true;
        }

        status.OldOverrideSorting = canvas.overrideSorting;
        status.OldSortingOrder = canvas.sortingOrder;
        status.oldSortingLayerName = canvas.sortingLayerName;

        //如果检测到目标对象
        bool oldActive = go.activeSelf;
        if (!oldActive)
        {
            go.SetActive(true);
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
        canvas.sortingLayerName = "Guide";

        if (!oldActive)
        {
            go.SetActive(false);
        }

        if(!m_CreateCanvasDict.ContainsKey(go))
        {
            m_CreateCanvasDict.Add(go, status);
            m_GuideList.Add(go);
        }
        else
        {
            Debug.LogError("m_CreateCanvasDict " + go);
        }
    }

    public void CancelGuideModel(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("go is null");
            return;
        }

        Canvas canvas = go.GetComponent<Canvas>();
        GraphicRaycaster graphic = go.GetComponent<GraphicRaycaster>();

        if (m_CreateCanvasDict.ContainsKey(go))
        {
            GuideChangeData status = m_CreateCanvasDict[go];

            if (graphic != null && status.isCreateGraphic)
            {
                DestroyImmediate(graphic);
            }

            if (canvas != null && status.isCreateCanvas)
            {
                DestroyImmediate(canvas);
            }
            else
            {
                if (canvas != null)
                {
                    canvas.overrideSorting = status.OldOverrideSorting;
                    canvas.sortingOrder = status.OldSortingOrder;
                    canvas.sortingLayerName = status.oldSortingLayerName;
                }
            }
        }
        else
        {
            Debug.LogError("m_CreateCanvasDict.ContainsKey(go) is error");
        }
    }

    protected struct GuideChangeData
    {
        public bool isCreateCanvas;
        public bool isCreateGraphic;

        public string oldSortingLayerName;
        public int OldSortingOrder;
        public bool OldOverrideSorting;
    }

    public void ClearGuideModel()
    {
        for (int i = 0; i < m_GuideList.Count; i++)
        {
            CancelGuideModel(m_GuideList[i]);
        }

        for (int i = 0; i < m_ChildList.Count; i++)
        {
            m_ChildList[i].ClearGuideModel();
        }

        m_GuideList.Clear();
        m_CreateCanvasDict.Clear();
    }

    #endregion

    #region 工具方法

    [ContextMenu("ObjectList 去重")]
    public void ClearObject()
    {
        List<GameObject> ls = new List<GameObject>();
        int len = m_objectList.Count;
        for (int i = 0; i < len; i++)
        {
            GameObject go = m_objectList[i];
            if (go != null)
            {
                if (!ls.Contains(go)) ls.Add(go);
            }
        }

        ls.Sort((a,b) => {
            return a.name.CompareTo(b.name);
        });

        m_objectList = ls;
    }

    #endregion
}

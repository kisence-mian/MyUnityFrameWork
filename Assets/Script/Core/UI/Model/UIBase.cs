
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class UIBase : MonoBehaviour , UILifeCycleInterface
{
    [HideInInspector]
    public Canvas m_canvas;

    #region 重载方法
    //当UI第一次打开时调用OnInit方法，调用时机在OnOpen之前
    public virtual void OnInit()
    {
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
        get { return UIName + "@" + m_UIID; }
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

    public void Init(string UIEventKey, int id)
    {
        if(UIEventKey != null)
        {
            UIName = null;
            UIName = UIEventKey + "_" + UIName;
        }

        m_UIID = id;
        m_canvas = GetComponent<Canvas>();
        CreateObjectTable();
        OnInit();
    }

    public void Dispose()
    {
        ClearGuideModel();
        RemoveAllListener();
        CleanAnim();
        CleanItem();
        CleanModelShowCameraList();

        ClearLoadSprite();
        try
        {
            OnUIDestroy();
        }
        catch(Exception e)
        {
            Debug.LogError("UIBase Dispose Exception -> UIEventKey: " + UIEventKey + " Exception: " + e.ToString());
        }

        DisposeLifeComponent();
    }

    #region 获取对象

    public List<GameObject> m_objectList = new List<GameObject>();

    //生成对象表，便于快速获取对象，并忽略层级
    void CreateObjectTable()
    {
        if (m_objects == null)
        {
            m_objects = new Dictionary<string, GameObject>();
        }
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

    Dictionary<string, GameObject> m_objects = null;
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

    public bool GetHasGameObject(string name)
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
                return false;
            }

            return true;
        }
        else
        {
            return false;
        }
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
        if (m_Toggle.ContainsKey(name))
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

    public void SetSizeDelta(float w, float h)
    {
        RectTransform.sizeDelta = new Vector2(w, h);
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
            m_OnClickEvents[i].RemoveListener();
        }
        m_OnClickEvents.Clear();

        for (int i = 0; i < m_LongPressEvents.Count; i++)
        {
            m_LongPressEvents[i].RemoveListener();
        }
        m_LongPressEvents.Clear();

        #region 拖动事件
        for (int i = 0; i < m_DragEvents.Count; i++)
        {
            m_DragEvents[i].RemoveListener();
        }
        m_DragEvents.Clear();

        for (int i = 0; i < m_BeginDragEvents.Count; i++)
        {
            m_BeginDragEvents[i].RemoveListener();
        }
        m_BeginDragEvents.Clear();

        for (int i = 0; i < m_EndDragEvents.Count; i++)
        {
            m_EndDragEvents[i].RemoveListener();
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
        info.AddListener();
        m_OnClickEvents.Add(info);
    }

    public void AddOnClickListenerByCreate(Button button, string compName, InputEventHandle<InputUIOnClickEvent> callback, string parm = null)
    {
        InputButtonClickRegisterInfo info = InputUIEventProxy.GetOnClickListener(button, UIEventKey, compName, parm, callback);
        info.AddListener();
        m_OnClickEvents.Add(info);
    }

    public void AddLongPressListener(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUILongPressEvent> info = InputUIEventProxy.GetLongPressListener(GetLongPressComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener();
        m_LongPressEvents.Add(info);
    }

    public void AddBeginDragListener(string compName, InputEventHandle<InputUIOnBeginDragEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUIOnBeginDragEvent> info = InputUIEventProxy.GetOnBeginDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener();
        m_BeginDragEvents.Add(info);
    }

    public void AddEndDragListener(string compName, InputEventHandle<InputUIOnEndDragEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUIOnEndDragEvent> info = InputUIEventProxy.GetOnEndDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener();
        m_EndDragEvents.Add(info);
    }

    public void AddOnDragListener(string compName, InputEventHandle<InputUIOnDragEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUIOnDragEvent> info = InputUIEventProxy.GetOnDragListener(GetDragComp(compName), UIEventKey, compName, parm, callback);
        info.AddListener();
        m_DragEvents.Add(info);
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

    public InputButtonClickRegisterInfo GetClickRegisterInfo(string buttonName, InputEventHandle<InputUIOnClickEvent> callback, string parm)
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
        info.RemoveListener();
    }

    public void RemoveLongPressListener(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm = null)
    {
        InputEventRegisterInfo<InputUILongPressEvent> info = GetLongPressRegisterInfo(compName, callback, parm);
        m_LongPressEvents.Remove(info);
        info.RemoveListener();
    }

    public InputEventRegisterInfo<InputUILongPressEvent> GetLongPressRegisterInfo(string compName, InputEventHandle<InputUILongPressEvent> callback, string parm)
    {
        string eventKey = InputUILongPressEvent.GetEventKey(UIName, compName, parm);
        for (int i = 0; i < m_LongPressEvents.Count; i++)
        {
            InputEventRegisterInfo<InputUILongPressEvent> info = (InputEventRegisterInfo<InputUILongPressEvent>)m_LongPressEvents[i];
            if (info.eventKey == eventKey
                && info.callBack == callback)
            {
                return info;
            }
        }

        throw new Exception("GetLongPressRegisterInfo Exception not find RegisterInfo by " + compName + " parm " + parm);
    }

    #endregion

    #endregion

    #region 创建对象

   protected List<UIBase> m_ChildList = new List<UIBase>();
    int m_childUIIndex = 0;
    public UIBase CreateItem(string itemName, GameObject parent, bool isActive)
    {
        GameObject item = GameObjectManager.CreateGameObjectByPool(itemName, parent, isActive);
        return SetItem(item);
    }
    public UIBase CreateItem(string itemName, string prantName, bool isActive)
    {
        GameObject parent = GetGameObject(prantName);
        return CreateItem(itemName,parent,isActive);
    }
    public UIBase CreateItem(GameObject itemObj, GameObject parent, bool isActive)
    {
        GameObject item = GameObjectManager.CreateGameObjectByPool(itemObj, parent, isActive);
        return SetItem(item);
    }
    public UIBase CreateItem(string itemName, string prantName)
    {
        return CreateItem(itemName, prantName, true);
    }
    private UIBase SetItem(GameObject item)
    {
        item.transform.localScale = Vector3.one;
        item.transform.localPosition = Vector3.zero;
        UIBase UIItem = item.GetComponent<UIBase>();

        if (UIItem == null)
        {
            throw new Exception("CreateItem Error : ->" + item.name + "<- don't have UIBase Component!");
        }

        UIItem.Init(UIEventKey, m_childUIIndex++);
        UIItem.UIName = UIEventKey + "_" + UIItem.UIName;

        m_ChildList.Add(UIItem);

        return UIItem;
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
            item.Dispose();
            GameObjectManager.DestroyGameObjectByPool(item.gameObject, isActive);
        }
    }

    public void DestroyItem(UIBase item, float t)
    {
        if (m_ChildList.Contains(item))
        {
            m_ChildList.Remove(item);
            item.Dispose();
            GameObjectManager.DestroyGameObjectByPool(item.gameObject, t);
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
            try
            {
                m_ChildList[i].Dispose();
                GameObjectManager.DestroyGameObjectByPool(m_ChildList[i].gameObject, isActive);
            }
            catch (Exception e)
            {
                Debug.LogError("CleanItem Error! UIName " + UIName + " Exception :" + e);
            }
        }

        m_ChildList.Clear();
        m_childUIIndex = 0;
    }
  

    public UIBase GetItemByIndex(string itemName, int index)
    {
        for (int i = 0; i < m_ChildList.Count; i++)
        {
            if (m_ChildList[i].name == itemName)
            {
                //Debug.Log("GetItemByIndex " + index, m_ChildList[i]);

                index--;
                if (index == 0)
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
        if (string.IsNullOrEmpty(content))
        {
            GetText(TextID).text = content;
        }
        else
        {
            GetText(TextID).text = content.Replace("\\n", "\n");
        }
    }

    public void SetImageColor(string ImageID, Color color)
    {
        GetImage(ImageID).color = color;
    }

    public void SetImageFillAmount(string ImageID, float value)
    {
        GetImage(ImageID).fillAmount = value;
    }

    public void SetTextColor(string TextID, Color color)
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
    public void SetTextByLanguage(string textID, string contentID, params object[] objs)
    {
        GetText(textID).text = LanguageManager.GetContent(LanguageManager.c_defaultModuleKey, contentID, objs);
    }

    void SetTextStyle(string textID, string work)
    {

    }

    public void SetTextByLangeage(string textID, string moduleName, string contentID, params object[] objs)
    {
        GetText(textID).text = LanguageManager.GetContent(moduleName, contentID, objs);
    }

    public void SetTextByLanguagePath(string textID, string languagePath, params object[] objs)
    {
        GetText(textID).text = LanguageManager.GetContentByKey(languagePath, objs);
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
    public void SetEnabeled(string ID, bool enable)
    {
        GetButton(ID).enabled = enable;
    }
    /// <summary>
    /// Only Button
    /// </summary>
    public void SetButtonInteractable(string ID, bool enable)
    {
        GetButton(ID).interactable = enable;
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

    public void SetAnchoredPosition(string ID,float x, float y)
    {
        GetRectTransform(ID).anchoredPosition = Vector2.right * x + Vector2.up * y;
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

    #region 动态加载Sprite赋值
    private Dictionary<string, int> loadSpriteNames = new Dictionary<string, int>();
    public void SetImageSprite(Image img, string name, bool is_nativesize = false)
    {
        if(ResourcesConfigManager.GetIsExitRes(name))
        {
            UGUITool.SetImageSprite(img, name, is_nativesize);
            if (!loadSpriteNames.ContainsKey(name))
                loadSpriteNames.Add(name, 1);
            else
                loadSpriteNames[name]++;
            //if (name == "CLT_border_TagBg_Hunter")
            //    Debug.Log("UIBase 加载图片：" + name + " ==>" + loadSpriteNames[name]);
        }
        else
        {
            Debug.LogError("SetImageSprite 资源不存在! ->" + name + "<-");
        }
    }

    private void ClearLoadSprite()
    {
        //Debug.Log("===>> ClearLoadSprite");
        foreach (var item in loadSpriteNames)
        {
            int num = item.Value;
            //if (item.Key == "CLT_border_TagBg_Hunter")
            //    Debug.Log("UIBase 回收图片：" + item.Key + ":" + num);
            ResourceManager.DestoryAssetsCounter(item.Key,num);
            
           
        }
        loadSpriteNames.Clear();
    }
    #endregion

    #region RawImageCamera

    List<UIModelShowTool.UIModelShowData> modelList = new List<UIModelShowTool.UIModelShowData>();

    public UIModelShowTool.UIModelShowData SetRawImageByModelShowCamera(string rawimageName, string modelName,
        string layerName = null,
        bool? orthographic = null,
        float? orthographicSize = null,
        Color? backgroundColor = null,
        Vector3? localPosition = null ,
        Vector3? localScale = null,
        Vector3? eulerAngles = null ,
        Vector3? texSize = null,
        float? nearClippingPlane = null,
        float? farClippingPlane = null)
    {
        var model = CreateModelShow(modelName, layerName, orthographic, orthographicSize, backgroundColor, localPosition, localScale, eulerAngles, texSize, nearClippingPlane, farClippingPlane);

        GetRawImage(rawimageName).texture = model.renderTexture;

        return model;
    }

    public void CleanModelShowCameraList()
    {
        for (int i = 0; i < modelList.Count; i++)
        {
            UIModelShowTool.DisposeModelShow(modelList[i]);
        }

        modelList.Clear();
    }

    public UIModelShowTool.UIModelShowData CreateModelShow(string modelName,
        string layerName = null,
        bool? orthographic = null,
        float? orthographicSize = null,
        Color? backgroundColor = null,
        Vector3? localPosition = null,
        Vector3? localScale = null,
        Vector3? eulerAngles = null,
        Vector3? texSize = null,
        float? nearClippingPlane = null,
        float? farClippingPlane = null)
    {
        var model = UIModelShowTool.CreateModelData(modelName, layerName, orthographic, orthographicSize, backgroundColor, localPosition, eulerAngles, localScale, texSize, nearClippingPlane, farClippingPlane);
        modelList.Add(model);

        return model;
    }

    public void RemoveModelShowCamera(UIModelShowTool.UIModelShowData data)
    {
        modelList.Remove(data);
        UIModelShowTool.DisposeModelShow(data);
    }

    #endregion

    #region 生命周期管理 

    protected List<UILifeCycleInterface> m_lifeComponent = new List<UILifeCycleInterface>();

    public void AddLifeCycleComponent(UILifeCycleInterface comp)
    {
        comp.Init(UIEventKey, m_lifeComponent.Count);
        m_lifeComponent.Add(comp);
    }

    void DisposeLifeComponent()
    {
        for (int i = 0; i < m_lifeComponent.Count; i++)
        {
            try
            {
                m_lifeComponent[i].Dispose();
            }
            catch( Exception e)
            {
                Debug.LogError("UIBase DisposeLifeComponent Exception -> UIEventKey: " + UIEventKey + " Exception: " + e.ToString());
            }

        }

        m_lifeComponent.Clear();
    }

    #endregion

    #region 新手引导使用


    protected Dictionary<GameObject, GuideHeightLightComponent> m_CreateCanvasDict = new Dictionary<GameObject, GuideHeightLightComponent>(); //保存Canvas的创建状态

    public List<GameObject> GetHeightLightObjects()
    {
        return new List<GameObject>(m_CreateCanvasDict.Keys);
    }
    public void SetGuideMode(string objName, int order = 1)
    {
        SetGuideMode(GetGuideFixGameObject(objName), order);
    }
    /// <summary>
    /// 获取新手引导的固定GameObject（当找固定Item里子节点时使用格式 PetItem1.Use）
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject GetGuideFixGameObject(string objName)
    {
        GameObject obj = null;
        if (objName.Contains("."))
        {
            string[] names = objName.Split('.');
            UIBase item = GetGameObject(names[0]).GetComponent<UIBase>();
            for (int i = 1; i < names.Length; i++)
            {
                string temp = names[i];
                GameObject tempObj = item.GetGameObject(temp);
                if (i == names.Length - 1)
                    obj = tempObj;
                else
                item = tempObj.GetComponent<UIBase>();
            }
        }
        else
        {
            obj = GetGameObject(objName);
        }
        return obj;
    }
    /// <summary>
    /// 新手引导获得动态创建Item  格式为：PetItem[0].Use（PetItem的Item上挂有UIBase脚本， [0] 该名字的第几个Item，Use：拖到PetItem上的GameObject），当index小于0，则表示动态创建的列表List.Count-index(如-1：表示List.Count-1)
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public GameObject GetGuideDynamicCreateItem(string itemName)
    {
        if(string.IsNullOrEmpty(itemName))
        {
            Debug.LogError("GetGuideDynamicCreateItem itemName is Null!! ");
            return null;
        }
        string firstName = "";
        string[] strArr = itemName.Split('.');
        Debug.Log("GetGuideDynamicCreateItem.itemName :" + itemName);
        string childName = "";
        GameObject obj = null;
        if (strArr.Length > 0)
        {
            UIBase uIBase = null;
            firstName = strArr[0];


            int index = int.Parse(firstName.SplitExtend("[", "]")[0]);
            if (index < 0)
            {
                index = m_ChildList.Count + index;
            }
            int tempIndex0 = firstName.IndexOf("[");
            firstName = firstName.Replace(firstName.Substring(tempIndex0), "");
            Debug.Log("UIBase : Index :" + index + "  firstName :" + firstName + " m_ChildList:"+ m_ChildList.Count);
            int tempIndex = 0;
            for (int i = 0; i < m_ChildList.Count; i++)
            {
                UIBase cItem = m_ChildList[i];
                Debug.Log("Item:" + cItem);
                if (cItem.name == firstName)
                {
                    
                    if (index == tempIndex)
                    {
                        uIBase = cItem;
                        obj = uIBase.gameObject;
                        break;
                    }
                    tempIndex++;
                }
            }

            if (strArr.Length > 1)
            {
                childName = strArr[1];
                Debug.Log("childName:" + childName);
                if (childName.Contains("["))
                {
                    childName = itemName.Replace(strArr[0] + ".", "");
                    Debug.Log("childName:" + childName);
                    obj = uIBase.GetGuideDynamicCreateItem(childName);
                }
                else
                {
                  string afterNames=  itemName.Replace(strArr[0] + ".", "");
                    strArr = afterNames.Split('.');
                    Debug.Log("afterNames :" + afterNames + "  UIBase:" + GetType().Name);
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        string findName = strArr[i];
                        obj = uIBase.GetGameObject(findName);
                        if (i < strArr[i].Length - 1)
                        {
                            uIBase = obj.GetComponent<UIBase>();
                        }
                    }
                       
                }
            }
        }

        if(obj == null)
        {
            Debug.LogError("GetGuideDynamicCreateItem error :UIEventKey " + UIEventKey + "itemName " + itemName);
        }

        return obj;
    }
    public void SetItemGuideMode(string itemName, int order = 1)
    {
        SetGuideMode(GetGuideDynamicCreateItem(itemName), order);
    }

    public void SetItemGuideModeByIndex(string itemName, int index, int order = 1)
    {
        SetGuideMode(GetItemByIndex(itemName, index).gameObject, order);
    }

    public void SetSelfGuideMode(int order = 1)
    {
        SetGuideMode(gameObject, order);
    }

    public GuideHeightLightComponent SetGuideMode(GameObject go, int order = 1)
    {
        GuideHeightLightComponent guideHeightLight = null;
        if (!m_CreateCanvasDict.ContainsKey(go))
        {
             guideHeightLight = go.AddComponent<GuideHeightLightComponent>();
            guideHeightLight.order = order;
            m_CreateCanvasDict.Add(go, guideHeightLight);
        }

        return guideHeightLight;
    }

    public void CancelGuideModel(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("go is null");
            return;
        }

        if (m_CreateCanvasDict.ContainsKey(go))
        {
            GuideHeightLightComponent guideHeightLight = m_CreateCanvasDict[go];
            guideHeightLight.ClearGuide();
            Destroy(guideHeightLight);
            m_CreateCanvasDict.Remove(go);
            //Debug.Log("ClearGuide______________");
        }
    }

 

    public void ClearGuideModel()
    {
        List<GameObject> m_GuideList = new List<GameObject>(m_CreateCanvasDict.Keys);
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

    #region 动画管理

    List<AnimData> animDataList = new List<AnimData>();

    public void AddAnimData(AnimData animData)
    {
        animDataList.Add(animData);
    }

    public void CleanAnim()
    {
        for (int i = 0; i < animDataList.Count; i++)
        {
            AnimSystem.StopAnim(animDataList[i]);
        }
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

        ls.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });

        m_objectList = ls;
    }


    //将世界坐标转换为 UI 坐标系中的位置
    public Vector3 WorldPosToUIPos(Vector3 worldPos, string cameraKey)
    {
        Vector3 scale = UIManager.UILayerManager.GetUICameraDataByKey(cameraKey).m_root.GetComponent<RectTransform>().localScale;
        Vector3 UIPos = new Vector3(worldPos.x / scale.x, worldPos.y / scale.y, worldPos.z / scale.z);
        return UIPos;
    }

    //将 UI 坐标系中的位置 转换为 世界坐标
    public Vector3 UIPosToWorldPos(Vector3 UIPos, string cameraKey)
    {
        Vector3 scale = UIManager.UILayerManager.GetUICameraDataByKey(cameraKey).m_root.GetComponent<RectTransform>().localScale;
        Vector3 worldPos = new Vector3(UIPos.x * scale.x, UIPos.y * scale.y, UIPos.z * scale.z);
        return worldPos;
    }


    #endregion
}

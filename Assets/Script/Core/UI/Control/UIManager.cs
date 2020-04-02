using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UIStackManager))]
[RequireComponent(typeof(UILayerManager))]
[RequireComponent(typeof(UIAnimManager))]
public class UIManager : MonoBehaviour
{
    private static GameObject s_UIManagerGo;
    private static UILayerManager s_UILayerManager; //UI层级管理器
    private static UIAnimManager s_UIAnimManager;   //UI动画管理器
    private static UIStackManager s_UIStackManager; //UI栈管理器

    private static EventSystem s_EventSystem;

    static public Dictionary<string, List<UIWindowBase>> s_UIs     = new Dictionary<string, List<UIWindowBase>>(); //打开的UI
    static public Dictionary<string, List<UIWindowBase>> s_hideUIs = new Dictionary<string, List<UIWindowBase>>(); //隐藏的UI

    #region 初始化

    static bool isInit;

    public static void Init()
    {
        if(!isInit)
        {
            isInit = true;

            GameObject instance = GameObject.Find("UIManager");

            if (instance == null)
            {
                instance = GameObjectManager.CreateGameObjectByPool("UIManager");
            }

            UIManagerGo = instance;

            s_UILayerManager = instance.GetComponent<UILayerManager>();
            s_UIAnimManager = instance.GetComponent<UIAnimManager>();
            s_UIStackManager = instance.GetComponent<UIStackManager>();
            s_EventSystem = instance.GetComponentInChildren<EventSystem>();

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(instance);
            }
        }
    }

    ///异步加载UIMnager
    //public static void InitAsync()
    //{
    //    GameObject instance = GameObject.Find("UIManager");

    //    if (instance == null)
    //    {
    //        GameObjectManager.CreateGameObjectByPoolAsync("UIManager",(obj)=> {
    //            SetUIManager(obj);
    //        });
    //    }
    //    else
    //    {
    //        SetUIManager(instance);
    //    }
    //}

    static void SetUIManager(GameObject instance)
    {
        UIManagerGo = instance;

        UILayerManager = instance.GetComponent<UILayerManager>();
        UIAnimManager = instance.GetComponent<UIAnimManager>();

        DontDestroyOnLoad(instance);
    }

    public static UILayerManager UILayerManager
    {
        get
        {
            if (s_UILayerManager == null)
            {
                Init();
            }
            return s_UILayerManager;
        }

        set
        {
            s_UILayerManager = value;
        }
    }

    public static UIAnimManager UIAnimManager
    {
        get
        {
            if (s_UILayerManager == null)
            {
                Init();
            }
            return s_UIAnimManager;
        }

        set
        {
            s_UIAnimManager = value;
        }
    }

    public static UIStackManager UIStackManager
    {
        get
        {
            if (s_UIStackManager == null)
            {
                Init();
            }
            return s_UIStackManager;
        }

        set
        {

            s_UIStackManager = value;
        }
    }

    public static EventSystem EventSystem
    {
        get
        {
            if (s_EventSystem == null)
            {
                Init();
            }
            return s_EventSystem;
        }

        set
        {
            s_EventSystem = value;
        }
    }

    public static GameObject UIManagerGo
    {
        get
        {
            if (s_UIManagerGo == null)
            {
                Init();
            }
            return s_UIManagerGo;
        }

        set
        {
            s_UIManagerGo = value;
        }
    }

    #endregion

    #region EventSystem

    public static void SetEventSystemEnable(bool enable)
    {
        if(EventSystem != null)
        {
            EventSystem.enabled = enable;
        }
        else
        {
            Debug.LogError("EventSystem.current is null !");
        }
    }

    #endregion

    #region UICamera

    public static string[] GetCameraNames()
    {
        string[] list = new string[UILayerManager.UICameraList.Count];

        for (int i = 0; i < UILayerManager.UICameraList.Count; i++)
        {
            list[i] = UILayerManager.UICameraList[i].m_key;
        }

        return list;
    }

    public static Camera GetCamera(string CameraKey = null)
    {
        var data = UILayerManager.GetUICameraDataByKey(CameraKey);
        return data.m_camera;
    }

    /// <summary>
    /// 将一个UI移动到另一个UICamera下
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="cameraKey"></param>
    public static void ChangeUICamera(UIWindowBase ui, string cameraKey)
    {
        UILayerManager.SetLayer(ui, cameraKey);
    }

    /// <summary>
    /// 将一个UI重新放回它原本的UICamera下
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="cameraKey"></param>
    public static void ResetUICamera(UIWindowBase ui)
    {
        UILayerManager.SetLayer(ui, ui.cameraKey);
    }

    #endregion

    #region UI的打开与关闭方法

    /// <summary>
    /// 创建UI,如果不打开则存放在Hide列表中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateUIWindow<T>() where T : UIWindowBase
    {
        return (T)CreateUIWindow(typeof(T).Name);
    }

    public static UIWindowBase CreateUIWindow(string UIName)
    {
        Debug.Log("CreateUIWindow " + UIName);

        GameObject UItmp = GameObjectManager.CreateGameObjectByPool(UIName, UIManagerGo);
        UIWindowBase UIWIndowBase = UItmp.GetComponent<UIWindowBase>();
        UISystemEvent.Dispatch(UIWIndowBase, UIEvent.OnInit);  //派发OnInit事件

        UIWIndowBase.windowStatus = UIWindowBase.WindowStatus.Create;

        try
        {
            UIWIndowBase.InitWindow(GetUIID(UIName));
        }
        catch(Exception e)
        {
            Debug.LogError(UIName + "OnInit Exception: " + e.ToString());}

        AddHideUI(UIWIndowBase);

        UILayerManager.SetLayer(UIWIndowBase);      //设置层级

        return UIWIndowBase;
    }

    /// <summary>
    /// 打开UI
    /// </summary>
    /// <param name="UIName">UI名</param>
    /// <param name="callback">动画播放完毕回调</param>
    /// <param name="objs">回调传参</param>`
    /// <returns>返回打开的UI</returns>
    public static UIWindowBase OpenUIWindow(string UIName, UICallBack callback = null, params object[] objs)  
    {
        UIWindowBase UIbase = GetHideUI(UIName);

        if (UIbase == null)
        {
            UIbase = CreateUIWindow(UIName);
        }

        RemoveHideUI(UIbase);
        AddUI(UIbase);

        UIStackManager.OnUIOpen(UIbase);
        UILayerManager.SetLayer(UIbase);      //设置层级

        UIbase.windowStatus = UIWindowBase.WindowStatus.OpenAnim;

        UISystemEvent.Dispatch(UIbase, UIEvent.OnOpen);  //派发OnOpen事件
        try
        {
            UIbase.OnOpen();
        }
        catch (Exception e)
        {
            Debug.LogError(UIName + " OnOpen Exception: " + e.ToString());
        }

        UISystemEvent.Dispatch(UIbase, UIEvent.OnOpened);  //派发OnOpened事件

        UIAnimManager.StartEnterAnim(UIbase, callback, objs); //播放动画
        return UIbase;
    }
    public static T OpenUIWindow<T>() where T : UIWindowBase
    {
        return (T)OpenUIWindow(typeof(T).Name);
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="UI">目标UI</param>
    /// <param name="isPlayAnim">是否播放关闭动画</param>
    /// <param name="callback">动画播放完毕回调</param>
    /// <param name="objs">回调传参</param>
    public static void CloseUIWindow(UIWindowBase UI,bool isPlayAnim = true ,UICallBack callback = null, params object[] objs)
    {
        RemoveUI(UI);        //移除UI引用
        UI.RemoveAllListener();

        if (isPlayAnim)
        {
            //动画播放完毕删除UI
            if (callback != null)
            {
                callback += CloseUIWindowCallBack;
            }
            else
            {
                callback = CloseUIWindowCallBack;
            }
            UI.windowStatus = UIWindowBase.WindowStatus.CloseAnim;
            UIAnimManager.StartExitAnim(UI, callback, objs);
        }
        else
        {
            CloseUIWindowCallBack(UI, objs);
        }
    }
    static void CloseUIWindowCallBack(UIWindowBase UI, params object[] objs)
    {
        UI.windowStatus = UIWindowBase.WindowStatus.Close;
        UISystemEvent.Dispatch(UI, UIEvent.OnClose);  //派发OnClose事件
        try
        {
            UI.OnClose();
        }
        catch (Exception e)
        {
            Debug.LogError(UI.UIName + " OnClose Exception: " + e.ToString());
        }

        UIStackManager.OnUIClose(UI);
        AddHideUI(UI);

        UISystemEvent.Dispatch(UI, UIEvent.OnClosed);  //派发OnClosed事件
    }
    public static void CloseUIWindow(string UIname, bool isPlayAnim = true, UICallBack callback = null, params object[] objs)
    {
        UIWindowBase ui = GetUI(UIname);

        if (ui == null)
        {
            Debug.LogError("CloseUIWindow Error UI ->" + UIname + "<-  not Exist!");
        }
        else
        {
            CloseUIWindow(GetUI(UIname), isPlayAnim, callback, objs);
        }
    }

    public static void CloseUIWindow<T>(bool isPlayAnim = true, UICallBack callback = null, params object[] objs) where T : UIWindowBase
    {
        CloseUIWindow(typeof(T).Name, isPlayAnim,callback, objs);
    }

    public static UIWindowBase ShowUI(string UIname)
    {
        UIWindowBase ui = GetUI(UIname);
        return ShowUI(ui);
    }

    public static UIWindowBase ShowUI(UIWindowBase ui)
    {
        ui.windowStatus = UIWindowBase.WindowStatus.Open;
        UISystemEvent.Dispatch(ui, UIEvent.OnShow);  //派发OnShow事件

        try
        {
            ui.Show();
            ui.OnShow();
        }
        catch (Exception e)
        {
            Debug.LogError(ui.UIName + " OnShow Exception: " + e.ToString());
        }

        return ui;
    }

    public static UIWindowBase HideUI(string UIname)
    {
        UIWindowBase ui = GetUI(UIname);
        return HideUI(ui);
    }

    public static UIWindowBase HideUI(UIWindowBase ui)
    {
        ui.windowStatus = UIWindowBase.WindowStatus.Hide;
        UISystemEvent.Dispatch(ui, UIEvent.OnHide);  //派发OnHide事件

        try
        {
            ui.Hide();
            ui.OnHide();
        }
        catch (Exception e)
        {
            Debug.LogError(ui.UIName + " OnShow Exception: " + e.ToString());
        }

        return ui;
    }

    public static void HideOtherUI(string UIName)
    {
        List<string> keys = new List<string>(s_UIs.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            List<UIWindowBase> list = s_UIs[keys[i]];
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].UIName != UIName)
                {
                    HideUI(list[j]);
                }
            }
        }
    }

    public static void ShowOtherUI(string UIName)
    {
        List<string> keys = new List<string>(s_UIs.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            List<UIWindowBase> list = s_UIs[keys[i]];
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].UIName != UIName)
                {
                    ShowUI(list[j]);
                }
            }
        }
    }

    /// <summary>
    /// 移除全部UI
    /// </summary>
    public static void CloseAllUI(bool isPlayerAnim = false)
    {
        List<string> keys = new List<string>(s_UIs.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            List<UIWindowBase> list = s_UIs[keys[i]];
            for(int j = 0;j<list.Count;j++)
            {
                CloseUIWindow(list[i], isPlayerAnim);
            }
        }
    }

    public static void CloseLastUI(UIType uiType = UIType.Normal)
    {
        UIStackManager.CloseLastUIWindow(uiType);
    }

#endregion

    #region UI的打开与关闭 异步方法

    public static void OpenUIAsync<T>( UICallBack callback, params object[] objs) where T : UIWindowBase
    {
        string UIName = typeof(T).Name;
        OpenUIAsync(UIName,callback,objs);
    }

    public static void OpenUIAsync(string UIName, UICallBack callback, params object[] objs)
    {
        ResourceManager.LoadAsync(UIName, (resObject) =>
        {
             OpenUIWindow(UIName, callback, objs);
        });
    }

#endregion

    #region UI内存管理

    public static void DestroyUI(UIWindowBase UI)
    {
        Debug.Log("UIManager DestroyUI " + UI.name);

        if (GetIsExitsHide(UI))
        {
            RemoveHideUI(UI);
        }
        else if(GetIsExits(UI))
        {
            RemoveUI(UI);   
        }

        UISystemEvent.Dispatch(UI, UIEvent.OnDestroy);  //派发OnDestroy事件
        try
        {
            UI.Dispose();
        }
        catch(Exception e)
        {
            Debug.LogError("OnDestroy :" + e.ToString());
        }
        GameObjectManager.DestroyGameObjectByPool(UI.gameObject);
    }

    public static void DestroyAllUI()
    {
        DestroyAllActiveUI();
        DestroyAllHideUI();
    }

#endregion

    #region 打开UI列表的管理

    /// <summary>
    /// 删除所有打开的UI
    /// </summary>
    public static void DestroyAllActiveUI()
    {
        foreach (List<UIWindowBase> uis in s_UIs.Values)
        {
            for (int i = 0; i < uis.Count; i++)
            {
                UISystemEvent.Dispatch(uis[i], UIEvent.OnDestroy);  //派发OnDestroy事件
                try
                {
                    uis[i].Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogError("OnDestroy :" + e.ToString());
                }
                GameObjectManager.DestroyGameObjectByPool(uis[i].gameObject);
            }
        }

        s_UIs.Clear();
    }

    public static T GetUI<T>() where T : UIWindowBase
    {
        return (T)GetUI(typeof(T).Name);
    }
    public static UIWindowBase GetUI(string UIname)
    {
        if (!s_UIs.ContainsKey(UIname))
        {
            //Debug.Log("!ContainsKey " + UIname);
            return null;
        }
        else
        {
            if (s_UIs[UIname].Count == 0)
            {
                //Debug.Log("s_UIs[UIname].Count == 0");
                return null;
            }
            else
            {
                //默认返回最后创建的那一个
                return s_UIs[UIname][s_UIs[UIname].Count - 1];
            }
        }
    }

    public static UIBase GetUIBaseByEventKey(string eventKey)
    {
        string UIkey = eventKey.Split('.')[0];
        string[] keyArray = UIkey.Split('_');

        string uiEventKey = "";

        UIBase uiTmp = null;
        for (int i = 0; i < keyArray.Length; i++)
        {
            if(i == 0)
            {
                uiEventKey = keyArray[0];
                uiTmp = GetUIWindowByEventKey(uiEventKey);
            }
            else
            {
                uiEventKey += "_" + keyArray[i];
                uiTmp = uiTmp.GetItemByKey(uiEventKey);
            }

            Debug.Log("uiEventKey " + uiEventKey);
        }

        return uiTmp;
    }

    static Regex uiKey = new Regex(@"(\S+)\d+");

    static UIWindowBase GetUIWindowByEventKey(string eventKey)
    {
        string UIname = uiKey.Match(eventKey).Groups[1].Value;

        if (!s_UIs.ContainsKey(UIname))
        {
            throw new Exception("UIManager: GetUIWindowByEventKey error dont find UI name: ->" + eventKey + "<-  " + UIname);
        }

        List<UIWindowBase> list = s_UIs[UIname];
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].UIEventKey == eventKey)
            {
                return list[i];
            }
        }

        throw new Exception("UIManager: GetUIWindowByEventKey error dont find UI name: ->" + eventKey + "<-  " + UIname);
    }

    static bool GetIsExits(UIWindowBase UI)
    {
        if (!s_UIs.ContainsKey(UI.name))
        {
            return false;
        }
        else
        {
            return s_UIs[UI.name].Contains(UI);
        }
    }

    static void AddUI(UIWindowBase UI)
    {
        if (!s_UIs.ContainsKey(UI.name))
        {
            s_UIs.Add(UI.name, new List<UIWindowBase>());
        }

        s_UIs[UI.name].Add(UI);

        UI.Show();
    }

    static void RemoveUI(UIWindowBase UI)
    {
        if (UI == null)
        {
            throw new Exception("UIManager: RemoveUI error UI is null: !");
        }

        if (!s_UIs.ContainsKey(UI.name))
        {
            throw new Exception("UIManager: RemoveUI error dont find UI name: ->" + UI.name + "<-  " + UI);
        }

        if (!s_UIs[UI.name].Contains(UI))
        {
            throw new Exception("UIManager: RemoveUI error dont find UI: ->" + UI.name + "<-  " + UI);
        }
        else
        {
            s_UIs[UI.name].Remove(UI);
        }
    }

    static int GetUIID(string UIname)
    {
        if (!s_UIs.ContainsKey(UIname))
        {
            return 0;
        }
        else
        {
            int id = s_UIs[UIname].Count;

            for (int i = 0; i < s_UIs[UIname].Count; i++)
			{
			    if(s_UIs[UIname][i].UIID == id)
                {
                    id++;
                    i = 0;
                }
			}

            return id;
        }
    }

    public static int GetNormalUICount()
    {
        return UIStackManager.m_normalStack.Count;
    }

#endregion

    #region 隐藏UI列表的管理

    /// <summary>
    /// 删除所有隐藏的UI
    /// </summary>
    public static void DestroyAllHideUI()
    {
        foreach (List<UIWindowBase> uis in s_hideUIs.Values)
        {
            for (int i = 0; i < uis.Count; i++)
            {
                UISystemEvent.Dispatch(uis[i], UIEvent.OnDestroy);  //派发OnDestroy事件
                try
                {
                    uis[i].Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogError("OnDestroy :" + e.ToString());
                }
                GameObjectManager.DestroyGameObjectByPool(uis[i].gameObject);
            }
        }

        s_hideUIs.Clear();
    }

    public static T GetHideUI<T>() where T:UIWindowBase
    {
        string UIname = typeof(T).Name;
        return (T)GetHideUI(UIname);
    }

    /// <summary>
    /// 获取一个隐藏的UI,如果有多个同名UI，则返回最后创建的那一个
    /// </summary>
    /// <param name="UIname">UI名</param>
    /// <returns></returns>
    public static UIWindowBase GetHideUI(string UIname)
    {
        if (!s_hideUIs.ContainsKey(UIname))
        {
            return null;
        }
        else
        {
            if (s_hideUIs[UIname].Count == 0)
            {
                return null;
            }
            else
            {
                UIWindowBase ui = s_hideUIs[UIname][s_hideUIs[UIname].Count - 1];
                //默认返回最后创建的那一个
                return ui;
            }
        }
    }

    static bool GetIsExitsHide(UIWindowBase UI)
    {
        if (!s_hideUIs.ContainsKey(UI.name))
        {
            return false;
        }
        else
        {
            return s_hideUIs[UI.name].Contains(UI);
        }
    }

    static void AddHideUI(UIWindowBase UI)
    {
        if (!s_hideUIs.ContainsKey(UI.name))
        {
            s_hideUIs.Add(UI.name, new List<UIWindowBase>());
        }

        s_hideUIs[UI.name].Add(UI);

        UI.Hide();
    }


    static void RemoveHideUI(UIWindowBase UI)
    {
        if (UI == null)
        {
            throw new Exception("UIManager: RemoveUI error l_UI is null: !");
        }

        if (!s_hideUIs.ContainsKey(UI.name))
        {
            throw new Exception("UIManager: RemoveUI error dont find: " + UI.name + "  " + UI);
        }

        if (!s_hideUIs[UI.name].Contains(UI))
        {
            throw new Exception("UIManager: RemoveUI error dont find: " + UI.name + "  " + UI);
        }
        else
        {
            s_hideUIs[UI.name].Remove(UI);
        }
    }

#endregion
}
    #region UI事件 代理 枚举

    /// <summary>
    /// UI回调
    /// </summary>
    /// <param name="objs"></param>
    public delegate void UICallBack(UIWindowBase UI, params object[] objs);
    public delegate void UIAnimCallBack(UIWindowBase UIbase, UICallBack callBack, params object[] objs);

    public enum UIType
    {
        GameUI=0,

        Fixed=1,
        Normal=2,
        TopBar=3,
        Upper = 4,
        PopUp =5,
     }

    public enum UIEvent
    {
        OnOpen,
        OnOpened,

        OnClose,
        OnClosed,

        OnHide,
        OnShow,

        OnInit,
        OnDestroy,

        OnRefresh,

        OnStartEnterAnim,
        OnCompleteEnterAnim,

        OnStartExitAnim,
        OnCompleteExitAnim,
    }
#endregion

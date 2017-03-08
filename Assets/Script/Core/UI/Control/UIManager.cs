using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UILayerManager))]
[RequireComponent(typeof(UIAnimManager))]
public class UIManager : MonoBehaviour
{
    public static GameObject s_UIManagerGo;
    public static UILayerManager s_UILayerManager; //UI层级管理器
    public static UIAnimManager s_UIAnimManager;   //UI动画管理器
    public static Camera s_UIcamera;               //UICamera

    static public Dictionary<string, List<UIWindowBase>> s_UIs     = new Dictionary<string, List<UIWindowBase>>(); //打开的UI
    static public Dictionary<string, List<UIWindowBase>> s_hideUIs = new Dictionary<string, List<UIWindowBase>>(); //隐藏的UI

    #region 初始化

    public static void Init()
    {
        GameObject l_instance = GameObject.Find("UIManager");

        if (l_instance == null)
        {
            l_instance = GameObjectManager.CreatGameObject("UIManager");
        }

        s_UIManagerGo = l_instance;

        s_UILayerManager = l_instance.GetComponent<UILayerManager>();
        s_UIAnimManager  = l_instance.GetComponent<UIAnimManager>();
        s_UIcamera       = l_instance.GetComponentInChildren<Camera>();

        DontDestroyOnLoad(l_instance);
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
        GameObject UItmp = GameObjectManager.CreatGameObject(UIName, s_UIManagerGo);
        UIWindowBase UIbase = UItmp.GetComponent<UIWindowBase>();
        UISystemEvent.Dispatch(UIbase, UIEvent.OnInit);  //派发OnInit事件
        try{
            UIbase.Init(GetUIID(UIName));
        }
        catch(Exception e)
        {
            Debug.LogError("OnInit Exception: " + e.ToString());}

        AddHideUI(UIbase);

        s_UILayerManager.SetLayer(UIbase);      //设置层级

        return UIbase;
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

        UISystemEvent.Dispatch(UIbase, UIEvent.OnOpen);  //派发OnOpen事件
        try{
            UIbase.OnOpen();
        }
        catch (Exception e)
        {
            Debug.LogError(UIName + " OnOpen Exception: " + e.ToString());
        }

        s_UILayerManager.SetLayer(UIbase);      //设置层级
        s_UIAnimManager.StartEnterAnim(UIbase, callback, objs); //播放动画
        return UIbase;
    }
    public static T OpenUIWindow<T>() where T : UIWindowBase
    {
        return (T)OpenUIWindow(typeof(T).Name);
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="l_UI">目标UI</param>
    /// <param name="isPlayAnim">是否播放关闭动画</param>
    /// <param name="l_callback">动画播放完毕回调</param>
    /// <param name="l_objs">回调传参</param>
    public static void CloseUIWindow(UIWindowBase l_UI,bool isPlayAnim = true ,UICallBack l_callback = null, params object[] l_objs)
    {
        RemoveUI(l_UI);        //移除UI引用
        l_UI.RemoveAllListener();

        if (isPlayAnim)
        {
            //动画播放完毕删除UI
            if (l_callback != null)
            {
                l_callback += CloseUIWindowCallBack;
            }
            else
            {
                l_callback = CloseUIWindowCallBack;
            }

            s_UIAnimManager.StartExitAnim(l_UI, l_callback, l_objs);
        }
        else
        {
            CloseUIWindowCallBack(l_UI, l_objs);
        }
    }
    static void CloseUIWindowCallBack(UIWindowBase l_UI, params object[] l_objs)
    {
        UISystemEvent.Dispatch(l_UI, UIEvent.OnClose);  //派发OnClose事件
        try
        {
            l_UI.OnClose();
        }
        catch (Exception e)
        {
            Debug.LogError(l_UI.UIName + " OnClose Exception: " + e.ToString());
        }

        AddHideUI(l_UI);
    }
    public static void CloseUIWindow(string l_UIname, bool isPlayAnim = true, UICallBack l_callback = null, params object[] l_objs)
    {
        UIWindowBase ui = GetUI(l_UIname);

        if (ui == null)
        {
            Debug.LogError("CloseUIWindow Error UI ->" + l_UIname + "<-  not Exist!");
        }
        else
        {
            CloseUIWindow(GetUI(l_UIname), isPlayAnim, l_callback, l_objs);
        }

    }
    public static void CloseUIWindow<T>(bool isPlayAnim = true, UICallBack l_callback = null, params object[] l_objs) where T : UIWindowBase
    {
        CloseUIWindow(typeof(T).Name, isPlayAnim,l_callback, l_objs);
    }

    public static UIWindowBase ShowUI(string UIname)
    {
        UIWindowBase ui = GetUI(UIname);
        return ShowUI(ui);
    }

    public static UIWindowBase ShowUI(UIWindowBase ui)
    {
        try
        {
            ui.gameObject.SetActive(true);
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
        UISystemEvent.Dispatch(ui, UIEvent.OnHide);  //派发OnShow事件

        try
        {
            ui.gameObject.SetActive(false);
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

    #endregion

    #region UI内存管理

    public static void DestroyUI(UIWindowBase l_UI)
    {
        if (GetIsExitsHide(l_UI))
        {
            RemoveHideUI(l_UI);
        }
        else if(GetIsExits(l_UI))
        {
            RemoveUI(l_UI);   
        }

        UISystemEvent.Dispatch(l_UI, UIEvent.OnDestroy);  //派发OnDestroy事件
        try
        {
            l_UI.DestroyUI();
        }
        catch(Exception e)
        {
            Debug.LogError("OnDestroy :" + e.ToString());
        }
        Destroy(l_UI.gameObject);
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
        foreach (List<UIWindowBase> l_uis in s_UIs.Values)
        {
            for (int i = 0; i < l_uis.Count; i++)
            {
                DestroyUI(l_uis[i]);
            }
        }
    }

    public static T GetUI<T>() where T : UIWindowBase
    {
        return (T)GetUI(typeof(T).Name);
    }
    public static UIWindowBase GetUI(string l_UIname)
    {
        if (!s_UIs.ContainsKey(l_UIname))
        {
            //Debug.Log("!ContainsKey " + l_UIname);
            return null;
        }
        else
        {
            if (s_UIs[l_UIname].Count == 0)
            {
                //Debug.Log("s_UIs[UIname].Count == 0");
                return null;
            }
            else
            {
                //默认返回最后创建的那一个
                return s_UIs[l_UIname][s_UIs[l_UIname].Count - 1];
            }
        }
    }

    static bool GetIsExits(UIWindowBase l_UI)
    {
        if (!s_UIs.ContainsKey(l_UI.name))
        {
            return false;
        }
        else
        {
            return s_UIs[l_UI.name].Contains(l_UI);
        }
    }

    static void AddUI(UIWindowBase l_UI)
    {
        if (!s_UIs.ContainsKey(l_UI.name))
        {
            s_UIs.Add(l_UI.name, new List<UIWindowBase>());
        }

        s_UIs[l_UI.name].Add(l_UI);

        l_UI.gameObject.SetActive(true);
    }

    static void RemoveUI(UIWindowBase l_UI)
    {
        if (l_UI == null)
        {
            throw new Exception("UIManager: RemoveUI error l_UI is null: !");
        }

        if (!s_UIs.ContainsKey(l_UI.name))
        {
            throw new Exception("UIManager: RemoveUI error dont find: " + l_UI.name + "  " + l_UI);
        }

        if (!s_UIs[l_UI.name].Contains(l_UI))
        {
            throw new Exception("UIManager: RemoveUI error dont find: " + l_UI.name + "  " + l_UI);
        }
        else
        {
            s_UIs[l_UI.name].Remove(l_UI);
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

    #endregion

    #region 隐藏UI列表的管理

    /// <summary>
    /// 删除所有隐藏的UI
    /// </summary>
    public static void DestroyAllHideUI()
    {
        foreach (List<UIWindowBase> l_uis in s_hideUIs.Values)
        {
            for (int i = 0; i < l_uis.Count; i++)
            {
                DestroyUI(l_uis[i]);
            }
        }
    }

    /// <summary>
    /// 获取一个隐藏的UI,如果有多个同名UI，则返回最后创建的那一个
    /// </summary>
    /// <param name="l_UIname">UI名</param>
    /// <returns></returns>
    public static UIWindowBase GetHideUI(string l_UIname)
    {
        if (!s_hideUIs.ContainsKey(l_UIname))
        {
            return null;
        }
        else
        {
            if (s_hideUIs[l_UIname].Count == 0)
            {
                return null;
            }
            else
            {
                UIWindowBase ui = s_hideUIs[l_UIname][s_hideUIs[l_UIname].Count - 1];
                //默认返回最后创建的那一个
                return ui;
            }
        }
    }

    static bool GetIsExitsHide(UIWindowBase l_UI)
    {
        if (!s_hideUIs.ContainsKey(l_UI.name))
        {
            return false;
        }
        else
        {
            return s_hideUIs[l_UI.name].Contains(l_UI);
        }
    }

    static void AddHideUI(UIWindowBase l_UI)
    {
        if (!s_hideUIs.ContainsKey(l_UI.name))
        {
            s_hideUIs.Add(l_UI.name, new List<UIWindowBase>());
        }

        s_hideUIs[l_UI.name].Add(l_UI);

        l_UI.gameObject.SetActive(false);
    }


    static void RemoveHideUI(UIWindowBase l_UI)
    {
        if (l_UI == null)
        {
            throw new Exception("UIManager: RemoveUI error l_UI is null: !");
        }

        if (!s_hideUIs.ContainsKey(l_UI.name))
        {
            throw new Exception("UIManager: RemoveUI error dont find: " + l_UI.name + "  " + l_UI);
        }

        if (!s_hideUIs[l_UI.name].Contains(l_UI))
        {
            throw new Exception("UIManager: RemoveUI error dont find: " + l_UI.name + "  " + l_UI);
        }
        else
        {
            s_hideUIs[l_UI.name].Remove(l_UI);
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
    public delegate void UIAnimCallBack(UIWindowBase l_UIbase, UICallBack callBack, params object[] objs);

    public enum UIType
    {
        GameUI,

        Fixed,
        Normal,
        TopBar,
        PopUp
    }

    public enum UIEvent
    {
        OnOpen,
        OnClose,
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

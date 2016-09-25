using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UILayerManager))]
[RequireComponent(typeof(UIAnimManager))]
public class UIManager : MonoBehaviour
{
    static UIManager s_instance;
    public static UILayerManager s_UILayerManager; //UI层级管理器
    public static UIAnimManager s_UIAnimManager;   //UI动画管理器

    static public Dictionary<string, List<UIWindowBase>> s_UIs     = new Dictionary<string, List<UIWindowBase>>(); //打开的UI
    static public Dictionary<string, List<UIWindowBase>> s_hideUIs = new Dictionary<string, List<UIWindowBase>>(); //隐藏的UI

    public static Camera s_UIcamera;

    #region 初始化

    public static UIManager s_Instance
    {
        get
        {
            if (s_instance == null)
            {
                Init();
            }
            return s_instance;
        }
    }

    public static void Init()
    {
        GameObject l_instance = GameObjectManager.CreatGameObject("UIManager");
        s_instance = l_instance.GetComponent<UIManager>();

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
    public static UIWindowBase CreateUIWindow<T>() where T : UIWindowBase
    {
        return CreateUIWindow(typeof(T).Name);
    }
    public static UIWindowBase CreateUIWindow(string l_UIname)
    {
        GameObject l_UItmp = GameObjectManager.CreatGameObject(l_UIname, s_Instance.gameObject);
        UIWindowBase l_UIbase = l_UItmp.GetComponent<UIWindowBase>();
        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnInit);  //派发OnInit事件
        l_UIbase.Init();

        AddHideUI(l_UIbase);

        return l_UIbase;
    }

    /// <summary>
    /// 打开UI
    /// </summary>
    /// <param name="l_UIname">UI名</param>
    /// <param name="l_callback">动画播放完毕回调</param>
    /// <param name="l_objs">回调传参</param>
    /// <returns>返回打开的UI</returns>
    public static UIWindowBase OpenUIWindow(string l_UIname, UICallBack l_callback = null, params object[] l_objs)
    {
        GameObject l_UItmp = GameObjectManager.CreatGameObject(l_UIname, s_Instance.gameObject);
        UIWindowBase l_UIbase = GetHideUI(l_UIname);

        if (l_UIbase == null)
        {
            l_UIbase = CreateUIWindow(l_UIname);
        }

        RemoveHideUI(l_UIbase);
        AddUI(l_UIbase);

        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnOpen);  //派发OnOpen事件
        l_UIbase.OnOpen();

        s_UILayerManager.SetLayer(l_UIbase);      //设置层级
        s_UIAnimManager.StartEnterAnim(l_UIbase, l_callback, l_objs); //播放动画
        return l_UIbase;
    }
    public static void OpenUIWindow<T>() where T : UIWindowBase
    {
        OpenUIWindow(typeof(T).Name);
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

            s_UIAnimManager.StartEnterAnim(l_UI, l_callback, l_objs);
        }
        else
        {
            CloseUIWindowCallBack(l_UI, l_objs);
        }
    }
    public static void CloseUIWindowCallBack(UIWindowBase l_UI, params object[] l_objs)
    {
        UISystemEvent.Dispatch(l_UI, UIEvent.OnDestroy);  //派发OnDestroy事件
        l_UI.OnClose();
        l_UI.RemoveAllEvent();
        AddHideUI(l_UI);
    }
    public static void CloseUIWindow(string l_UIname, bool isPlayAnim = true, UICallBack l_callback = null, params object[] l_objs)
    {
        CloseUIWindow(GetUI(l_UIname), isPlayAnim, l_callback, l_objs);
    }
    public static void CloseUIWindow<T>(bool isPlayAnim = true, UICallBack l_callback = null, params object[] l_objs) where T : UIWindowBase
    {
        CloseUIWindow(typeof(T).Name, isPlayAnim,l_callback, l_objs);
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
        if (s_hideUIs.ContainsKey(l_UI.name))
        {
            RemoveHideUI(l_UI);
        }
        else if (s_UIs.ContainsKey(l_UI.name))
        {
            RemoveUI(l_UI);   
        }

        l_UI.OnDestroy();
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

    public static UIWindowBase GetUI<T>() where T : UIWindowBase
    {
        return GetUI(typeof(T).Name);
    }
    public static UIWindowBase GetUI(string l_UIname)
    {
        if (!s_UIs.ContainsKey(l_UIname))
        {
            Debug.Log("!ContainsKey " + l_UIname);
            return null;
        }
        else
        {
            if (s_UIs[l_UIname].Count == 0)
            {
                Debug.Log("s_UIs[UIname].Count == 0");
                return null;
            }
            else
            {
                //默认返回最后创建的那一个
                return s_UIs[l_UIname][s_UIs[l_UIname].Count - 1];
            }
        }
    }

    static void AddUI(UIWindowBase l_UI)
    {
        if (!s_UIs.ContainsKey(l_UI.name))
        {
            s_UIs.Add(l_UI.name, new List<UIWindowBase>());
        }

        Debug.Log("AddUI: " + l_UI.name);

        s_UIs[l_UI.name].Add(l_UI);
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

    #endregion

    #region 隐藏UI列表的管理

    /// <summary>
    /// 删除所有隐藏的UI
    /// </summary>
    public static void DestroyAllHideUI()
    {
        foreach (List<UIWindowBase> l_uis in s_hideUIs.Values)
        {
            for (int i = 0; i < s_hideUIs.Count; i++)
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
            Debug.Log("!ContainsKey " + l_UIname);
            return null;
        }
        else
        {
            if (s_hideUIs[l_UIname].Count == 0)
            {
                Debug.Log("s_UIs[UIname].Count == 0");
                return null;
            }
            else
            {
                //默认返回最后创建的那一个
                return s_hideUIs[l_UIname][s_UIs[l_UIname].Count - 1];
            }
        }
    }

    static void AddHideUI(UIWindowBase l_UI)
    {
        if (!s_hideUIs.ContainsKey(l_UI.name))
        {
            s_hideUIs.Add(l_UI.name, new List<UIWindowBase>());
        }

        s_hideUIs[l_UI.name].Add(l_UI);
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

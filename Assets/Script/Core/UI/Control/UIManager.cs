using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UISystemEvent))]
[RequireComponent(typeof(UILayerManager))]
[RequireComponent(typeof(UIAnimManager))]
public class UIManager : MonoBehaviour
{
    static UIManager s_instance;
    public static UILayerManager s_UILayerManager; //UI层级管理器
    public static UIAnimManager  s_UIAnimManager;  //UI动画管理器

    static public Dictionary<string, List<UIWindowBase>> s_UIs = new Dictionary<string, List<UIWindowBase>>();

    public static UIManager s_Instance
    {
        get {
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
        s_UIAnimManager = l_instance.GetComponent<UIAnimManager>();

        DontDestroyOnLoad(l_instance);
    }

    public static void OpenUIWindow(string l_UIname,UICallBack l_callback = null,params object[] l_objs)
    {
        GameObject l_UItmp = GameObjectManager.CreatGameObject(l_UIname, s_Instance.gameObject);

        UIWindowBase l_UIbase = l_UItmp.GetComponent<UIWindowBase>();
        AddUI(l_UIbase);

        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnInit);  //派发OnInit事件
        l_UIbase.OnInit();

        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnOpen);  //派发OnOpen事件
        l_UIbase.OnOpen();

        s_UILayerManager.SetLayer(l_UIbase);      //设置层级
        s_UIAnimManager.StartEnterAnim(l_UIbase, l_callback,l_objs); //播放动画
    }

    public static void OpenUIWindow<T>() where T : UIWindowBase
    {
        OpenUIWindow(typeof(T).Name);
    }

    public static void DestroyUIWindow(UIWindowBase l_UI, UICallBack l_callback = null, params object[] l_objs)
    {
        RemoveUI(l_UI);        //移除UI引用

        //动画播放完毕删除UI
        if (l_callback!= null)
        {
            l_callback += DestroyUIWindowCallBack;
        }
        else
        {
            l_callback = DestroyUIWindowCallBack;
        }

        s_UIAnimManager.StartEnterAnim(l_UI, l_callback, l_objs);

    }

    public static void DestroyUIWindowCallBack(UIWindowBase l_UI, params object[] l_objs)
    {
        UISystemEvent.Dispatch(l_UI, UIEvent.OnDestroy);  //派发OnDestroy事件
        l_UI.OnDestroy();
        Destroy(l_UI.gameObject);
    }

    public static void DestroyUIWindow(string l_UIname, UICallBack l_callback = null, params object[] l_objs)
    {
        DestroyUIWindow(GetUI(l_UIname), l_callback,l_objs);
    }

    public static void DestroyUIWindow<T>(string l_UIname, UICallBack l_callback = null, params object[] l_objs) where T : UIWindowBase
    {
        DestroyUIWindow(typeof(T).Name, l_callback, l_objs);
    }

    public static void ShowUIWindow(string l_UIname, UICallBack l_callback = null, params object[] l_objs)
    {
        UIWindowBase l_UI = GetUI(l_UIname);
        if (l_UI != null)
        {
            UISystemEvent.Dispatch(l_UI, UIEvent.OnShow);  //派发OnShow事件
            l_UI.OnShow();
            l_UI.gameObject.SetActive(true);

            s_UIAnimManager.StartEnterAnim(l_UI, l_callback, l_objs); //播放动画
        }
        else
        {
            OpenUIWindow(l_UIname, l_callback, l_objs);
        }
    }

    public static void ShowUIWindow<T>( UICallBack l_callback = null, params object[] l_objs) where T:UIWindowBase
    {
        ShowUIWindow(typeof(T).Name, l_callback,l_objs);
    }

    public static void HideUIWindow(string l_UIname, UICallBack l_callback = null, params object[] l_objs)
    {
        UIWindowBase l_UI = GetUI(l_UIname);
        if (l_UI != null)
        {
            //动画播放完毕删除UI
            if (l_callback != null)
            {
                l_callback += HideUIWindowCallBack;
            }
            else
            {
                l_callback = HideUIWindowCallBack;
            }

            s_UIAnimManager.StartExitAnim(l_UI, l_callback, l_objs); //播放动画
        }
        else
        {
            Debug.LogError("UIManager HideUIWindow error dont find " + l_UIname);
        }
    }

    public static void HideUIWindowCallBack(UIWindowBase l_UI, params object[] l_objs)
    {
        UISystemEvent.Dispatch(l_UI, UIEvent.OnHide);  //派发OnHide事件
        l_UI.OnHide();
        l_UI.gameObject.SetActive(false);
    }

    public static void HideUIWindow<T>(UICallBack l_callback = null, params object[] l_objs) where T : UIWindowBase
    {
        HideUIWindow(typeof(T).Name, l_callback, l_objs);
    }


    static void AddUI(UIWindowBase l_UI)
    {
        if(!s_UIs.ContainsKey(l_UI.name) )
        {
            s_UIs.Add(l_UI.name,new List<UIWindowBase>());
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

    public static UIWindowBase GetUI<T>() where T:UIWindowBase
    {
        return GetUI(typeof(T).Name);
    }
}

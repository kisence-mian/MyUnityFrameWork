using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISystemEvent
{
    public static Dictionary<UIEvent, UICallBack> s_allUIEvents = new Dictionary<UIEvent, UICallBack>();
    public static Dictionary<string, Dictionary<UIEvent, UICallBack>> s_singleUIEvents = new Dictionary<string, Dictionary<UIEvent, UICallBack>>();

    /// <summary>
    /// 每个UI都会派发的事件
    /// </summary>
    /// <param name="Event">事件类型</param>
    /// <param name="callback">回调函数</param>
    public static void RegisterAllUIEvent(UIEvent UIEvent, UICallBack l_CallBack)
    {
        if (s_allUIEvents.ContainsKey(UIEvent))
        {
            s_allUIEvents[UIEvent] += l_CallBack;
        }
        else
        {
            s_allUIEvents.Add(UIEvent,l_CallBack);
        }
    }

    public static void RemoveAllUIEvent(UIEvent UIEvent, UICallBack l_CallBack)
    {
        if (s_allUIEvents.ContainsKey(UIEvent))
        {
            s_allUIEvents[UIEvent] -= l_CallBack;
        }
        else
        {
            Debug.LogError("RemoveAllUIEvent don't exits: " + UIEvent);
        }
    }

    /// <summary>
    /// 注册单个UI派发的事件
    /// </summary>
    /// <param name="Event">事件类型</param>
    /// <param name="callback"回调函数></param>
    public static void RegisterEvent(string l_UIName,UIEvent l_UIEvent, UICallBack l_CallBack)
    {
        if (s_singleUIEvents.ContainsKey(l_UIName))
        {
            if (s_singleUIEvents[l_UIName].ContainsKey(l_UIEvent))
            {
                s_singleUIEvents[l_UIName][l_UIEvent] += l_CallBack;
            }
            else
            {
                s_singleUIEvents[l_UIName].Add(l_UIEvent,l_CallBack);
            }
        }
        else
        {
            s_singleUIEvents.Add(l_UIName,new Dictionary<UIEvent,UICallBack>());
            s_singleUIEvents[l_UIName].Add(l_UIEvent, l_CallBack);
        }
    }

    public static void Dispatch(UIWindowBase l_UI, UIEvent l_UIEvent,params object[] l_objs)
    {
        if (l_UI == null)
        {
            Debug.LogError("Dispatch l_UI is null!");

            return;
        }

        if (s_allUIEvents.ContainsKey(l_UIEvent))
        {
            try
            {
                if(s_allUIEvents[l_UIEvent] != null)
                    s_allUIEvents[l_UIEvent](l_UI, l_objs);
            }
            catch (Exception e)
            {
                Debug.LogError("UISystemEvent Dispatch error:" + e.ToString());
            }
        }

        if (s_singleUIEvents.ContainsKey(l_UI.name))
        {
            if (s_singleUIEvents[l_UI.name].ContainsKey(l_UIEvent))
            {
                try
                {
                    if (s_singleUIEvents[l_UI.name][l_UIEvent] != null)
                        s_singleUIEvents[l_UI.name][l_UIEvent](l_UI, l_objs);
                }
                catch (Exception e)
                {
                    Debug.LogError("UISystemEvent Dispatch error:" + e.ToString());
                }
            }
        }
    }
}

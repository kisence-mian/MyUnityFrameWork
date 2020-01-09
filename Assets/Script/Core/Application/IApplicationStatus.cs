using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class IApplicationStatus
{
    #region UI 管理

    List<UIWindowBase> m_uiList = new List<UIWindowBase>();

    /// <summary>
    /// 获取现在ApplicationStatus管理的打开的UI的个数
    /// </summary>
    /// <returns></returns>
    public int GetUIListCount()
    {
        return m_uiList.Count;
    }

    public List<UIWindowBase> GetUIList(bool isSort = false)
    {
        List < UIWindowBase > list= new List<UIWindowBase>( m_uiList);
        if(isSort)
        list.Sort(SortUIWindow);
        return list;
    }
    /// <summary>
    /// 根据window放的位置来排序UIType，以及同一Type，用界面上先后排序
    /// </summary>
    /// <param name="w"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int SortUIWindow(UIWindowBase w, UIWindowBase b)
    {
        if (w.m_UIType - b.m_UIType > 0)
            return 1;
        else if (w.m_UIType - b.m_UIType < 0)
        {
            return -1;
        }
        else
        {
            int childCount = w.transform.parent.childCount;
            int index0 = -1;
            int index1 = -1;
            for (int i = 0; i < childCount; i++)
            {
                Transform tra = w.transform.parent.GetChild(i);
                if (tra == w.transform)
                {
                    index0 = i;
                }
                else if (tra == b.transform)
                {
                    index1 = i;
                }
            }
            if (index0 > index1)
            {
                return 1;
            }
            else
                return -1;
        }
    }
    /// <summary>
    /// 获取status打开的window的最上层Window
    /// </summary>
    /// <returns></returns>
    public UIWindowBase GetStatusTopUIWindow()
    {
        if (m_uiList.Count > 0)
            return m_uiList[m_uiList.Count - 1];

        return null;
    }

    public UIWindowBase GetUI(string name)
    {
        foreach (var item in m_uiList)
        {
            if (item.name == name)
                return item;
        }
        return null;
    }
    public T GetUI<T>() where T : UIWindowBase
    {
        foreach (var item in m_uiList)
        {
            if (item.name == typeof(T).Name)
                return (T)item;
        }
        return default(T);
    }

    public T OpenUI<T>() where T: UIWindowBase
    {
        UIWindowBase ui =  UIManager.OpenUIWindow<T>();

        m_uiList.Add(ui);

        return (T)ui;
    }

    public UIWindowBase OpenUI(string name)  
    {
        UIWindowBase ui = UIManager.OpenUIWindow(name);

        m_uiList.Add(ui);

        return ui;
    }

    public void OpenUIAsync<T>() where T:UIWindowBase
    {
        UIManager.OpenUIAsync<T>((ui,objs)=>
        {
            m_uiList.Add(ui);
        });
    }

    public void CloseUI<T>(bool isPlayAnim = true) where T:UIWindowBase
    {
        UIWindowBase ui = UIManager.GetUI<T>();
        if (ui == null)
        {
            Debug.LogError("UI window no open from status :" + typeof(T));
        }
        CloseUI(ui,isPlayAnim);
    }

    public void CloseUI(UIWindowBase ui, bool isPlayAnim = true)
    {
        if (ui!=null && m_uiList.Contains(ui))
        {
            m_uiList.Remove(ui);
            UIManager.CloseUIWindow(ui);
        }
        else
        {
            Debug.LogError("UI window no open from status :" + ui);
        }

    }

    public bool IsUIOpen<T>() where T : UIWindowBase
    {
        for (int i = 0; i < m_uiList.Count; i++)
        {
            UIWindowBase tempWin = m_uiList[i];
            if (tempWin.GetType() == typeof(T) 
                &&(tempWin.windowStatus == UIWindowBase.WindowStatus.Open 
                || tempWin.windowStatus == UIWindowBase.WindowStatus.OpenAnim))
                return true;
        }
        return false;
    }

    public void CloseAllUI(bool isPlayAnim = true)
    {
        for (int i = 0; i < m_uiList.Count; i++)
        {
            //Debug.Log("CloseAllUI " + m_uiList[i]);
            UIManager.CloseUIWindow(m_uiList[i],isPlayAnim);
        }
        m_uiList.Clear();
    }

    #endregion

    #region 生命周期

    /// <summary>
    /// 当状态第一次创建时调用（生命周期里只调用一次）
    /// </summary>
    public virtual void OnCreate()
    {

    }
    /// <summary>
    /// 测试使用，直接进入游戏某个流程时，这里可以初始化测试数据
    /// </summary>
    public virtual void EnterStatusTestData()
    {

    }

    /// <summary>
    /// 进入某个状态调用
    /// </summary>
    public virtual void OnEnterStatus()
    {

    }

    /// <summary>
    /// 退出某个状态时调用
    /// </summary>
    public virtual void OnExitStatus()
    {

    }

    /// <summary>
    /// 该状态每帧调用
    /// </summary>
    public virtual void OnUpdate()
    {

    }

    public virtual void OnGUI()
    {

    }
    public virtual IEnumerator InChangeScene(ChangSceneFinish handle)
    {
        if (handle != null)
        {
            try
            {
                handle();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        yield break;
    }

    #endregion

    public delegate void ChangSceneFinish();
}

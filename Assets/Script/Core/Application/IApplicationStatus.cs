using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class IApplicationStatus
{
    #region UI 管理

    List<UIWindowBase> m_uiList = new List<UIWindowBase>();

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

    public void CloseUI<T>() where T:UIWindowBase
    {
        UIWindowBase ui = UIManager.GetUI<T>();
        m_uiList.Remove(ui);
        UIManager.CloseUIWindow(ui);
    }

    public void CloseUI(UIWindowBase ui)
    {
        m_uiList.Remove(ui);
        UIManager.CloseUIWindow(ui);
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

using UnityEngine;
using System.Collections;

public class IApplicationStatus
{
    public StatusType m_statusType;
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
}

 public enum StatusType
 {
     HotUpdate,
     Game,
 }

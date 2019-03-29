using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SDKInterfaceBase 
{
    [HideInInspector]
    public string m_SDKName;

    public virtual void Init()
    {

    }

    /// <summary>
    /// 额外初始化，当SDK需要特殊的初始化时机时使用
    /// </summary>
    public virtual void ExtraInit()
    {

    }
}

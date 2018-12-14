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
}

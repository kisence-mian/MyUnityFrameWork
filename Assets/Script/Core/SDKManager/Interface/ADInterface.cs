using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADInterface  : SDKInterfaceBase 
{
    //[HideInInspector]
    public CallBack m_ADLoadFinish;


    public override void Init()
    {

    }

    public virtual void LoadAD(ADType adType,string tag = "")
    {

    }

    public virtual void PlayAD(ADType adType, string tag = "")
    {

    }

    public virtual void CloseAD(ADType adType, string tag = "")
    {

    }

    public virtual bool IsLoaded(ADType adType, string tag = "")
    {
        return true;
    }
}

public enum ADType
{
    Banner,
    Reward,
    Interstitial,
    Video,
}

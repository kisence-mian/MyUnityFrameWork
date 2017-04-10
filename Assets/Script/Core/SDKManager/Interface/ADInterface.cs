using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADInterface  : SDKInterfaceBase 
{
    public override void Init()
    {

    }

    public virtual void LoadAD(ADType adType)
    {

    }

    public virtual void PlayAD(ADType adType)
    {

    }

    public virtual void CloseAD(ADType adType)
    {

    }
}

public enum ADType
{
    Banner,
    Reward,
    Interstitial,
}

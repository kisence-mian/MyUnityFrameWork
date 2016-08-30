using UnityEngine;
using System.Collections;
using System;

public class UIAnimManager : MonoBehaviour 
{
    //开始调用进入动画
    public void StartEnterAnim(UIWindowBase l_UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnStartEnterAnim);
        StartCoroutine(l_UIbase.EnterAnim(EndEnterAnim, callBack, objs));
    }

    //进入动画播放完毕回调
    public void EndEnterAnim(UIWindowBase l_UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnCompleteEnterAnim);
        l_UIbase.OnCompleteEnterAnim();

        try
        {
            if (callBack!= null)
            {
                callBack(l_UIbase, objs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    //开始调用退出动画
    public void StartExitAnim(UIWindowBase l_UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnStartExitAnim);
        StartCoroutine(l_UIbase.ExitAnim(EndExitAnim, callBack, objs));
    }

    //退出动画播放完毕回调
    public void EndExitAnim(UIWindowBase l_UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(l_UIbase, UIEvent.OnCompleteExitAnim);
        l_UIbase.OnCompleteExitAnim();

        try
        {
            if (callBack != null)
            {
                callBack(l_UIbase, objs);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}

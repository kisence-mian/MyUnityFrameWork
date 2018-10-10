using UnityEngine;
using System.Collections;
using System;

public class UIAnimManager : MonoBehaviour 
{
    //开始调用进入动画
    public void StartEnterAnim(UIWindowBase UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(UIbase, UIEvent.OnStartEnterAnim);
        StartCoroutine(UIbase.EnterAnim(EndEnterAnim, callBack, objs));
    }

    //进入动画播放完毕回调
    public void EndEnterAnim(UIWindowBase UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(UIbase, UIEvent.OnCompleteEnterAnim);
        UIbase.OnCompleteEnterAnim();
        UIbase.windowStatus = UIWindowBase.WindowStatus.Open;

        try
        {
            if (callBack!= null)
            {
                callBack(UIbase, objs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    //开始调用退出动画
    public void StartExitAnim(UIWindowBase UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(UIbase, UIEvent.OnStartExitAnim);
        StartCoroutine(UIbase.ExitAnim(EndExitAnim, callBack, objs));
    }

    //退出动画播放完毕回调
    public void EndExitAnim(UIWindowBase UIbase, UICallBack callBack, params object[] objs)
    {
        UISystemEvent.Dispatch(UIbase, UIEvent.OnCompleteExitAnim);
        UIbase.OnCompleteExitAnim();
        UIbase.windowStatus = UIWindowBase.WindowStatus.Close;

        try
        {
            if (callBack != null)
            {
                callBack(UIbase, objs);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}

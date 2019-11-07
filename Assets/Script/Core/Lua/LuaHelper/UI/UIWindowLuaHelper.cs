using UnityEngine;
using System.Collections;
using System;

public class UIWindowLuaHelper : UIWindowBase
{
#if USE_LUA
    public override void OnInit()
    {
        UIEventCallBackHelper.CallOnUIInit(this);
    }

    public override void OnOpen()
    {
        UIEventCallBackHelper.CallOnUIOpen(this);
    }

    public override void OnShow()
    {
        UIEventCallBackHelper.CallOnUIShow(this);
    }

    public override void OnHide()
    {
        UIEventCallBackHelper.CallOnUIHide(this);
    }

    public override IEnumerator EnterAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        float time = 0;

        try
        {
            time = (float)UIEventCallBackHelper.CallOnEnterAnim(this);
        }
        catch (Exception e)
        {
            Debug.LogError(name + " EnterAnim Error " + e.ToString());
        }

        yield return new WaitForSeconds(time);

        l_animComplete(this, l_callBack, objs);
    }

    public override void OnCompleteEnterAnim()
    {
        UIEventCallBackHelper.CallOnCompleteEnterAnim(this);
    }

    public override void OnRefresh()
    {
        UIEventCallBackHelper.CallOnRefresh(this);
    }

    public override void OnClose()
    {
        UIEventCallBackHelper.CallOnClose(this);
    }

    public override IEnumerator ExitAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        double time = 0;

        try
        {
            time = UIEventCallBackHelper.CallOnExitAnim(this);
        }
        catch (Exception e)
        {
            Debug.LogError(name + "ExitAnim Error " + e.ToString());
        }

        yield return new WaitForSeconds((float)time);
        l_animComplete(this, l_callBack, objs);
    }

    public override void OnCompleteExitAnim()
    {
        UIEventCallBackHelper.CallOnCompleteExitAnim(this);
    }

    protected override void OnUIDestroy()
    {
        UIEventCallBackHelper.CallOnUIDestroy(this);
    }
#endif
}
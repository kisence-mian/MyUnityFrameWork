using UnityEngine;
using System.Collections;

public class UILuaEventCallBackHelper
{

#if USE_LUA
    public static void CallOnUIInit(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnInit").Call(UI);
    }

    public static void CallOnUIOpen(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnOpen").Call(UI);
    }

    public static double CallOnEnterAnim(UIWindowBase UI)
    {
        return (double)LuaManager.LuaState.GetFunction("LuaUIManager.UIOnEnterAnim").Call(UI)[0];
    }
    public static void CallOnCompleteEnterAnim(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnCompleteEnterAnim").Call(UI);
    }

    public static void CallOnRefresh(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnRefresh").Call(UI);
    }

    public static void CallOnClose(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnClose").Call(UI);
    }

    public static double CallOnExitAnim(UIWindowBase UI)
    {
        return (double)LuaManager.LuaState.GetFunction("LuaUIManager.UIOnExitAnim").Call(UI)[0];
    }

    public static void CallOnCompleteExitAnim(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnCompleteExitAnim").Call(UI);
    }

    public static void CallOnUIDestroy(UIWindowBase UI)
    {
        LuaManager.LuaState.GetFunction("LuaUIManager.UIOnDestroy").Call(UI);
    }
#endif
}
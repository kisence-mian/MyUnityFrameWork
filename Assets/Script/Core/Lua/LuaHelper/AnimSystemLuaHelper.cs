using UnityEngine;
using System.Collections;

public class AnimSystemLuaHelper 
{
    public static void UguiAlpha(GameObject go,float form,float to,float time)
    {
        AnimSystem.UguiAlpha(go, form, to, time);
    }
}

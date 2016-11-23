using UnityEngine;
using System.Collections;
using LuaInterface;

public class LuaManager  
{
    static LuaState state = new LuaState();

    public static void Init()
    {
        state.Start();
        LuaBinder.Bind(state);

        ApplicationManager.s_OnApplicationUpdate += Update;   
    }

    static void Update()
    {

    }
}

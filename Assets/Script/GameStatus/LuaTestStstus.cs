using UnityEngine;
using System.Collections;

#if USE_LUA

public class LuaTestStstus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        LuaManager.LoadLua();
        LuaManager.LaunchLua();
    }
}

#endif

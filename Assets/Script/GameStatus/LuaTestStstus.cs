using UnityEngine;
using System.Collections;

public class LuaTestStstus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
#if USE_LUA
        LuaManager.LoadLua();
        LuaManager.LaunchLua();
#endif
    }
}

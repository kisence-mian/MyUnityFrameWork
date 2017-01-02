using UnityEngine;
using System.Collections;

public class TestStstus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        LuaManager.LoadLua();
        LuaManager.LaunchLua();
    }
}

using UnityEngine;
using System.Collections;

public class TestStstus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        LuaManager.StartLua();

        UIManager.OpenUIWindow("LuaTestWindow");


        //UIManagerLuaHelper.CloseUIWindow("qqqWindow");

        //UIManagerLuaHelper.OpenUIWindow("qqqWindow");
        //UIManagerLuaHelper.OpenUIWindow("qqqWindow");
    }
}

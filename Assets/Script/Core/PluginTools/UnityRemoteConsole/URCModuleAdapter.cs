using UnityEngine;
using System.Collections;
using UnityRemoteConsole;

public class URCModuleAdapter : AppModuleBase
{

    public override void OnCreate()
    {
        URCServerStarter.ConsoleStart();
    }
}

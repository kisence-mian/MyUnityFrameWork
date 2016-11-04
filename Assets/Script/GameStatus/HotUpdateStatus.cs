using UnityEngine;
using System.Collections;

public class HotUpdateStatus : IApplicationStatus
{

    public override void OnEnterStatus()
    {
        RescourcesConfigManager.Initialize();
        UIManager.Init();
    }
}

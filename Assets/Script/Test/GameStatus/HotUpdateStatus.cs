using UnityEngine;
using System.Collections;

public class HotUpdateStatus : IApplicationStatus
{

    public override void OnEnterStatus()
    {
        BundleConfigManager.Initialize();
        UIManager.Init();
    }
}

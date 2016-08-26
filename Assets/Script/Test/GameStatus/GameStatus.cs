using UnityEngine;
using System.Collections;

public class GameStatus : IApplicationStatus 
{
    public override void OnEnterStatus()
    {
        BundleConfigManager.Initialize();
        UIManager.Init();
    }
}

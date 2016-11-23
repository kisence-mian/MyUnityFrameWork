using UnityEngine;
using System.Collections;

public abstract class GameStatus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        ResourcesConfigManager.Initialize();
        UIManager.Init();
    }
}

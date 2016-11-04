using UnityEngine;
using System.Collections;

public class GameStatus : IApplicationStatus 
{
    public override void OnEnterStatus()
    {
        RescourcesConfigManager.Initialize();
        UIManager.Init();
    }
}

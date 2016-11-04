using UnityEngine;
using System.Collections;

public class GameStatus : IApplicationStatus 
{
    public override void OnEnterStatus()
    {
        RecourcesConfigManager.Initialize();
        UIManager.Init();
    }
}

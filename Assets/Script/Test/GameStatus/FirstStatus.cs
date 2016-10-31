using UnityEngine;
using System.Collections;

public class FirstStatus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        GameObjectManager.CreatGameObjectByPool("Cube");

        ConfigManager.GetData("adasda")["qqq"].GetString();
    }
}

using UnityEngine;
using System.Collections;

public class FirstStatus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        GameObjectManager.CreatGameObjectByPool("gogo");

        //ConfigManager.GetData("adasda")["qqq"].GetString();

        Debug.Log("hello");
    }
}

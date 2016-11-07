using UnityEngine;
using System.Collections;

public class FirstStatus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        GameObject go =  GameObjectManager.CreatGameObjectByPool("gogo");

        go.transform.position = new Vector3(0,0,300);

        go.transform.localScale = new Vector3(100, 100, 100);

        UIManager.OpenUIWindow<testWindow>();

        AnimSystem.Move(go, null, new Vector3(0, 0, 600), time:1,repeatType: RepeatType.PingPang);

        //ConfigManager.GetData("adasda")["qqq"].GetString();

        Debug.Log("hello");
    }
}

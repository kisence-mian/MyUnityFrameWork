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

        InputUIEvent iue =  new InputUIEvent();

        iue.m_ComponentName = "Button";
        iue.m_UIName = "TestWindow";

        iue.m_EventType = InputUIEventType.OnClick;

        Debug.Log("iue: " + iue.Serialize());
        Debug.Log("key: " + iue.GetEventKey());

        InputNetworkEvent ine =  new InputNetworkEvent();

        ine.m_MessgaeType = "qweasd";
        ine.m_content     = "content";

        Debug.Log("key: " + ine.GetEventKey());
        Debug.Log("ine: " + ine.Serialize());
       //iue.Serialize();
    }
}

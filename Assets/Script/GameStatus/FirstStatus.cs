using UnityEngine;
using System.Collections;

public class FirstStatus : IApplicationStatus 
{

    public override void OnEnterStatus()
    {
        Debug.Log("DPI :" + Screen.dpi + " FontSize" + GUIUtil.FontSize);

        InputOperationEventProxy.LoadEventCreater<CustomEvent>();
        InputManager.AddListener<CustomEvent>(OnEventCallBack);

        GameObject go =  GameObjectManager.CreatGameObjectByPool("gogo");

        go.transform.position = new Vector3(0,0,300);

        go.transform.localScale = new Vector3(100, 100, 100);

        UIManager.OpenUIWindow<testWindow>();

        AnimSystem.Move(go, null, new Vector3(0, 0, 600), time:1,repeatType: RepeatType.PingPang);

        //ConfigManager.GetData("adasda")["qqq"].GetString();

        Debug.Log("hello");

        //InputManager.LoadDispatcher(typeof(InputNetworkEvent).Name);

        ////InputManager.LoadDispatcher<InputNetworkEvent>();

        //InputNetworkEvent e = new InputNetworkEvent();

        //InputManager.AddListener<InputNetworkEvent>(e.EventKey, CallBackTest);

        //////InputManager.re

        //InputManager.Dispatch<InputNetworkEvent>(e);

        AnimSystem.Move(go, null, Vector3.one, callBack: (object[] obj) => { }
            , parameter: new object[] { });
    }

    //public void CallBackTest(InputNetworkEvent e)
    //{
    //    Debug.Log(e.Serialize());
    //}

    public void OnEventCallBack(CustomEvent e)
    {
        Debug.Log(e.Serialize());
    }
}

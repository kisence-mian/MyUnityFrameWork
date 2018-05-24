using FrameWork;
using HDJ.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

/// <summary>
/// Json消息类的接受转换和发送
/// </summary>
public class JsonMessageProcessingController
{
    public static void Init()
    {
        InputManager.AddAllEventListener<InputNetworkMessageEvent>(MessageReceiveCallBack);
    }
   // static Deserializer deserializer = new Deserializer();

    private static void MessageReceiveCallBack(InputNetworkMessageEvent inputEvent)
    {
        Debug.Log("MessageReceiveCallBack ;" + JsonUtils.ToJson(inputEvent));

        Type type = Type.GetType(inputEvent.m_MessgaeType);

        if(type == null)
        {
            Debug.LogError("No MessgaeType :" + inputEvent.m_MessgaeType);
            return;
        }

        object dataObj = JsonUtils.JsonToClassOrStruct( inputEvent.Data["Content"].ToString(),type);// deserializer.Deserialize(type, inputEvent.Data["Content"].ToString());

        MethodInfo method= type.GetMethod("DispatchMessage");
        method.Invoke(dataObj, null);
    }
    static Dictionary<string, object> mesDic = new Dictionary<string, object>();
    public static void SendMessage<T>(T data) 
    {
        mesDic.Clear();
        string mt = typeof(T).Name;
        string content = JsonUtils.ToJson(data); //Serializer.Serialize(data);
        Debug.Log("SendMessage : MT:" + mt + " msg :" + content);
        mesDic.Add("Content", content);
        NetworkManager.SendMessage(mt, mesDic);
    }
}


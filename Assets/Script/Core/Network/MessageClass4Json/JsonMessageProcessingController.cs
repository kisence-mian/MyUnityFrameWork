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
    public const string ErrorCodeMessage = "ErrorCodeMessage";
    public static void Init()
    {
        InputManager.AddAllEventListener<InputNetworkMessageEvent>(MessageReceiveCallBack);
    }
   // static Deserializer deserializer = new Deserializer();

    private static void MessageReceiveCallBack(InputNetworkMessageEvent inputEvent)
    {
        //心跳包
        if (inputEvent.m_MessgaeType == "HB")
        {
            return;
        }

        if (ApplicationManager.Instance.m_AppMode != AppMode.Release)
            Debug.Log("MessageReceiveCallBack ;" + JsonUtils.ToJson(inputEvent));

        Type type = Type.GetType(inputEvent.m_MessgaeType);

        if(type == null)
        {
            Debug.LogError("No MessgaeType :" + inputEvent.m_MessgaeType);
            return;
        }

        object dataObj = JsonUtils.FromJson(type, inputEvent.Data["Content"].ToString());// deserializer.Deserialize(type, inputEvent.Data["Content"].ToString());
        MessageClassInterface msgInterface = (MessageClassInterface)dataObj;
        msgInterface.DispatchMessage();

        if(msgInterface is CodeMessageBase)
        {
            CodeMessageBase codeMsg = (CodeMessageBase)msgInterface;

            GlobalEvent.DispatchEvent(ErrorCodeMessage, codeMsg);
        }
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


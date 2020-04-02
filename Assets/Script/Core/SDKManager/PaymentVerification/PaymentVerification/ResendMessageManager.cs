using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 重发消息管理器
/// </summary>
public class ResendMessageManager
{
    private const string ResendMsgFile = "ResendMsgFile";
    public static float resendTime = 2f;
    /// <summary>
    /// 开始重发，一般是登陆后
    /// </summary>
    public static bool startResend = false;
    //private static int indexCode = 0;
    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += Update;
        //ApplicationManager.s_OnApplicationQuit += OnApplicationQuit;
        InputManager.AddAllEventListener<InputNetworkMessageEvent>(MessageReceiveCallBack);
        LoginGameController.OnUserLogin += OnUserLogin;
       
    }
    private static User user;
    private static void OnUserLogin(UserLogin2Client t)
    {
        if (t.code != 0)
            return;
        user = t.user;
        LoadRecord();
    }

    private static void MessageReceiveCallBack(InputNetworkMessageEvent inputEvent)
    {
       

        if (msgs==null|| msgs.Count == 0)
            return;
        foreach(ResendMessage m in msgs)
        {
            if(m.removeMT==inputEvent.m_MessgaeType)
            {
                Debug.Log("移除重发：" + m.removeMT);
                msgs.Remove(m);
                SaveRecord();
                if (m.callBack != null)
                {
                    MessageClassInterface msgInterface = null;
                    Type type = Type.GetType(inputEvent.m_MessgaeType);

                    if (type == null)
                    {
                        Debug.LogError("No MessgaeType :" + inputEvent.m_MessgaeType);

                    }
                    else
                    {
                        object dataObj = JsonUtils.FromJson(type, inputEvent.Data["Content"].ToString());
                        msgInterface = (MessageClassInterface)dataObj;
                    }

                    m.callBack(msgInterface);
                }
                break;
            }
        }
    }

    private static void LoadRecord()
    {
        if (user == null)
            return;
        List<ResendMessage> list = GetData<ResendMessage>(user.userID);
        if (list != null)
        {
            if (msgs != null)
            {
                msgs.AddRange(list);
            }
            else
            {
                msgs = list;
            }
           

        }
      
        list = GetData<ResendMessage>("0");
        if (list != null)
            msgs.AddRange(list);
        RecordManager.SaveRecord(ResendMsgFile, "0", "");
        Debug.Log("加载重发记录：" + msgs.Count);
    }

    private static List<T> GetData<T>(string key)
    {
        string res = RecordManager.GetStringRecord(ResendMsgFile, key, "");
        if (string.IsNullOrEmpty(res))
            return null;
      List<T>  msgs = JsonUtils.FromJson<List<T>>(res);
        return msgs;
    }
    private static void SaveRecord()
    {
        if (user == null)
            return;
        String json = JsonUtils.ToJson(msgs);
        RecordManager.SaveRecord(ResendMsgFile, user.userID, json);
        Debug.Log("保持重发记录:" + msgs.Count);
    }

    private static float tempResendTime = 0;
    private static void Update()
    {
        //Debug.Log(msgs.Count+" :"+ startResend+" :"+ tempResendTime);
        if (msgs == null)
        {
            msgs = new List<ResendMessage>();
        }
        if (msgs.Count == 0)
            return;
        if (!startResend)
            return;

        if (tempResendTime > 0)
        {
            tempResendTime -= Time.deltaTime;
            return;
        }
        tempResendTime = resendTime;
        foreach (ResendMessage m in msgs)
        {
            //Debug.LogWarning(m.mt + " :" + m.noSend);
            if (m.noSend)
                continue;
            JsonMessageProcessingController.SendMessage(m.mt, m.content);
        }
    }
    static List<ResendMessage> msgs = new List<ResendMessage>();


    public static void AddResendMessage<T>(T data,string removeMT,CallBack<MessageClassInterface> callBack, bool noSend=false)
    {
        string content = JsonUtils.ToJson(data);
        string mt = typeof(T).Name;
        ResendMessage msgResnd = null;
        foreach (ResendMessage m in msgs)
        {
            if (m.content == content)
            {
                msgResnd = m;
                break;
            }
        }
          
        //Debug.LogError("noSend:" + noSend);
        if (msgResnd != null)
        {
            msgResnd.removeMT = removeMT;
            msgResnd.content = content;
            msgResnd.callBack = callBack;
            msgResnd.noSend = noSend;
            //Debug.LogError("msgResnd.noSend:" + msgResnd.noSend);
        }
        else
        {
            ResendMessage msg = new ResendMessage(removeMT, mt, content, callBack, noSend);
            msgs.Add(msg);
        }
        SaveRecord();
    }

    public class ResendMessage
    {
        public string removeMT;
        public string mt;
        public string content;
        /// <summary>
        /// 不发消息（也不重发），只监听接收
        /// </summary>
        public bool noSend = false;
        public CallBack<MessageClassInterface> callBack;
        public ResendMessage() { }
        public ResendMessage(string removeMT, string mt, string content,CallBack<MessageClassInterface> callBack, bool noSend)
        {
            this.removeMT = removeMT;
            this.mt = mt;
            this.content = content;
            this.callBack = callBack;
            this.noSend = noSend;
        }
    }
}


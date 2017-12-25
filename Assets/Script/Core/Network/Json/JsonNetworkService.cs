using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using FrameWork;

public class JsonNetworkService : INetworkInterface 
{
    /// <summary>
    /// 消息结尾符
    /// </summary>
    public const char c_endChar = '&';

    /// <summary>
    /// 文本中如果有结尾符则替换成这个
    /// </summary>
    public const string c_endCharReplaceString = "<FCP:AND>";

    public override void SpiltMessage(byte[] data, ref int offset, int length)
    {
        DealMessage(Encoding.UTF8.GetString(data, offset, length));
        offset = 0;
    }

    //发送消息
    public override void SendMessage(string MessageType, Dictionary<string, object> data)
    {
        try
        {
            if (!data.ContainsKey("MT"))
            {
                data.Add("MT", MessageType);
            }

            string mes = Json.Serialize(data);
            mes = mes.Replace(c_endChar.ToString(), c_endCharReplaceString);
            byte[] bytes = Encoding.UTF8.GetBytes(mes + "&");

            m_socketService.Send(bytes);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    StringBuilder m_buffer = new StringBuilder();
    public void DealMessage(string s)
    {
        bool isEnd = false;

        if(s.Substring(s.Length-1,1) == c_endChar.ToString())
        { 
            isEnd = true;
        }

        m_buffer.Append(s);

        string buffer = m_buffer.ToString();

        m_buffer.Remove(0,m_buffer.Length);

        string[] str = buffer.Split(c_endChar);

        for (int i = 0; i < str.Length; i++)
        {
            if (i != str.Length - 1)
            {
                CallBack(str[i]);
            }
            else
            {
                if (isEnd)
                {
                    CallBack(str[i]);
                }
                else
                {
                    m_buffer.Append(str[i]);
                }
            }
        }
    }

    public void CallBack(string s)
    {
        try
        {
            if(s != null && s != "")
            {
                NetWorkMessage msg = new NetWorkMessage();

                s = WWW.UnEscapeURL(s);
                s = s.Replace(c_endCharReplaceString, c_endChar.ToString());
                Dictionary<string, object> data = Json.Deserialize(s) as Dictionary<string, object>;

                msg.m_data = data;
                msg.m_MessageType = data["MT"].ToString();

                m_messageCallBack(msg);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Message error ->" + s +"<-\n" + e.ToString());
        }
    }
}

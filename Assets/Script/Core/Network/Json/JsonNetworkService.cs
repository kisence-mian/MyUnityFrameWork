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
    public int m_msgCode = 0;

    /// <summary>
    /// 消息结尾符
    /// </summary>
    public const char c_endChar = '&';

    /// <summary>
    /// 文本中如果有结尾符则替换成这个
    /// </summary>
    public const string c_endCharReplaceString = "<FCP:AND>";

    public override void Connect()
    {
        m_msgCode = 0;
        base.Connect();
    }

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
            byte[] bytes = Encoding.UTF8.GetBytes(mes + c_endChar);

            m_socketService.Send(bytes);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public override void SendMessage(string MessageType, string content)
    {
        try
        {
            content = content.Replace(c_endChar.ToString(), c_endCharReplaceString);
            byte[] bytes = Encoding.UTF8.GetBytes(content + c_endChar);

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
                //Debug.Log("MessageReceive ->" + s);

                NetWorkMessage msg = new NetWorkMessage();

                //s = WWW.UnEscapeURL(s);
                //Debug.Log("MessageReceive0 ->" + s);
                s = s.Replace(c_endCharReplaceString, c_endChar.ToString());
                //Debug.Log("MessageReceive1 ->" + s);
                Dictionary<string, object> data = Json.Deserialize(s) as Dictionary<string, object>;

                msg.m_data = data;
                msg.m_MessageType = data["MT"].ToString();

                if(data.ContainsKey("MsgCode"))
                {
                    msg.m_MsgCode = int.Parse(data["MsgCode"].ToString());

                    if(m_msgCode != msg.m_MsgCode)
                    {
                        if (msg.m_MsgCode > m_msgCode)
                        {
                            m_msgCode = msg.m_MsgCode;
                            m_msgCode++;
                        }

                        Debug.LogError("MsgCode error currentCode " + m_msgCode + " server code " + msg.m_MsgCode);
                        //throw new Exception();
                    }
                    else
                    {
                        m_msgCode++;
                        m_messageCallBack(msg);
                    }
                }
                else
                {
                    m_messageCallBack(msg);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Message error ->" + s +"<-\n" + e.ToString());
        }
    }
}

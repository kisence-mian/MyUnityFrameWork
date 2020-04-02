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
        EncryptionService.Init();

        m_msgCode = 0;
        m_buffer = new StringBuilder();

        base.Connect();
    }

    Queue<string> mesQueue = new Queue<string>(); //消息队列

    public override void SpiltMessage(byte[] data, ref int offset, int length)
    {
        lock(mesQueue)
        {
            mesQueue.Enqueue(Encoding.UTF8.GetString(data, offset, length));
            DealMessage(mesQueue.Dequeue());
            offset = 0;
        }
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
            //mes = mes.Replace(c_endChar.ToString(), c_endCharReplaceString);

            SendMessage(MessageType, mes);

            //byte[] bytes = Encoding.UTF8.GetBytes(mes + c_endChar);

            //m_socketService.Send(bytes);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString() + " MT:" + MessageType);
        }
    }

    public override void SendMessage(string MessageType, string content)
    {
        try
        {
            content = content.Replace(c_endChar.ToString(), c_endCharReplaceString);

            try
            {
                if (msgCompress != null)
                    content = msgCompress.CompressString(content);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
           

            //加密
            if (MessageType != HeartBeatBase.c_HeartBeatMT && EncryptionService.IsSecret)
            {
                content = EncryptionService.Encrypt(content);
            }
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
        lock(m_buffer)
        {
            //Debug.Log("DealMessage s ->" + s);i

            bool isEnd = false;
            if (s.Substring(s.Length - 1, 1) == c_endChar.ToString())
            {
                isEnd = true;
            }

            m_buffer.Append(s);

            string buffer = m_buffer.ToString();

            m_buffer.Remove(0, m_buffer.Length);

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
    }

    public void CallBack(string s)
    {
        //Debug.Log("CallBack s ->" + s);

        try
        {
            if(s != null && s != "")
            {
                if (msgCompress != null)
                {
                    try
                    {
                        s = msgCompress.DecompressString(s);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    
                }

                //解密
                if(EncryptionService.IsSecret)
                {
                    s = EncryptionService.Decrypt(s);
                }

                NetWorkMessage msg = new NetWorkMessage();

                s = s.Replace(c_endCharReplaceString, c_endChar.ToString());
                Dictionary<string, object> data = Json.Deserialize(s) as Dictionary<string, object>;

                msg.m_data = data;
                msg.m_MessageType = data["MT"].ToString();

                if(data.ContainsKey("MsgCode"))
                {
                    msg.m_MsgCode = int.Parse(data["MsgCode"].ToString());

                    if(m_msgCode != msg.m_MsgCode)
                    {
                        Debug.LogError("MsgCode error currentCode " + m_msgCode + " server code " + msg.m_MsgCode);
                        if (msg.m_MsgCode > m_msgCode)
                        {
                            m_msgCode = msg.m_MsgCode;
                            m_msgCode++;
                            m_messageCallBack(msg);
                        }
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

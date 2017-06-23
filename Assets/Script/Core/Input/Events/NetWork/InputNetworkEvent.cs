using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FrameWork;

public class InputNetworkMessageEvent : IInputEventBase
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public string m_MessgaeType = "";

    /// <summary>
    /// 消息内容
    /// </summary>
    public string m_content = "";

    Dictionary<string, object> m_data = null;

    public Dictionary<string, object> Data
    {
        get {
            if (m_data == null)
            {
                if (m_content != null && m_content != "")
                {
                    m_data = Json.Deserialize(m_content) as Dictionary<string, object>;
                }
                else
                {
                    m_data = new Dictionary<string, object>();
                }

            }

            return m_data;
        }
        set
        {
            m_data = value;
        }
    }

    protected override string GetEventKey()
    {
        return m_MessgaeType;
    }

    public override string Serialize()
    {
        if (m_content == null || m_content == "")
        {
            m_content = Json.Serialize(Data);
        }

        return base.Serialize();
    }

}

public class InputNetworkConnectStatusEvent : IInputEventBase
{
    public NetworkState m_status;

    public InputNetworkConnectStatusEvent()
    {
        m_status = NetworkState.ConnectBreak;
    }

    public InputNetworkConnectStatusEvent(NetworkState status)
    {
        m_status = status;
    }
}

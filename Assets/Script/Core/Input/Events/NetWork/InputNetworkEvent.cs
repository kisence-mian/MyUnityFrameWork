using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

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
                m_data = Json.Deserialize(m_content) as Dictionary<string, object>;
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

}

public class InputNetworkConnectStatusEvent : IInputEventBase
{
    public NetworkState m_status;

    public InputNetworkConnectStatusEvent(NetworkState status)
    {
        m_status = status;
    }
}

using UnityEngine;
using System.Collections;

public class InputNetworkEvent : IInputEventBase
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public string m_MessgaeType = "";

    /// <summary>
    /// 消息内容
    /// </summary>
    public string m_content = "";

    protected override string GetEventKey()
    {
        return m_MessgaeType;
    }
}

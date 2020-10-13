
using System.Collections;

namespace SimpleTCP
{
    /// <summary>
    /// Disconnect reason that you receive in OnPeerDisconnected event
    /// </summary>
    public enum DisconnectReason
    {
        None,
        ConnectionFailed,

        /// <summary>
        /// 本地主动断开连接
        /// </summary>
        DisconnectPeerCalled,
        
    }
}

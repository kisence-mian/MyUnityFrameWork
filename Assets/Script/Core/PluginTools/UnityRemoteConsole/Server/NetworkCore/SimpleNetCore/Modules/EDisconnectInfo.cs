using System.Net.Sockets;
namespace SimpleNetCore
{
    /// <summary>
    /// Disconnect reason that you receive in OnPeerDisconnected event
    /// </summary>
    public enum EDisconnectReason
    {
        ConnectionFailed,
        /// <summary>
        /// 心跳包超时
        /// </summary>
        Timeout,
        HostUnreachable,
        NetworkUnreachable,
        RemoteConnectionClose,
        /// <summary>
        /// 本地主动断开连接
        /// </summary>
        DisconnectPeerCalled,
        ConnectionRejected,
        InvalidProtocol,
        UnknownHost,
        Reconnect
    }

    /// <summary>
    /// Additional information about disconnection
    /// </summary>
    public struct EDisconnectInfo
    {
        /// <summary>
        /// Additional info why peer disconnected
        /// </summary>
        public EDisconnectReason Reason;

        /// <summary>
        /// Error code (if reason is SocketSendError or SocketReceiveError)
        /// </summary>
        public SocketError SocketErrorCode;


        public EDisconnectInfo(EDisconnectReason Reason, SocketError SocketErrorCode)
        {
            this.Reason = Reason;
            this.SocketErrorCode = SocketErrorCode;
        }
    }
}

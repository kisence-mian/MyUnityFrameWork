namespace SimpleNetCore
{
    public delegate void MessageHandlerDelegate(NetMessageData msgHandler);
    /// <summary>
    /// 消息分发处理
    /// </summary>
    public abstract class  IMessageHandler 
    {
        protected bool isServer;
        public  IMessageHandler(bool isServer)
        {
            this.isServer = isServer;
        }
        public abstract  void DispatchMessage(Session session, object msgType,object msgData);

        public virtual void OnConnectedEvent(Session session) { }

        public virtual void OnDisconnectedEvent(Session session, EDisconnectInfo disconnectInfo) { }

        public virtual void RegisterMsgEvent<T>(MessageHandlerDelegate handlerDelegate) { }

        public virtual void UnregisterMsgEvent<T>(MessageHandlerDelegate handlerDelegate) { }
    }
}
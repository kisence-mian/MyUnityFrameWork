namespace SimpleNetCore
{
    public abstract class INetworkTransport
    {
        public bool IsServer { get; private set; }
       public INetworkTransport(bool isServer)
        {
            IsServer = isServer;
        }
       public abstract bool Start(int port);
        public abstract bool Connect(string address, int port);

        public abstract bool Receive(out TransportEventData eventData);
        public abstract void Destroy();


        public abstract bool Send(long connectionId, byte[] data);
        public abstract bool Disconnect(long connectionId,EDisconnectReason disconnectReason);
    }
}

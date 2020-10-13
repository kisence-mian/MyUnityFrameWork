// incoming message queue of <connectionId, message>
// (not a HashSet because one connection can have multiple new messages)
// -> a struct to minimize GC
namespace SimpleTCP
{
    public struct Message
    {
        public readonly int connectionId;
        public readonly EventType eventType;
        public readonly byte[] data;
        public readonly DisconnectReason disconnectReason;
        public Message(int connectionId, EventType eventType, byte[] data)
        {
            this.connectionId = connectionId;
            this.eventType = eventType;
            this.data = data;
            this.disconnectReason =  DisconnectReason.None;
        }
        public Message(int connectionId, EventType eventType, byte[] data, DisconnectReason disconnectReason)
        {
            this.connectionId = connectionId;
            this.eventType = eventType;
            this.data = data;
            this.disconnectReason = disconnectReason;
        }
    }
}

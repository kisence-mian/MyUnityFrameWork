
namespace SimpleNetCore
{
    public class NetMessageData
    {
        public object msgType { get; private set; }
        public Session session { get; private set; }
        public object msgData { get; private set; }
        public NetMessageData(object msgType, Session session, object msgData)
        {
            this.msgType = msgType;
            this.session = session;
            this.msgData = msgData;

        }

        public T GetMessage<T>() where T:new()
        {
            //int pos = message.Position;
            //T t = serializer.Deserialize<T>(message);
            //message.SetSource(message.RawData, pos);
            //Debug.Log("ReceveMsg <=::" + msgType + " ==>:" + JsonUtility.ToJson(t));
            if (msgData != null)
                return (T)msgData;
            return default(T);
        }
    }
}

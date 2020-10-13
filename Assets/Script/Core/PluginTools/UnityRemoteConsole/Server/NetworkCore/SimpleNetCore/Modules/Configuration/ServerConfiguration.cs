
using System.Collections;

namespace SimpleNetCore
{
    public class ServerConfiguration : NetConfiguration
    {
        public ServerConfiguration(INetworkTransport transport) : base(transport)
        {
        }

        /// <summary>
        ///  默认服务端配置
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="serializer"></param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public static ServerConfiguration NewDefaultConfiguration(INetworkTransport transport, INetMsgSerializer serializer, IMessageHandler messageHandler)
        {
            return (ServerConfiguration)new ServerConfiguration(transport)
                .AddPlugin(new DataTypeMsgProcessPlugin())
                .AddPlugin(new NetHeartBeatPongPlugin())
                .AddPlugin(new NetPongPlugin())
                .AddMsgSerializer(serializer)
                .AddMessageHander(messageHandler)
                 .SetByteOrder(ByteOrder.BIG_ENDIAN);
        }


    }
}


using System.Collections;
namespace SimpleNetCore
{
    public class ClientConfiguration : NetConfiguration
    {
        public ClientConfiguration(INetworkTransport transport) : base(transport)
        {
        }
        /// <summary>
        /// 默认客户端配置
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="serializer"></param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public static ClientConfiguration NewDefaultConfiguration(INetworkTransport transport, INetMsgSerializer serializer, IMessageHandler messageHandler)
        {
            return (ClientConfiguration)new ClientConfiguration(transport)
                .AddPlugin(new DataTypeMsgProcessPlugin())
                .AddPlugin(new NetHeartBeatPingPlugin())
                .AddPlugin(new NetPingPlugin())
                .AddPlugin(new AutoReconnectPlugin())
                 .AddMsgSerializer(serializer)
                .AddMessageHander(messageHandler)
                .SetByteOrder(ByteOrder.BIG_ENDIAN);
        }

        #region 启用Ping 仅限客户端使用
        private bool enablePing = false;
        public NetConfiguration EnablePing()
        {
            enablePing = true;
            return this;
        }
        #endregion


        private bool enableReconnect = false;
        public NetConfiguration EnableReconnect()
        {
            enableReconnect = true;
            return this;
        }

        internal AutoReconnectPlugin GetReconnectPlugin()
        {
            AutoReconnectPlugin pingPlugin = null;
            NetMsgProcessPluginBase p = GetPlugin(99);
            if (p != null)
            {
                 pingPlugin = (AutoReconnectPlugin)p;
                
            }
            return pingPlugin;
        }
        internal override void Init(NetworkCommon networkCommon)
        {
            {
                NetMsgProcessPluginBase p = GetPlugin((byte)NetProperty.Pong);
                if (p != null)
                {
                    NetPingPlugin pingPlugin = (NetPingPlugin)p;
                    pingPlugin.Enable = enablePing;
                }
            }
            {
                AutoReconnectPlugin p = GetReconnectPlugin();
                if (p != null)
                {
                    p.Enable = enableReconnect;
                }
            }
            base.Init(networkCommon);
        }
    }
}

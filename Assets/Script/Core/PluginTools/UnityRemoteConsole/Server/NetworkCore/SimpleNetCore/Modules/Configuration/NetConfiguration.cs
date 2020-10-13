
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNetCore
{
    public class NetConfiguration 
    {

        private INetworkTransport transport;
        public INetworkTransport Transport
        {
            get
            {
                return transport;
            }
        }
        public  NetConfiguration(INetworkTransport transport)
        {
            this.transport = transport;
        }

        #region 设置大端，小端，字节顺序
        /// <summary>
        /// 设置大端，小端，字节顺序
        /// </summary>
        public ByteOrder byteOrder { get; private set; }
        public NetConfiguration SetByteOrder(ByteOrder byteOrder)
        {
            this.byteOrder = byteOrder;
            return this;
        }
        #endregion

        #region 启用统计
        public bool UseStatistics { get; private set; }
        /// <summary>
        /// 启用统计
        /// </summary>
        /// <returns></returns>
        public NetConfiguration EnableStatistics()
        {
            UseStatistics = true;
            return this;
        }
        #endregion;

        #region 消息处理类
        private IMessageHandler messageHander;
        public NetConfiguration AddMessageHander(IMessageHandler messageHander)
        {
            this.messageHander = messageHander;
            return this;
        }
        public IMessageHandler GetMessageHander()
        {
            return messageHander;
        }
        #endregion

        #region 消息序列化
        private INetMsgSerializer serializer;

        public NetConfiguration AddMsgSerializer(INetMsgSerializer serializer)
        {
            this.serializer = serializer;
            return this;
        }
        public INetMsgSerializer GetMsgSerializer()
        {
            return serializer;
        }
        #endregion

        #region 消息类型处理插件
        private Dictionary<byte, NetMsgProcessPluginBase> plugins = new Dictionary<byte, NetMsgProcessPluginBase>();
        private List<NetMsgProcessPluginBase> pluginList = new List<NetMsgProcessPluginBase>();
        public NetConfiguration AddPlugin(NetMsgProcessPluginBase plugin)
        {
            if (plugin == null)
            {
                throw new System.Exception("NetConfiguration.AddPlugin Exception");
            }
            if (plugins.ContainsKey(plugin.GetNetProperty()))
            {
                NetMsgProcessPluginBase old = plugins[plugin.GetNetProperty()];
                plugins.Remove(plugin.GetNetProperty());
                pluginList.Remove(old);
            }

            plugins.Add(plugin.GetNetProperty(), plugin);
            pluginList.Add(plugin);

            return this;
        }

        public NetMsgProcessPluginBase GetPlugin(byte property)
        {
            NetMsgProcessPluginBase plugin = null;

            plugins.TryGetValue(property, out plugin);

            return plugin;
        }
        public List<NetMsgProcessPluginBase> GetNetMsgProcessPlugins()
        {
            return pluginList;
        }
        #endregion

        #region 加密
        private MsgEncryptionBase msgEncryption = new MsgEncryptionRC4(0);
        public bool IsEncryption { get; private set; }
        /// <summary>
        /// 开启加密
        /// </summary>
        /// <returns></returns>
        public NetConfiguration EnableEncryption()
        {
            IsEncryption = true;
            return this;
        }
        //public NetConfiguration AddMsgEncryption(MsgEncryptionBase msgEncryption)
        //{
        //    this.msgEncryption = msgEncryption;
        //    return this;
        //}
        public MsgEncryptionBase GetMsgEncryption()
        {
            return this.msgEncryption;
        }
        #endregion

        #region 消息压缩
        private Dictionary<byte, MsgCompressBase> compressFun = new Dictionary<byte, MsgCompressBase>();
        private MsgCompressBase sendMsgCompress = null;

        private bool isAddAllCompressFun = false;
        private void AddAllCompressFun()
        {
            if (isAddAllCompressFun)
                return;
            isAddAllCompressFun = true;

          Type[] types=  ReflectionTool.FastGetChildTypes(typeof(MsgCompressBase),false);
            foreach (var t in types)
            {
                MsgCompressBase obj = (MsgCompressBase)ReflectionTool.CreateDefultInstance(t);
                compressFun.Add(obj.CompressType,obj);
            }

        }
        /// <summary>
        /// 设置默认发消息压缩方式
        /// </summary>
        /// <param name="compressTypeName">"gzip"</param>
        /// <returns></returns>
        public NetConfiguration SetMsgCompress(string compressTypeName)
        {
            AddAllCompressFun();

            if (string.IsNullOrEmpty(compressTypeName))
                return this;
            compressTypeName = compressTypeName.ToLower();
            foreach (var item in compressFun.Values)
            {
                if (item.CompressTypeName.ToLower() == compressTypeName)
                {
                    sendMsgCompress = item;
                    return this;
                }
            }
            throw new Exception("No Compress compressTypeName：" + compressTypeName);
           
        }
        /// <summary>
        /// 使用byte编号设置发消息压缩方式
        /// </summary>
        /// <param name="compressType"></param>
        /// <returns></returns>
        public NetConfiguration SetMsgCompress(byte compressType)
        {
            AddAllCompressFun();

            foreach (var item in compressFun.Values)
            {
                if (item.CompressType == compressType)
                {
                    sendMsgCompress = item;
                    return this;
                }
            }
            throw new Exception("No Compress compressType：" + compressType);

        }
        public MsgCompressBase GetSendCompressFunction()
        {
            return sendMsgCompress;
        }
        public MsgCompressBase GetCompressFunction(byte compressType)
        {
            MsgCompressBase compress = null;

            compressFun.TryGetValue(compressType, out compress);

            return compress;
        }
        #endregion

        public bool UseMultithreading { get { return useMultithreading; } }
        private bool useMultithreading = true;
        /// <summary>
        /// 不使用多线程收发消息
        /// </summary>
        /// <returns></returns>
        public NetConfiguration DisableMultithreading()
        {
            useMultithreading = false;
            return this;
        }
        // Use this for initialization
        internal virtual void Init(NetworkCommon networkCommon)
        {
            foreach (var plugin in plugins)
            {
                plugin.Value.Init(networkCommon);
            }
            AddAllCompressFun();
            foreach (var compress in compressFun)
            {
                compress.Value.Init();
            }

            if (msgEncryption != null)
            {
                msgEncryption.Init(byteOrder);
            }

            if (serializer != null)
            {
                serializer.Init(this);
            }
        }

        // Update is called once per frame
        internal void Release()
        {
            foreach (var plugin in plugins)
            {
                plugin.Value.Release();
            }
            plugins.Clear();
            pluginList.Clear();

            foreach (var compress in compressFun)
            {
                compress.Value.Release();
            }
            compressFun.Clear();
            if (msgEncryption != null)
            {
                msgEncryption.Release();
                msgEncryption = null;
            }

            if (transport != null)
            {
                transport.Destroy();
                transport = null;
            }
        }
    }
}

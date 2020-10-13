
using System;
using System.Collections;
namespace SimpleNetCore
{
    public struct MsgPackest
    {
        /// <summary>
        /// 当前消息版本
        /// </summary>
        public const byte ProtocolVersion = 0;

       
        #region 消息头
        /// <summary>
        /// 协议版本
        /// </summary>
        public byte protocolVer;
        /// <summary>
        /// 消息类型
        /// </summary>
        public byte msgProperty;
        /// <summary>
        /// 是否加密 0:不加密，1加密
        /// </summary>
        public byte isEncryption;
        /// <summary>
        /// 是否压缩 0：不压缩，1压缩
        /// </summary>
        public byte isCompress;
        /// <summary>
        /// 压缩类型
        /// </summary>
        public byte compressType;

        #endregion
      
        /// <summary>
        /// 消息内容
        /// </summary>
        public byte[] contents;


        /// <summary>
        /// 收发消息计数器（标明消息序列号）
        /// </summary>
        //public uint counter;
        public Session session;

        private  NetDataReader reader ;
        internal ByteOrder byteOrder;
        internal MsgPackest(ByteOrder byteOrder, Session session, byte[] data)
        {
            if (data.Length < 5)
                throw new Exception("消息Data 没有完整的消息头");

            //counter = 0;
           this. session = session;
            this.byteOrder = byteOrder;
            reader = new NetDataReader(byteOrder);

            reader.SetSource(data, 0);
            protocolVer = reader.GetByte();
            msgProperty = reader.GetByte();
            isEncryption = reader.GetByte();
            isCompress = reader.GetByte();
            compressType = reader.GetByte();
            contents = new byte[data.Length - 5];
            reader.GetBytes(contents, data.Length - 5);

            if (protocolVer != ProtocolVersion)
            {
                throw new Exception("消息版本不一致：" + protocolVer + " Loacal:" + ProtocolVersion);
            }
        }
        
        internal static byte[] Write2Bytes(ByteOrder byteOrder, byte isEncryption, byte isCompress,byte compressType, byte msgProperty, byte[] contents)
        {
            NetDataWriter writer = new NetDataWriter(byteOrder);
            writer.Reset();

            writer.Put(ProtocolVersion);
            writer.Put(msgProperty);
            writer.Put(isEncryption);
            writer.Put(isCompress);
            writer.Put(compressType);
            writer.PutByteSource(contents);
            return writer.CopyData();
        }
    }
}

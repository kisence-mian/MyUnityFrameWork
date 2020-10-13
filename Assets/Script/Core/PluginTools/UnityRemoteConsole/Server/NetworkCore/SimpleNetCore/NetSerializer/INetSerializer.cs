namespace SimpleNetCore
{
    public  interface INetMsgSerializer
    {

        void Init(NetConfiguration configuration);
        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object GetMsgType(object data);
        /// <summary>
        /// 消息序列化
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Serialize(object msgType, object data);
        /// <summary>
        /// 消息反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        object Deserialize(byte[] datas, out object msgType);
    }
}

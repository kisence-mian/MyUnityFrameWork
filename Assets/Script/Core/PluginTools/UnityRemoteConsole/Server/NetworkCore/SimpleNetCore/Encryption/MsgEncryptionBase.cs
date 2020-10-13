
using System.Collections;
namespace SimpleNetCore
{
    /// <summary>
    /// 消息加密,解密
    /// </summary>
    public abstract class MsgEncryptionBase
    {
       public int SecurityLevel { get; private set; }
        public MsgEncryptionBase(int securityLevel)
        {
            SecurityLevel = securityLevel;
        }
        public virtual void Init(ByteOrder byteOrder) { }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public abstract byte[] Encryption(Session session, byte[] datas);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public abstract byte[] Decryption(Session session, byte[] datas);

        public virtual void Release() { }
    }
}

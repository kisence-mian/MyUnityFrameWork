
using System.Collections;
namespace SimpleNetCore
{
    public abstract class MsgCompressBase
    {
        public abstract byte CompressType
        {
            get;
        }
        /// <summary>
        /// 全部小写
        /// </summary>
        public abstract string CompressTypeName
        {
             get;
        }
        public abstract void Init();
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public abstract byte[] Compress(byte[] datas);
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public abstract byte[] Decompress(byte[] datas);

        public abstract void Release();
    }
}

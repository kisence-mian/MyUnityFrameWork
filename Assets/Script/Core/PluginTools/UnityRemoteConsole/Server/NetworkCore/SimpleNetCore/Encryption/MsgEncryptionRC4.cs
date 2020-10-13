using System.Text;
using System;

namespace SimpleNetCore
{
    public class MsgEncryptionRC4 : MsgEncryptionBase
    {
        /// <summary>
        /// 随机使用的序列号
        /// </summary>
        private const string Key_Index = "rc4_Index";
        /// <summary>
        /// 生成的密码
        /// </summary>
        private const string Key_Password = "rc4_Password";

        private EndianBitConverter bitConverter;
        public MsgEncryptionRC4(int securityLevel) : base(securityLevel)
        {
        }
        public override void Init(ByteOrder byteOrder)
        {
            bitConverter = EndianBitConverter.GetBitConverter(byteOrder);
        }
        public override byte[] Decryption(Session session, byte[] datas)
        {
            int index = bitConverter.ToInt32(datas, 0);
            byte[] dataArray = new byte[datas.Length - 4];
            //NetDebug.Log("获得Index：" + index);
            //Debug.Log("property:" + property + " tempEventData.data.Length:" + tempEventData.data.Length + " dataArray:" + dataArray.Length) ;
            Array.Copy(datas, 4, dataArray, 0, dataArray.Length);

            byte[] pwd = GetPassword(session, index);

            return Decrypt(pwd, dataArray);
        }

        public override byte[] Encryption(Session session, byte[] datas)
        {
            int index = 0;
            byte[] pwd = null;
            if (!session.ContainsAttributeKey(Key_Index))
            {
                 index= new FixRandom(DateTime.Now.Second).Range(1, 999999);
                session.SetAttribute(Key_Index, index);
                 pwd = GetRandomKey(16, index);
                session.SetAttribute(Key_Password, pwd);
            }
            //NetDebug.Log("加密的Index：" + index);
            pwd = GetPassword(session, index);

            datas = Encrypt(pwd, datas);

            byte[] pBytes = bitConverter.GetBytes(index);
            byte[] allDatas = new byte[pBytes.Length + datas.Length];
            pBytes.CopyTo(allDatas, 0);
            datas.CopyTo(allDatas, pBytes.Length);
            return allDatas;

        }

        private byte[] GetPassword(Session session,int index)
        {
            int localIndex = 0;
            if (session.ContainsAttributeKey(Key_Index))
            {
                localIndex = (int)session.GetAttribute(Key_Index);
            }
            byte[] pwd = null;
            if (localIndex == 0|| index!=localIndex)
            {
                session.SetAttribute(Key_Index, index);
                pwd = GetRandomKey(16, index);
                session.SetAttribute(Key_Password, pwd);
                //Debug.Log("生成pwd");
            }
            else
            {
                pwd = (byte[])session.GetAttribute(Key_Password);
                //Debug.Log("获取pwd:"+pwd);
            }
            return pwd;
        }

     private readonly  char[] arrChar = new char[]{
       'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
       '0','1','2','3','4','5','6','7','8','9',
       'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
    };
        /// <summary>
        /// 随机生成密钥
        /// </summary>
        /// <param name="n">位数</param>
        /// <returns></returns>
        private  byte[] GetRandomKey(int n,int seed)
        {
            StringBuilder num = new StringBuilder();

            FixRandom rnd = new FixRandom(seed);
            for (int i = 0; i < n; i++)
            {
                num.Append(arrChar[rnd.Range(0, arrChar.Length)].ToString());
            }
            //NetDebug.Log("seed:"+ seed+ "生成密码：" +num);
            return Encoding.UTF8.GetBytes(num.ToString());
        }
    

        public override void Release()
        {
            
        }
        private byte[] Encrypt(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            //打乱密码,密码,密码箱长度
            cipher = new byte[data.Length];
            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {

                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }

        private byte[] Decrypt(byte[] pwd, byte[] data)
        {
            return Encrypt(pwd, data);
        }

    }
}

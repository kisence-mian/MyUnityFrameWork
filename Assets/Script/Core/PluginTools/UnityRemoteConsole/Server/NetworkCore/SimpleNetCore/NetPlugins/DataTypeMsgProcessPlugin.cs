using System;

namespace SimpleNetCore
{
    public class DataTypeMsgProcessPlugin : NetMsgProcessPluginBase
    {
        public override byte GetNetProperty()
        {
            return (byte)NetProperty.Data;
        }

        public override void Release()
        {
            
        }

        protected override void OnInit()
        {
           
        }
        private EndianBitConverter bitConverter=null;
        public override void ReceveProcess( MsgPackest packest)
        {
            Session session = packest.session;
            if (packest.isEncryption == 1)
            {
                MsgEncryptionBase encryption = networkCommon.Configuration.GetMsgEncryption();
                if (encryption == null)
                {
                    NetDebug.LogError("不支持消息解密："+packest);
                    return;
                }
                else
                {
                    try
                    {
                        packest.contents = encryption.Decryption(packest.session, packest.contents);
                    }
                    catch (System.Exception e)
                    {
                        NetDebug.LogError("消息解密错误：" + packest+" \n"+e);
                        return;
                    }
                }

            }
            if (packest.isCompress == 1)
            {
                MsgCompressBase compress = networkCommon.Configuration.GetCompressFunction(packest.compressType);
                if (compress == null)
                {
                    NetDebug.LogError("不支持的压缩方式：" + packest.compressType);
                    return;
                }
                else
                {
                    packest.contents = compress.Decompress(packest.contents);
                    //NetDebug.Log("解压缩:"+ packest.contents.Length);
                }
            }
            if(bitConverter==null || bitConverter.byteOrder != packest.byteOrder)
            {
                bitConverter = EndianBitConverter.GetBitConverter(packest.byteOrder);
            }
            //收发消息计数器（标明消息序列号）
            uint counter = bitConverter.ToUInt32(packest.contents, 0);

            //if (session.CheckReceiveMsgCounter(counter))
            //{
            //    session.SetReceiveCounter(counter);
            //}
            //else
            //{
            //    NetDebug.LogError("packest.counter error:" + counter + "  session.ReceiveMsgCounter：" + (session.ReceiveMsgCounter + 1));
            //    return;
            //}
            byte[] dataArray = new byte[packest.contents.Length - 4];
            //NetDebug.Log("packest.contents.Length:" + packest.contents.Length + " dataArray:" + dataArray.Length) ;
            Array.Copy(packest.contents, 4, dataArray, 0, dataArray.Length);
            packest.contents = dataArray;
            networkCommon.ReceiveMsgPackest(packest);
        }

        public override byte[] SendProcess(Session session, byte msgProperty, byte[] datas)
        {
            ByteOrder byteOrder = networkCommon.Configuration.byteOrder;
            byte compressType = 0;
            byte isEncryption = 0;
            byte isCompress = 0;

            if (bitConverter == null || bitConverter.byteOrder != byteOrder)
            {
                bitConverter = EndianBitConverter.GetBitConverter(byteOrder);
            }
            byte[] pBytes = bitConverter.GetBytes(session.AddSendCounter());
            byte[] allDatas = new byte[pBytes.Length + datas.Length];
            pBytes.CopyTo(allDatas, 0);
            datas.CopyTo(allDatas, pBytes.Length);
            datas = allDatas;

            MsgCompressBase compress = networkCommon.Configuration.GetSendCompressFunction();
            if (compress != null)
            {
                isCompress = 1;
                try
                {

                    //NetDebug.Log("压缩消息：" + datas.Length);
                    datas = compress.Compress(datas);
                    //NetDebug.Log("压缩后消息：" + datas.Length);
                }
                catch (Exception e)
                {
                    NetDebug.LogError("压缩错误:" + e);
                    return null;
                }
                compressType = compress.CompressType;

            }
            else
            {
                isCompress = 0;
            }


            MsgEncryptionBase encryption = networkCommon.Configuration.GetMsgEncryption();
            if (networkCommon.Configuration.IsEncryption && encryption != null)
            {
                isEncryption = 1;
                try
                {
                    datas = encryption.Encryption(session, datas);
                }
                catch (Exception e)
                {
                    NetDebug.LogError("加密错误：" + e);
                    return null;
                }
            }
            else
            {
                isEncryption = 0;
            }


            byte[] res = MsgPackest.Write2Bytes(networkCommon.Configuration.byteOrder, isEncryption, isCompress, compressType, msgProperty, datas);
            return res;
        }

    }
}

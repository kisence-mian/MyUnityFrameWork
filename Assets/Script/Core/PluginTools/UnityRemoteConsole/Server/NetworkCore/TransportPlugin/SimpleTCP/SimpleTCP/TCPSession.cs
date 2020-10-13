
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleTCP
{
    public class TCPSession
    {
        private Socket m_Socket = null;
        public Socket Socket
        {
            get
            {
                return m_Socket;
            }
        }
        private DynamicBuffer receiveBuffer;

        private bool _Connected = false;
        public bool Connected
        {
            get
            {
                if (m_Socket == null)
                    return false;
                return _Connected;
            }
        }
        private int maxPacketLength = 50 * 1024 * 1024;
        public int MaxPacketLength
        {
            get
            {
                return maxPacketLength;
            }
            set
            {
                maxPacketLength = value;
            }
        }

        public Action<byte[]> OnRecevePackets;
        public Action<DisconnectReason> OnDisconnect;

        private byte[] m_readData = new byte[1024 * 1024];
        private int m_offset = 0;
        //Thread receiveThread;
        public TCPSession()
        {
           
            receiveBuffer = new DynamicBuffer(m_readData.Length);
        }
        public void Connect(string ip, int port)
        {

            IPAddress ipAddr = null;
            if (!IPAddress.TryParse(ip, out ipAddr))
            {
                IPHostEntry IPinfo = Dns.GetHostEntry(ip);
                IPAddress[] ipList = IPinfo.AddressList;
                ipAddr = ipList[0];
            }

            IPEndPoint ipe = new IPEndPoint(ipAddr, port);
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.Connect(ipe);


            _Connected = true;
            receiveBuffer.Clear();


            StartReceive();
        }
        void StartReceive()
        {
            m_Socket.BeginReceive(m_readData, m_offset, m_readData.Length, SocketFlags.None, EndReceive, m_Socket);
        }

        void EndReceive(IAsyncResult iar) //接收数据
        {
            Socket remote = (Socket)iar.AsyncState;
            int recvCount = remote.EndReceive(iar);
            if (recvCount > 0)
            {
                //Logger.LogError("1.接收到了数据：" + recvCount + " receiveBuffer.Length:" + receiveBuffer.Length);
                receiveBuffer.WriteBuffer(m_readData, m_offset, recvCount);
               
                //Logger.LogError("2.接收到了数据：" + recvCount + " receiveBuffer.Length:" + receiveBuffer.Length);
                int packetCount = 0;
                while (ReadData())
                {
                    packetCount++;
                }
                //Logger.LogError("3.接收到了数据：" + recvCount + " receiveBuffer.Length:" + receiveBuffer.Length+ " packetCount:"+ packetCount);
            }

            StartReceive();
        }

        private bool ReadData()
        {
            //Logger.Log("接收到了数据receiveBuffer：" + receiveBuffer.Length);
            int headerLength = sizeof(int);
            if (receiveBuffer.Length > headerLength)
            {

                byte[] header = new byte[headerLength];
                if (receiveBuffer.ReadBuffer(header, 0, headerLength))
                {
                    //按照长度分包  
                    int packetLength = Utils.BytesToIntBigEndian(header); //获取包长度  
                    //Logger.Log("包大小：" + packetLength);
                    if ((packetLength > maxPacketLength) | (receiveBuffer.Length > maxPacketLength)) //最大Buffer异常保护  
                    {
                        receiveBuffer.Clear();
                        Logger.Log("Buffer异常保护：" + receiveBuffer.Length);
                        return false;
                    }
                    else
                    {
                        //缓存的数据不够一个完整的包
                        if (receiveBuffer.Length < (headerLength + packetLength))
                            return false;
                        //Logger.Log("Buffer大小：" + receiveBuffer.Length);
                        byte[] dataBytes = new byte[packetLength];
                        if (receiveBuffer.ReadBuffer(dataBytes, headerLength, packetLength))
                        {
                            //Logger.Log("ReadBuffer：" + receiveBuffer.Length);
                            receiveBuffer.Remove(headerLength + packetLength);
                            if (OnRecevePackets != null)
                            {
                                OnRecevePackets(dataBytes);
                            }
                            //Logger.Log("ReadBuffer Remove：" + receiveBuffer.Length);
                        }
                    }

                }


            }
            else
            {
                return false;
            }
            return true;
        }


        public void Send(byte[] data)
        {
            try
            {
                m_Socket.Send(data);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                Disconnect( DisconnectReason.ConnectionFailed);
            }
           
        }
        public void Disconnect(DisconnectReason disconnectReason)
        {
            //Logger.Log("TCp Disconnect");
            if (_Connected)
            {
                m_Socket?.Close();
                if (OnDisconnect != null)
                {
                    OnDisconnect(disconnectReason);
                }
            }
            _Connected = false;   
        }
        public void Close(DisconnectReason disconnectReason)
        {
            Disconnect(disconnectReason);
            receiveBuffer.Clear();
        }

    }
}

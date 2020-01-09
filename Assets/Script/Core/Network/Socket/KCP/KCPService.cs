#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class KCPService : SocketBase
{
    private static readonly DateTime utc_time = new DateTime(1970, 1, 1);

    public static UInt32 iclock()
    {
        return (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(utc_time).TotalMilliseconds) & 0xffffffff);
    }

    private const UInt32 CONNECT_TIMEOUT = 5000;
    private const UInt32 RESEND_CONNECT = 500;

    private UdpClient mUdpClient;
    private IPEndPoint mIPEndPoint;
    private IPEndPoint mSvrEndPoint;

    private KCP mKcp;
    private bool mNeedUpdateFlag;
    private UInt32 mNextUpdateTime;

    //private bool mInConnectStage;
    //private bool mConnectSucceed;
    private UInt32 mConnectStartTime;
    private UInt32 mLastSendConnectTime;

    private SwitchQueue<byte[]> mRecvQueue = new SwitchQueue<byte[]>(128);

    public override void Connect()
    {
        Debug.Log("Connect " + m_IPaddress + " : " + m_port);

        mSvrEndPoint = new IPEndPoint(IPAddress.Parse(m_IPaddress), m_port);
        mUdpClient = new UdpClient(m_IPaddress, m_port);
        m_connectStatusCallback(NetworkState.Connecting);
        try
        {
            mUdpClient.Connect(mSvrEndPoint);
            isConnect = true;
            m_connectStatusCallback(NetworkState.Connected);
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }


        reset_state();
        init_kcp(1);
        //mInConnectStage = true;
        mConnectStartTime = iclock();

        mUdpClient.BeginReceive(ReceiveCallback, this);
       
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        Byte[] data = (mIPEndPoint == null) ?
            mUdpClient.Receive(ref mIPEndPoint) :
            mUdpClient.EndReceive(ar, ref mIPEndPoint);

        if (null != data)
            OnData(data);

        if (mUdpClient != null)
        {
            // try to receive again.
            mUdpClient.BeginReceive(ReceiveCallback, this);
        }
    }

    void OnData(byte[] buf)
    {
        //Debug.Log("收到消息");
        mRecvQueue.Push(buf);
    }

    void reset_state()
    {
        mNeedUpdateFlag = false;
        mNextUpdateTime = 0;

        //mInConnectStage = false;
        //mConnectSucceed = false;
        mConnectStartTime = 0;
        mLastSendConnectTime = 0;
        mRecvQueue.Clear();
        mKcp = null;
    }

    string dump_bytes(byte[] buf, int size)
    {
        var sb = new StringBuilder(size * 2);
        for (var i = 0; i < size; i++)
        {
            sb.Append(buf[i]);
            sb.Append(" ");
        }
        return sb.ToString();
    }

    void init_kcp(UInt32 conv)
    {
        mKcp = new KCP(conv, (byte[] buf, int size) =>
        {
            try
            {
                mUdpClient.Send(buf, size);
            }catch(Exception e)
            {
                Debug.LogError(e);
            }
        });

        mKcp.NoDelay(1, 10, 2, 1);
        mKcp.WndSize(128, 128);
    }

    public override void Send(byte[] buf)
    {
        mKcp.Send(buf);
        mNeedUpdateFlag = true;
    }

    public override void Update()
    {
        update(iclock());
    }

    public override void Close()
    {
        mUdpClient.Close();
        //evHandler(cliEvent.Disconnect, null, "Closed");
        m_connectStatusCallback(NetworkState.ConnectBreak);
    }

    //void process_connect_packet()
    //{
    //    mRecvQueue.Switch();

    //    if (!mRecvQueue.Empty())
    //    {
    //        var buf = mRecvQueue.Pop();

    //        //UInt32 conv = 1;
    //        //KCP.ikcp_decode32u(buf, 0, ref conv);

    //        //if (conv <= 0)
    //        //    throw new Exception("inlvaid connect back packet");

    //        //init_kcp(conv);

    //        //mInConnectStage = false;
    //        //mConnectSucceed = true;

    //        //m_connectStatusCallback(NetworkState.Connected);
    //        //evHandler(cliEvent.Connected, null, null);
    //    }
    //}

    void process_recv_queue(UInt32 current)
    {
        mRecvQueue.Switch();

        while (!mRecvQueue.Empty())
        {
           
            var buf = mRecvQueue.Pop();
          
           int input= mKcp.Input(buf);
            mNeedUpdateFlag = true;
            //mKcp.Update(current);
            //Debug.Log("process_recv_queue :" + buf.Length + "  input:"+ input+" PeekSize:" + mKcp.PeekSize());
            for (var size = mKcp.PeekSize(); size > 0; size = mKcp.PeekSize())
            {
                var buffer = new byte[size];
                if (mKcp.Recv(buffer) > 0)
                {
                    int offset = 0;
                    //Debug.Log("m_byteCallBack ---------");
                    m_byteCallBack(buffer, ref offset, buffer.Length);
                }
            }
        }
    }

    bool connect_timeout(UInt32 current)
    {
        return current - mConnectStartTime > CONNECT_TIMEOUT;
    }

    bool need_send_connect_packet(UInt32 current)
    {
        return current - mLastSendConnectTime > RESEND_CONNECT;
    }

    void update(UInt32 current)
    {
        //if (mInConnectStage)
        //{
        //    if (connect_timeout(current))
        //    {
        //        m_connectStatusCallback(NetworkState.ConnectBreak);
        //        isConnect = false;
        //        mInConnectStage = false;
        //        return;
        //    }

        //    if (need_send_connect_packet(current))
        //    {
        //        mLastSendConnectTime = current;
        //        mUdpClient.Send(new byte[4] { 0, 0, 0, 0 }, 4);
        //    }

        //    process_connect_packet();

        //    return;
        //}

        if (isConnect)
        {
            process_recv_queue(current);

            if (mNeedUpdateFlag || current >= mNextUpdateTime)
            {
                mKcp.Update(current);
                mNextUpdateTime = mKcp.Check(current);
                //Debug.Log("mNextUpdateTime :" + (current));
                mNeedUpdateFlag = false;
            }
        }
        //SendHeartPackage(current);
    }
    private const uint SEND_HEART_PACK_TIME = 2000;
    private uint tempTime;
    //private void SendHeartPackage(UInt32 current)
    //{
    //    if(tempTime < current)
    //    {
    //        tempTime = current + SEND_HEART_PACK_TIME;
    //        Send(new byte[4] { 0, 0, 0, 0 });
    //        Debug.Log("SendHeartPackage");
    //    }
    //}
}

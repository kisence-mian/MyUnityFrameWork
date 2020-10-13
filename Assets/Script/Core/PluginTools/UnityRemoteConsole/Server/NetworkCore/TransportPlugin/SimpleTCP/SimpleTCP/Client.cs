using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleTCP
{
    public class Client : Common
    {
        Thread receiveThread;
        Thread sendThread;

        TCPSession session = new TCPSession();

        public bool Connected
        {
            get
            {
                if (session != null)
                {
                    return session.Connected;
                }
                return false;
            }
        }

        // send queue
        // => SafeQueue is twice as fast as ConcurrentQueue, see SafeQueue.cs!
        SafeQueue<byte[]> sendQueue = new SafeQueue<byte[]>();

        // ManualResetEvent to wake up the send thread. better than Thread.Sleep
        // -> call Set() if everything was sent
        // -> call Reset() if there is something to send again
        // -> call WaitOne() to block until Reset was called
        ManualResetEvent sendPending = new ManualResetEvent(false);

        public void Connect(string ip, int port)
        {
            // not if already started
            if (Connected) return; 
             
            // clear old messages in queue, just to be sure that the caller
            // doesn't receive data from last time and gets out of sync.
            // -> calling this in Disconnect isn't smart because the caller may
            //    still want to process all the latest messages afterwards
            receiveQueue = new ConcurrentQueue<Message>();
            sendQueue.Clear();

            receiveThread = new Thread(() => { receiveLoop( session, ip, port); });
            receiveThread.IsBackground = true;
            receiveThread.Start();
           
          
          
            //Logger.Log("2 session.Connected" + session.Connected);
        }
        private void receiveLoop(TCPSession session, string ip, int port)
        {
            try
            {
                session.Connect(ip, port);
            }
            catch (Exception e)
            {
                receiveQueue.Enqueue(new Message(0, EventType.Disconnected, null, DisconnectReason.ConnectionFailed));
                throw e;
            }
            //Logger.Log("0 session.Connected" + session.Connected);
            // run the receive loop
            ReceiveLoop(0, session, receiveQueue);
            //Logger.Log("1 session.Connected" + session.Connected);
            //sendThread?.Interrupt();
            sendThread = new Thread(() => { SendLoop(0, session, sendQueue, sendPending); });
            sendThread.IsBackground = true;
            sendThread.Start();
        }
        public void Disconnect()
        {

            session.Disconnect( DisconnectReason.DisconnectPeerCalled);
            sendThread?.Interrupt();
            receiveThread?.Interrupt();
        }

        public bool Send(byte[] data)
        {
            // add to send queue and return immediately.
            // calling Send here would be blocking (sometimes for long times
            // if other side lags or wire was disconnected)
            sendQueue.Enqueue(data);
            sendPending.Set(); // interrupt SendThread WaitOne()
            return true;
        }
    }
}

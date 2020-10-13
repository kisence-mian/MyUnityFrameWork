// common code used by server and client
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace SimpleTCP
{
    public abstract class Common
    {
        // common code /////////////////////////////////////////////////////////
        // incoming message queue of <connectionId, message>
        // (not a HashSet because one connection can have multiple new messages)
        protected ConcurrentQueue<Message> receiveQueue = new ConcurrentQueue<Message>();

        // queue count, useful for debugging / benchmarks
        public int ReceiveQueueCount
        {
            get
            {
                return receiveQueue.Count;
            }
        }

        // warning if message queue gets too big
        // if the average message is about 20 bytes then:
        // -   1k messages are   20KB
        // -  10k messages are  200KB
        // - 100k messages are 1.95MB
        // 2MB are not that much, but it is a bad sign if the caller process
        // can't call GetNextMessage faster than the incoming messages.
        public static int messageQueueSizeWarning = 100000;

        // removes and returns the oldest message from the message queue.
        // (might want to call this until it doesn't return anything anymore)
        // -> Connected, Data, Disconnected events are all added here
        // -> bool return makes while (GetMessage(out Message)) easier!
        // -> no 'is client connected' check because we still want to read the
        //    Disconnected message after a disconnect
        public bool GetNextMessage(out Message message)
        {
            return receiveQueue.TryDequeue(out message);
        }

        // NoDelay disables nagle algorithm. lowers CPU% and latency but
        // increases bandwidth
        public bool NoDelay = true;

        // Prevent allocation attacks. Each packet is prefixed with a length
        // header, so an attacker could send a fake packet with length=2GB,
        // causing the server to allocate 2GB and run out of memory quickly.
        // -> simply increase max packet size if you want to send around bigger
        //    files!
        // -> 16KB per message should be more than enough.
        //public int MaxMessageSize = 16 * 1024;

        // Send would stall forever if the network is cut off during a send, so
        // we need a timeout (in milliseconds)
        public int SendTimeout = 5000;

        // avoid header[4] allocations but don't use one buffer for all threads
        [ThreadStatic] static byte[] header;


        // static helper functions /////////////////////////////////////////////
        // send message (via stream) with the <size,content> message structure
        // this function is blocking sometimes!
        // (e.g. if someone has high latency or wire was cut off)
        protected static bool SendMessagesBlocking(TCPSession session, byte[][] messages)
        {
            // stream.Write throws exceptions if client sends with high
            // frequency and the server stops
            try
            {
                // we might have multiple pending messages. merge into one
                // packet to avoid TCP overheads and improve performance.
                int packetSize = 0;
                for (int i = 0; i < messages.Length; ++i)
                    packetSize += sizeof(int) + messages[i].Length; // header + content


                byte[] payload = new byte[packetSize];

                // create the packet
                int position = 0;
                for (int i = 0; i < messages.Length; ++i)
                {
                    // create header buffer if not created yet
                    if (header == null)
                        header = new byte[4];

                    // construct header (size)
                    Utils.IntToBytesBigEndianNonAlloc(messages[i].Length, header);

                    // copy header + message into buffer
                    Array.Copy(header, 0, payload, position, header.Length);
                    Array.Copy(messages[i], 0, payload, position + header.Length, messages[i].Length);
                    position += header.Length + messages[i].Length;
                }

                // write the whole thing
                session.Send(payload);
                //Logger.LogWarning("已发送 size:"+packetSize+ " packetNum:"+messages.Length);
                return true;
            }
            catch (Exception exception)
            {
                // log as regular message because servers do shut down sometimes
                Logger.Log("Send: stream.Write exception: " + exception);
                return false;
            }
        }


        // thread receive function is the same for client and server's clients
        // (static to reduce state for maximum reliability)
        protected static void ReceiveLoop(int connectionId, TCPSession session, ConcurrentQueue<Message> receiveQueue)
        {

            // add connected event to queue with ip address as data in case
            // it's needed
            receiveQueue.Enqueue(new Message(connectionId, EventType.Connected, null));


            session.OnRecevePackets = (content) =>
            {
                receiveQueue.Enqueue(new Message(connectionId, EventType.Data, content));

            };

            session.OnDisconnect = (DisconnectReason disconnectReason) =>
            {
                receiveQueue.Enqueue(new Message(connectionId, EventType.Disconnected, null,disconnectReason));
            };
        }

        // thread send function
        // note: we really do need one per connection, so that if one connection
        //       blocks, the rest will still continue to get sends
        protected static void SendLoop(int connectionId, TCPSession session, SafeQueue<byte[]> sendQueue, ManualResetEvent sendPending)
        {
            // get NetworkStream from client

            try
            {
                //Logger.Log("1.5 session.Connected" + session.Connected);
                while (session.Connected) // try this. client will get closed eventually.
                {
                    // reset ManualResetEvent before we do anything else. this
                    // way there is no race condition. if Send() is called again
                    // while in here then it will be properly detected next time
                    // -> otherwise Send might be called right after dequeue but
                    //    before .Reset, which would completely ignore it until
                    //    the next Send call.
                    sendPending.Reset(); // WaitOne() blocks until .Set() again

                    // dequeue all
                    // SafeQueue.TryDequeueAll is twice as fast as
                    // ConcurrentQueue, see SafeQueue.cs!
                    byte[][] messages;
                    if (sendQueue.TryDequeueNum(out messages,50))
                    {
                        // send message (blocking) or stop if stream is closed
                        if (!SendMessagesBlocking(session, messages))
                            break; // break instead of return so stream close still happens!
                    }

                    // don't choke up the CPU: wait until queue not empty anymore
                    sendPending.WaitOne();
                }
                //Logger.Log("session.Connected:" + session.Connected);
            }
            catch (ThreadAbortException )
            {
                // happens on stop. don't log anything.
                //Logger.LogError(e.ToString());
            }
            catch (ThreadInterruptedException )
            {
                // happens if receive thread interrupts send thread.
                //Logger.LogError(e.ToString());
            }
            catch (Exception exception)
            {
                // something went wrong. the thread was interrupted or the
                // connection closed or we closed our own connection or ...
                // -> either way we should stop gracefully
                Logger.Log("SendLoop Exception: connectionId=" + connectionId + " reason: " + exception);
            }
            finally
            {
                // clean up no matter what
                // we might get SocketExceptions when sending if the 'host has
                // failed to respond' - in which case we should close the connection
                // which causes the ReceiveLoop to end and fire the Disconnected
                // message. otherwise the connection would stay alive forever even
                // though we can't send anymore.

                session.Close( DisconnectReason.ConnectionFailed);
               
            }
        }
    }
}

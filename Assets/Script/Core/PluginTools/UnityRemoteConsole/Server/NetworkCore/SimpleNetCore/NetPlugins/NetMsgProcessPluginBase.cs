using System.Collections.Generic;

namespace SimpleNetCore
{
    public abstract class NetMsgProcessPluginBase
    {

        protected NetworkCommon networkCommon;
        public  void Init(NetworkCommon networkCommon)
        {
            this.networkCommon = networkCommon;
            OnInit();
        }
        // Use this for initialization
        protected abstract void OnInit();
        public abstract void Release();
        public abstract byte GetNetProperty();


        // (主线程调用的)Update is called once per frame
        public virtual void Update(float deltaTime) { }
        public virtual void ReceveProcess( MsgPackest packest)
        {

        }

        public virtual byte[] SendProcess( Session session,  byte msgProperty, byte[] datas) { return null; }
      

        public virtual void PeerConnectedEvent(Session session)
        {
           
        }

        public virtual void DisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {
            
        }
    }
    public abstract class NetMsgProcessPackestPluginBase: NetMsgProcessPluginBase
    {
        protected Queue<MsgPackest> msgQueue = new Queue<MsgPackest>();

        public override void ReceveProcess( MsgPackest packest)
        {
            msgQueue.Enqueue(packest);
            OnMsgReceve();
        }
        protected virtual void OnMsgReceve() { }

    }
}

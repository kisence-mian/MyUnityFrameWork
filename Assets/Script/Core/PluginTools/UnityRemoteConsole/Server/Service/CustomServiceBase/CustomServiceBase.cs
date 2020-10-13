using SimpleNetManager;
using SimpleNetCore;

namespace UnityRemoteConsole
{
    public abstract class CustomServiceBase:ServiceBase
    {
        protected bool isOpenFunction=true;
        public bool IsOpenFunction
        {
            get { return isOpenFunction; }
            set
            {
                if (isOpenFunction == value)
                    return;

                isOpenFunction = value;
                if (isOpenFunction)
                {
                    OnFunctionOpen();
                }
                else
                {
                    OnFunctionClose();
                }
            }
        }
        public abstract string FunctionName
        {
            get;
        }

        protected abstract void OnFunctionClose();

        protected abstract void OnFunctionOpen();

        public override abstract void OnStart();
        protected virtual void OnPlayerLogin(SimpleNetManager.Player player) { }
        protected virtual void OnPlayerLoginAfter(SimpleNetManager.Player player) { }

        public override void OnInit()
        {
            LoginService loginService = serviceManager.Get<LoginService>();
            loginService.OnPlayerLogin += OnPlayerLoginEvent;
            loginService.OnPlayerLoginAfter += OnPlayerLoginAfter;

            msgManager.RegisterMsgEvent<FunctionSwitch2Server>(OnMsgFunctionSwitch);
        }

        private void OnPlayerLoginAfterEvent(SimpleNetManager.Player player)
        {
            OnPlayerLoginAfter(player);
        }

        private void OnMsgFunctionSwitch(NetMessageData msgHandler)
        {
            FunctionSwitch2Server msg = msgHandler.GetMessage<FunctionSwitch2Server>();
            if (msg.functionName == FunctionName)
            {
                IsOpenFunction = msg.isOpenFunction;
                //Debug.Log("server 接受isOpenFunction:" + IsOpenFunction);
                SendSwitchState2Client(msgHandler.session);
            }
        }
        protected void SendSwitchState2Client(Session session)
        {
            FunctionSwitch2Client msg = new FunctionSwitch2Client();
            msg.functionName = FunctionName;
            msg.isOpenFunction = isOpenFunction;
            netManager.Send(session, msg);
        }
        private void OnPlayerLoginEvent(SimpleNetManager.Player player)
        {
            SendSwitchState2Client(player.session);

            OnPlayerLogin(player);

        }
    }
}

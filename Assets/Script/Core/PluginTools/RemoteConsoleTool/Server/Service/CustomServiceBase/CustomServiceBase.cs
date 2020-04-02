using LiteNetLibManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameConsoleController
{
    public abstract class CustomServiceBase:ServiceBase
    {
        private bool isOpenFunction=true;
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
        protected virtual void OnPlayerLogin(LiteNetLibManager.Player player) { }
        protected virtual void OnPlayerLoginAfter(LiteNetLibManager.Player player) { }

        public override void OnInit()
        {
            LoginService loginService = serviceManager.Get<LoginService>();
            loginService.OnPlayerLogin += OnPlayerLoginEvent;
            loginService.OnPlayerLoginAfter += OnPlayerLoginAfter;

            msgManager.RegisterMessage<FunctionSwitch2Server>(OnMsgFunctionSwitch);
        }

        private void OnPlayerLoginAfterEvent(LiteNetLibManager.Player player)
        {
            OnPlayerLoginAfter(player);
        }

        private void OnMsgFunctionSwitch(NetMessageHandler msgHandler)
        {
            FunctionSwitch2Server msg = msgHandler.GetMessage<FunctionSwitch2Server>();
            if (msg.functionName == FunctionName)
            {
                IsOpenFunction = msg.isOpenFunction;

                SendSwitchState2Client(msgHandler.player);
            }
        }
        private void SendSwitchState2Client(LiteNetLibManager.Player player)
        {
            FunctionSwitch2Client msg = new FunctionSwitch2Client();
            msg.functionName = FunctionName;
            msg.isOpenFunction = isOpenFunction;
            netManager.Send(player, msg);
        }
        private void OnPlayerLoginEvent(LiteNetLibManager.Player player)
        {
            SendSwitchState2Client(player);

            OnPlayerLogin(player);

        }
    }
}

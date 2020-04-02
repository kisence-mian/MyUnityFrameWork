using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public class ClientControllerBase
    {
       

        private bool enable = false;
       

        public bool Enable
        {
            get
            {
                return enable;
            }
            set
            {
                if (enable && value)
                    return;
                enable = value;
                if (enable)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        protected MessageManager msgManager;
        protected NetworkClientManager netManager;
        protected ClientControllerManager controllerManager;
        public void SetMessageManager(MessageManager msgManager)
        {
            this.msgManager = msgManager;
        }
        public void SetNetworkClientManager(NetworkClientManager netManager)
        {
            this.netManager = netManager;
        }
        public void SetNetControllerManager(ClientControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;
        }
        /// <summary>
        /// AddService call
        /// </summary>
        public virtual void OnInit() { }
        /// <summary>
        /// 服务器启动时调用
        /// </summary>
        public virtual void OnStart() { }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnStop() { }

        public virtual void OnUpdate(float deltaTime) { }
    }
}

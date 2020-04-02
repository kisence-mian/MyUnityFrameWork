using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
  public abstract class ServiceBase
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
                if (enable == value)
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

        protected ServiceManager serviceManager;
        protected MessageManager msgManager;
        protected NetworkServerManager netManager;

        public void SetNetworkServerManager(NetworkServerManager netManager)
        {
            this.netManager = netManager;
        }
        public void SetMessageManager(MessageManager msgManager)
        {
            this.msgManager = msgManager;
        }
        public void SetServiceManager(ServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
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

        internal virtual void OnUpdate(float deltaTime) { }
    }
}

using System;
using System.Collections.Generic;

using UnityEngine;

namespace UnityRemoteConsole
{
  public  class KeyboardBootConsole : BootFunctionBase
    {

        private Action OnTriggerBoot;
        private UnityRemoteConsoleSettingData config;

        public void OnInit(UnityRemoteConsoleSettingData config, Action OnTriggerBoot)
        {
            this.config = config;
            this.OnTriggerBoot = OnTriggerBoot;

            Debug.Log("KeyboardBootConsole.init");
        }
        public void OnGUI()
        {

        }

        private bool isBoot = false;
        public void OnUpdate()
        {
            if (isBoot)
                return;
            if (Input.GetKey( KeyCode.F12)
                &&
                Input.GetKey(KeyCode.A) &&
                Input.GetKey(KeyCode.LeftShift)
                )
            {
                Debug.Log("KeyboardBootConsole.OnTriggerBoot");
                if (OnTriggerBoot != null)
                {
                    OnTriggerBoot();
                }
                isBoot = true;
            }
        }
    }
}

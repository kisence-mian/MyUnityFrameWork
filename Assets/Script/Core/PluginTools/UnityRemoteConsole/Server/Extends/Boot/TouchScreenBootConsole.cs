using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRemoteConsole
{
    class TouchScreenBootConsole : BootFunctionBase
    {

        private Action OnTriggerBoot;
        private URCSettingData config;
        public int tapCount;
        public void OnInit(URCSettingData config, Action OnTriggerBoot)
        {
            this.config = config;
            this.OnTriggerBoot = OnTriggerBoot;

           
        }
        public void OnGUI()
        {

        }
        private bool isBoot = false;
        public void OnUpdate()
        {
            if (isBoot)
                return;
            if (Input.touchCount == 1)
            {
                Touch myTouch = Input.touches[0];
              //  touchPos = new Vector2(0, Screen.height - Screen.height / 3);
                if (myTouch.tapCount >= tapCount &&
                    (myTouch.position.x>0&&myTouch.position.x<Screen.width/3)&&
                    (myTouch.position.y>(Screen.height - Screen.height / 3)&&myTouch.position.y<Screen.height)
                    )
                {
                    Debug.Log("TouchScreenBootConsole.OnTriggerBoot");
                    if (OnTriggerBoot != null)
                    {
                        OnTriggerBoot();
                    }
                    isBoot = true;
                }
            }
        }
    }
}

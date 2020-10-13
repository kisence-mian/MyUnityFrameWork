using System;


namespace UnityRemoteConsole
{
    public interface BootFunctionBase
    {
        void OnInit(URCSettingData config, Action OnTriggerBoot);
        void OnUpdate();

        void OnGUI();

    }
}

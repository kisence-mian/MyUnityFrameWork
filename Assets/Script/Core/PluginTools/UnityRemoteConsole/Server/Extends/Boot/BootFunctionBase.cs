using System;


namespace UnityRemoteConsole
{
    public interface BootFunctionBase
    {
        void OnInit(UnityRemoteConsoleSettingData config, Action OnTriggerBoot);
        void OnUpdate();

        void OnGUI();

    }
}

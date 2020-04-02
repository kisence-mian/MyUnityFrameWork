using System;


namespace GameConsoleController
{
    public interface BootFunctionBase
    {
        void OnInit(GameConsolePanelSettingConfig config, Action OnTriggerBoot);
        void OnUpdate();

        void OnGUI();

    }
}

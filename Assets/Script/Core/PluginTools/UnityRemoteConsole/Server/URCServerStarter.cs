
using SimpleNetManager;
using System;
using UnityEngine;

namespace UnityRemoteConsole
{

    public class URCServerStarter : MonoBehaviour
    {
        public static URCServerStarter Instance;
        private bool isInit = false;
        private void Awake()
        {
            if (isInit)
                return;
            if (Instance)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            Init();
            ConsoleStart();
        }

        private static void Init()
        {
            if (Instance == null)
            {
                Instance = new GameObject("[ConsoleServer]").AddComponent<URCServerStarter>();
            }
            if (Instance.isInit)
                return;
            Instance.isInit = true;
            DontDestroyOnLoad(Instance.gameObject);
        }

        private static bool isStart = false;
        public static bool ConsoleStart()
        {
            Init();

            if (isStart)
                return false;

            RemoteDeviceInfo deviceInfo = RemoteDeviceInfo.GetLocalDeviceInfo();
            deviceInfo.otherData.Add("ServerVersion", ServerVersionInfo.Version);
            deviceInfo.otherData.Add("MinClientVersion", ServerVersionInfo.MinClientVersion);

            URCSettingData config = URCSettingData.GetCofig();

            try
            {
                string deviceInfoStr = SimpleJsonUtils.ToJson(deviceInfo);
                NetServer.Start(config.netPort);
                NetServer.DiscoverServer.Start(config.netPort, deviceInfoStr);

                LoginService loginService = NetServer.ServiceManager.Get<LoginService>();
                loginService.SetPlayerLoginHandler(new SimplePlayerLoginHandler());

                //LogService logService = NetServer.ServiceManager.Get<LogService>();
                //logService.logSwitchForceControl = logSwitchForceControl;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            Debug.Log("URC NetServer.port:" + config.netPort);
            isStart = true;
            return true;

        }
        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            NetServer.Update(deltaTime);

            ConsoleBootManager.OnUpdate();
        }

        private void OnGUI()
        {
            ConsoleBootManager.OnGUI();
        }
        private void OnApplicationQuit()
        {
            NetServer.Stop();
        }

    }
}

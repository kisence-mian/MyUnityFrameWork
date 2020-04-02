using LiteNetLib;
using LiteNetLibManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace GameConsoleController
{

    public class ServerConsoleControllerStarter :MonoBehaviour
    {
        public static ServerConsoleControllerStarter Instance;
        private void Start()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // gameObject.name = "[ConsoleServer]";
            Init();
        }
        /// <summary>
        /// PlayerPrefs记录下次自动启动状态
        /// </summary>
        private const string PF_WhenFirstCustomBootThenAutoBoot = "whenFirstCustomBootThenAutoBoot";
        public static void Init()
        {
            GameConsolePanelSettingConfig config = GameConsolePanelSettingConfig.GetCofig();

            if (config.autoBoot)
            {
                ConsoleToolStart();
            }
            else
            {
                Debug.Log("ConsoleBootManager.init");
                int state = PlayerPrefs.GetInt(PF_WhenFirstCustomBootThenAutoBoot, 0);
                if (state == 1)
                {
                    ConsoleToolStart();
                    return;
                }
                ConsoleBootManager.Init(config, () =>
                 {
                     if (config.whenFirstCustomBootThenAutoBoot)
                     {
                         PlayerPrefs.SetInt(PF_WhenFirstCustomBootThenAutoBoot, 1);
                     }
                     ConsoleToolStart();
                 });
            }
        }

        private static bool isStart = false;
        public static bool ConsoleToolStart()
        {
            if (isStart)
                return false;
           
            RemoteDeviceInfo deviceInfo = RemoteDeviceInfo.GetLocalDeviceInfo();

            GameConsolePanelSettingConfig config = GameConsolePanelSettingConfig.GetCofig();

            try
            {
                string deviceInfoStr = SimpleJsonUtils.ToJson(deviceInfo);

                LitNetServer.SetNetworkServerManager(deviceInfoStr, config.netPort);
                LitNetServer.Start();
                LoginService loginService = LitNetServer.ServiceManager.Get<LoginService>();
                loginService.SetPlayerLoginHandler(new SimplePlayerLoginHandler());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }


            Debug.Log("LitNetServer.port:" + config.netPort);
            isStart = true;
            return true;

        }
        //Vector2 v2;
        //private void OnGUI()
        //{
        //    v2 = GUILayout.BeginScrollView(v2);
        //    LogService service = LitNetServer.ServiceManager.Get<LogService>();
        //    foreach (var item in service.GetLogDatas())
        //    {
        //        GUILayout.Label(item.ToString());
        //    }

        //    GUILayout.EndScrollView();
        //}
        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            LitNetServer.Update(deltaTime);

            ConsoleBootManager.OnUpdate();
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    Debug.Log("------log----");
            //}
        }

        private void OnGUI()
        {
            ConsoleBootManager.OnGUI();
        }
        private void OnApplicationQuit()
        {
            LitNetServer.Stop();
        }

    }
}

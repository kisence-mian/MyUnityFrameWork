using SimpleNetManager;
using SimpleNetCore;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityRemoteConsole
{
    public class LogService : CustomServiceBase
    {
        public override string FunctionName
        {
            get
            {
                return "Log";
            }
        }

        private static List<LogData> logDatas = new List<LogData>();

        public static List<LogData> GetLogDatas()
        {
            return logDatas;
        }

        private static Action<LogData> OnLogEvent;

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LogEventRegister()
        {
            Application.logMessageReceivedThreaded += LogMessageReceived;
        }
        public override void OnStart()
        {
            OnLogEvent += SendAllPlayerLog;

            isOpenFunction = GetSaveDebugState();
            if (IsOpenFunction != GetUnityDebugSwitch())
            {
                SetUnityDebugSwitch(IsOpenFunction);
            }
            msgManager.RegisterMsgEvent<ClearLog2Server>(OnClearLogEvent);
        }

        private void OnClearLogEvent(NetMessageData msgHandler)
        {
            logDatas.Clear();
        }

        private static int indexCounter = 0;
        private static void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            LogData data = new LogData(indexCounter,type, condition, stackTrace);
            logDatas.Add(data);
            indexCounter++;

            if (OnLogEvent != null)
            {
                OnLogEvent(data);
            }
            
        }

        protected override void OnPlayerLoginAfter(SimpleNetManager.Player player)
        {
            List<LogData> list = new List<LogData>(logDatas);
            
            foreach (var data in list)
            {
                SendLog(player, data);
            }

        }
        protected override void OnFunctionClose()
        {
            SetUnityDebugSwitch(false);
            SetSaveDebugState(false);
        }

        protected override void OnFunctionOpen()
        {
            SetUnityDebugSwitch(true);
            SetSaveDebugState(true);
        }
        internal override void OnUpdate(float deltaTime)
        {
            if(GetUnityDebugSwitch()!= IsOpenFunction)
            {
                isOpenFunction = GetUnityDebugSwitch();
               
                SimpleNetManager.Player[] players = PlayerManager.GetAllPlayers();
                foreach (SimpleNetManager.Player player in players)
                {
                    SendSwitchState2Client(player.session);
    ;
                }
            }
        }

        private void SendAllPlayerLog(LogData data)
        {
            SimpleNetManager.Player[] players= PlayerManager.GetAllPlayers();
            foreach(SimpleNetManager.Player player in players)
            {
                SendLog(player, data)
;            }
        }
        private void SendLog(SimpleNetManager.Player player, LogData data)
        {
            LogData2Client msg = new LogData2Client();
            msg.logData = data;
            netManager.Send(player.session, msg);
        }
        #region save debug switch

        public bool GetSaveDebugState()
        {
            int code = PlayerPrefs.GetInt(FunctionName, -1);
            bool isOpen = false;
            if (code == -1)
            {
                isOpen = GetUnityDebugSwitch();
            }
            else
            {
                isOpen = code == 0 ? false : true;
            }
            return isOpen;
        }
        public void SetSaveDebugState(bool isOpen)
        {
            int code = isOpen ? 1 : 0;
            PlayerPrefs.SetInt(FunctionName, code);
            PlayerPrefs.Save();
        }

        #endregion
        #region 开关debug
        private void SetUnityDebugSwitch(bool open)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.unityLogger.logEnabled = open;
#else
        Debug.logger.logEnabled = open;
#endif
        }
        private bool GetUnityDebugSwitch()
        {
#if UNITY_2017_1_OR_NEWER
          return  Debug.unityLogger.logEnabled;
#else
        return Debug.logger.logEnabled ;
#endif
        }
        #endregion
    }
}

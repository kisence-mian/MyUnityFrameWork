using LiteNetLibManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameConsoleController
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

        private List<LogData> logDatas = new List<LogData>();

        public List<LogData> GetLogDatas()
        {
            return logDatas;
        }
        public override void OnStart()
        {
            Application.logMessageReceivedThreaded += LogMessageReceived;

            IsOpenFunction = GetSaveDebugState();
            msgManager.RegisterMessage<ClearLog2Server>(OnClearLogEvent);
        }

        private void OnClearLogEvent(NetMessageHandler msgHandler)
        {
            logDatas.Clear();
        }

        private static int indexCounter = 0;
        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            LogData data = new LogData(indexCounter,type, condition, stackTrace);
            logDatas.Add(data);
            indexCounter++;

            SendAllPlayerLog(data);
            
        }

        protected override void OnPlayerLoginAfter(LiteNetLibManager.Player player)
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

        private void SendAllPlayerLog(LogData data)
        {
            LiteNetLibManager.Player[] players= PlayerManager.GetAllPlayers();
            foreach(LiteNetLibManager.Player player in players)
            {
                SendLog(player, data)
;            }
        }
        private void SendLog(LiteNetLibManager.Player player, LogData data)
        {
            LogData2Client msg = new LogData2Client();
            msg.logData = data;
            netManager.Send(player, msg);
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

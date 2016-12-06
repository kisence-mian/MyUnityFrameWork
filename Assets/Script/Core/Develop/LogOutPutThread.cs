using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;

/// <summary>
/// 日志输出线程
/// 这里借鉴了QLog
/// </summary>
public class LogOutPutThread
{

       //string mDevicePersistentPath = Application.persistentDataPath;

		public static string LogPath = "Log";
        public static string expandName = "txt";

		private Queue<LogInfo> mWritingLogQueue = null;
		private Queue<LogInfo> mWaitingLogQueue = null;
		private object mLogLock = null;
        private Thread mFileLogThread = null;
		private bool mIsRunning = false;
		private StreamWriter mLogWriter = null;

        public void Init()
        {
            try
            {
                //#if !UNITY_EDITOR

                ApplicationManager.s_OnApplicationQuit += Close;

                this.mWritingLogQueue = new Queue<LogInfo>();
                this.mWaitingLogQueue = new Queue<LogInfo>();

                this.mLogLock = new object();
                System.DateTime now = System.DateTime.Now;
                string logName = string.Format("Log{0}{1}{2}#{3}:{4}_{5}",
                    now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                string logPath = PathTool.GetAbsolutePath(ResLoadType.Persistent, PathTool.GetRelativelyPath(LogPath, logName, expandName));

                UpLoadLogic(logPath);

                if (File.Exists(logPath))
                    File.Delete(logPath);
                string logDir = Path.GetDirectoryName(logPath);

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                this.mLogWriter = new StreamWriter(logPath);
                this.mLogWriter.AutoFlush = true;
                this.mIsRunning = true;
                this.mFileLogThread = new Thread(new ThreadStart(WriteLog));
                this.mFileLogThread.Start();
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }

        //#endif
        }

		void WriteLog()
		{
			while (this.mIsRunning)
			{
				if (this.mWritingLogQueue.Count == 0)
				{
					lock (this.mLogLock)
					{
						while (this.mWaitingLogQueue.Count == 0)
							Monitor.Wait(this.mLogLock);
						Queue<LogInfo> tmpQueue = this.mWritingLogQueue;
						this.mWritingLogQueue = this.mWaitingLogQueue;
						this.mWaitingLogQueue = tmpQueue;
					}
				}
				else
				{
					while (this.mWritingLogQueue.Count > 0)
					{
						LogInfo log = this.mWritingLogQueue.Dequeue();
						if (log.m_logType == LogType.Error
                            ||log.m_logType == LogType.Exception
                            ||log.m_logType == LogType.Assert
                            )
						{
							this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------");
							this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.m_logContent + "\n");
							this.mLogWriter.WriteLine(log.m_logTrack);
							this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------"); 
						}
						else
						{
							this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.m_logContent);
						}
					}
				}
			}
		}

		public void Log(LogInfo logData)
		{
			lock (this.mLogLock)
			{
				this.mWaitingLogQueue.Enqueue(logData);
				Monitor.Pulse(this.mLogLock);
			}
		}

		public void Close()
		{
            ExitLogic();
			this.mIsRunning = false;
			this.mLogWriter.Close();
		}

        public void Pause()
        {
            ExitLogic();
        }

        Dictionary<string, object> m_logData;

        //string ConfigName = "LogInfo";
        //string isCrashKey = "isCrash";
        //string logPathKey = "logPath";

        public void UpLoadLogic(string logPath)
        {
            //m_logData = ConfigManager.GetData(ConfigName);

            //if (m_logData.ContainsKey(ConfigName) && (bool)m_logData[isCrashKey] == true)
            //{
            //    Debug.Log("上传");
            //    //上传
            //    HTTPTool.Upload_Request(URLManager.GetURL("LogUpLoadURL"), (string)m_logData[logPathKey]);
            //}

            ////初始化数据
            //if (m_logData.ContainsKey(isCrashKey))
            //{
            //    m_logData[isCrashKey] = false;
            //    m_logData[logPathKey] = logPath;
            //}
            //else
            //{
            //    m_logData.Add(isCrashKey, false);
            //    m_logData.Add(logPathKey, logPath);
            //}
        }

        
        public void ExitLogic()
        {
            //if (m_logData.ContainsKey(isCrashKey))
            //{
            //    m_logData[isCrashKey] = false;
            //}
            //else
            //{
            //    m_logData.Add(isCrashKey, false);
            //}

            //ConfigManager.SaveData(ConfigName, m_logData);
        }

        public static string[] GetLogFileNameList()
        {
            FileTool.CreatPath(PathTool.GetAbsolutePath(ResLoadType.Persistent, LogPath));

            List<string> relpayFileNames = new List<string>();
            string[] allFileName = Directory.GetFiles(PathTool.GetAbsolutePath(ResLoadType.Persistent, LogPath));
            foreach (var item in allFileName)
            {
                if (item.EndsWith(".txt"))
                {
                    string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                    relpayFileNames.Add(configName);
                }
            }

            return relpayFileNames.ToArray() ?? new string[0];
        }

        public static string LoadLogContent(string logName)
        {
            return ResourceIOTool.ReadStringByFile(GetPath(logName));
        }

        static string GetPath(string logName)
        {
            return PathTool.GetAbsolutePath(ResLoadType.Persistent, PathTool.GetRelativelyPath(LogPath, logName, expandName));
        }
}

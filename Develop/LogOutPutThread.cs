using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

/// <summary>
/// 日志输出线程
/// 这里借鉴了QLog
/// </summary>
public class LogOutPutThread
{

	#if UNITY_EDITOR
		string mDevicePersistentPath = Application.dataPath + "/../";
		#elif UNITY_STANDALONE_WIN
		string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
		#elif UNITY_STANDALONE_OSX
		string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
		#else
		string mDevicePersistentPath = Application.persistentDataPath;
		#endif

		static string LogPath = ".Log";

		private Queue<LogInfo> mWritingLogQueue = null;
		private Queue<LogInfo> mWaitingLogQueue = null;
		private object mLogLock = null;
		private Thread mFileLogThread = null;
		private bool mIsRunning = false;
		private StreamWriter mLogWriter = null;

        public void Init()
        {

            ApplicationManager.s_OnApplicationQuit += Close;

            this.mWritingLogQueue = new Queue<LogInfo>();
            this.mWaitingLogQueue = new Queue<LogInfo>();
            
            this.mLogLock = new object();
            System.DateTime now = System.DateTime.Now;
            string logName = string.Format("Log{0}{1}{2}#{3}{4}{5}",
                now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);


            string logPath = string.Format("{0}/{1}/{2}.txt", mDevicePersistentPath, LogPath, logName);

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

            Debug.Log(logPath);
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

        string ConfigName = "LogInfo";
        string isCrashKey = "isCrash";
        string logPathKey = "logPath";

        public void UpLoadLogic(string logPath)
        {
            m_logData = ConfigManager.GetData(ConfigName);

            if ((bool)m_logData[isCrashKey] == true)
            {
                //上传
                HTTPTool.Upload_Request(URLManager.GetURL("LogUpLoadURL"), (string)m_logData[logPathKey]);
            }

            //初始化数据
            if (m_logData.ContainsKey(isCrashKey))
            {
                m_logData[isCrashKey] = false;
                m_logData[logPathKey] = logPath;
            }
            else
            {
                m_logData.Add(isCrashKey, false);
                m_logData.Add(logPathKey, logPath);
            }
        }

        
        public void ExitLogic()
        {
            if (m_logData.ContainsKey(isCrashKey))
            {
                m_logData[isCrashKey] = false;
            }
            else
            {
                m_logData.Add(isCrashKey, false);
            }

            ConfigManager.SaveData(ConfigName, m_logData);
        }
}

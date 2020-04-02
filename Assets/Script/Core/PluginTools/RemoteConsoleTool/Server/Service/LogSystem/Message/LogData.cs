using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameConsoleController
{
    [Serializable]
    public class LogData: INetSerializable
    {
        public int index;
        /// <summary>
        /// log时间
        /// </summary>
        public long logTime;
        public string condition;
        public string stackTrace;
        public LogType logType;

        public void Deserialize(NetDataReader reader)
        {
            index = reader.GetInt();
            logTime = reader.GetLong();
            condition = reader.GetString();
            stackTrace = reader.GetString();
            logType = (LogType)reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(index);
            writer.Put(logTime);
            writer.Put(condition);
            writer.Put(stackTrace);
            writer.Put((int)logType);
        }
        public LogData() { }
        public LogData(int index, LogType type, string condition, string stackTrace)
        {
            this.index = index;
            logTime = DateTime.Now.Ticks;
            this.logType = type;
            this.condition = condition;
            this.stackTrace = stackTrace;
        }

        public override string ToString()
        {
            DateTime time = new DateTime(logTime);
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            builder.Append(time.ToString("HH:mm:ss"));
            builder.Append("]");
            builder.Append(" [");
            builder.Append(index);
            builder.Append("] ");
            builder.Append(condition);
            if (!string.IsNullOrEmpty(stackTrace))
            {
                builder.Append("\n");
                builder.Append(stackTrace);
            }
            return builder.ToString();
        }
    }
}

using UnityEngine;
using System.Collections;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System;
using LiteNetLibManager;

namespace GameConsoleController
{
    [Serializable]
    public class ParamsData : INetSerializable
    {
        public string descriptionName;
        public string paraName;
        public string paraTypeFullName;
        /// <summary>
        /// 默认值的json字符串值
        /// </summary>
        public string defaultValueStr;
        /// <summary>
        /// 当前参数的值可供选择的项(仅能对String类型参数使用)
        /// </summary>
        public string[] selectItemValues;
        public void Deserialize(NetDataReader reader)
        {
            descriptionName = reader.GetString();
            paraName = reader.GetString();
            paraTypeFullName = reader.GetString();
            defaultValueStr = reader.GetString();
            selectItemValues = reader.GetStringArray();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(descriptionName);
            writer.Put(paraName);
            writer.Put(paraTypeFullName);
            writer.Put(defaultValueStr);
            writer.PutArray(selectItemValues);
        }
        public Type GetParamValueType()
        {
            return ReflectionTool.GetTypeByTypeFullName(paraTypeFullName);
        }
        public object GetDefaultValue()
        {
            Type type = GetParamValueType();
            if (type == null)
                return null;
            if (string.IsNullOrEmpty(defaultValueStr))
            {
                return ReflectionTool.CreateDefultInstance(type);
            }

            return SimpleJsonUtils.FromJson(type, defaultValueStr);
        }
    }
    [Serializable]
    public class MethodData : INetSerializable
    {
        public string methodType;

        public string showName;
        public string description;

        public string classFullName;
        public string methodName;
        public List<ParamsData> paramsDatas = new List<ParamsData>();
        public void Deserialize(NetDataReader reader)
        {
            methodType = reader.GetString();
            showName = reader.GetString();
            description = reader.GetString();
            classFullName = reader.GetString();
            methodName = reader.GetString();

            paramsDatas = NetDataReaderExtend.GetListData<ParamsData>(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(methodType);
            writer.Put(showName);
            writer.Put(description);

            writer.Put(classFullName);
            writer.Put(methodName);
            NetDataWriterExtend.PutListData(writer, paramsDatas);

        }
    }
}

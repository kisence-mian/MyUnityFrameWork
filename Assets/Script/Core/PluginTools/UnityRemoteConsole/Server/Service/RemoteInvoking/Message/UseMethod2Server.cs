using SimpleNetManager;
using SimpleNetCore;
using System;
using System.Collections.Generic;

namespace UnityRemoteConsole
{
    public class UseMethod2Server : INetSerializable
    {
        public string classFullName;
        public string methodName;
        public Dictionary<string, string> paramNameValues;
        public void Deserialize(NetDataReader reader)
        {

            classFullName = reader.GetString();
            methodName = reader.GetString();
            {
                string[] keys = reader.GetStringArray();
                string[] values = reader.GetStringArray();
                paramNameValues = new Dictionary<string, string>();
                for (int i = 0; i < keys.Length; i++)
                {
                    paramNameValues.Add(keys[i], values[i]);
                }
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(classFullName);
            writer.Put(methodName);
            {
                string[] keys = new List<string>(paramNameValues.Keys).ToArray();
                writer.Put(keys);

                string[] values = new List<string>(paramNameValues.Values).ToArray();
                writer.Put(values);
            }


        }
    }
}

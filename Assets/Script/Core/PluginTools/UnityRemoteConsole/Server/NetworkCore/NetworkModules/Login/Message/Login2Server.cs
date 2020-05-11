using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public enum LoginType
    {
        Account,
        SDK,
    }
   
    public struct Login2Server : INoLoginMsg, INetSerializable
    {

        public LoginType loginType;

        public string key;
        public string password;

        public void Deserialize(NetDataReader reader)
        {
            loginType =(LoginType)reader.GetInt();
            key = reader.GetString();
            password = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int)loginType);
            writer.Put(key);
            writer.Put(password);
        }
    }
}

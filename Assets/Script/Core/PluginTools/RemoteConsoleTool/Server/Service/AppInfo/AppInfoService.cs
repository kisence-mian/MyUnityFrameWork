using UnityEngine;
using LiteNetLibManager;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine.SceneManagement;

namespace GameConsoleController
{
    public class AppInfoService : CustomServiceBase
    {
        public override string FunctionName
        {
            get
            {
                return "AppInfo";
            }
        }

        private static List<ShowInfoData> infoDatas = new List<ShowInfoData>();
        public override void OnStart()
        {

        }

        protected override void OnFunctionClose()
        {

        }

        protected override void OnFunctionOpen()
        {

        }
        protected override void OnPlayerLogin(LiteNetLibManager.Player player)
        {
            foreach (var item in infoDatas)
            {
                Send2Client(player, item);
            }
        }

        private static void Send2Client(LiteNetLibManager.Player player, ShowInfoData data)
        {
            if (LitNetServer.NetManager != null)
            {
                AppInfoData2Client msg = new AppInfoData2Client();
                msg.data = data;

                LitNetServer.NetManager.Send(player, msg);
            }
        }
        private static void Send2AllPlayer(ShowInfoData data)
        {
            LiteNetLibManager.Player[] players = LiteNetLibManager.PlayerManager.GetAllPlayers();
            foreach (var p in players)
            {
                Send2Client(p, data);
            }
        }

        private static ShowInfoData GetShowInfoData(string typeName, string label, string key)
        {
            foreach (var item in infoDatas)
            {
                if (item.typeName == typeName &&
                    item.label == label &&
                    item.key == key)
                {
                    return item;
                }
            }
            return null;
        }
     
        public static void AddInfoValue(string typeName, string label, string key, object value, string description = null)
        {
            try
            {
                if (string.IsNullOrEmpty(typeName) ||
               string.IsNullOrEmpty(label) ||
               string.IsNullOrEmpty(key))
                {

                    Debug.LogError("typeName or label or key cant be null");
                    return;
                }
                if (value == null)
                {
                    Debug.LogError("value cant be null");
                    return;
                }

                ShowInfoData data = GetShowInfoData(typeName, label, key);
                string valueStr = SimpleJsonUtils.ToJson(value);
                string valueTypeStr = value.GetType().FullName;

                bool isSend = false;
                if (data == null)
                {
                    data = new ShowInfoData();
                    data.typeName = typeName;
                    data.label = label;
                    data.key = key;
                    data.value = valueStr;
                    data.valueTypeStr = valueTypeStr;
                    data.discription = description;
                    infoDatas.Add(data);

                    isSend = true;

                }
                else
                {
                    if (data.valueTypeStr != valueTypeStr)
                    {
                        Debug.LogError(" Path:" + data.GetPath() + " already have value Type:" + data.valueTypeStr + " can not set Value Type:" + valueStr);
                        return;
                    }
                    else
                    {

                        if (data.value != valueStr)
                        {
                            data.value = valueStr;
                            isSend = true;

                        }
                        if (!string.IsNullOrEmpty(description) && data.discription != description)
                        {
                            data.discription = description;
                            isSend = true;
                        }

                    }
                }
                if (isSend)
                {
                    Send2AllPlayer(data);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

        }


    }
    public class ShowInfoData : INetSerializable
    {

        /// <summary>
        /// Type Name 
        /// </summary>
        public string typeName;
        /// <summary>
        /// like CPU in System Info
        /// </summary>
        public string label;
        /// <summary>
        /// value name ，like 
        /// </summary>
        public string key;

        public string valueTypeStr;
        public string value;
        public string discription;

        public void Deserialize(NetDataReader reader)
        {
            typeName = reader.GetString();
            label = reader.GetString();
            key = reader.GetString();
            valueTypeStr = reader.GetString();
            value = reader.GetString();
            discription = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(typeName);
            writer.Put(label);
            writer.Put(key);
            writer.Put(valueTypeStr);
            writer.Put(value);
            writer.Put(discription);
        }
        public string GetPath()
        {
            return typeName + "/" + label + "/" + key;
        }
    }
}


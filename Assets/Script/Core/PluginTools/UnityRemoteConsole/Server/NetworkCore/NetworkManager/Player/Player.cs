using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public class Player
    {
        public long connectionId;
        public string playerID;

        private Dictionary<Type, object> playerDataDic = new Dictionary<Type, object>();

        public Player(long connectionId)
        {
            this.connectionId = connectionId;
        }
        public T GetData<T>()
        {
            Type type = typeof(T);
            if (playerDataDic.ContainsKey(type))
            {
                return (T)playerDataDic[type];
            }
            return default(T);
        }
        public void AddData(object data)
        {
            if (data == null)
                return;
            Type type = data.GetType();
            if (playerDataDic.ContainsKey(type))
            {
                playerDataDic[type] = data;
            }
            else
            {
                playerDataDic.Add(type, data);
            }
        }
    }
}

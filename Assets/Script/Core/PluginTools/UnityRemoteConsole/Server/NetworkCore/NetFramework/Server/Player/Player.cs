using SimpleNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleNetManager
{
    public class Player
    {
        public Session session;
        public string playerID;

        private Dictionary<Type, object> playerDataDic = new Dictionary<Type, object>();

        public Player(Session session)
        {
            this.session = session;
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

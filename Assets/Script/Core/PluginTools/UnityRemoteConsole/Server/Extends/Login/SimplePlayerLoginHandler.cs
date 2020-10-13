using SimpleNetManager;
using SimpleNetCore;
using System;
using UnityEngine;

namespace UnityRemoteConsole
{


    public class SimplePlayerLoginHandler : PlayerLoginHandlerBase
    {
        public override uint LoginLogic(Login2Server msg, Session session, out SimpleNetManager.Player player)
        {
            URCSettingData config = URCSettingData.GetCofig();

            string key = msg.key;
            string pw = msg.password;
          

            if(config.loginKey.Equals(key)&& config.loginPassword.Equals(pw))
            {
                player = new SimpleNetManager.Player(session);
                player.playerID =Guid.NewGuid().ToString();

                return 0;
            }
            player = null;
            return 102;
        }

    }
}

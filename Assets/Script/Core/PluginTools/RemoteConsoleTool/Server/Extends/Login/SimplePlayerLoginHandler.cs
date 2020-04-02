using LiteNetLibManager;
using System;

namespace GameConsoleController
{


    public class SimplePlayerLoginHandler : PlayerLoginHandlerBase
    {
        public override uint LoginLogic(Login2Server msg, long connectId, out LiteNetLibManager.Player player)
        {
            GameConsolePanelSettingConfig config = GameConsolePanelSettingConfig.GetCofig();

            string key = msg.key;
            string pw = msg.password;
            if(config.loginKey.Equals(key)&& config.loginPassword.Equals(pw))
            {
                player = new LiteNetLibManager.Player(connectId);
                player.playerID =Guid.NewGuid().ToString();

                return 0;
            }
            player = null;
            return 102;
        }

    }
}

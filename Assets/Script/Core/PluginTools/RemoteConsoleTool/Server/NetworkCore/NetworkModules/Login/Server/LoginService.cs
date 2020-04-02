using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using UnityEngine;

namespace LiteNetLibManager
{
    public class LoginService : ServiceBase
    {
        public Action<Player> OnPlayerLogin;
        /// <summary>
        /// when call after call OnPlayerLogin
        /// </summary>
        public Action<Player> OnPlayerLoginAfter;
        public Action<Player> OnPlayerLogout;
        public override void OnStart()
        {
            msgManager.RegisterMessage<Login2Server>(OnLoginMsg);
            msgManager.RegisterMessage<Logout2Server>(OnLogoutMsg);
            netManager.OnPeerDisconnected += OnPeerDisconnected;
        }
        private PlayerLoginHandlerBase playerLoginHandler;
        public void SetPlayerLoginHandler(PlayerLoginHandlerBase handler)
        {
            playerLoginHandler = handler;
        }
        private void OnPeerDisconnected(long connectionId, DisconnectInfo info)
        {
            LiteNetLibManager. Player player = LiteNetLibManager.PlayerManager.GetPlayer(connectionId);
            LogoutAction(player);
        }

        private void OnLogoutMsg(NetMessageHandler messageHandler)
        {
            Debug.Log("服务端接收登出:"+ messageHandler.player);
            LogoutAction(messageHandler.player);
        }
        private void LogoutAction(Player player)
        {
            if (player == null)
                return;
            if (LiteNetLibManager.PlayerManager.IsLogin(player.connectionId))
            {
                LiteNetLibManager.PlayerManager.RemovePlayer(player);
                if (OnPlayerLogout != null)
                {
                    OnPlayerLogout(player);
                }
                netManager.Send(player.connectionId, new Logout2Client());
            }
        }

        private void OnLoginMsg(NetMessageHandler messageHandler)
        {
            Debug.Log("接受到登陆消息!");
            Login2Server msg = messageHandler.GetMessage<Login2Server>();

            Login2Client resMsg = new Login2Client();
            Player player =null;
            if (LiteNetLibManager.PlayerManager.IsLogin(messageHandler.connectionId))
            {
                resMsg.code = 100;
            }

           else if (playerLoginHandler!=null)
            {
                resMsg.code = playerLoginHandler.LoginLogic(msg, messageHandler.connectionId, out player);
                // player = new Player(messageHandler.connectionId);
                //player.playerID = "1000";
                if (resMsg.code == 0)
                {
                    if (LiteNetLibManager.PlayerManager.IsLogin(player.playerID))
                    {
                        //当前账号已登录
                        resMsg.code = 103;
                    }
                    else
                    {
                        resMsg.playerID = player.playerID;

                        resMsg.appData = new AppData();
                        resMsg.appData.serverAppName = Application.productName;
                        resMsg.appData.serverAppVersion = Application.version;
                        resMsg.appData.bundleIdentifier = Application.identifier;

                        LiteNetLibManager.PlayerManager.AddPlayer(player);

                    }
                }
               
            }
            else
            {
                resMsg.code = 101;
            }

            netManager.Send(messageHandler.connectionId, resMsg);
            if (resMsg.code==0)
            {
                if (OnPlayerLogin != null)
                    OnPlayerLogin(player);

                if (OnPlayerLoginAfter != null)
                {
                    OnPlayerLoginAfter(player);
                }
            }
        }
    }
}

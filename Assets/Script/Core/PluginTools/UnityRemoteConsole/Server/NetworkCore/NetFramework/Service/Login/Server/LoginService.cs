using SimpleNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace SimpleNetManager
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
            msgManager.RegisterMsgEvent<Login2Server>(OnLoginMsg);
            msgManager.RegisterMsgEvent<Logout2Server>(OnLogoutMsg);
            netManager.OnPeerDisconnected += OnPeerDisconnected;
        }
        private PlayerLoginHandlerBase playerLoginHandler;
        public void SetPlayerLoginHandler(PlayerLoginHandlerBase handler)
        {
            playerLoginHandler = handler;
        }
        private void OnPeerDisconnected(Session session, EDisconnectInfo info)
        {
            SimpleNetManager. Player player = SimpleNetManager.PlayerManager.GetPlayer(session);
            LogoutAction(player);
        }

        private void OnLogoutMsg(NetMessageData messageHandler)
        {
            SimpleNetManager.Player player = SimpleNetManager.PlayerManager.GetPlayer(messageHandler.session);
            Debug.Log("服务端接收登出:"+ player);
          
            LogoutAction(player);
        }
        private void LogoutAction(Player player)
        {
            if (player == null)
                return;
            if (SimpleNetManager.PlayerManager.IsLogin(player.session))
            {
                SimpleNetManager.PlayerManager.RemovePlayer(player);
                if (OnPlayerLogout != null)
                {
                    OnPlayerLogout(player);
                }
                netManager.Send(player.session, new Logout2Client());
            }
        }

        private void OnLoginMsg(NetMessageData messageHandler)
        {
            Debug.Log("接受到登陆消息!");
            Login2Server msg = messageHandler.GetMessage<Login2Server>();

            bool isRightDecryptPW = true;
            //密码解码
            if (!string.IsNullOrEmpty(msg.password))
            {
                try
                {
                    string temp = msg.password;
                    //获得密码md5串长度
                    int length = int.Parse(temp.Substring(0, 4));
                    //Debug.Log("length:" + temp.Substring(0, 4));

                    string md5Ery = temp.Substring(4,length);
                    //Debug.Log("md5Ery:" + md5Ery);
                    string aesKey = temp.Substring(4 + length);
                    //Debug.Log("aesKey:" + aesKey);
                    string pwMD5 = AESUtils.AESDecrypt(md5Ery, aesKey);
                    //Debug.Log("pwMD5:" + pwMD5);
                    msg.password = pwMD5;
                }
                catch (Exception e)
                {
                    Debug.LogError("password Decrypt error:"+ msg.password+"\n"+e);
                    isRightDecryptPW = false;
                   
                }
              
            }

            Login2Client resMsg = new Login2Client();
            resMsg.appData = new AppData();
            resMsg.appData.serverAppName = Application.productName;
            resMsg.appData.serverAppVersion = Application.version;
            resMsg.appData.bundleIdentifier = Application.identifier;

            Player player =null;
            if (isRightDecryptPW)
            {
                if (SimpleNetManager.PlayerManager.IsLogin(messageHandler.session))
                {
                    //重复登陆
                    resMsg.code = 100;
                }

                else if (playerLoginHandler != null)
                {
                    resMsg.code = playerLoginHandler.LoginLogic(msg, messageHandler.session, out player);
                    // player = new Player(messageHandler.connectionId);
                    //player.playerID = "1000";
                    if (resMsg.code == 0)
                    {
                        if (SimpleNetManager.PlayerManager.IsLogin(player.playerID))
                        {
                            //当前账号已登录
                            resMsg.code = 103;
                        }
                        else
                        {
                            resMsg.playerID = player.playerID;

                        }
                    }

                }
                else
                {
                    //其他错误
                    resMsg.code = 101;
                }
            }
            else
            {
                //密码解析错误
                resMsg.code = 104;
            }

            netManager.Send(messageHandler.session, resMsg);
            SimpleNetManager.PlayerManager.AddPlayer(player);
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

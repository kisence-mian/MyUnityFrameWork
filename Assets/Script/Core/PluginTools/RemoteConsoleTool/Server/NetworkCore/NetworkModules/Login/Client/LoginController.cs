using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using UnityEngine;

namespace LiteNetLibManager
{
    public class LoginController : ClientControllerBase
    {
        private bool isLogin;
        public bool IsLogin
        {
            get
            {
                return isLogin;
            }
        }

        public Action<Login2Client> Onlogin;
        public Action<Logout2Client> OnLogout;

        public override void OnStart()
        {
            netManager.MsgManager.RegisterMessage<Login2Client>(OnLoginEvent);
            netManager.MsgManager.RegisterMessage<Logout2Client>(OnLogoutEvent);
            netManager.OnDisconnected += OnDisconnected;
        }

        private void OnDisconnected(DisconnectInfo obj)
        {
            Debug.Log("断开连接！");
            RemovePlayer();
        }

        private void RemovePlayer()
        {
            long connectionId = netManager.ConnectionId;
            Player player = LiteNetLibManager.PlayerManager.GetPlayer(connectionId);
            if (player != null)
            {
                LiteNetLibManager.PlayerManager.RemovePlayer(player);
            }
            isLogin = false;
        }
        private void OnLogoutEvent(NetMessageHandler messageHandler)
        {
            Debug.Log("客户端接收登出");
            if (IsLogin)
            {
               
                Logout2Client msg = messageHandler.GetMessage<Logout2Client>();
                if (OnLogout != null)
                {
                    OnLogout(msg);
                }
                RemovePlayer();
            }
        }

        private void OnLoginEvent(NetMessageHandler messageHandler)
        {
            Login2Client msg = messageHandler.GetMessage<Login2Client>();
            if (msg.code == 0)
            {
                isLogin = true;
                if(string.IsNullOrEmpty(msg.playerID))
                {
                    msg.playerID = "001";
                }
                Player player = new Player(messageHandler.connectionId);
                player.playerID = msg.playerID;
                player.AddData(msg.appData);

                LiteNetLibManager.PlayerManager.AddPlayer(player);

            }
            if (Onlogin != null)
            {
                Onlogin(msg);
            }
            Debug.Log("登录返回:" + msg.code);
        }

        private string key;
        private string password;
        public void LoginByAccount(string key,string password)
        {
            this.key = key;
            this.password = password;

            Login2Server msg = new Login2Server();
            msg.loginType = LoginType.Account;
            msg.key = key;
            msg.password = password;
            netManager.Send(msg);

        }

        public void ReLogin()
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("No record key password to login!");
                return;
            }
            LoginByAccount(key, password);
        }
        public void Logout()
        {
            if (IsLogin)
            {
                netManager.Send(new Logout2Server());
            }
        }
    }
}

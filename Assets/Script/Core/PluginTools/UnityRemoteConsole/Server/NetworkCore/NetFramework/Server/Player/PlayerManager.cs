using SimpleNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleNetManager
{
    public class PlayerManager
    {
        private static Dictionary<Session, string> connect2PlayerIDs = new Dictionary<Session, string>();
        private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    
        public static void AddPlayer(Player player)
        {
            connect2PlayerIDs.Add(player.session, player.playerID);
            players.Add(player.playerID, player);
        }
        public static void RemovePlayer(Player player)
        {
            connect2PlayerIDs.Remove(player.session);
            players.Remove(player.playerID);
        }
        public static bool IsLogin(Session session)
        {
            return GetPlayer(session) != null;
        }
        public static bool IsLogin(string playerID)
        {
            return GetPlayer(playerID) != null;
        }

        public static Player GetPlayer(Session session)
        {
            string playerID = null;
            if (connect2PlayerIDs.ContainsKey(session))
            {
                playerID = connect2PlayerIDs[session];
            }
            else
            {
                return null;
            }

            return GetPlayer(playerID);
        }
        public static Player GetPlayer(string playerID)
        {
            if (players.ContainsKey(playerID))
                return players[playerID];

            return null;
        }

        public static Player[] GetAllPlayers()
        {
            List<Player> list = new List<Player>(players.Values);
            return list.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public class PlayerManager
    {
        private static Dictionary<long, string> connect2PlayerIDs = new Dictionary<long, string>();
        private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    
        public static void AddPlayer(Player player)
        {
            connect2PlayerIDs.Add(player.connectionId, player.playerID);
            players.Add(player.playerID, player);
        }
        public static void RemovePlayer(Player player)
        {
            connect2PlayerIDs.Remove(player.connectionId);
            players.Remove(player.playerID);
        }
        public static bool IsLogin(long connectionId)
        {
            return GetPlayer(connectionId) != null;
        }
        public static bool IsLogin(string playerID)
        {
            return GetPlayer(playerID) != null;
        }

        public static Player GetPlayer(long connectionId)
        {
            string playerID = null;
            if (connect2PlayerIDs.ContainsKey(connectionId))
            {
                playerID = connect2PlayerIDs[connectionId];
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

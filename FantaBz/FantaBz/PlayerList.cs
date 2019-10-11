using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class PlayerList
    {
        private static List<Player> players = new List<Player>();


        public static void addPlayer(Player p) {
            players.Add(p);
        }

        public static Player getPlayer(string pid) {
            Player p = null;
            foreach (Player player in players)
            {
                if (player.Id.Equals(pid)) {
                    return player;
                }
            }
            return p;
        }
    }
}

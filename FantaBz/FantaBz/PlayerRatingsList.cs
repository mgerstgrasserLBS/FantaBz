using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class PlayerRatingsList
    {

        private static List<PlayerRating> playersRatings = new List<PlayerRating>();


        public static void addPlayerRating(PlayerRating p)
        {
            playersRatings.Add(p);
        }

        public static PlayerRating getPlayerRating(string pid)
        {
            PlayerRating p = null;
            foreach (PlayerRating playerRating in playersRatings)
            {
                if (playerRating.Id.Equals(pid))
                {
                    return playerRating;
                }
            }
            return p;
        }
    }
}

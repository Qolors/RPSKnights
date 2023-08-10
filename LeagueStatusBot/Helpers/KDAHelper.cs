using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Helpers
{
    public static class KDAHelper
    {
        /// <summary>
        /// Calculates the KDA (Kill/Death/Assist) ratio.
        /// </summary>
        /// <param name="kills">Number of kills.</param>
        /// <param name="deaths">Number of deaths.</param>
        /// <param name="assists">Number of assists.</param>
        /// <returns>The calculated KDA value.</returns>
        public static double GetKDA(int kills, int deaths, int assists)
        {
            if (deaths == 0)
            {
                // Prevent division by zero.
                return kills + assists; // Some games consider infinite KDA when deaths are zero. Adjust if necessary.
            }

            return (double)(kills + assists) / deaths;
        }
    }
}

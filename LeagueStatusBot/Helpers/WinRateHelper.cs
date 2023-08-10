using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Helpers
{
    public static class WinRateHelper
    {
        /// <summary>
        /// Calculates the win rate percentage.
        /// </summary>
        /// <param name="wins">Number of wins.</param>
        /// <param name="losses">Number of losses.</param>
        /// <returns>The calculated win rate percentage.</returns>
        public static double GetWinRate(int wins, int losses)
        {
            int totalGames = wins + losses;

            if (totalGames == 0)
            {
                // Prevent division by zero.
                return 0.0; // Return 0% win rate if no games are played.
            }

            return (double)wins / totalGames * 100; // The result is in percentage.
        }
    }
}

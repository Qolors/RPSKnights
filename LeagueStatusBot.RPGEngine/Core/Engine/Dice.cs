using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public static class Dice
    {
        private static Random _random = new Random();
        public static int Roll(int numberOfDice, int sides)
        {
            int total = 0;
            for (int i = 0; i < numberOfDice; i++)
            {
                total += _random.Next(1, sides + 1);
            }
            return total;
        }
    }
}

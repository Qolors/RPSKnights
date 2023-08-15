using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Stats
    {
        public int Strength { get; set; } = 10; //Attack Damage
        public int Luck { get; set; } = 10; //Crit Chance
        public int Endurance { get; set; } = 10; //Max HP
        public int Charisma { get; set; } = 10; //Healing & Buffs
        public int Intelligence { get; set; } = 10; //Magic Damage
        public int Agility { get; set; } = 10; //Dodge Chance & Initiative
    }
}

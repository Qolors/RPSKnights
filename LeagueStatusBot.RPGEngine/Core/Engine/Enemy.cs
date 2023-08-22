using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Enemy : Being
    {
        public Enemy(string name)
        {
            Name = name;
            ClassName = "Monster";
            HitPoints = 20;
            MaxHitPoints = 20;
            BaseStats = new Stats
            {
                Agility = 6,
                Charisma = 6,
                Endurance = 8,
                Intelligence = 10,
                Strength = 14,
                Luck = 6
            };
        }
    }
}

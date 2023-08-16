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
            ClassName = "Enemy";
            Attack = 1;
            Defense = 1;
            HitPoints = 25;
            MaxHitPoints = 25;
            BaseStats = new Stats
            {
                Agility = 8,
                Charisma = 8,
                Endurance = 8,
                Intelligence = 8,
                Strength = 8,
                Luck = 8
            };
        }
    }
}

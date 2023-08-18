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
            HitPoints = 20;
            MaxHitPoints = 20;
            BaseStats = new Stats
            {
                Agility = 5,
                Charisma = 5,
                Endurance = 5,
                Intelligence = 5,
                Strength = 5,
                Luck = 5
            };
        }
    }
}

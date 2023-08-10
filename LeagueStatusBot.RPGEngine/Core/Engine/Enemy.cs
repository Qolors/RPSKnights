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
            Attack = 1;
            Defense = 1;
            HitPoints = 10;
            MaxHitPoints = 10;
        }
    }
}

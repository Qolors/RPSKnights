using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public abstract class Ability
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cooldown { get; set; } = 0;
        public DamageType DamageType { get; set; }
        public abstract float Activate(Being user, Being? target);
        public abstract double ExpectedDamage(Being user);
    }
}

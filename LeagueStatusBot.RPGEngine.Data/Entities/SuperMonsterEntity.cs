using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class FirstAbility
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Damage { get; set; }
        public string Effect { get; set; }
        public int Targets { get; set; }
    }

    public class SuperMonsterEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public FirstAbility FirstAbility { get; set; }
        public SecondAbility SecondAbility { get; set; }
    }

    public class SecondAbility
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Damage { get; set; }
        public string Effect { get; set; }
        public int Targets { get; set; }
    }
}

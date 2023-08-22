using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class ArmorEffectEntity
    {
        public int EffectId { get; set; }
        public string EffectFor { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

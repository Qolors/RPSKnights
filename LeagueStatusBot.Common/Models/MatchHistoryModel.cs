using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Common.Models
{
    public class MatchHistoryModel
    {
        public string Victory { get; set; }
        public string Champion { get; set; }
        public double KDA { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public bool IsCombat { get; set; }
        public int DamageDealt { get; set; }
        public int HealingDone { get; set; }
        public int ShieldingDone { get; set; }
    }
}

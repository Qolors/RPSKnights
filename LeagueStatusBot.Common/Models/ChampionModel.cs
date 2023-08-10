using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Common.Models
{
    public class ChampionModel
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assist { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int KillingSprees { get; set; }
        public int TotalDamageDealtToChampions { get; set; }
        public int TotalHealingOnTeammates { get; set; }
        public int TotalShieldingOnTeammates { get; set; }
    }
}

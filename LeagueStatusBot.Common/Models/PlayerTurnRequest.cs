using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Common.Models
{
    public class PlayerTurnRequest
    {
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public ulong UserId { get; set; }
        public string Attack { get; set; }
        public string Defend { get; set; }
    }
}

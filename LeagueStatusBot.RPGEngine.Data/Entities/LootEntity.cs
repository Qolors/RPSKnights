using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class LootEntity
    {
        public ulong DiscordId { get; set; }
        public int LootCount { get; set; }
    }
}

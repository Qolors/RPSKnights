using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Events
{
    public class TurnStartPlayerEventArgs : EventArgs
    {
        public ulong DiscordId { get; set; }
        public TurnStartPlayerEventArgs(Player player)
        {
            DiscordId = player.DiscordId;
        }
    }
}

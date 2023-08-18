using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Player : Being
    {
        public Player(ulong discordId, string name)
        {
            Name = name;
            DiscordId = discordId;
            HitPoints = 10;
            MaxHitPoints = 10;
            IsHuman = true;
        }
    }
}

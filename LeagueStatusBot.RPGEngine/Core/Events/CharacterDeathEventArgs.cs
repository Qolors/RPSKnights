using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Events
{
    public class CharacterDeathEventArgs
    {
        public ulong CharacterId { get; set; }
        public string DeathSentence { get; set; }
        public bool IsHuman { get; set; }

        public CharacterDeathEventArgs(ulong CharacterId, string DeathSentence, bool IsHuman)
        {
            this.CharacterId = CharacterId;
            this.DeathSentence = DeathSentence;
            this.IsHuman = IsHuman;
        }
    }
}

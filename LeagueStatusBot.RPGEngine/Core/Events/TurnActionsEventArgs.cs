using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Events
{
    public class TurnActionsEventArgs : EventArgs
    {
        public List<string> CombatLogs { get; set; }
        public Being ActivePlayer { get; set; }
        public ActionPerformed ActionPerformed { get; set; }

        public TurnActionsEventArgs(List<string> combatLogs, Being activePlayer, ActionPerformed actionPerformed)
        {
            CombatLogs = combatLogs;
            ActivePlayer = activePlayer;
            ActionPerformed = actionPerformed;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Party
    {
        public event EventHandler<string> PartyMemberDeath;
        public event EventHandler<string> PartyEvent;
        public List<Being> Members { get; set; } = new List<Being>();
        public bool IsAlive => Members.Any(m => m.IsAlive);

        public void AddPartyMember(ulong discordId, string name)
        {
            var player = new Player(discordId, name);
            AddPartyMember(player);
        }

        public void AddPartyMember(Being being)
        {
            being.Killed += OnMemberKilled;
            being.ActionPerformed += OnMemberActionPerformed;
            being.DamageTaken += OnMemberDamageTaken;

            Members.Add(being);
        }
        private void OnMemberKilled(object? sender, EventArgs e)
        {
            var being = sender as Being;
            if (being == null) return;

            being.Killed -= OnMemberKilled;
            being.ActionPerformed -= OnMemberActionPerformed;
            being.DamageTaken -= OnMemberDamageTaken;

            Members.Remove(being);

            PartyMemberDeath?.Invoke(this, $"- {being.Name} has been slain!.\n");
        }
        private void OnMemberActionPerformed(object? sender, string e)
        {
            var being = sender as Being;
            if (being == null) return;

            PartyEvent?.Invoke(this, e);
        }

        private void OnMemberDamageTaken(object? sender, string e)
        {
            var being = sender as Being;
            if (being == null) return;

            PartyEvent?.Invoke(this, e);
        }

    }
}

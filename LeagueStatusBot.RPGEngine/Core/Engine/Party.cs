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
        public event EventHandler<string> PartyMemberEffectApplied;
        public event EventHandler<string> PartyMemberEffectRemoved;
        public List<Being> Members { get; set; } = new List<Being>();
        public bool IsAlive => Members.Any(m => m.IsAlive);
        public void AddPartyMember(ulong discordId, string name)
        {
            var player = new Player(discordId, name);

            player.DiscordId = discordId;
            player.Name = name;

            AddPartyMember(player);
        }

        public void AddPartyMember(Being being)
        {
            being.Killed += OnMemberKilled;
            being.ActionPerformed += OnMemberActionPerformed;
            being.DamageTaken += OnMemberDamageTaken;
            being.EffectApplied += OnMemberEffectApplied;
            being.EffectRemoved += OnMemberEffectRemoved;

            Members.Add(being);
        }
        private void OnMemberKilled(object? sender, EventArgs e)
        {
            Being being = (Being)sender;
            if (being == null) return;

            being.Killed -= OnMemberKilled;
            being.ActionPerformed -= OnMemberActionPerformed;
            being.DamageTaken -= OnMemberDamageTaken;
            being.EffectApplied -= OnMemberEffectApplied;
            being.EffectRemoved -= OnMemberEffectRemoved;

            Members.Remove(being);

            PartyMemberDeath?.Invoke(this, $"- {being.Name} has been slain!.\n");
        }
        private void OnMemberActionPerformed(object? sender, string e)
        {
            PartyEvent?.Invoke(this, e);
        }

        private void OnMemberDamageTaken(object? sender, string e)
        {
            PartyEvent?.Invoke(this, e);
        }

        private void OnMemberEffectApplied(object sender, string e)
        {
            PartyMemberEffectApplied?.Invoke(this, e);
        }

        private void OnMemberEffectRemoved(object sender, string e)
        {
            PartyMemberEffectRemoved?.Invoke(this, e);
        }

    }
}

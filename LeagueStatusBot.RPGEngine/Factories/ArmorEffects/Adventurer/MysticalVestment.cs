using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ArmorEffects.Adventurer
{
    public class MysticalVestment : IArmorEffect
    {
        public bool IsUsed { get; set; } = false;
        public int EffectId { get; set; } = 4;
        public string Name { get; set; } = "Mystical Vestment";
        public string EffectFor { get; set; } = "Adventurer";
        public string Description { get; set; } = "Activate to redirect the next attack's damage as magical energy back to the attacker.";

        public void ActivateArmor(Being self, Being? target, float damage)
        {
            self.BroadCast("Mystical Vestment - Redirecting damage back to attacker");
            damage = damage + (self.BaseStats.Intelligence * 0.2f);
            target?.TakeDamage(damage, DamageType.Magic, self);
            self.LastActionPerformed = Common.Models.ActionPerformed.ArmorAbility;
            IsUsed = true;
        }
    }

}

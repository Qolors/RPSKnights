using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects.Adventurer
{
    public class ChargingBullBlade : IItemEffect
    {
        public int EffectId { get; set; }
        public string Name { get; set; } = "Blade of the Charging Bull";
        public string Description { get; set; } = "Every consecutive hit increases your damage by 2%. Resets on active use, dealing 2x the stacked Bonus Damage.";

        public bool IsUsed { get; set; } = false;

        private float bonusDamage = 0.00f;

        public void Register(Being being)
        {
            being.OnDamageGiven += OnExecutePassive;
        }

        public void OnExecutePassive(Being being)
        {
            being.CurrentDamage += (being.CurrentDamage *= bonusDamage);
            bonusDamage += 0.02f; // Incremental bonus damage
        }

        public void OnExecuteActive(Being being)
        {
            being.OnDamageGiven -= OnExecutePassive;
            being.CurrentDamage += (being.CurrentDamage *= bonusDamage) * 2; // A big burst of damage
            being.BroadCast(this.Name);
            bonusDamage = 0; // Resets passive increment

            IsUsed = true;
        }
    }
}

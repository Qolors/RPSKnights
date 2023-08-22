using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects.Adventurer
{
    public class SwordOfSteadiness : IItemEffect
    {
        public int EffectId { get; set; }
        public string Name { get; set; } = "Sword of Steadiness";
        public string Description { get; set; } = "After three consecutive turns without taking damage, your next attack is a guaranteed super critical.";
        public bool IsUsed { get; set; } = false;
        private int counter = 0;
        public string PrintPassiveStatus()
        {
            return $"Current Consecutive Turns Without Taking Damage: {counter}/3";
        }
        public void Register(Being being)
        {
            being.OnDamageGiven += OnExecutePassive;
            being.OnDamageTaken += OnDamageTaken;
        }

        public void OnExecutePassive(Being being)
        {
            counter++;
            if (counter >= 3)
            {
                being.CurrentDamage *= 2; // Double the damage for critical hit
                counter = 0; // Reset the counter
            }
        }

        public void OnDamageTaken(Being being)
        {
            counter = 0;
        }

        public void OnExecuteActive(Being being)
        {
            being.OnDamageGiven -= OnExecutePassive;
            being.CurrentDamage *= 3; // Triple the damage for this turn
            being.BroadCast(this.Name);
            counter = 0; // Reset the counter

            IsUsed = true;
        }
    }

}

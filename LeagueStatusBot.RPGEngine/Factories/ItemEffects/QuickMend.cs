using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects;

public class QuickMend : IItemEffect
{
    public int EffectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Define the chance percentage to trigger the quick mend
    private const float MEND_TRIGGER_CHANCE = 0.10f;  // 10%

    // Define the percentage of lost hit points to recover
    private const float RECOVER_PERCENTAGE = 0.20f;  // 20%

    // Random generator to determine if quick mend triggers
    private Random rand = new Random();

    private void OnTakeDamageHandler(Being attacker, Being victim)
    {
        // Store health before the damage is applied
        float healthBeforeDamage = victim.HitPoints;

        // Assuming damage is applied after this event handler, we calculate the health after damage
        float healthAfterDamage = healthBeforeDamage - attacker.CurrentDamage;

        // Check for the chance to trigger quick mend
        if (rand.NextDouble() <= MEND_TRIGGER_CHANCE)
        {
            // Calculate the lost hit points and recover a percentage of them
            float lostHitPoints = healthBeforeDamage - healthAfterDamage;
            float recoveryAmount = lostHitPoints * RECOVER_PERCENTAGE;

            victim.EffectsLog.Add($"Quick Mend Activated! Healed {victim.HitPoints.ToString()} + {recoveryAmount.ToString()}");

            victim.HitPoints += (int)recoveryAmount;

            // Ensure health doesn't exceed max health
            if (victim.HitPoints > victim.MaxHitPoints)
            {
                victim.HitPoints = victim.MaxHitPoints;
            }
        }
    }

    public void Execute(Being being)
    {
        being.OnTakeDamage += OnTakeDamageHandler;
    }
}


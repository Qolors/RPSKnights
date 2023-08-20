using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects;

public class DivineShield : IItemEffect
{
    public int EffectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Define the chance percentage to trigger the shield
    private const float SHIELD_TRIGGER_CHANCE = 0.05f;  // 5%

    // Define the percentage of damage to heal allies
    private const float HEAL_PERCENTAGE = 0.30f;  // 30%

    // Random generator to determine if the shield triggers
    private Random rand = new Random();

    private void OnTakeDamageHandler(Being attacker, Being victim)
    {
        // Check for the chance to trigger the shield
        if (rand.NextDouble() <= SHIELD_TRIGGER_CHANCE)
        {
            // Calculate the amount to heal allies
            int healingAmount = (int)(attacker.CurrentDamage * HEAL_PERCENTAGE);

            // Negate the damage the victim would have taken
            attacker.CurrentDamage = 0;

            // Heal all allies
            // Assumes there's a method or a list to get all allies of the victim
            List<Being> allies = GetAllAlliesOf(victim);
            foreach (Being ally in allies)
            {
                ally.HitPoints += healingAmount;

                // Ensure health doesn't exceed max health
                if (ally.HitPoints > ally.MaxHitPoints)
                {
                    ally.HitPoints = ally.MaxHitPoints;
                }
            }
        }
    }

    public void Execute(Being being)
    {
        being.OnTakeDamage += OnTakeDamageHandler;
    }

    // Mock method to get all allies of a being; implement as per your game logic
    private List<Being> GetAllAlliesOf(Being victim)
    {
        // Placeholder implementation
        // You would typically fetch this based on your game's context
        return new List<Being>();
    }
}

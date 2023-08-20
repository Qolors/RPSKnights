using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects;

public class ChainLightning : IItemEffect
{
    public int EffectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Define the chance percentage for the attack to chain
    private const float CHAIN_CHANCE = 0.15f;  // 15%

    // Define the damage multiplier for the chained attack
    private const float CHAINED_DAMAGE_MULTIPLIER = 0.50f;  // 50%

    // Random generator to determine if the attack chains
    private Random rand = new Random();

    private void OnAttackHandler(Being target, Being user)
    {
        // Check for the chance to trigger the chain lightning
        if (rand.NextDouble() <= CHAIN_CHANCE)
        {
            // Fetch a secondary target, ensuring it's not the original target
            Being secondaryTarget = GetSecondaryTarget(target);

            // If a valid secondary target is found, deal the chained damage
            if (secondaryTarget != null)
            {
                float chainedDamage = user.CurrentDamage * CHAINED_DAMAGE_MULTIPLIER;
                secondaryTarget.HitPoints -= (int)chainedDamage;

                // Optional: Ensure secondary target's health doesn't drop below zero
                if (secondaryTarget.HitPoints < 0)
                {
                    secondaryTarget.HitPoints = 0;
                }
            }
        }
    }

    public void Execute(Being being)
    {
        being.OnAttackEvent += OnAttackHandler;
    }

    // Mock method to fetch a secondary target; implement as per your game's mechanics
    private Being GetSecondaryTarget(Being originalTarget)
    {
        // Placeholder implementation
        // You would typically fetch a valid secondary target based on your game's context
        // ensuring it's not the same as the original target.
        return null; // Modify this based on how you track and manage enemies in your game.
    }
}


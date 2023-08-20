using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects;
public class EvasiveCloak : IItemEffect
{
    public int EffectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Define the chance percentage to trigger the dodge
    private const float DODGE_CHANCE = 0.15f;  // 15%

    // Random generator to determine if dodge triggers
    private Random rand = new Random();

    private void OnTakeDamageHandler(Being attacker, Being victim)
    {
        // Check for the chance to trigger the dodge
        if (rand.NextDouble() <= DODGE_CHANCE)
        {
            // Negate the damage by setting the attacker's current damage to zero
            attacker.CurrentDamage = 0;
        }
    }

    public void Execute(Being being)
    {
        being.OnTakeDamage += OnTakeDamageHandler;
    }
}


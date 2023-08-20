using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects;

public class FortifiedArmor : IItemEffect
{
    public int EffectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Define the hit points increase
    private const int HIT_POINTS_INCREASE = 50;

    public void Execute(Being being)
    {
        EquipArmor(being);
    }

    private void EquipArmor(Being being)
    {
        int currenthp = being.MaxHitPoints;
        being.MaxHitPoints += HIT_POINTS_INCREASE;

        if (currenthp == being.HitPoints)
        {
            being.HitPoints = being.MaxHitPoints;
        }

        // Ensure current health doesn't exceed max health
        if (being.HitPoints > being.MaxHitPoints)
        {
            being.HitPoints = being.MaxHitPoints;
        }
    }

}


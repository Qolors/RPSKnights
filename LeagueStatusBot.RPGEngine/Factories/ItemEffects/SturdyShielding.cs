using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects;

public class SturdyShielding : IItemEffect
{
    public int EffectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Define the armor class increase
    private const float ARMOR_CLASS_INCREASE = 0.05f;

    public void Execute(Being being)
    {
        EquipArmor(being);
    }

    private void EquipArmor(Being being)
    {
        being.ArmorClassValue += ARMOR_CLASS_INCREASE;

        // Since ArmorClass doesn't have a max value (unlike HitPoints), 
        // we don't need to check if it exceeds any limit.
    }

    // Optional: If you want a method to unequip the armor and revert the effect
    public void UnEquipArmor(Being being)
    {
        being.ArmorClassValue -= ARMOR_CLASS_INCREASE;
    }
}

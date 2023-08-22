using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ArmorEffects.Adventurer
{
    public class BulwarkBreastplate : IArmorEffect
    {
        public bool IsUsed { get; set; } = false;
        public int EffectId { get; set; } = 2;
        public string EffectFor { get; set; } = "Adventurer";
        public string Name { get; set; } = "Bulwark Breastplate";
        public string Description { get; set; } = "Activate to absorb all damage from a single attack.";

        public void ActivateArmor(Being self, Being? target, float damage)
        {
            self.BroadCast("Bulwark Breastplate - nullifying the attack");
            IsUsed = true;
            self.LastActionPerformed = Common.Models.ActionPerformed.ArmorAbility;
        }
    }

}

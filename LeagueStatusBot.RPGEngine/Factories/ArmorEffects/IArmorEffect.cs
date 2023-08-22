using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ArmorEffects
{
    public interface IArmorEffect
    {
        public bool IsUsed { get; set; }
        public int EffectId { get; set; }
        public string EffectFor { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public void ActivateArmor(Being self, Being? target, float damage);
    }
}

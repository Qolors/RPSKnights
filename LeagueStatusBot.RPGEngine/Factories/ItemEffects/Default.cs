using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects
{
    public class Default : IItemEffect
    {
        public int EffectId { get; set; }
        public string Name { get; set; } = "Default";
        public string Description { get; set; } = "Armor that does nothing.";
        public bool IsUsed { get; set; } = false;
        public void Register(Being being)
        {
            // Do nothing
        }

        public void OnExecutePassive(Being being)
        {
            // Do nothing
        }

        public void OnExecuteActive(Being being)
        {
            // Do nothing
        }
    }
}

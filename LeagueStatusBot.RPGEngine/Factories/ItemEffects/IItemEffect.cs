using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects
{
    public interface IItemEffect
    {
        public int EffectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUsed { get; set; }
        public string PrintPassiveStatus();
        public void OnExecutePassive(Being being);
        public void OnExecuteActive(Being being);
        public void Register(Being being);
    }
}

using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.ArmorEffects
{
    public class ArmorDefault : IArmorEffect
    {
        public bool IsUsed { get; set; } = true;
        public int EffectId { get; set; } = 1;
        public string EffectFor { get; set; } = "Adventurer";
        public string Name { get; set; } = "Default";
        public string Description { get; set; } = "Does nothing special.";
        public void ActivateArmor(Being self, Being? target, float damage)
        {
            
        }
    }
}

using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.ArmorEffects.Adventurer
{
    public class ChestOfFaith : IArmorEffect
    {
        public bool IsUsed { get; set; } = false;
        public int EffectId { get; set; } = 3;
        public string EffectFor { get; set; } = "Adventurer";
        public string Name { get; set; } = "Chest of Faith";
        public string Description { get; set; } = "Activate to heal your party for 50% of the attack";

        public void ActivateArmor(Being self, Being? target, float damage)
        {
            self.BroadCast("Chest of Faith - healing allies for 50% of the damage taken");
            float healingAmount = damage * 0.5f;
            self.TakeDamage(damage, DamageType.Normal, target);
            self.HealAOEDamage(healingAmount);
            IsUsed = true;
        }
    }
}

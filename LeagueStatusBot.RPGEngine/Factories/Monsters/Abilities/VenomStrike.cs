using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class VenomStrike : Ability
    {
        public VenomStrike(AbilityTemplate template)
        {
            Name = template.Name;
            Description = template.Description;
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float baseDamage = (user.BaseStats.Strength * 0.7f) + (user.BaseStats.Intelligence * 0.3f);

            float poisonDamage = user.BaseStats.Intelligence * 0.5f;

            user.BroadCast($"{user.Name} used {this.Name} on {target.Name}!");

            target.AddEffect(new Effect() { Duration = 3, Description = $"Poisoned (-{poisonDamage}hp per Turn", Type = EffectType.Poison, Name = this.Name, BufferDuration = 0, ModifierAmount = poisonDamage });

            // Apply the initial venom strike damage.
            target.TakeDamage(baseDamage, DamageType, user);
            return baseDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            // Immediate damage + potential poison damage over its duration
            return (user.BaseStats.Strength * 0.7) + (user.BaseStats.Intelligence * 0.3) + (3 * (user.BaseStats.Intelligence * 0.5));
        }
    }

}

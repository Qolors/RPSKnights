using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Adventurer
{
    public class ParryingStrike : Ability
    {
        public ParryingStrike()
        {
            Name = "Parrying Strike";
            Description = "A Defensive Strike - Dealing 0.5x Damage, but receive 25% Damage Reduction on Enemy Turns.\n\n - Cooldown: 1 Turn";
            DamageType = DamageType.Normal;

        }

        public override float Activate(Being user, Being? target)
        {
            user.AddEffect(new Effect()
            {
                Name = "Parrying Strike - 25% Damage Reduction",
                Type = EffectType.DamageReduction,
                Duration = 1,
                ModifierAmount = 0.25f
            });
            
            Cooldown = 2;

            return 1 + (user.BaseStats.Strength * 0.05f);
        }

        public override double ExpectedDamage(Being user)
        {
            return Math.Round(1 + (double)(user.BaseStats.Strength * 0.05f), 2);
        }
    }
}

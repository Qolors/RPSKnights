using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Vagabond
{
    public class FallBack : Ability
    {
        public FallBack()
        {
            Name = "Fall Back";
            Description = "Fall back gaining distance. Increasing Damage Reduction this round by 20%. The following 2 rounds, increase your Basic SKill damage output by 20%\n\n - Cooldown: 4 Turns";
        }

        public override float Activate(Being user, Being? target)
        {
            user.AddEffect(new Effect()
            {
                Name = "Fall Back: 20% Damage Reduction",
                Type = EffectType.DamageReduction,
                Duration = 1,
                BufferDuration = 0,
                ModifierAmount = 0.2f
            });

            user.AddDelayedEffect(new Effect()
            {
                Name = "Fall Back: 20% Damage Increase",
                Type = EffectType.DamageBoost,
                Duration = 2,
                BufferDuration = 1,
                ModifierAmount = 0.2f
            });

            Cooldown = 5;

            return 0.0f;
        }

        public override double ExpectedDamage(Being user)
        {
            return user.BasicAttack(strAdRatio: 1.25f);
        }
    }
}

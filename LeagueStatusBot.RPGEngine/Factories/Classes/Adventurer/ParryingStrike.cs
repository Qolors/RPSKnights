using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Adventurer
{
    public class ParryingStrike : Ability
    {
        public ParryingStrike()
        {
            Name = "Parrying Strike";
            Description = "A Defensive Strike - Dealing 0.5x Damage, but receive 25% Damage Reduction on Enemy Turns.\n\n - Cooldown: 3 Turns";
            DamageType = DamageType.Normal;

        }

        public override float Activate(Being user, Being? target)
        {
            user.AddEffect(new Effect()
            {
                Name = "Parrying Strike - 25% Damage Reduction",
                Type = EffectType.DamageReduction,
                Duration = 2,
                ModifierAmount = 0.25f
            });
            
            Cooldown = 4;

            user.LastActionPerformed = Common.Models.ActionPerformed.FirstAbility;

            return (user.BasicAttack() / 2);
        }

        public override double ExpectedDamage(Being user)
        {
            return Math.Round((double)(user.BasicAttack() / 2), 2);
        }
    }
}

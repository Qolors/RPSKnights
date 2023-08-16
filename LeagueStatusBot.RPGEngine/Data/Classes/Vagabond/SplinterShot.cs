using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Vagabond
{
    public class SplinterShot : Ability
    {
        public SplinterShot()
        {
            Name = "Splinter shot";
            Description = "Fire a powerful shot, causing the arrow to splinter on hit dealing Basic Attack Damage, but causes 1 + (1% Max Health) bleeding for 2 rounds after\n\n - Cooldown: 2 Turns";
            DamageType = DamageType.Ranged;
        }

        public override float Activate(Being user, Being? target)
        {
            target?.AddEffect(new Effect()
            {
                Name = "Splinter Shot",
                Type = EffectType.Bleed,
                BufferDuration = 0,
                Duration = 2,
                ModifierAmount = 0.1f
            });

            Cooldown = 3;

            return user.BasicAttack();
        }

        public override double ExpectedDamage(Being user)
        {
            return user.BasicAttack();
        }
    }
}

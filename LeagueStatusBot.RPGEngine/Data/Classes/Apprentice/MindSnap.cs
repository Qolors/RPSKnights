using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Apprentice
{
    public class MindSnap : Ability
    {
        public MindSnap()
        {
            Name = "Mind Snap";
            Description = "A Snap of the fingers sends a concussive blow to the target's mind, causing them to be stunned and skip their next turns\n\n - Cooldown: 3 Turns";
            DamageType = DamageType.Arcane;
        }

        public override float Activate(Being user, Being? target)
        {
            target?.AddEffect(new Effect()
            {
                Name = "Mind Snap",
                Type = EffectType.Stun,
                Duration = 2,
                ModifierAmount = 0
            });

            Cooldown = 4;

            return 0.0f;
        }

        public override double ExpectedDamage(Being user)
        {
            return 0;
        }
    }
}

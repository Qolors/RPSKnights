using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Apprentice
{
    public class MindSnap : Ability
    {
        public MindSnap()
        {
            Name = "Mind Snap";
            Description = "A Snap of the fingers sends a concussive blow to the target's mind, causing them to be stunned and skip their next 2 turns\n\n - Cooldown: 5 Turns";
            DamageType = DamageType.Arcane;
        }

        public override float Activate(Being user, Being? target)
        {
            target?.AddEffect(new Effect()
            {
                Name = "Mind Snap",
                Description = "Causes the player to be stunned, unable to take action.",
                Type = EffectType.Stun,
                Duration = 2,
                ModifierAmount = 0
            });

            Cooldown = 6;

            user.LastActionPerformed = Common.Models.ActionPerformed.SecondAbility;

            return 0.0f;
        }

        public override double ExpectedDamage(Being user)
        {
            return 0;
        }
    }
}

using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class TailSwipe : Ability
    {
        public TailSwipe(AbilityTemplate template)
        {
            Name = template.Name;
            Description = template.Description;
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);  // assuming DamageType is an enum
        }

        public TailSwipe()
        {
            Name = "Tail Swipe";
            Description = "Strong tail has a chance to stun enemies";
            Cooldown = 0;
            DamageType = DamageType.Normal;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float baseDamage = user.BaseStats.Strength * 1.5f;

            float stunChance = 20f + (user.BaseStats.Charisma > 10 ? user.BaseStats.Charisma - 10 : 0);
            bool isStunned = new Random().Next(100) < stunChance;

            user.BroadCast($"{user.Name} used {this.Name} on {target.Name}!");

            if (isStunned)
            {
                // Add a stun effect to the target. This is pseudocode since the mechanism isn't provided.
                // It's assumed that the `Being` class or some manager handles active effects on characters.
                target.AddEffect(new Effect()
                {
                    Name = "Tail Swipe",
                    Duration = 2,
                    Description = "Stunned for 2 rounds",
                    Type = EffectType.Stun,
                    ModifierAmount = 1,
                });  // e.g., stun for 1 turn

                user.BroadCast($"{user.Name} stunned {target.Name} for 2 rounds!");
            }

            //target.TakeDamage(baseDamage, DamageType, user);
            Cooldown = 2;
            return baseDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            // Simplified calculation. Consider other factors in a real scenario.
            return user.BaseStats.Strength * 1.5;
        }
    }

}

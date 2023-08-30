using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class DiveBomb : Ability
    {
        public DiveBomb(AbilityTemplate template)
        {
            Name = "Dive Bomb";
            Description = "Dives from the sky to strike the enemy with great force, can apply Confuse";
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public DiveBomb()
        {
            Name = "Dive Bomb";
            Description = "Dives from the sky to strike the enemy with great force, can apply Confuse";
            Cooldown = 0;
            DamageType = DamageType.Pierce;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float baseDamage = user.BaseStats.Agility * 2.5f;

            // High crit chance due to the nature of the attack
            bool isCrit = new Random().Next(100) < user.BaseStats.Luck * 3;

            float totalDamage = isCrit ? baseDamage * 2 : baseDamage;

            if (isCrit)
            {
                user.BroadCast($"**{this.Name}** on **{target.Name}** - **CRITICAL HIT**");
            }
            else
            {
                user.BroadCast($"**{this.Name}** on **{target.Name}**");
            }
            


            // If it's a crit, daze the target for a turn, simulating the shock from the high-speed impact
            if (isCrit)
            {
                target.AddDelayedEffect(new Effect() { BufferDuration = 1, Description = "Disables Spell Casting", Duration = 3, ModifierAmount = 1, Name = this.Name, Type = EffectType.Confuse }); // Assuming a function in Being class that makes the target dazed for a turn
                user.BroadCast($"and applied Dazed and Confused on {target.Name}");
            }

            Cooldown = 3;

            return totalDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            // Considering the high crit chance and bonus damage from it
            return user.BaseStats.Agility * 2.5 * 1.5;
        }
    }

}

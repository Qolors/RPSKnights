using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class WingSlash : Ability
    {
        public WingSlash(AbilityTemplate template)
        {
            Name = template.Name;
            Description = template.Description;
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public WingSlash()
        {
            Name = "Wing Slash";
            Description = "Uses powerful wings to deliver rapid slashes to opponents.";
            Cooldown = 0;
            DamageType = DamageType.Normal;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            int numberOfStrikes = new Random().Next(2, 4); // Randomly determine if 2 or 3 strikes occur
            float totalDamage = 0;

            for (int i = 0; i < numberOfStrikes; i++)
            {
                float baseDamage = user.BaseStats.Agility * 0.8f; // Lowered damage per hit due to multiple strikes

                // Retaining the high crit chance
                bool isCrit = new Random().Next(100) < user.BaseStats.Luck * 2.5;

                float damageThisStrike = isCrit ? baseDamage * 1.5f : baseDamage;

                totalDamage += damageThisStrike;

                if (isCrit)
                {
                    user.BroadCast($"{user.Name}'s {this.Name} critically strikes {target.Name}!");
                }
            }

            user.BroadCast($"{user.Name} used {this.Name} on {target.Name}, striking {numberOfStrikes} times!");

            // target.TakeDamage(totalDamage, DamageType, user);

            Cooldown = 2;

            return totalDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            // Factoring in the potential of multiple strikes and the high crit chance
            return user.BaseStats.Agility * 0.8 * 1.4 * 2.5; // Assuming an average of 2.5 strikes per activation
        }
    }

}

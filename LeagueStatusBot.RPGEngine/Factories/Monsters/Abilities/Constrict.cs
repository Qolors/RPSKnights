using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class Constrict : Ability
    {
        public Constrict(AbilityTemplate template)
        {
            Name = "Constrict";
            Description = "High Crit Chance Ability";
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public Constrict()
        {
            Name = "Constrict";
            Description = "High Ability Crit Chance";
            Cooldown = 0;
            DamageType = DamageType.Normal;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float baseDamage = user.BasicAttack();
            bool isCrit = new Random().Next(100) < user.BaseStats.Luck * 2;
            float totalDamage = isCrit ? baseDamage * 1.5f : baseDamage;

            if (isCrit)
            {
                user.BroadCast($"**{this.Name}** on **{target.Name}** - **CRITICAL HIT**");
            }
            else
            {
                user.BroadCast($"**{this.Name}** on **{target.Name}**");
            }

            Cooldown = 3;

            return totalDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            // Assuming a 25% boost for expected damage
            return user.BaseStats.Strength * 1.5 * 1.25;
        }
    }

}

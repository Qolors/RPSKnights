using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class JustTwo : Ability
    {
        public JustTwo(AbilityTemplate template)
        {
            Name = template.Name;
            Description = template.Description;
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public JustTwo()
        {
            Name = "Just Two";
            Description = "Can Strike Twice";
            Cooldown = 0;
            DamageType = DamageType.Normal;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float damage = user.BasicAttack() * 2;

            user.BroadCast($"{user.Name} used {this.Name} on {target.Name} attacking twice");

            //target.TakeDamage(damage, DamageType, user);

            Cooldown = 4;

            return damage;
        }

        public override double ExpectedDamage(Being user)
        {
            return (user.BaseStats.Strength * 0.7) + (user.BaseStats.Intelligence * 0.3) + (3 * (user.BaseStats.Intelligence * 0.5));
        }
    }
}

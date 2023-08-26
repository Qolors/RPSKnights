using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class Roar : Ability
    {
        public Roar(AbilityTemplate template)
        {
            Name = template.Name;
            Description = template.Description;
            Cooldown = template.Cooldown;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public Roar()
        {
            Name = "Roar";
            Description = "Unleashes a terrifying roar, disorienting and damaging nearby enemies.";
            Cooldown = 0;
            DamageType = DamageType.Magic;
        }

        public override float Activate(Being user, Being? target)
        {
            float totalDamage = 0;

            user.BroadCast($"{user.Name} used {this.Name}, causing fear in its foes!");

            Cooldown = 4;

            return totalDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            // Factoring in the potential damage to multiple targets, though the primary aim is crowd control
            return user.BaseStats.Endurance * 0.5 * 1.1; // Assuming an average scenario 
        }
    }

}

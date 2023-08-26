using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class VerbalAssault : Ability
    {
        public VerbalAssault(AbilityTemplate template) 
        {
            Name = template.Name;
            Description = template.Description;
            Cooldown = 0;
            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public VerbalAssault()
        {
            Name = "Verbal Assault";
            Description = "Weakens opponent's Strength";
            Cooldown = 0;
            DamageType = DamageType.Magic;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float baseDamage = (user.BaseStats.Strength * 0.7f) + (user.BaseStats.Intelligence * 0.3f);

            user.BroadCast($"{user.Name} used {this.Name} on {target.Name} unnecessarily!");

            target.AddEffect(new Effect() { Duration = 3, Description = "Debuff (-5 Str)", Type = EffectType.Debuff, Name = this.Name, BufferDuration = 0, ModifierAmount = 5 });

            //target.TakeDamage(baseDamage, DamageType, user);

            Cooldown = 3;

            return baseDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            return (user.BaseStats.Strength * 0.7) + (user.BaseStats.Intelligence * 0.3) + (3 * (user.BaseStats.Intelligence * 0.5));
        }
    }
}

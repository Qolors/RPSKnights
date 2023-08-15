using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Adventurer
{
    public class ParryingStrike : Ability
    {
        public ParryingStrike()
        {
            Name = "Parrying Strike";
            Description = "A Defensive Strike - Dealing 0.5x Damage, but receive 25% Damage Reduction on Enemy Turn";
        }

        public override float Activate(Being user, Being? target)
        {
            user.AddEffect(new Effect()
            {
                Name = "Parrying Strike - 25% Damage Reduction",
                Type = EffectType.DamageReduction,
                Duration = 1,
                ModifierAmount = 0.25f
            });

            return 1 + (user.BaseStats.Strength * 0.05f);
        }
    }
}

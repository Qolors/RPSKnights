using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Apprentice
{
    public class Apprentice : Being
    {
        public Apprentice()
        {
            ClassName = "Apprentice";
            ArmorClassValue = 0.1f;
            FirstAbility = new ArcaneBolt();
            SecondAbility = new MindSnap();
            MaxHitPoints = 30;
            HitPoints = 30;
            BaseStats = new()
            {
                Agility = 10,
                Charisma = 12,
                Luck = 10,
                Endurance = 10,
                Strength = 10,
                Intelligence = 12,
            };
        }

        public override float BasicAttack(float strAdRatio = 1)
        {
            float dmg = 0.1f;

            foreach (Effect effect in ActiveEffects)
            {
                if (effect.Type == EffectType.BasicDamageBoost)
                {
                    dmg = effect.ModifierAmount;
                }
            }

            return (1 + dmg * this.BaseStats.Intelligence) * strAdRatio;
        }
    }
}

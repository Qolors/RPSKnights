﻿using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Apprentice
{
    public class Apprentice : Being
    {
        public Apprentice()
        {
            ClassName = "Apprentice";
            MaxHitPoints = 18;
            HitPoints = 18;
            FirstAbility = new ArcaneBolt();
            SecondAbility = new MindSnap();
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

﻿using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.ItemEffects.Adventurer
{
    public class WhirlwindAxe : IItemEffect
    {
        public int EffectId { get; set; }
        public string Name { get; set; } = "Whirlwind Axe";
        public string Description { get; set; } = "Passively increases your Endurance by 1 for each hit taken, stacking up to 5 times. Active: Deal AoE damage to all enemies (10% Endurance * stacks).";
        public bool IsUsed { get; set; } = false;
        public int enduranceCounter = 0;

        public void Register(Being being)
        {
            being.OnDamageGiven += OnExecutePassive;
        }

        public void OnExecutePassive(Being being)
        {
            
            if (enduranceCounter < 5)
            {
                being.BaseStats.Endurance += 1;
                enduranceCounter++;
            }

            
        }

        public void OnExecuteActive(Being being)
        {
            being.OnDamageGiven -= OnExecutePassive;

            float damage = (0.1f * being.BaseStats.Endurance) * enduranceCounter;
            being.BaseStats.Endurance -= enduranceCounter;
            being.BroadCast(this.Name);
            // Logic to deal AoE damage to all nearby enemies
            being.DealAOEDamage(damage);

            IsUsed = true;
        }
    }

}
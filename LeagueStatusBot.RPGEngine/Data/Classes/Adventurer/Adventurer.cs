using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Adventurer
{
    public class Adventurer : Being
    {
        public Adventurer()
        {
            ClassName = "Adventurer";
            FirstAbility = new ParryingStrike();
            SecondAbility = new FirstAid();
            MaxHitPoints = 20;
            HitPoints = 20;

        }
    }
}

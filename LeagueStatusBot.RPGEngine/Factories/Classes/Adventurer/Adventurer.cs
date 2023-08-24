using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Adventurer
{
    public class Adventurer : Being
    {

        public Adventurer()
        {
            ClassName = "Adventurer";
            ArmorClassValue = 0.2f;
            FirstAbility = new ParryingStrike();
            SecondAbility = new FirstAid();
            BaseStats = new Stats();
            HitPoints = MaxHitPoints;
        }
    }
}

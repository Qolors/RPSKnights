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
            ArmorClassValue = 0.2f;
            FirstAbility = new ArcaneBolt();
            SecondAbility = new MindSnap();
            BaseStats = new();
            HitPoints = MaxHitPoints;
        }
    }
}

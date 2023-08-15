using LeagueStatusBot.RPGEngine.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Adventurer
{
    public class FirstAid : Ability
    {
        public FirstAid() 
        {
            Name = "First Aid";
            Description = "The User heals themselves for 50% of Max Health";
        }

        public override float Activate(Being user, Being? target)
        {
            if((int)(user.MaxHitPoints * 0.5f) > user.MaxHitPoints)
            {
                user.HitPoints = user.MaxHitPoints;
            }
            else
            {
                user.HitPoints += (int)(user.MaxHitPoints * 0.5f);
            }

            return 0.0f;
        }
    }
}

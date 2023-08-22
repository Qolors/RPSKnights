using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Adventurer
{
    public class FirstAid : Ability
    {
        public FirstAid() 
        {
            Name = "First Aid";
            Description = "Heals a target for 50% of your Max Health\n - Cooldown: 4 Turns";
            DamageType = DamageType.Heal;
        }

        public override float Activate(Being user, Being? target)
        {
            if((int)(user.MaxHitPoints * 0.5f + target?.HitPoints) > target?.MaxHitPoints)
            {
                target.HitPoints = target.MaxHitPoints;
            }
            else
            {
                target.HitPoints += (int)(user.MaxHitPoints * 0.5f);
            }

            Cooldown = 5;

            user.LastActionPerformed = Common.Models.ActionPerformed.SecondAbility;

            return 0.0f;
        }

        public override double ExpectedDamage(Being user)
        {
            return 0;
        }
    }
}

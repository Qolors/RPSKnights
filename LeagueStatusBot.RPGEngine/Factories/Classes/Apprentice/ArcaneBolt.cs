using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Apprentice
{
    public class ArcaneBolt : Ability
    {
        private int _consecutiveHits = 0;
        
        private Being? _previousTarget;
        public ArcaneBolt()
        {
            Name = "Arcane Bolt";
            Description = "A Bolt of Arcane Energy. 2 Consecutive attacks on the same enemy target increases the next Arcane Bolt's damage by 25%\n\n";
            DamageType = DamageType.Arcane;
        }

        public override float Activate(Being user, Being? target)
        {
            double baseDamage = 10.0f; // Adjust as needed
            double growthRate = 1.04; // Adjust as needed
            baseDamage *= Math.Pow(growthRate, user.BaseStats.Intelligence);

            if (_previousTarget is not null && _previousTarget.Name == target.Name)
            {
                _consecutiveHits++;

                if (_consecutiveHits == 2)
                {
                    user.BroadCast("Arcane Bolt's Extra Damage!");
                    baseDamage *= 1.25f; // Apply 25% damage boost
                    _consecutiveHits = 0;

                }
            }
            else
            {
                _consecutiveHits = 0;
            }

            _previousTarget = target;

            user.LastActionPerformed = Common.Models.ActionPerformed.FirstAbility;

            Cooldown = 2;

            return (float)baseDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            return Math.Round(1 + (double)(user.BaseStats.Intelligence * 0.2f), 2);
        }
    }
}

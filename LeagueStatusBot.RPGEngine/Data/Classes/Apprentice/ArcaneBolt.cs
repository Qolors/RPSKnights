using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Data.Classes.Apprentice
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
            float baseDamage = 1 + user.BaseStats.Intelligence * 0.2f; // Example base damage formula

            if (_previousTarget.Name == target.Name)
            {
                _consecutiveHits++;

                if (_consecutiveHits == 2)
                {
                    baseDamage *= 1.25f; // Apply 25% damage boost
                }
            }
            else
            {
                _consecutiveHits = 0;
            }

            _previousTarget = target;

            return baseDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            return Math.Round(1 + (double)(user.BaseStats.Intelligence * 0.2f), 2);
        }
    }
}

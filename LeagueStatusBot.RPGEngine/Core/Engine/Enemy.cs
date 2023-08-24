
namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Enemy : Being
    {
        public Enemy(string name, int enemyPowerScore)
        {
            Name = name;
            ClassName = "Monster";
            BaseStats = Stats.RollStatsToPowerScore(enemyPowerScore);
            HitPoints = MaxHitPoints;
        }

        public override float BasicAttack(float strAdRatio = 1)
        {
            return base.BasicAttack(strAdRatio);
        }

        public override void TakeDamage(float damage, DamageType damageType, Being attacker)
        {
            base.TakeDamage(damage, damageType, attacker);
        }

    }
}

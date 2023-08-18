using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Vagabond
{
    public class Vagabond : Being
    {
        public Vagabond()
        {
            ClassName = "Vagabond";
            ArmorClassValue = 0.15f;
            FirstAbility = new FallBack();
            SecondAbility = new SplinterShot();
            MaxHitPoints = 20;
            HitPoints = 20;
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

            return (1 + dmg * this.BaseStats.Agility) * strAdRatio;
        }
    }
}

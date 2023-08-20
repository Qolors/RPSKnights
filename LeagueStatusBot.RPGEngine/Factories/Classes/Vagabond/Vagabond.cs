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
            MaxHitPoints = 30;
            HitPoints = 30;
            BaseStats = new()
            {
                Agility = 10,
                Charisma = 10,
                Luck = 10,
                Endurance = 12,
                Strength = 12,
                Intelligence = 10,
            };
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

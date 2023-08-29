using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Classes.Vagabond
{
    public class Vagabond : Being
    {
        public Vagabond()
        {
            ClassName = "Vagabond";
            ArmorClassValue = 0.2f;
            FirstAbility = new FallBack();
            SecondAbility = new SplinterShot();
            BaseStats = new();
            HitPoints = MaxHitPoints;
        }
    }
}

using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Data.Classes.BladeMaster
{
    public class BladeMaster : Being
    {
        public BladeMaster(ClassTemplates template)
        {
            BaseStats = template.Blademaster;
        }

        public override float ArmorClassValue => 0.2f;

    }
}

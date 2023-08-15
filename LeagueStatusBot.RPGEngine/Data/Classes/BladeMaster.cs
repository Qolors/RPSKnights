using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Data.Classes
{
    public class BladeMaster : Being
    {
        public BladeMaster(ClassTemplates template) 
        {
            this.BaseStats = template.Blademaster;
        }

        public override float ArmorClassValue => 0.2f;

    }
}

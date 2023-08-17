using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Data.Classes.Adventurer;
using LeagueStatusBot.RPGEngine.Data.Classes.Apprentice;
using LeagueStatusBot.RPGEngine.Data.Classes.Vagabond;

namespace LeagueStatusBot.RPGEngine.Data
{
    public static class ClassFactory
    {
        //TODO --> IMPLEMENT CLASS TEMPLATE JSON FILE
        public static Being CreateAdventurer()
        {
            return new Adventurer();
        }

        public static Being CreateVagabond()
        {
            return new Vagabond();
        }

        public static Being CreateApprentice()
        {
            return new Apprentice();
        }
    }
}

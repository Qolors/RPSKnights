using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Factories.Classes.Adventurer;
using LeagueStatusBot.RPGEngine.Factories.Classes.Apprentice;
using LeagueStatusBot.RPGEngine.Factories.Classes.Vagabond;

namespace LeagueStatusBot.RPGEngine.Factories
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

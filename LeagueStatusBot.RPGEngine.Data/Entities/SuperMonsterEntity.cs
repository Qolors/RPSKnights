using LeagueStatusBot.Common.Models;

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class SuperMonsterEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public Super FirstSuper { get; set; }
        public Super SecondSuper { get; set; }
    }
}

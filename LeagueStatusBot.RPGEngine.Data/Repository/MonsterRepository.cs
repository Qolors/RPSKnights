using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Data.Entities;

namespace LeagueStatusBot.RPGEngine.Data.Repository
{
    public class MonsterRepository
    {
        private GameDbContext context;
        public MonsterRepository(GameDbContext context)
        {
            this.context = context;
        }

        public SuperMonsterEntity GetRandomSuperMonster()
        {
            var effects = context.SuperMonsters.ToList();
            var random = new Random();
            var randomMonster = effects.SingleOrDefault(x => x.Name == "Abyssion");
            return randomMonster;
        }

        public CampaignEntity GetMonsterCampaign(string name)
        {
            return context
                .Campaigns.SingleOrDefault(b => b.MonsterName == name);
        }
    }
}

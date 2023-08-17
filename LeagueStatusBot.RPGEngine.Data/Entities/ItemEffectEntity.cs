

using LeagueStatusBot.RPGEngine.Data.Models;

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class ItemEffectEntity
    {
        public string EffectName { get; set; }
        public string EffectType { get; set; }
        public int EffectId { get; set; }
        public EffectClass EffectClass { get; set; }
        public string Description { get; set; }
        
    }
}

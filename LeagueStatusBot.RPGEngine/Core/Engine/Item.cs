

using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Factories.ItemEffects;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
        public ItemRarity Rarity { get; set; }
        public IItemEffect Effect { get; set; }
    }
}

using LeagueStatusBot.Common.Models;

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class ItemEntity
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; } // Slot Allowed
        public ItemRarity Rarity { get; set; } // enum - Common has no Item Effects, Enchanted has Item Effects
        public int ItemEffect { get; set; }

    }
}

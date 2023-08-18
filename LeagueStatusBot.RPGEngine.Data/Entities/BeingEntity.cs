

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class BeingEntity
    {
        public ulong DiscordId { get; set; }
        public int Weapon { get; set; }
        public int Helm { get; set; }
        public int Chest { get; set; }
        public int Gloves { get; set; }
        public int Boots { get; set; }
        public int Legs { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public int Strength { get; set; }
        public int Luck { get; set; }
        public int Endurance { get; set; }
        public int Charisma { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public int MaxHitPoints { get; set; }
        public List<int> Inventory { get; set; }
    }
}

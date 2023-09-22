

namespace LeagueStatusBot.RPGEngine.Data.Entities
{
    public class BeingEntity
    {
        public ulong DiscordId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EloRating { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public ulong ServerId { get; set; } // Foreign key
        public ServerEntity Server { get; set; } // Navigation property
    }
}

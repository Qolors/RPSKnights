using LeagueStatusBot.RPGEngine.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeagueStatusBot.RPGEngine.Data.Contexts
{
    public class GameDbContext : DbContext
    {
        public DbSet<BeingEntity> Beings { get; set; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<ItemEffectEntity> ItemEffects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=game.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemEntity>().HasKey(x => x.ItemId);
            modelBuilder.Entity<ItemEffectEntity>().HasKey(x => x.EffectId);
            modelBuilder.Entity<BeingEntity>().HasKey(x => x.DiscordId);
            modelBuilder.Entity<BeingEntity>()
                .Property(e => e.Inventory)
                .HasConversion(
                v => string.Join(',', v),    // Convert List<int> to string for storage
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse).ToList());
        }
    }
}

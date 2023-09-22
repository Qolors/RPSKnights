using LeagueStatusBot.RPGEngine.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeagueStatusBot.RPGEngine.Data.Contexts;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<ServerEntity> Servers { get; set; }
    public DbSet<BeingEntity> Beings { get; set; }

    // Use when performing a migration through CLI
    private static DbContextOptions GetOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
        optionsBuilder.UseSqlite("Data Source=game.db");
        return optionsBuilder.Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeingEntity>()
            .HasKey(x => x.DiscordId);
        
        modelBuilder.Entity<BeingEntity>()
            .HasOne(b => b.Server)
            .WithMany(s => s.Beings)
            .HasForeignKey(b => b.ServerId); // Setting up the foreign key relationship
    }
}


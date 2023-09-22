using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Data.Entities;

namespace LeagueStatusBot.RPGEngine.Data.Repository
{
    public class PlayerRepository
    {
        private GameDbContext dbContext;
        public PlayerRepository(GameDbContext gameDbContext)
        {
            this.dbContext = gameDbContext;
        }

        public async Task UpdateOrAddPlayer(ulong serverId, ulong playerId, string playerName, int eloChange, bool isWin)
    {
        // Check if the server exists
        var server = await dbContext.Servers.FindAsync(serverId);
        if (server == null)
        {
            server = new ServerEntity { ServerId = serverId };
            dbContext.Servers.Add(server);
        }

        // Check if the player exists
        var player = await dbContext.Beings.FindAsync(playerId);
        if (player == null)
        {
            player = new BeingEntity
            {
                DiscordId = playerId,
                Name = playerName,
                EloRating = 1200 + eloChange, // Assuming 1200 as the default starting ELO rating
                Wins = isWin ? 1 : 0,
                Losses = isWin ? 0 : 1,
                ServerId = serverId
            };
            dbContext.Beings.Add(player);
        }
        else
        {
            player.EloRating += eloChange;
            if (isWin) player.Wins += 1;
            else player.Losses += 1;
        }

        await dbContext.SaveChangesAsync();
    }
    }
}

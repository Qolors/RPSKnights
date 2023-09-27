using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Data.Entities;
using Microsoft.EntityFrameworkCore;

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
                player.EloRating = eloChange;
                if (isWin) player.Wins += 1;
                else player.Losses += 1;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<int?> GetPlayerElo(ulong serverId, ulong userId)
        {
            
            // Attempt to find the player in the current server
            var player = await dbContext.Beings
                .Where(b => b.ServerId == serverId && b.DiscordId == userId)
                .FirstOrDefaultAsync();

            // If the player doesn't exist, return null
            if (player == null)
            {
                return null;
            }

            // Return the player's current ELO rating
            return player.EloRating;

        }

        public async Task<List<string>> GetLeaderboard(ulong serverId)
        {
            // Retrieve the top 10 players with the highest EloRating score
            var topPlayers = await dbContext.Beings
                .Where(b => b.ServerId == serverId)
                .OrderByDescending(b => b.EloRating)
                .Take(10)
                .ToListAsync();

            // If there are not 10 or more players, get as many as there are
            var playerNames = topPlayers.Select(p => p.Name + "-" + p.EloRating + "&" + "W/L -" + p.Wins + "/" + p.Losses).ToList();

            return playerNames;
            
        }
    }
}

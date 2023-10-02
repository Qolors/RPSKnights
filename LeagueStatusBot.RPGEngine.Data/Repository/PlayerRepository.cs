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

            var player = await dbContext.Beings.FindAsync(playerId);

            if (player == null)
            {
                player = new BeingEntity
                {
                    DiscordId = playerId,
                    Name = playerName,
                    EloRating = 1200 + eloChange,
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

            var player = await dbContext.Beings
                .Where(b => b.ServerId == serverId && b.DiscordId == userId)
                .FirstOrDefaultAsync();

            if (player == null)
            {
                return null;
            }

            return player.EloRating;

        }

        public async Task<List<string>> GetLeaderboard(ulong serverId)
        {
            var topPlayers = await dbContext.Beings
                .Where(b => b.ServerId == serverId)
                .OrderByDescending(b => b.EloRating)
                .Take(10)
                .ToListAsync();

            var playerNames = topPlayers.Select(p => p.Name + "-" + p.EloRating + "&" + "W/L -" + p.Wins + "/" + p.Losses).ToList();

            return playerNames;
        }
    }
}

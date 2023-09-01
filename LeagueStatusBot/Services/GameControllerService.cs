
using Discord.WebSocket;
using System.Collections.Generic;
using System.Timers;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using LeagueStatusBot.RPGEngine.Data.Repository;

namespace LeagueStatusBot.Services
{
    public class GameControllerService
    {
        private Timer timer;
        private readonly GameManager gameManager;
        private const ulong GUILD_ID = 402652836606771202;
        private const ulong CHANNEL_ID = 702684769200111716;

        public GameControllerService(DiscordSocketClient client, GameManager gameManager)
        {
        }
    }
}

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.RPGEngine.Data.Repository;
using LeagueStatusBot.Services;
using System;
using System.Threading.Tasks;

namespace LeagueStatusBot.Modules
{
    public class RPGModule : InteractionModuleBase<SocketInteractionContext>
    {
        private GameControllerService gameControllerService;
        private DiscordSocketClient client;
        private InteractiveService interactiveService;

        public RPGModule(GameControllerService gameControllerService, DiscordSocketClient client, InteractiveService interactiveService)
        {
            this.gameControllerService = gameControllerService;
            this.interactiveService = interactiveService;
            this.client = client;
        }
    }
}

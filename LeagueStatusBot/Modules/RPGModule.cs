using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Modules
{
    public class RPGModule : InteractionModuleBase<SocketInteractionContext>
    {
        private GameControllerService gameControllerService;
        private DiscordSocketClient client;
        private const string emoji = "\u2694\uFE0F";
        public RPGModule(GameControllerService gameControllerService, DiscordSocketClient client)
        {
            this.gameControllerService = gameControllerService;
            this.client = client;
        }

        [SlashCommand("join", "Join the adventure")]
        public async Task Join()
        {
            if (!gameControllerService.IsLobbyOpen)
            {
                await RespondAsync("There is no Party currently active", ephemeral: true);
                return;
            }

            gameControllerService.JoinLobby(Context.User.Id, Context.User.Username);

            await RespondAsync($"You joined the Adventure! :)", ephemeral: true);

            await PostToFeedChannel.SendChannelMessage($"- {Context.User.Username} has joined the party! {emoji}", client);
        }
    }
}

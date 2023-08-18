using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.RPGEngine.Data.Repository;
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

        [SlashCommand("roll", "Roll for a new Character!")]
        public async Task GenerateNewCharacter()
        {
            await DeferAsync();

            if (gameControllerService.CheckIfPlayerExists(Context.User.Id))
            {
                Console.WriteLine("1");
                await FollowupAsync("You already have a character created!", ephemeral: true);
                return;
            }

            if (gameControllerService.AddNewCharacter(Context.User.Id, Context.User.Username))
            {
                Console.WriteLine("2");
                await FollowupAsync("Character Created!", ephemeral: true);
            }
            else
            {
                Console.WriteLine("3");
                await FollowupAsync("Character Creation Error..", ephemeral: true);
            }

            
        }
    }
}

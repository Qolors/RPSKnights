using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using LeagueStatusBot.Services;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueStatusBot.Modules
{
    public class RPGModule : InteractionModuleBase<SocketInteractionContext>
    {
        private HttpClient hclient;
        private GameControllerService gameControllerService;
        private GameManager gameManager;
        private DiscordSocketClient client;
        private InteractiveService interactiveService;

        public RPGModule(GameControllerService gameControllerService, DiscordSocketClient client, InteractiveService interactiveService)
        {
            this.gameControllerService = gameControllerService;
            this.interactiveService = interactiveService;
            this.client = client;
            gameManager = new();
        }

        [SlashCommand("attack", "Attack user")]
        public async Task GenerateGif(SocketUser user)
        {
            await DeferAsync();
            hclient = new HttpClient();
            try
            {
                using (var bytes = await hclient.GetStreamAsync(user.GetAvatarUrl()))
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(bytes))
                {
                    if (gameManager.ExecuteTurn(image))
                    {
                        await FollowupWithFileAsync("animation.gif");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }
}

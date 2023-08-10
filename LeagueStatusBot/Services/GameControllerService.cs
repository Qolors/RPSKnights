
using Discord.WebSocket;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LeagueStatusBot.Services
{
    public class GameControllerService
    {
        private Timer timer;
        private GameManager gameManager;
        private DiscordSocketClient client;
        public List<ulong> Members { get; set; } = new List<ulong>();

        public GameControllerService(DiscordSocketClient client)
        {
            this.gameManager = new GameManager();
            this.client = client;
        }

        public async Task InitializeAsync()
        {
            client.Ready += SetupTimer;

            gameManager.GameStarted += OnGameStarted;
            gameManager.GameEnded += OnGameEnded;
            gameManager.GameEvent += OnGameEvent;
            gameManager.GameDeath += OnGameDeath;

        }

        private async Task SetupTimer()
        {
            timer = new Timer(10000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        public void AddMember(ulong id)
        {
            Members.Add(id);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();
            gameManager.StartGame();
        }
        private void OnGameStarted(object sender, EventArgs e)
        {
        }

        private async void OnGameEnded(object sender, EventArgs e)
        {
            await SendEventHistoryAsync();
        }

        private void OnGameEvent(object sender, string e)
        {
            
        }

        private void OnGameDeath(object sender, string e)
        {
            
        }

        private async Task SendEventHistoryAsync()
        {
            ulong guildId = 402652836606771202;
            ulong channelId = 702684769200111716;

            var channel = client.GetGuild(guildId).GetTextChannel(channelId);

            // You can either send the whole history as one big message:
            // await channel.SendMessageAsync(string.Join("\n", _gameManager.EventHistory));

            // Or you can chunk it if you're concerned about hitting message length limits:
            const int chunkSize = 10; // send in chunks of 10 events, for example
            for (int i = 0; i < gameManager.EventHistory.Count; i += chunkSize)
            {
                var chunk = gameManager.EventHistory.Skip(i).Take(chunkSize);
                string result = "```csharp\n" + string.Join("\n", chunk) + "```";
                await channel.SendMessageAsync(result);
            }
        }
    }
}

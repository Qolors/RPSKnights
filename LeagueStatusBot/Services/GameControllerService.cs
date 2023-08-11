
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

        public bool IsLobbyOpen { get; set; } = false;

        private const ulong GUILD_ID = 402652836606771202;
        private const ulong CHANNEL_ID = 702684769200111716;
        private const int LOBBY_DURATION = 1000 * 60 * 2;
        private const int FINAL_MINUTE_DURATION = 1000 * 60;
        public Dictionary<ulong, string> Members { get; set; } = new();

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
            gameManager.RoundEnded += OnRoundEnded;

        }

        private async Task SetupTimer()
        {
            timer = new Timer(LOBBY_DURATION - FINAL_MINUTE_DURATION);
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        private async void OnLobbyOpen(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();

            await SendChannelMessage("**An adventure is starting in 60 seconds...**\n */join to join the adventure*\n");

            IsLobbyOpen = true;

            timer = new Timer(FINAL_MINUTE_DURATION); // Set for the final minute
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            IsLobbyOpen = false;
            timer.Dispose();

            if (Members.Any())
            {
                //ADDING MEMBERS TO PARTY TO TEST
                Members.Add(402652836696745202, "Chris Minion1");
                Members.Add(402122836606771202, "Chris Minion2");
                await gameManager.StartGameAsync(Members);
            }

            Members.Clear();
        }

        public async void JoinLobby(ulong id, string name)
        {
            if (Members.ContainsKey(id))
            {
                await SendChannelMessage("You are already in the party!");
            }
            else
            {
                Members.Add(id, name);
            }
        }
        private async void OnGameStarted(object sender, EventArgs e)
        {
            string battleBeginString = "__Battle Has Started!__\n";

            battleBeginString += "The Enemy:\n";

            foreach (var member in gameManager.CurrentEncounter.EncounterParty.Members)
            {
                battleBeginString += $"- **{member.Name}**  Hit Points: {member.MaxHitPoints}\n";
            }

            battleBeginString += "The Heroic Party:\n";

            foreach (var member in gameManager.CurrentEncounter.PlayerParty.Members)
            {
                battleBeginString += $"- **{member.Name}**  Hit Points: {member.MaxHitPoints}\n";
            }

            await SendChannelMessage(battleBeginString);
        }

        private async void OnGameEnded(object sender, string e)
        {
            await SendChannelMessage($"**{e}**");
        }

        private void OnGameEvent(object sender, string e)
        {
            
        }

        private void OnGameDeath(object sender, string e)
        {
            
        }

        private async void OnRoundEnded(object sender, EventArgs e)
        {
            await SendEventHistoryAsync();
        }

        private async Task SendEventHistoryAsync()
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            await channel.SendMessageAsync("```csharp\n" + string.Join("\n", gameManager.EventHistory) + "\n```");
        }

        private async Task SendChannelMessage(string message)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            await channel?.SendMessageAsync(message);
        }
    }
}

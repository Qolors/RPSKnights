
using Discord;
using Discord.WebSocket;
using LeagueStatusBot.Common.Models;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace LeagueStatusBot.Services
{
    public class GameControllerService
    {
        private Timer timer;
        private GameManager gameManager;
        private DiscordSocketClient client;
        private HttpClient webClient;

        public bool IsLobbyOpen { get; set; } = false;

        private const ulong GUILD_ID = 402652836606771202;
        private const ulong CHANNEL_ID = 702684769200111716;
        private const int LOBBY_DURATION = 1000 * 60 * 2;
        private const int FINAL_MINUTE_DURATION = 1000 * 60;

        public ulong TurnRequestMessageId { get; set; } = 0;
        public ulong TurnEvenHistoryMessagedId { get; set; } = 0;
        public Dictionary<ulong, string> Members { get; set; } = new();

        public GameControllerService(DiscordSocketClient client)
        {
            this.gameManager = new GameManager();
            this.client = client;
            webClient = new HttpClient();
        }

        public async Task InitializeAsync()
        {
            client.Ready += SetupTimer;

            gameManager.GameStarted += OnGameStarted;
            gameManager.GameEnded += OnGameEnded;

            gameManager.GameEvent += OnGameEvent;
            gameManager.GameDeath += OnGameDeath;

            gameManager.RoundEnded += OnRoundEnded;
            gameManager.RoundStarted += OnRoundStarted;

            gameManager.TurnStarted += OnTurnStarted;
            gameManager.TurnEnded += OnTurnEnded;

        }

        private async Task SetupTimer()
        {
            timer = new Timer(15000);
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

            battleBeginString += "LOCATION - GENERATE FANTASY WORLD";
            battleBeginString += "AREA - GENERATE AREA";

            battleBeginString += "The Enemy:\n";

            foreach (var member in gameManager.CurrentEncounter.EncounterParty.Members)
            {
                battleBeginString += $"- **{member.Name}**  Hit Points: {member.MaxHitPoints}\n";
                battleBeginString += "- Player: false";
                battleBeginString += "- Class: Monster Weapon: Claws\n";
                battleBeginString += """
                    Stats:
                    "strength": 5,
                    "dexterity": 5,
                    "constitution": 5,
                    "intelligence": 10,
                    "wisdom": 5,
                    "charisma": 5
                    """;
            }

            battleBeginString += "The Heroic Party:\n";

            foreach (var member in gameManager.CurrentEncounter.PlayerParty.Members)
            {
                battleBeginString += $"- **{member.Name}**  Hit Points: {member.MaxHitPoints}\n";
                battleBeginString += "- Player: true";
                battleBeginString += "- Class: Human Weapon: Fists\n";
                battleBeginString += """
                    Stats:
                    "strength": 10,
                    "dexterity": 10,
                    "constitution": 10,
                    "intelligence": 10,
                    "wisdom": 10,
                    "charisma": 10
                    """;
            }

            var narrative = await webClient.GetAsync($"http://api:80/NarrativeAssist?prompt={battleBeginString}");
            var response = await narrative.Content.ReadAsStringAsync();

            await SendChannelMessage(response);

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

        private async void OnRoundEnded(object sender, List<string> e)
        {
            var narrative = await webClient.GetAsync($"http://api:80/NarrativeAddOn?prompt={string.Join("\n", e)}");
            var response = await narrative.Content.ReadAsStringAsync();

            if (response.StartsWith("^GAME^"))
            {
                Console.WriteLine("Game Determined Over");
                return;
            }

            gameManager.UpdatePlayerStats(PostToFeedChannel.ExtractFromGPT(response));

            await SendEventHistoryAsync(PostToFeedChannel.ExtractTextBeforeHash(response));
        }

        private async void OnRoundStarted(object sender, EventArgs e)
        {
           
        }

        private async void OnTurnStarted(object sender, PlayerTurnRequest e)
        {
           await SendTurnRequest(e);
        }

        private async void OnTurnEnded(object sender, List<string> e)
        {
            
        }

        private async Task SendEventHistoryAsync(string turnEvents)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            if (TurnEvenHistoryMessagedId == 0)
            {
                var eventMessage = await channel.SendMessageAsync("```csharp\n" + string.Join("\n", turnEvents) + "\n```");
                TurnEvenHistoryMessagedId = eventMessage.Id;
            }
            else
            {
                var console = await channel.GetMessageAsync(TurnEvenHistoryMessagedId);

                await channel.ModifyMessageAsync(TurnEvenHistoryMessagedId, m => 
                {
                    m.Content = string.Join("\n", turnEvents);
                });
            }
            
        }

        private async Task SendChannelMessage(string message)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            await channel?.SendMessageAsync(message);
        }

        private async Task SendTurnRequest(PlayerTurnRequest player)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            var builder = new ComponentBuilder()
                .WithButton("Perform Turn Actions", "perform-actions");

            if (TurnRequestMessageId == 0)
            {
                var consoleBlock = await channel?.SendMessageAsync("```csharp\n \n```");
                var newMessage = await channel?.SendMessageAsync($"Turn Started for **@{player.Name}**. You have 30 seconds to decide your actions.", components: builder.Build());

                TurnRequestMessageId = newMessage.Id;
                TurnEvenHistoryMessagedId = consoleBlock.Id;
            }
            else
            {
                await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                {
                    m.Content = $"Turn Started for **@{player.Name}**. You have 30 seconds to decide your actions.";
                    m.Components = builder.Build();
                });
            }

            

            
        }

        public PlayerTurnRequest ReceiveRequest(ulong id)
        {
            PlayerTurnRequest playerTurnRequest = gameManager.CheckIfActivePlayer(id);

            return playerTurnRequest;
        }

        public async void SetPlayerTarget(PlayerTurnRequest playerTurnRequest, string name)
        {
            gameManager.SetPlayerTarget(playerTurnRequest.UserId, name);
        }

        public async void SetPlayerTargetString(ulong id, string name)
        {
            gameManager.SetPlayerTarget(id, name);
        }

        public List<string> GetEnemies()
        {
            return gameManager.GetEnemyPartyNames();
        }
    }
}

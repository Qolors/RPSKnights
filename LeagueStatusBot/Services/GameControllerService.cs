
using Discord;
using Discord.WebSocket;
using LeagueStatusBot.Common.Models;
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
        private const int FINAL_MINUTE_DURATION = 1000 * 30;

        public ulong TurnRequestMessageId { get; set; } = 0;
        public ulong TurnEvenHistoryMessagedId { get; set; } = 0;
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
            gameManager.RoundStarted += OnRoundStarted;

            gameManager.TurnStarted += OnTurnStarted;
            gameManager.TurnEnded += OnTurnEnded;

        }

        private async Task SetupTimer()
        {
            timer = new Timer(10000);
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        private async void OnLobbyOpen(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();

            await SendChannelMessage("**An adventure is starting in 30 seconds...**\n */join to join the adventure*\n");

            IsLobbyOpen = true;

            timer = new Timer(30000); // Set for the final minute
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
            string battleBeginString = "**The Battle Has Begun!**\n";

            var embed = new EmbedBuilder()
                .WithTitle("The Player Party")
                .WithDescription("The Players recollect themselves as they land from the drop.");


            foreach (var member in gameManager.CurrentEncounter.PlayerParty.Members)
            {
                embed.AddField(f => f.WithName(member.Name).WithValue($"- Hit Points: {member.MaxHitPoints}\n - Class: Fighter").WithIsInline(false));
            }

            var embed2 = new EmbedBuilder()
                .WithTitle("The Enemy Party")
                .WithDescription("Nostrils flaring, teeth gritting, these monsters came for blood..");
                

            foreach (var member in gameManager.CurrentEncounter.EncounterParty.Members)
            {
                embed2.AddField(f => f.WithName(member.Name).WithValue($"- Hit Points: {member.MaxHitPoints}\n - Class: Monster").WithIsInline(false));
            }

            await SendChannelMessage(battleBeginString, new Embed[] { embed.Build(), embed2.Build() });
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
           await SendEventHistoryAsync(e);
        }

        private async Task SendEventHistoryAsync(List<string> turnEvents)
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
                    m.Content = ("```csharp\n" + string.Join("\n", turnEvents) + "\n```");
                });
            }
            
        }

        private async Task SendChannelMessage(string message, Embed[] embeds = null)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            if (embeds != null)
            {
                await channel?.SendMessageAsync(message, embeds: embeds);
            }
            else
            {
                await channel?.SendMessageAsync(message);
            }
            
        }

        private async Task SendTurnRequest(PlayerTurnRequest player)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            var builder = new ComponentBuilder()
                .WithButton("Perform Turn Actions", "perform-actions");

            var mentionName = channel.GetUser(player.UserId);

            if (TurnRequestMessageId == 0)
            {
                var consoleBlock = await channel?.SendMessageAsync("```csharp\n \n```");

                var newMessage = await channel?.SendMessageAsync($"Turn Started for **{mentionName.Mention}**. You have 12 seconds to decide your actions.", components: builder.Build());

                TurnRequestMessageId = newMessage.Id;
                TurnEvenHistoryMessagedId = consoleBlock.Id;
            }
            else
            {
                await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                {
                    m.Content = $"Turn Started for **@{mentionName.Mention}**. You have 12 seconds to decide your actions.";
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

        public List<string> GetEnemies()
        {
            return gameManager.GetEnemyPartyNames();
        }
    }
}

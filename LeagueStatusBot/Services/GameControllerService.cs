
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Data.Repository;

namespace LeagueStatusBot.Services
{
    public class GameControllerService
    {
        private Timer timer;
        private GameManager gameManager;
        private DiscordSocketClient client;
        private PlayerRepository playerRepository;
        private ItemRepository itemRepository;

        private Random random = new Random();

        public bool IsLobbyOpen { get; set; } = false;

        private const ulong GUILD_ID = 402652836606771202;
        private const ulong CHANNEL_ID = 702684769200111716;
        private const int LOBBY_DURATION = 1000 * 60 * 2;
        private const int FINAL_MINUTE_DURATION = 1000 * 30;
        private const string emoji = "\u2694\uFE0F";

        public ulong TurnRequestMessageId { get; set; } = 0;
        public ulong TurnEvenHistoryMessagedId { get; set; } = 0;
        public Dictionary<ulong, string> Members { get; set; } = new();

        public GameControllerService(DiscordSocketClient client, PlayerRepository playerRepository, ItemRepository itemRepository)
        {
            this.playerRepository = playerRepository;
            this.itemRepository = itemRepository;
            this.gameManager = new GameManager();
            this.client = client;

            client.Ready += SetupTimer;
        }

        public async Task InitializeAsync()
        {
            gameManager.GameStarted += OnGameStarted;
            gameManager.GameEnded += OnGameEnded;

            gameManager.GameEvent += OnGameEvent;
            gameManager.GameDeath += OnGameDeath;

            gameManager.RoundEnded += OnRoundEnded;
            gameManager.RoundStarted += OnRoundStarted;

            gameManager.TurnStarted += OnTurnStarted;
            gameManager.TurnEnded += OnTurnEnded;

            TurnEvenHistoryMessagedId = 0;
            TurnRequestMessageId = 0;
        }

        private async Task SetupTimer()
        {
            timer = new Timer(10000);
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        private double GetRandomInterval()
        {
            const double twoMinutesInMilliseconds = 480000; // 2 minutes
            const double oneMinuteInMilliseconds = 300000;   // 1 minute
            return twoMinutesInMilliseconds + random.NextDouble() * oneMinuteInMilliseconds;
        }

        private async void OnLobbyOpen(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();

            var button = new ComponentBuilder()
                .WithButton("Join Party", "join-party");

            await PostToFeedChannel.SendChannelMessage("**An adventure is starting in 30 seconds...**\n", client, messageComponent: button.Build());

            TurnEvenHistoryMessagedId = 0;
            TurnRequestMessageId = 0;
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
                var playerList = new List<Being>();
                foreach (var member in Members)
                {
                    playerList.Add(Mapper.BeingEntityToDomainModel(playerRepository.GetBeingByDiscordId(member.Key)));
                }

                await gameManager.StartGameAsync(playerList);
            }

            Members.Clear();

            timer = new Timer(GetRandomInterval());
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        public async void JoinLobby(ulong id, string name)
        {
            await PostToFeedChannel.SendChannelMessage($"- **{name}** has joined the party! {emoji}", client);
            Members.Add(id, name);
        }

        public bool CheckIfPlayerExists(ulong id)
        {
            return playerRepository.Exists(id);
        }

        private async void OnGameStarted(object sender, EventArgs e)
        {
            string battleBeginString = "**The Battle Has Begun!**\n";

            var embed = new EmbedBuilder()
                .WithTitle("The Player Party")
                .WithDescription("The Players recollect themselves as they land from the drop.");


            foreach (var member in gameManager.CurrentEncounter.PlayerParty.Members)
            {
                embed.AddField(f => f.WithName(member.Name).WithValue($"- Hit Points: {member.MaxHitPoints}\n - Class: {member.ClassName}").WithIsInline(false));
            }

            var embed2 = new EmbedBuilder()
                .WithTitle("The Enemy Party")
                .WithDescription("Nostrils flaring, teeth gritting, these monsters came for blood..");
                

            foreach (var member in gameManager.CurrentEncounter.EncounterParty.Members)
            {
                embed2.AddField(f => f.WithName(member.Name).WithValue($"- Hit Points: {member.MaxHitPoints}\n - Class: {member.ClassName}").WithIsInline(false));
            }

            await PostToFeedChannel.SendChannelMessage(battleBeginString, client, new Embed[] { embed.Build(), embed2.Build() });
        }

        private async void OnGameEnded(object sender, string e)
        {
            await PostToFeedChannel.SendChannelMessage($"**{e}**", client);
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

        private async void OnTurnStarted(object sender, Being e)
        {
            if (e.IsHuman)
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

        private async Task SendTurnRequest(Being player)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            var currentUser = channel.GetUser(player.DiscordId);

            bool isFirst = false;
            bool isSecond = false;

            if (player.FirstAbility.Cooldown > 0)
            {
                isFirst = true;
            }

            if (player.SecondAbility.Cooldown > 0)
            {
                isSecond = true;
            }

            var builder = new ComponentBuilder()
                .WithButton("Basic Attack", "basic-attack")
                .WithButton(player.FirstAbility.Name, "first-ability", disabled: isFirst)
                .WithButton(player.SecondAbility.Name, "second-ability", disabled: isSecond);

            var characterSheet = new EmbedBuilder()
                .AddField($"{player.FirstAbility.Name}", $"{player.FirstAbility.Description}\n- Expected Damage: {player.FirstAbility.ExpectedDamage(player)}")
                .AddField($"{player.SecondAbility.Name}", $"{player.SecondAbility.Description}\n- Expected Damage: {player.SecondAbility.ExpectedDamage(player)}")
                .WithThumbnailUrl(currentUser.GetAvatarUrl())
                .WithTitle(player.Name)
                .WithDescription($"""
                Class: {player.ClassName}

                HP: {player.HitPoints}/{player.MaxHitPoints}
                
                Active Effects:
                {string.Join("\n", player.ActiveEffects.Select(x => x.Name))}
                """);

            var mentionName = channel.GetUser(player.DiscordId);

            if (TurnRequestMessageId == 0)
            {
                var consoleBlock = await channel?.SendMessageAsync("```csharp\n \n```");

                var newMessage = await channel?.SendMessageAsync($"Turn Started for **{mentionName.Mention}**", components: builder.Build());

                TurnRequestMessageId = newMessage.Id;
                TurnEvenHistoryMessagedId = consoleBlock.Id;
            }
            else
            {
                await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                {
                    m.Content = $"Turn Started for **{mentionName.Mention}**.";
                    m.Embed = characterSheet.Build();
                    m.Components = builder.Build();
                });
            }
        }

        public async Task HandleActionAsync(SocketMessageComponent component, string attackType)
        {
            var playerTurn = this.ReceiveRequest(component.User.Id);

            if (playerTurn == null)
            {
                await component.RespondAsync("It is not your turn!", ephemeral: true);
            }
            else
            {
                switch (attackType)
                {
                    case "basic":
                        playerTurn.AttackTarget();
                        break;

                    case "ability1":
                        playerTurn.ChosenAbility(playerTurn.FirstAbility);
                        break;

                    case "ability2":
                        playerTurn.ChosenAbility(playerTurn.SecondAbility);
                        break;

                    default:
                        break;
                }

                await component.UpdateAsync(m =>
                {
                    m.Content = $"**{playerTurn.Name} completed turn.**";
                    m.Components = new ComponentBuilder().Build();
                });

                gameManager.CurrentEncounter?.RaisePlayerActionChosen(EventArgs.Empty);
            }
        }

        public Being ReceiveRequest(ulong id)
        {
            Being playerTurnRequest = gameManager.CheckIfActivePlayer(id);

            return playerTurnRequest;
        }

        public async void SetPlayerTarget(Being playerTurnRequest, string name)
        {
            gameManager.SetPlayerTarget(playerTurnRequest, name);
        }

        public List<string> GetEnemies()
        {
            return gameManager.GetEnemyPartyNames();
        }

        public List<string> GetAllies()
        {
            return gameManager.GetPlayerPartyNames();
        }

        public bool AddNewCharacter(ulong id, string name)
        {
            var being = gameManager.AssignRandomClass();
            being.Helm = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Weapon = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Chest = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Gloves = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Boots = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Legs = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Name = name;
            being.DiscordId = id;
            being.Inventory = new List<Item>();

            Console.WriteLine("Adding");

            return playerRepository.AddPlayerByDiscordId(Mapper.BeingToEntityModel(being));
        }
    }
}

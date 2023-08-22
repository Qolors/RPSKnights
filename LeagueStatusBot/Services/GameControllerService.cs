
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
using LeagueStatusBot.RPGEngine.Core.Events;
using LeagueStatusBot.Common.Models;

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

            Mapper.Initialize(this.playerRepository, this.itemRepository);
            UrlGetter.Initialize();

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
            timer = new Timer(20000);
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        private double GetRandomInterval()
        {
            const double twoMinutesInMilliseconds = 300000; // 2 minutes
            const double oneMinuteInMilliseconds = 300000;   // 1 minute
            return twoMinutesInMilliseconds + random.NextDouble() * oneMinuteInMilliseconds;
        }

        private async void OnLobbyOpen(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();

            await PostToFeedChannel.SendPortalMessage(client);

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

                await PostToFeedChannel.EditOldMessage("The Portal has closed..", client);
                await gameManager.StartGameAsync(playerList);
                
            }
            else
            {
                await PostToFeedChannel.EditOldMessage("The Portal has closed..", client);

                timer = new Timer(GetRandomInterval());
                timer.Elapsed += OnLobbyOpen;
                timer.AutoReset = false;
                timer.Start();
            }

            Members.Clear();

            
        }

        public async void JoinLobby(ulong id, string name)
        {
            await PostToFeedChannel.SendChannelMessage($"- **{name}** has entered. {emoji}", client);
            Members.Add(id, name);
        }

        public bool CheckIfPlayerExists(ulong id)
        {
            return playerRepository.Exists(id);
        }

        private async void OnGameStarted(object sender, EventArgs e)
        {
            string battleBeginString = "**The Battle Begins.**\n";

            List<Embed> embeds = new List<Embed>();

            foreach (var member in gameManager.CurrentEncounter.PlayerParty.Members)
            {
                var currentUser = client.GetUser(member.DiscordId);
                var embed = new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .WithImageUrl(currentUser.GetAvatarUrl())
                    .WithTitle(member.Name)
                    .AddField($"    Class: - {member.ClassName}", "..")
                    .AddField($"HitPoints: - {member.MaxHitPoints}/{member.MaxHitPoints}", "..")
                    .Build();
                embeds.Add(embed);
            }
                

            foreach (var member in gameManager.CurrentEncounter.EncounterParty.Members)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithImageUrl(UrlGetter.GetMonsterPortrait(member.Name))
                    .WithTitle(member.Name)
                    .AddField($"    Class: - {member.ClassName}", "..")
                    .AddField($"HitPoints: - {member.MaxHitPoints}/{member.MaxHitPoints}", "..")
                    .Build();
                embeds.Add(embed);
            }

            await PostToFeedChannel.SendChannelMessage(battleBeginString, client, embeds: embeds.ToArray());
        }

        private async void OnGameEnded(object sender, GameEndedEventArgs e)
        {
            await PostToFeedChannel.SendChannelMessage($"**{e.Announcement}**", client);

            if (e.IsVictory)
            {
                foreach (var member in e.PlayerParty)
                {
                    if (!member.IsAlive) continue;

                    var currentUser = client.GetUser(member.DiscordId);
                    var embed = new EmbedBuilder()
                        .WithColor(Color.Blue)
                        .WithThumbnailUrl(currentUser.GetAvatarUrl())
                        .WithTitle(member.Name)
                        .AddField($"    Class: - {member.ClassName}", "..")
                        .AddField($"HitPoints: - {member.MaxHitPoints}/{member.MaxHitPoints}", "..")
                        .Build();
                    var menu = new SelectMenuBuilder()
                        .WithPlaceholder("Select a Skill")
                        .WithCustomId("skill-select")
                        .WithMinValues(1)
                        .WithMaxValues(1)
                        .AddOption($"Strength ({member.BaseStats.Strength}) + 1", $"strength&{member.DiscordId}")
                        .AddOption($"Charisma ({member.BaseStats.Charisma}) + 1", $"charisma&{member.DiscordId}")
                        .AddOption($"Agility ({member.BaseStats.Agility}) + 1", $"agility&{member.DiscordId}")
                        .AddOption($"Endurance ({member.BaseStats.Endurance}) + 1", $"endurance&{member.DiscordId}")
                        .AddOption($"Luck ({member.BaseStats.Luck}) + 1", "luck&{member.DiscordId}")
                        .AddOption($"Intelligence ({member.BaseStats.Intelligence}) + 1", $"intelligence&{member.DiscordId}");
                    var comp = new ComponentBuilder()
                        .WithSelectMenu(menu);


                    await PostToFeedChannel.SendSkillUpMessage($"**{member.Name}** has earned themselves a skill point.", client, embeds: new[] { embed }, messageComponent: comp.Build());
                }
            }

            timer = new Timer(GetRandomInterval());
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        public async Task UpdateBeing(Being being, Skill skill)
        {
            switch (skill)
            {
                case Skill.Strength:
                    being.BaseStats.Strength++;
                    break;
                case Skill.Charisma:
                    being.BaseStats.Charisma++;
                    break;
                case Skill.Agility:
                    being.BaseStats.Agility++;
                    break;
                case Skill.Endurance:
                    being.BaseStats.Endurance++;
                    break;
                case Skill.Luck:
                    being.BaseStats.Luck++;
                    break;
                case Skill.Intelligence:
                    being.BaseStats.Intelligence++;
                    break;
            }

            Console.WriteLine("Pushing Update");

            await Task.Run(() => playerRepository.UpdatePlayerStats(being.DiscordId, being.BaseStats.Strength, being.BaseStats.Luck, being.BaseStats.Endurance, being.BaseStats.Charisma, being.BaseStats.Intelligence, being.BaseStats.Agility));

        }

        private void OnGameEvent(object sender, string e)
        {
            
        }

        private async void OnGameDeath(object sender, CharacterDeathEventArgs e)
        {
            if (e.IsHuman)
            {
                await Task.Run(() => { playerRepository.DeletePlayerByDiscordId(e.CharacterId); });
            }
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
            {
                await SendTurnRequest(e);
            }
                
        }

        private async void OnTurnEnded(object sender, TurnActionsEventArgs e)
        {
           await SendEventHistoryAsync(e);
        }

        private async Task SendEventHistoryAsync(TurnActionsEventArgs turnSummary)
        {
            if (turnSummary.CombatLogs.Count == 0) return;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            if (turnSummary.ActivePlayer.IsHuman)
            {
                var user = await client.GetUserAsync(turnSummary.ActivePlayer.DiscordId);

                var attackName = turnSummary.ActionPerformed switch
                {
                    ActionPerformed.BasicAttack => UrlGetter.GetAbilityImage("Basic"),
                    ActionPerformed.FirstAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.FirstAbility.Name),
                    ActionPerformed.SecondAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.SecondAbility.Name),
                    ActionPerformed.ArmorAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.Chest.Name),
                    ActionPerformed.WeaponAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.Weapon.Effect.Name),
                };

                var embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle(turnSummary.CombatLogs[0])
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithImageUrl(attackName)
                .WithDescription(string.Join("\n", turnSummary.CombatLogs.Skip(1)));

                if (TurnEvenHistoryMessagedId == 0)
                {
                    var eventMessage = await channel.SendMessageAsync(embed: embed.Build());
                    TurnEvenHistoryMessagedId = eventMessage.Id;
                }
                else
                {
                    await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                    {
                        m.Embed = embed.Build();
                    });
                }
            }
            else
            {
                var embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle(turnSummary.CombatLogs[0])
                .WithThumbnailUrl(UrlGetter.GetMonsterPortrait(turnSummary.ActivePlayer.Name))
                .WithImageUrl(UrlGetter.GetAbilityImage("Basic"))
                .WithDescription(string.Join("\n", turnSummary.CombatLogs.Skip(1)));

                if (TurnEvenHistoryMessagedId == 0)
                {
                    var eventMessage = await channel.SendMessageAsync(embed: embed.Build());
                    TurnEvenHistoryMessagedId = eventMessage.Id;
                }
                else
                {
                    await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                    {
                        m.Embed = embed.Build();
                    });
                }
            }
            

            
        }

        private async Task SendTurnRequest(Being player)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            var currentUser = await client.GetUserAsync(player.DiscordId);

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

            var builder = new ComponentBuilder();
            var characterSheet = new EmbedBuilder();

            if (this.ReceiveRequest(player.DiscordId) == null)
            {
                var attacker = gameManager.CurrentEncounter.CurrentTurn;

                builder
                .WithButton($"{player.Chest.Name}", "chest-ability", disabled: player.Chest.IsUsed)
                .WithButton("Take Damage", "take-damage");


                characterSheet
                    .AddField($"{player.Chest.Name}", $"{player.Chest.Description}\n")
                    .WithThumbnailUrl(currentUser.GetAvatarUrl())
                    .WithTitle($"{currentUser.Mention} is being attacked by {attacker.Name}")
                    .WithImageUrl(UrlGetter.GetMonsterPortrait(attacker.Name))
                    .WithDescription($"""
                Class: {attacker.ClassName}

                 Your HP: {player.HitPoints}/{player.MaxHitPoints}
                Their HP: {attacker.HitPoints}/{attacker.MaxHitPoints}
                
                Their Active Effects:
                {string.Join("\n", attacker.ActiveEffects.Select(x => x.Name))}
                """);
            }
            else
            {
                builder
                .WithButton("Basic Attack", "basic-attack")
                .WithButton(player.FirstAbility.Name, "first-ability", disabled: isFirst)
                .WithButton(player.SecondAbility.Name, "second-ability", disabled: isSecond)
                .WithButton(player.Weapon.ItemName, "weapon-active", disabled: player.Weapon.Effect.IsUsed);


                characterSheet
                    .AddField($"{player.Weapon.ItemName}", $"{player.Weapon.Effect.PrintPassiveStatus()}")
                    .WithThumbnailUrl(currentUser.GetAvatarUrl())
                    .WithTitle(player.Name)
                    .WithDescription($"""
                Class: {player.ClassName}

                HP: {player.HitPoints}/{player.MaxHitPoints}
                
                Active Effects:
                {string.Join("\n", player.ActiveEffects.Select(x => x.Name))}
                """);
            }

            

            var mentionName = channel.GetUser(player.DiscordId);

            if (TurnRequestMessageId == 0)
            {
                var consoleBlock = await channel?.SendMessageAsync("```\n \n```");

                var newMessage = await channel?.SendMessageAsync($"Turn Started for **{mentionName.Mention}**", components: builder.Build());

                TurnRequestMessageId = newMessage.Id;
                TurnEvenHistoryMessagedId = consoleBlock.Id;
            }
            else
            {
                if (this.ReceiveRequest(player.DiscordId) == null)
                {
                    await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                    {
                        m.Content = $"Defense Action for **{mentionName.Mention}**.";
                        m.Embed = characterSheet.Build();
                        m.Components = builder.Build();
                    });
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

                    case "weapon-active":
                        playerTurn.UseWeaponActive();
                        break;

                    case "take-damage":
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

        public async Task HandleDefenseAsync(SocketMessageComponent component, string attackType)
        {
            if (!gameManager.CurrentEncounter.CurrentTurn.Target.IsHuman || gameManager.CurrentEncounter.CurrentTurn.Target.DiscordId != component.User.Id)
            {
                await component.RespondAsync("It is not your turn!", ephemeral: true);
            }
            else
            {
                switch (attackType)
                {
                    case "chest-ability":
                        gameManager.CurrentEncounter.CurrentTurn.Target.ActiveDefenseItem = gameManager.CurrentEncounter.CurrentTurn.Target.Chest;
                        break;

                    case "take-damage":
                        gameManager.CurrentEncounter.CurrentTurn.Target.ActiveDefenseItem = null;
                        break;

                    default:
                        break;
                }

                await component.UpdateAsync(m =>
                {
                    m.Content = $"**{gameManager.CurrentEncounter.CurrentTurn.Target.Name} completed Defense Action.**";
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
            being.Helm = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Weapon = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
            being.Chest = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GenerateRandomChestArmor());
            being.Gloves = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Boots = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Legs = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Name = name;
            being.DiscordId = id;
            being.Inventory = new List<Item>();

            return playerRepository.AddPlayerByDiscordId(Mapper.BeingToEntityModel(being));
        }

        public Being GetCharacterInfo(ulong id)
        {
            var beingEntity = playerRepository.GetBeingByDiscordId(id);
            return Mapper.BeingEntityToDomainModel(beingEntity);
        }
    }
}

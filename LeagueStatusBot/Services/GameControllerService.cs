
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
        private readonly GameManager gameManager;
        private readonly DiscordSocketClient client;
        private readonly PlayerRepository playerRepository;
        private readonly ItemRepository itemRepository;
        private readonly MonsterRepository monsterRepository;
        private readonly Random random = new Random();
        public bool IsLobbyOpen { get; set; } = false;

        private const ulong GUILD_ID = 402652836606771202;
        private const ulong CHANNEL_ID = 702684769200111716;
        private const string emoji = "\u2694\uFE0F";

        public ulong TurnRequestMessageId { get; set; } = 0;
        public ulong TurnEvenHistoryMessagedId { get; set; } = 0;
        public Dictionary<ulong, string> Members { get; set; } = new();

        public GameControllerService(DiscordSocketClient client, PlayerRepository playerRepository, ItemRepository itemRepository, MonsterRepository monsterRepository)
        {
            this.playerRepository = playerRepository;
            this.itemRepository = itemRepository;
            this.gameManager = new GameManager();
            this.client = client;

            Mapper.Initialize(this.playerRepository, this.itemRepository, this.monsterRepository);
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
            const double oneMinuteInMilliseconds = 60000;   // 1 minute
            return twoMinutesInMilliseconds + (random.NextDouble() * oneMinuteInMilliseconds);
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

                Monster monster = Mapper.MonsterEntityToDomainModel(monsterRepository.GetRandomSuperMonster());
                Campaign campaign = Mapper.CampaignEntityToDomainModel(monsterRepository.GetMonsterCampaign(monster.Name));

                await PostToFeedChannel.EditOldMessage("The Portal has closed..", client);

                await PlayCampaignCinematic(campaign);

                await gameManager.StartGameAsync(playerList, monster);
                
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
            await PostToFeedChannel.SendChannelMessage($"**{name}** has entered. {emoji}", client);
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

            foreach (var player in gameManager.CurrentEncounter.PlayerParty.Members)
            {
                embeds.Add(CreatePlayerEmbed(player));
            }

            foreach (var monster in gameManager.CurrentEncounter.EncounterParty.Members)
            {
                embeds.Add(CreateMonsterEmbed(monster));
            }

            await PostToFeedChannel.SendChannelMessage(battleBeginString, client, embeds: embeds.ToArray());
        }

        private Embed CreatePlayerEmbed(Being player)
        {
            var currentUser = client.GetUser(player.DiscordId);
            return new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithThumbnailUrl(currentUser.GetAvatarUrl())
                .WithTitle(player.Name)
                .AddField($"Class: {player.ClassName}", "\u200b")
                .AddField($"\u2665\uFE0F HitPoints: {player.MaxHitPoints}/{player.MaxHitPoints}", "\u200b")
                .AddField("📊 Stats", "\n​​​​\u200b" +
                             $"    Strength: **{player.BaseStats.Strength}** \n" +
                             $"    Charisma: **{player.BaseStats.Charisma}** \n" +
                             $"     Agility: **{player.BaseStats.Agility}** \n" +
                             $"   Endurance: **{player.BaseStats.Endurance}** \n" +
                             $"        Luck: **{player.BaseStats.Luck}** \n" +
                             $"Intelligence: **{player.BaseStats.Intelligence}** \n")
                .Build();
        }

        private Embed CreateMonsterEmbed(Being monster)
        {
            return new EmbedBuilder()
                .WithColor(Color.Red)
                .WithThumbnailUrl(UrlGetter.GetMonsterPortrait(monster.Name))
                .WithTitle(monster.Name)
                .AddField($"Class: {monster.ClassName}", "\u200b")
                .AddField($"\u2665\uFE0F HitPoints: {monster.MaxHitPoints}/{monster.MaxHitPoints}", "\u200b")
                .AddField("\u2139\uFE0F Combat Description", $"-{monster.FirstAbility.Description}\n-{monster.SecondAbility.Description}")
                .AddField("📊 Stats", "\n​​​​\u200b" +
                             $"    Strength: **{monster.BaseStats.Strength}** \n" +
                             $"    Charisma: **{monster.BaseStats.Charisma}** \n" +
                             $"     Agility: **{monster.BaseStats.Agility}** \n" +
                             $"   Endurance: **{monster.BaseStats.Endurance}** \n" +
                             $"        Luck: **{monster.BaseStats.Luck}** \n" +
                             $"Intelligence: **{monster.BaseStats.Intelligence}** \n")
                .Build();
        }

        private async void OnGameEnded(object sender, GameEndedEventArgs e)
        {
            await AnnounceGameEnd(e.Announcement);

            if (e.IsVictory)
            {
                HandleVictory(e.PlayerParty);
            }

            await CleanUp();
            StartNewLobbyTimer();
        }

        private async Task AnnounceGameEnd(string announcement)
        {
            await PostToFeedChannel.SendChannelMessage($"**{announcement}**", client);
        }

        private async void HandleVictory(List<Being> playerParty)  // Assuming the type is "Player".
        {
            foreach (var member in playerParty)
            {
                if (!member.IsAlive) continue;

                await AwardSkillPointToPlayer(member);
                await AwardLootToPlayer(member);
            }
        }

        private async Task AwardSkillPointToPlayer(Being member)
        {
            var embed = CreatePlayerEmbed(member);  // Use a method similar to the one we refactored earlier.

            var menu = new SelectMenuBuilder()
                        .WithPlaceholder("Select a Skill")
                        .WithCustomId("skill-select")
                        .WithMinValues(1)
                        .WithMaxValues(1)
                        .AddOption($"Strength ({member.BaseStats.Strength}) + 1", $"strength&{member.DiscordId}")
                        .AddOption($"Charisma ({member.BaseStats.Charisma}) + 1", $"charisma&{member.DiscordId}")
                        .AddOption($"Agility ({member.BaseStats.Agility}) + 1", $"agility&{member.DiscordId}")
                        .AddOption($"Endurance ({member.BaseStats.Endurance}) + 1", $"endurance&{member.DiscordId}")
                        .AddOption($"Luck ({member.BaseStats.Luck}) + 1", $"luck&{member.DiscordId}")
                        .AddOption($"Intelligence ({member.BaseStats.Intelligence}) + 1", $"intelligence&{member.DiscordId}");

            var comp = new ComponentBuilder().WithSelectMenu(menu);

            await PostToFeedChannel.SendSkillUpMessage($"**{member.Name}** has earned themselves a skill point.", client, embeds: new[] { embed }, messageComponent: comp.Build());
        }

        private async Task AwardLootToPlayer(Being member)
        {
            await Task.Run(() => playerRepository.AddToPlayerLootTable(member.DiscordId));
            await PostToFeedChannel.SendChannelMessage($"**{member.Name}** has discovered an Item Shard! Added to Inventory.", client);
        }

        private void StartNewLobbyTimer()
        {
            timer = new Timer(GetRandomInterval());
            timer.Elapsed += OnLobbyOpen;
            timer.AutoReset = false;
            timer.Start();
        }

        private async Task CleanUp()
        {
            Console.WriteLine("Cleaning up..");
            PostToFeedChannel.MessageCache.Add(TurnEvenHistoryMessagedId);
            PostToFeedChannel.MessageCache.Add(TurnRequestMessageId);
            await Task.Run(() => PostToFeedChannel.Flush(client));
        }

        public async Task UpdateBeing(ulong id, Skill skill)
        {

            Being being = this.GetCharacterInfo(id);

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

            Console.WriteLine($"Pushing Update for {being.Name}");

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

        private EmbedBuilder CreateEmbed(TurnActionsEventArgs turnSummary, string title)
        {
            var attackName = turnSummary.ActionPerformed switch
            {
                ActionPerformed.BasicAttack => UrlGetter.GetAbilityImage("Basic"),
                ActionPerformed.FirstAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.FirstAbility.Name),
                ActionPerformed.SecondAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.SecondAbility.Name),
                ActionPerformed.ArmorAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.Chest.Name),
                ActionPerformed.WeaponAbility => UrlGetter.GetAbilityImage(turnSummary.ActivePlayer.Weapon.Effect.Name),
            };

            var embed = new EmbedBuilder()
                            .WithColor(turnSummary.ActivePlayer.IsHuman ? Color.Blue : Color.Red)
                            .WithTitle("Turn for " + title)
                            .WithImageUrl(attackName);

            foreach (var log in turnSummary.CombatLogs)
            {
                embed.AddField(new Emoji("\u2757") + " **Event:**", log);
            }

            return embed;
        }

        private async Task SendEventHistoryAsync(TurnActionsEventArgs turnSummary)
        {
            if (turnSummary.CombatLogs.Count == 0) return;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            string title = turnSummary.ActivePlayer.IsHuman
                            ? (await client.GetUserAsync(turnSummary.ActivePlayer.DiscordId)).Username
                            : turnSummary.ActivePlayer.Name;

            var embed = CreateEmbed(turnSummary, title);
            await SendOrModifyEmbedMessage(channel, embed);

            var playersInfo = GeneratePlayersInfo(gameManager.CurrentEncounter.PlayerParty.Members)
                            .Concat(GeneratePlayersInfo(gameManager.CurrentEncounter.EncounterParty.Members))
                            .ToList();

            await SendOrModifyPlayersInfoMessage(channel, playersInfo);
        }

        private async Task SendOrModifyEmbedMessage(ITextChannel channel, EmbedBuilder embed)
        {
            if (TurnRequestMessageId == 0)
            {
                var eventMessage = await channel.SendMessageAsync(embed: embed.Build());
                TurnRequestMessageId = eventMessage.Id;
            }
            else
            {
                await channel.ModifyMessageAsync(TurnRequestMessageId, m => m.Embed = embed.Build());
            }
        }

        private IEnumerable<string> GeneratePlayersInfo(IEnumerable<Being> members)
        {
            foreach (var member in members)
            {
                yield return $"**{member.Name}**:\n" +
                             $"\u2665\uFE0F **HitPoints**: {member.HitPoints}/{member.MaxHitPoints} \n" +
                             $"\uD83D\uDCA2 **Statuses**: {(member.ActiveEffects.Count != 0 ? string.Join("\n", member.ActiveEffects.Select(x => "- " + x.Name + " - " + x.Description)) : "none")} \n";
            }
        }

        private async Task SendOrModifyPlayersInfoMessage(ITextChannel channel, List<string> playersInfo)
        {
            if (TurnEvenHistoryMessagedId == 0)
            {
                var eventMessage = await channel.SendMessageAsync(embed: new EmbedBuilder().WithDescription(string.Join("\n", playersInfo)).WithTitle("Battlefield Report").WithThumbnailUrl("https://i.imgur.com/f8M2Y5s.png").Build());
                TurnEvenHistoryMessagedId = eventMessage.Id;
            }
            else
            {
                await channel.ModifyMessageAsync(TurnEvenHistoryMessagedId, m => m.Embed = new EmbedBuilder().WithDescription(string.Join("\n", playersInfo)).WithThumbnailUrl("https://i.imgur.com/f8M2Y5s.png").WithTitle("Battlefield Report").Build());
            }
        }

        private async Task SendTurnRequest(Being player)
        {
            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);
            var currentUser = await client.GetUserAsync(player.DiscordId);
            var isDefending = this.ReceiveRequest(player.DiscordId) == null;

            var builder = new ComponentBuilder();
            var characterSheet = isDefending ? BuildDefenseEmbed(player, currentUser) : BuildAttackEmbed(player, currentUser);

            SetupComponentButtons(builder, player, isDefending);

            var mentionName = channel.GetUser(player.DiscordId);

            await SendOrUpdateTurnRequestMessage(channel, mentionName, characterSheet, builder, isDefending);
        }

        private EmbedBuilder BuildDefenseEmbed(Being player, IUser currentUser)
        {
            var attacker = gameManager.CurrentEncounter.CurrentTurn;
            return new EmbedBuilder()
                .AddField($"{player.Chest.Name}", $"{player.Chest.Description}\n")
                .WithThumbnailUrl(currentUser.GetAvatarUrl())
                .WithTitle($"**{currentUser.Mention}** is being attacked by **{attacker.Name}**")
                .WithImageUrl(UrlGetter.GetMonsterPortrait(attacker.Name));
        }

        private EmbedBuilder BuildAttackEmbed(Being player, IUser currentUser)
        {
            return new EmbedBuilder()
                .AddField($"{player.Weapon.ItemName}", $"{player.Weapon.Effect.PrintPassiveStatus()}")
                .WithThumbnailUrl(currentUser.GetAvatarUrl())
                .WithImageUrl(UrlGetter.GetClassImage(player.ClassName))
                .WithTitle("Turn for " + player.Name);
        }

        private void SetupComponentButtons(ComponentBuilder builder, Being player, bool isDefending)
        {
            if (isDefending)
            {
                builder.WithButton($"{player.Chest.Name}", "chest-ability", disabled: player.Chest.IsUsed)
                       .WithButton("Take No Action", "take-damage");
            }
            else
            {
                builder.WithButton("Basic Attack", "basic-attack")
                       .WithButton(player.FirstAbility.Name, "first-ability", disabled: player.FirstAbility.Cooldown > 0)
                       .WithButton(player.SecondAbility.Name, "second-ability", disabled: player.SecondAbility.Cooldown > 0)
                       .WithButton(player.Weapon.ItemName, "weapon-active", disabled: player.Weapon.Effect.IsUsed);
            }
        }

        private async Task SendOrUpdateTurnRequestMessage(ITextChannel channel, SocketGuildUser mentionName, EmbedBuilder characterSheet, ComponentBuilder builder, bool isDefending)
        {
            if (TurnRequestMessageId == 0)
            {
                var consoleBlock = await channel.SendMessageAsync("```\n \n```");
                var newMessage = await channel.SendMessageAsync($"Turn Started for **{mentionName.Mention}**", components: builder.Build());

                TurnRequestMessageId = newMessage.Id;
                TurnEvenHistoryMessagedId = consoleBlock.Id;
            }
            else
            {
                var messageContent = isDefending ? $"Defense Action for **{mentionName.Mention}**." : $"Turn Started for **{mentionName.Mention}**.";

                await channel.ModifyMessageAsync(TurnRequestMessageId, m =>
                {
                    m.Content = messageContent;
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
            if (gameManager.CurrentEncounter == null)
            {
                await component.RespondAsync("The Encounter has ended.", ephemeral: true);
                return;
            }
                

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

        private async Task PlayCampaignCinematic(Campaign campaign)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Memories begin to flood in..")
                .WithDescription(campaign.IntroductionPost)
                .WithImageUrl(campaign.IntroductionImageUrl);

            ulong storyId = await PostToFeedChannel.SendStoryMessage(client, embed.Build());

            await Task.Delay(7000);
            await PostToFeedChannel.EditStoryMessage(client, storyId, campaign.MidPost, campaign.MidPostImageUrl);
            await Task.Delay(7000);
            await PostToFeedChannel.EditStoryMessage(client, storyId, campaign.PreFightPost, campaign.PreFightPostImageUrl);
            await Task.Delay(7000);
            
        }

        public bool AddNewCharacter(ulong id, string name, string className)
        {
            var being = GameManager.AssignRandomClass(className);
            being.Helm = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Weapon = Mapper.ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(1));
            being.Chest = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Gloves = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Boots = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Legs = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(1));
            being.Name = name;
            being.DiscordId = id;
            being.Inventory = new List<Item>();

            if (!playerRepository.HasLootTableCreated(id))
            {
                playerRepository.AddPlayerLootTable(id);
            }

            return playerRepository.AddPlayerByDiscordId(Mapper.BeingToEntityModel(being));
        }

        public void UpdatePlayerEquipment(ulong id, ItemType type, int itemId)
        {
            _ = playerRepository.UpdateEquipment(id, type, itemId);
        }

        public Being GetCharacterInfo(ulong id)
        {
            var beingEntity = playerRepository.GetBeingByDiscordId(id);
            return Mapper.BeingEntityToDomainModel(beingEntity);
        }
    }
}

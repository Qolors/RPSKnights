using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.RPGEngine.Data.Repository;
using LeagueStatusBot.Services;
using System;
using System.Threading.Tasks;

namespace LeagueStatusBot.Modules
{
    public class RPGModule : InteractionModuleBase<SocketInteractionContext>
    {
        private GameControllerService gameControllerService;
        private DiscordSocketClient client;
        private InteractiveService interactiveService;
        private readonly ItemRepository itemRepository;
        private readonly PlayerRepository playerRepository;

        private const string emoji = "\u2694\uFE0F";
        public RPGModule(GameControllerService gameControllerService, DiscordSocketClient client, ItemRepository itemRepository, PlayerRepository playerRepository, InteractiveService interactiveService)
        {
            this.playerRepository = playerRepository;
            this.itemRepository = itemRepository;
            this.gameControllerService = gameControllerService;
            this.interactiveService = interactiveService;
            this.client = client;
        }

        [SlashCommand("roll", "Roll for a new Character!")]
        public async Task GenerateNewCharacter()
        {
            await DeferAsync(true);

            if (gameControllerService.CheckIfPlayerExists(Context.User.Id))
            {
                Console.WriteLine("1");
                await FollowupAsync("You already have a character created!", ephemeral: true);
                return;
            }

            var pages = new[]
            {
                new PageBuilder()
                .WithTitle("Grand Wizard Po")
                .WithImageUrl("https://i.imgur.com/s6A594B.png")
                .WithDescription("Welcome, brave souls, to a realm where memories long forgotten are reborn, and destinies are reshaped. I am the Grand Wizard Po, the keeper of time's tapestry and guardian of lost legends. This world, you'll soon discover, is a nexus of battles once lost, now awaiting redemption.\r\n\r\nEach portal you see beckons with tales of heroes past, their spirits longing for another chance to confront the adversaries that once bested them. And you, chosen ones, bear the weight of their hopes. Guided by my hand and the magic that binds this realm, you shall step into their shoes, live their memories, and challenge the very foes that led to their downfall.\r\n\r\nYour journey will span vast oceans haunted by abyssal behemoths, and skies guarded by celestial beings. Remember, while the past cannot be changed, the outcomes of these battles can be reshaped by your hands.\nThe Following pages will help you select your archetype.")
                .WithFooter(f => f.WithText("Pre-Alpha Build v0.1")),
                new PageBuilder()
                .WithTitle("The Adventurer Archetype")
                .AddField("Starter Skill: First Aid", "A burst heal (50% of your Max Hitpoints) to anyone in your party")
                .AddField("Starter Skill: Parrying Strike", "A weakened strike that reduces the damage you take for multiple rounds")
                .WithImageUrl(UrlGetter.GetClassImage("Adventurer"))
                .WithDescription("A versatile skill set ideal for front-line / melee builds"),
                new PageBuilder()
                .WithTitle("The Apprentice Archetype")
                .AddField("Starter Skill: Arcane Bolt", "A ramping damage ability that can be used frequently")
                .AddField("Starter Skill: Mind Snap", "A solid utility spell that knocks out a targeted enemy")
                .WithImageUrl(UrlGetter.GetClassImage("Apprentice"))
                .WithDescription("A high damage skill set that can also bring good control of the battlefield."),
                new PageBuilder()
                .WithTitle("The Vagabond Archetype")
                .AddField("Starter Skill: Splintered Shot", "A ranged attack causing the target to bleed")
                .AddField("Starter Skill: Fall Back", "A defensive skill allowing you to regroup and increase your next attack's damage")
                .WithImageUrl(UrlGetter.GetClassImage("Vagabond"))
                .WithDescription("A versatile skill set with good dot damage and status effects."),
            };

            var paginator = new StaticPaginatorBuilder()
                .AddUser(Context.User) // Only allow the user that executed the command to interact with the selection.
                .WithPages(pages) // Set the pages the paginator will use. This is the only required component.
                .WithActionOnCancellation(ActionOnStop.DeleteMessage)
                .WithActionOnTimeout(ActionOnStop.DisableInput)
                .Build();

            

            // Send the paginator to the source channel and wait until it times out after 10 minutes.
            await interactiveService.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(1));

            Console.WriteLine("Starting");

            string selectedClass = string.Empty;

            switch (paginator.CurrentPageIndex)
            {
                case 0:
                    selectedClass = "Adventurer";
                    break;
                case 1:
                    selectedClass = "Apprentice";
                    break;
                case 2:
                    selectedClass = "Vagabond";
                    break;

            }

            if (gameControllerService.AddNewCharacter(Context.User.Id, Context.User.Username, selectedClass))
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

        [SlashCommand("getcharacterinfo", "Show Character Loadout")]
        public async Task ShowCharacterInfo()
        {
            await DeferAsync(true);

            if (!gameControllerService.CheckIfPlayerExists(Context.User.Id))
            {
                await FollowupAsync("You don't have a character created!", ephemeral: true);
                return;
            }

            var player = gameControllerService.GetCharacterInfo(Context.User.Id);

            var embed = new EmbedBuilder()
                .WithTitle(player.Name)
                .WithDescription($"""
                {player.ClassName}
                _Stats_
                -     Strength: {player.BaseStats.Strength}
                -    Endurance: {player.BaseStats.Endurance}
                -         Luck: {player.BaseStats.Luck}
                - Intelligence: {player.BaseStats.Intelligence}
                -     Charisma: {player.BaseStats.Charisma}
                -      Agility: {player.BaseStats.Agility}
                """)
                .AddField(player.FirstAbility.Name, player.FirstAbility.Description)
                .AddField(player.SecondAbility.Name, player.SecondAbility.Description)
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .WithImageUrl(UrlGetter.GetClassImage(player.ClassName))
                .Build();

            var embed3 = new EmbedBuilder()
                .WithTitle(player.Weapon.ItemName)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Weapon.Effect.Description)
                .Build();

            var embed2 = new EmbedBuilder()
                .WithTitle(player.Helm.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Helm.Description)
                .Build();

            var embed6 = new EmbedBuilder()
                .WithTitle(player.Chest.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Chest.Description)
                .Build();

            var embed4 = new EmbedBuilder()
                .WithTitle(player.Gloves.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Gloves.Description)
                .Build();

            var embed7 = new EmbedBuilder()
                .WithTitle(player.Legs.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Legs.Description)
                .Build();

            var embed5 = new EmbedBuilder()
                .WithTitle(player.Boots.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Boots.Description)
                .Build();

            await FollowupAsync("**Aye..**", embeds: new Embed[] {embed, embed3, embed2, embed6, embed4, embed7, embed5}, ephemeral: true);
        }

        [SlashCommand("showcharacterinfo", "Show Character Loadout to others")]
        public async Task RevealCharacterInfo()
        {
            await DeferAsync();

            if (!gameControllerService.CheckIfPlayerExists(Context.User.Id))
            {
                await FollowupAsync("You don't have a character created!", ephemeral: true);
                return;
            }

            var player = gameControllerService.GetCharacterInfo(Context.User.Id);

            var embed = new EmbedBuilder()
                .WithTitle(player.Name)
                .WithDescription($"""
                {player.ClassName}
                __Stats__
                -     Strength: {player.BaseStats.Strength}
                -    Endurance: {player.BaseStats.Endurance}
                -         Luck: {player.BaseStats.Luck}
                - Intelligence: {player.BaseStats.Intelligence}
                -     Charisma: {player.BaseStats.Charisma}
                -      Agility: {player.BaseStats.Agility}
                """)
                .AddField(player.FirstAbility.Name, player.FirstAbility.Description)
                .AddField(player.SecondAbility.Name, player.SecondAbility.Description)
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .WithImageUrl(UrlGetter.GetClassImage(player.ClassName))
                .Build();

            var embed3 = new EmbedBuilder()
                .WithTitle(player.Weapon.ItemName)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Weapon.Effect.Description)
                .Build();

            var embed2 = new EmbedBuilder()
                .WithTitle(player.Helm.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Helm.Description)
                .Build();

            var embed6 = new EmbedBuilder()
                .WithTitle(player.Chest.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Chest.Description)
                .Build();

            var embed4 = new EmbedBuilder()
                .WithTitle(player.Gloves.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Gloves.Description)
                .Build();

            var embed7 = new EmbedBuilder()
                .WithTitle(player.Legs.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Legs.Description)
                .Build();

            var embed5 = new EmbedBuilder()
                .WithTitle(player.Boots.Name)
                .WithThumbnailUrl(Context.User.GetDefaultAvatarUrl())
                .WithDescription(player.Boots.Description)
                .Build();

            await FollowupAsync("**Aye..**", embeds: new Embed[] { embed, embed3, embed2, embed6, embed4, embed7, embed5 }, ephemeral: false);
        }

        [SlashCommand("useshard", "Break open an Item Shard, Conjuring a random item.")]
        public async Task RollAnItem()
        {
            await DeferAsync(true);

            if (!gameControllerService.CheckIfPlayerExists(Context.User.Id))
            {
                await FollowupAsync("You don't have a character created!", ephemeral: true);
                return;
            }

            if (!playerRepository.SubtractFromPlayerLootTable(Context.User.Id))
            {
                await FollowupAsync("You don't have any Item Shards to break!", ephemeral: true);
                return;
            }

            var beingEnt = playerRepository.GetBeingByDiscordId(Context.User.Id);

            if (new Random().Next(2) == 0)
            {
                var chest = Mapper.ArmorEffectEntityToDomainModel(itemRepository.GenerateRandomChestArmor());
                PostToFeedChannel.MessageCache.Add((await FollowupAsync(embed: PostToFeedChannel.ShowArmorPiece(chest), components: PostToFeedChannel.ArmorButtons(chest, beingEnt.DiscordId))).Id);
            }
            else
            {
                var weapon = Mapper.ItemEntityToDomainModel(itemRepository.GenerateRandomWeapon());
                PostToFeedChannel.MessageCache.Add((await FollowupAsync(embed: PostToFeedChannel.ShowWeaponPiece(weapon), components: PostToFeedChannel.WeaponButtons(weapon, beingEnt.DiscordId))).Id);
            }


        }
    }
}

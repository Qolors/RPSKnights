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
        private ItemRepository itemRepository;
        private PlayerRepository playerRepository;

        private const string emoji = "\u2694\uFE0F";
        public RPGModule(GameControllerService gameControllerService, DiscordSocketClient client, ItemRepository itemRepository, PlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
            this.itemRepository = itemRepository;
            this.gameControllerService = gameControllerService;
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

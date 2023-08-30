using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Factories.ArmorEffects;

namespace LeagueStatusBot.Helpers
{
    public static class PostToFeedChannel
    {
        public static ulong PortalMessage { get; set; }
        public static List<ulong> MessageCache { get; set; } = new List<ulong>();
        public static async Task PostNasaToFeedChannelAsync(SpaceModel spaceModel, DiscordSocketClient client)
        {
            try
            {
                ulong guildId = ulong.Parse(Environment.GetEnvironmentVariable("DISCORD_MAIN_GUILD"));
                ulong channelId = ulong.Parse(Environment.GetEnvironmentVariable("DISCORD_TEXT_CHANNEL_FEED"));

                var embed = new EmbedBuilder();

                embed
                    .WithTitle($"Astronomy of the Day - {spaceModel.Title}")
                    .WithDescription(spaceModel.Explanation)
                    .WithImageUrl(spaceModel.Hdurl)
                    .WithCurrentTimestamp();


                await client.GetGuild(guildId).GetTextChannel(channelId).SendMessageAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.Message);
            }

        }


        public static async Task SendChannelMessage(string message, DiscordSocketClient client, Embed[] embeds = null, MessageComponent messageComponent = null, bool eph = false)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            if (embeds != null)
            {
                MessageCache.Add((await channel?.SendMessageAsync(message, embeds: embeds)).Id);
            }
            else if (messageComponent != null)
            {
                MessageCache.Add((await channel?.SendMessageAsync(message, components: messageComponent)).Id);
            }
            else
            {
                MessageCache.Add((await channel?.SendMessageAsync(message)).Id);
            }

        }

        public static async Task SendSkillUpMessage(string message, DiscordSocketClient client, Embed[] embeds, MessageComponent messageComponent, bool eph = false)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            MessageCache.Add((await channel?.SendMessageAsync(message, embeds: embeds, components: messageComponent)).Id);

        }

        public static async Task SendPortalMessage(DiscordSocketClient client)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            var button = new ComponentBuilder()
                .WithButton("Join Party", "join-party");

            var embed = new EmbedBuilder()
                .WithImageUrl(UrlGetter.GetPortalImage())
                .WithTitle("The Portal Opens..")
                .WithColor(Color.Green);

            var msg = await channel?.SendMessageAsync(embed: embed.Build(), components: button.Build());
            PortalMessage = msg.Id;
        }

        public static async Task EditOldMessage(string message, DiscordSocketClient client)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            var button = new ComponentBuilder()
                .WithButton("Join Party", "join-party", disabled: true)
                .Build();

            var closingEmbed = new EmbedBuilder()
                .WithTitle("The Portal Closes..")
                .WithImageUrl(UrlGetter.GetPortalImage())
                .WithColor(Color.Red)
                .Build();

            await channel?.ModifyMessageAsync(PortalMessage, msg => { msg.Components = button; msg.Embed = closingEmbed; });
            await Task.Delay(10000);
            await channel?.DeleteMessageAsync(PortalMessage);
        }

        public static Embed ShowArmorPiece(IArmorEffect armor)
        {
            return new EmbedBuilder()
                .WithTitle($"Has Conjured up {armor.Name}!")
                .AddField("Active", $"{armor.Description}")
                .WithImageUrl(UrlGetter.GetArmorImage(armor.Name))
                .WithColor(Color.Teal)
                .Build();
        }

        public static MessageComponent ArmorButtons(IArmorEffect armor, ulong id)
        {
            return new ComponentBuilder()
                .WithButton("Toss", "trash", style: ButtonStyle.Danger)
                .WithButton("Equip", $"chest&{armor.EffectId}&{id}", style: ButtonStyle.Success)
                .Build();
        }

        public static Embed ShowWeaponPiece(Item item)
        {
            return new EmbedBuilder()
                .WithTitle($"Has Conjured up {item.ItemName}!")
                .AddField("Description", $"{item.Effect.Description}")
                .WithImageUrl(UrlGetter.GetWeaponImage(item.ItemName))
                .WithColor(Color.Teal)
                .Build();
        }

        public static MessageComponent WeaponButtons(Item item, ulong id)
        {
            return new ComponentBuilder()
                .WithButton("Toss", "trash", style: ButtonStyle.Danger)
                .WithButton("Equip", $"weapon&{item.ItemId}&{id}", style: ButtonStyle.Success)
                .Build();
        }

        public static async Task Flush(DiscordSocketClient client)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            await Task.Delay(20000);

            await channel.DeleteMessagesAsync(MessageCache);

            MessageCache.Clear();
        }
    }
}

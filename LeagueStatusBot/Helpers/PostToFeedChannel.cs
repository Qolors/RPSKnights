using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LeagueStatusBot.Common.Models;

namespace LeagueStatusBot.Helpers
{
    public static class PostToFeedChannel
    {
        public static ulong PortalMessage { get; set; }
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

        public static async Task PostGameUpdate(string update, DiscordSocketClient client)
        {
            const ulong guildId = 402652836606771202;
            const ulong channelId = 702684769200111716;

            await client.GetGuild(guildId).GetTextChannel(channelId).SendMessageAsync(update);
        }


        public static async Task SendChannelMessage(string message, DiscordSocketClient client, Embed[] embeds = null, MessageComponent messageComponent = null, bool eph = false)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            if (embeds != null)
            {
                await channel?.SendMessageAsync(message, embeds: embeds);
            }
            else if (messageComponent != null)
            {
                await channel?.SendMessageAsync(message, components: messageComponent);
            }
            else
            {
                await channel?.SendMessageAsync(message);
            }

        }

        public static async Task SendSkillUpMessage(string message, DiscordSocketClient client, Embed[] embeds, MessageComponent messageComponent, bool eph = false)
        {
            const ulong GUILD_ID = 402652836606771202;
            const ulong CHANNEL_ID = 702684769200111716;

            var channel = client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID);

            await channel?.SendMessageAsync(message, embeds: embeds, components: messageComponent);

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

        public static string GetHealthBar(int currentHealth, int maxHealth)
        {
            int barWidth = 20;
            int healthRatio = (int)((double)(currentHealth / maxHealth) * barWidth);
            return "**|" + new string('#', healthRatio) + new string(' ', barWidth - healthRatio) + "|**"; // Note the zero-width space in place of ' '.
        }
    }
}

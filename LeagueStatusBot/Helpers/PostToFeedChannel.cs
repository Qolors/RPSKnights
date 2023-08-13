using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LeagueStatusBot.Common.Models;
using Newtonsoft.Json;

namespace LeagueStatusBot.Helpers
{
    public static class PostToFeedChannel
    {
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
            ulong guildId = 402652836606771202;
            ulong channelId = 702684769200111716;

            await client.GetGuild(guildId).GetTextChannel(channelId).SendMessageAsync(update);
        }

        public static async Task SendChannelMessage(string message, DiscordSocketClient client)
        {
            ulong guildId = 402652836606771202;
            ulong channelId = 702684769200111716;

            var channel = client.GetGuild(guildId).GetTextChannel(channelId);

            await channel?.SendMessageAsync(message);
        }

        public static List<LiveGameData> ExtractFromGPT(string s)
        {
            int index = s.IndexOf("#");

            if (index == -1)
            {
                return null;
            }

            string jsonStr = s.Substring(index + 1);

            var liveGameData = JsonConvert.DeserializeObject<List<LiveGameData>>(jsonStr);

            return liveGameData;
        }

        public static string ExtractTextBeforeHash(string s)
        {
            int index = s.IndexOf('#');

            if (index == -1)
            {
                return s; // Return the whole string if '#' is not found.
            }

            return s.Substring(0, index);
        }
    }
}

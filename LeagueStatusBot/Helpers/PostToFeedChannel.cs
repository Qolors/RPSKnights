using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.Helpers
{
    public static class PostToFeedChannel
    {
        public static ulong PortalMessage { get; set; }
        public static List<ulong> MessageCache { get; set; } = new List<ulong>();
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
    }
}

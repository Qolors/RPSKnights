using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Fergun.Interactive;

namespace LeagueStatusBot.Factories;

/// <summary>
/// MessageFactory is a static builder for all repeating messages
/// </summary>
public static class MessageFactory
{
    public async static Task UpdateAttachmentMessage(RestUserMessage attachmentMessage, FileAttachment file)
    {
        await attachmentMessage.ModifyAsync(x => x.Attachments = new FileAttachment[] { file });
    }

    public async static Task<RestUserMessage> InitializeAttachmentMessage(SocketInteractionContext context, RestUserMessage attachmentMessage, FileAttachment file, FileAttachment starterFile)
    {
        if (attachmentMessage == null)
        {
            attachmentMessage = await context.Channel.SendFileAsync(starterFile);
        }
        else
        {
            await attachmentMessage.ModifyAsync(x => x.Attachments = new FileAttachment[] { file });
        }
        return attachmentMessage;
    }

    public static PageBuilder CreatePageBuilder(SocketUser context, Color color, List<string> playerChoices, SocketUser otherUser, int[] hitpoints, string status, int round)
    {
        return new PageBuilder()
            .WithTitle($"Round #{round}")
            .WithThumbnailUrl(context.GetAvatarUrl())
            .WithColor(color)
            .WithDescription($"{status ?? ""}\n**{context.Username}**\n\u2665 {hitpoints[0]}/3\n**{otherUser.Username}**\n\u2665 {hitpoints[1]}/3\n*{context.Mention} you need to make **{1 - playerChoices.Count}** more Actions*");
    }

    public static PageBuilder CreatePageBuilder(SocketUser context, Color color, List<string> playerChoices, SocketUser otherUser, int[] hitpoints, int player1Energy, int player2Energy, string status, int round)
    {
        return new PageBuilder()
            .WithTitle($"Round #{round}")
            .WithThumbnailUrl(context.GetAvatarUrl())
            .WithColor(color)
            .WithDescription($"{status ?? ""}\n**{context.Username}**\n\u2665 {hitpoints[0]}/3\n\u25AA {player1Energy}/5\n**{otherUser.Username}**\n\u2665 {hitpoints[1]}/3\n\u25AA {player2Energy}/5\n*{context.Mention} you need to make **{1 - playerChoices.Count}** more Actions*");
    }

    public static PageBuilder CreateChallengeMessage(string challenger, string avatarUrl, string mention)
    {
        return new PageBuilder()
                .WithTitle($"{challenger} challenges you to a duel")
                .WithThumbnailUrl(avatarUrl)
                .WithDescription($"*{mention}, What is your response?*");
    }

    public static PageBuilder CreateEndGameMessage(string winner)
    {
        return new PageBuilder()
                .WithTitle($"**{winner} wins the duel.**");
    }

    public static PageBuilder CreateChallengeNeglectedMessage(string username)
    {
        return new PageBuilder()
                .WithDescription($"*{username}'s duel expired..*");
    }

    public static Embed[] BuildIntroMessage(string firstMention, string secondMention, string firstAvatar, string secondAvatar)
    {
        var embed = new EmbedBuilder()
                .WithTitle("A Battle Begins!")
                .WithDescription($"**\uD83D\uDD35 {firstMention}** VS **\uD83D\uDD34 {secondMention}**");

        return new Embed[] { embed.Build() };
    }

    public static Embed[] BuildLeaderboard(List<string> players)
    {
        var embed = new EmbedBuilder()
            .WithTitle("Top Player Ranking");

        for (int i = 0; i < players.Count; i++)
        {
            var split = players[i].Split("&");

            embed.AddField($"#{i + 1} - {split[0]}", split[1]);
        }

        return new Embed[] { embed.Build() };
    }


}